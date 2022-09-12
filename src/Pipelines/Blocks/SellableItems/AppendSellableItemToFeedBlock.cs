// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportSellableItemBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Commands;
using Ajsuth.Sample.Discover.Engine.FrameworkExtensions;
using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Ajsuth.Sample.Discover.Engine.Policies;
using Ajsuth.Sample.Discover.Engine.Service;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Commerce.Plugin.Management;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Commerce.Plugin.Shops;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Blocks
{
    /// <summary>Defines the asynchronous executing ExportSellableItem pipeline block</summary>
    /// <seealso cref="AsyncPipelineBlock{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.Blocks.AppendSellableItemToFeed)]
    public class AppendSellableItemToFeedBlock : AsyncPipelineBlock<SellableItem, SellableItem, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commerce commander.</summary>
        protected ExtendedCatalogCommander Commander { get; set; }

        /// <summary>The export result model.</summary>
        protected FeedResult Result { get; set; }

        /// <summary>The problem objects model.</summary>
        protected ProblemObjects ProblemObjects { get; set; }

        /// <summary>The product settings.</summary>
        protected SellableItemFeedPolicy ProductSettings { get; set; }

        /// <summary>The site settings.</summary>
        protected List<SitePolicy> SiteSettings { get; set; }

        /// <summary>Initializes a new instance of the <see cref="AppendSellableItemToFeedBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public AppendSellableItemToFeedBlock(ExtendedCatalogCommander commander)
        {
            this.Commander = commander;
        }

        /// <summary>Executes the pipeline block's code logic.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="SellableItem"/>.</returns>
        public override async Task<SellableItem> RunAsync(SellableItem sellableItem, CommercePipelineExecutionContext context)
        {
            Condition.Requires(sellableItem).IsNotNull($"{Name}: The sellable item can not be null");

            Result = context.CommerceContext.GetObject<FeedResult>();
            ProblemObjects = context.CommerceContext.GetObject<ProblemObjects>();

            var exportSettings = context.CommerceContext.GetObject<FeedEntitiesArgument>();
            ProductSettings = exportSettings.ProductSettings;
            SiteSettings = exportSettings.SiteSettings;

            var requiresVariants = sellableItem.RequiresVariantsForOrderCloud();

            if (!ProductSettings.IncludeStandaloneProducts && !requiresVariants
                || !ProductSettings.IncludeProductsWithVariants && requiresVariants)
            {
                Result.Products.ItemsSkipped++;
                ProblemObjects.Products.Add(sellableItem.FriendlyId.ToValidDiscoverId());

                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Information,
                        "ProductTypeNotSupported",
                        new object[] { sellableItem.Id },
                        $"Ok| Sellable Item {sellableItem.Id} is of type {(requiresVariants ? "'product with variants'" : "'standalone product'")}").ConfigureAwait(false),
                    context);

                return null;
            }

            // 1. Create Product
            var product = await CreateDiscoverProduct(context, sellableItem, requiresVariants);

            if (product == null)
            {
                return null;
            }

            if (!requiresVariants)
            {
                AppendProductToFeed(context, product);
            }
            else
            {
                // 2. Create/Update Variants
                //var variants = await CreateSKURows(context, sellableItem, product);
                //if (variants == null)
                //{
                //    return null;
                //}
            }

            return sellableItem;
        }

        protected async Task<DiscoverProduct> CreateDiscoverProduct(CommercePipelineExecutionContext context, SellableItem sellableItem, bool requiresVariants)
        {
            var productId = sellableItem.FriendlyId.ToValidDiscoverId();

            try
            {
                var product = new DiscoverProduct
                {
                    Id = productId,
                    Name = sellableItem.DisplayName.ToUTF8(),
                    Url = ConstructProductRelativeUrl(productId),
                    ImageUrl = ProductSettings.IncludeImages ? await GetFirstProductImage(context, sellableItem) : "",
                    Description = sellableItem.Description,
                    SKU = productId,
                    CcIds = await GetCategoryIds(context, sellableItem),
                    Price = GetListPrice(sellableItem),
                    SalePrice = GetSalePrice(sellableItem),
                    IsActive = 1,
                    ProductType = sellableItem.TypeOfGood,
                    SearchKeywords = string.Join('|', sellableItem.Tags.Select(t => t.Name)),
                    Brand = sellableItem.Brand,
                };

                await CreateCustomProductXpMappings(context, sellableItem, product);

                return product;
            }
            catch (Exception e)
            {
                Result.Products.ItemsErrored++;
                ProblemObjects.Products.Add(productId);

                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        DiscoverConstants.Errors.CreateProductFailed,
                        new object[]
                        {
                            Name,
                            productId,
                            e.Message,
                            e
                        },
                        $"{Name}: Ok| Create product '{productId}' failed.\n{e.Message}\n{e}").ConfigureAwait(false),
                    context);

                return null;
            }
        }

        protected void AppendProductToFeed(CommercePipelineExecutionContext context, DiscoverProduct product)
        {
            context.Logger.LogInformation($"Appending product; Product ID: {product.Id}");

            FeedService.AppendToFeedFile<Category>(ProductSettings.ProductFeedFilePath, product);

            Result.Products.ItemsAppended++;
        }

        protected virtual string ConstructProductRelativeUrl(string productId)
        {
            var encodedProductId = HttpUtility.UrlEncode(productId);

            return $"products/{encodedProductId}";
        }

        protected virtual async Task<string> GetCategoryIds(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            var categoryIds = new List<string>();
            var sitecoreIds = sellableItem.GetParentCategoriesSitecoreIds();
            foreach (var id in sitecoreIds)
            {
                var parentItem = await Commander.GetParentCatalogItem(id, context);

                if (parentItem == null)
                {
                    context.Logger.LogInformation($"Category parent is not found. Possibly orphaned; Parent CatalogBaseItem ID: {parentItem.FriendlyId}, Sellable Item ID: {sellableItem.FriendlyId}");
                    continue;
                }

                if (!(parentItem is Category))
                {
                    context.Logger.LogInformation($"Category parent is not category. Possible loss of catalog-product assignment; Parent CatalogBaseItem ID: {parentItem.FriendlyId}, Sellable Item ID: {sellableItem.FriendlyId}");
                    continue;
                }

                var friendlyIdParts = parentItem.FriendlyId.Split("-");
                var catalogId = friendlyIdParts[0];
                if (catalogId != SiteSettings[0].Catalog)
                {
                    context.Logger.LogInformation($"Parent category belongs to other catalog; Parent CatalogBaseItem ID: {parentItem.FriendlyId}, Sellable Item ID: {sellableItem.FriendlyId}");
                    continue;
                }
                var categoryId = friendlyIdParts[1].ToValidDiscoverId();
                categoryIds.Add(categoryId);
            }

            return string.Join('|', categoryIds);
        }

        protected virtual float GetListPrice(SellableItem sellableItem)
        {
            var listPricePolicy = sellableItem.GetPolicy<ListPricingPolicy>();
            var price = listPricePolicy.Prices.FirstOrDefault(p => p.CurrencyCode == ProductSettings.DefaultCurrency);
            return (float)price?.Amount;
        }

        protected virtual float? GetSalePrice(SellableItem sellableItem)
        {
            return null;
        }

        protected virtual async Task<int?> GetInventoryFromDefaultInventorySet(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            var inventory = await GetInventoryInformation(context, sellableItem, sellableItem.ItemVariations, ProductSettings.InventorySetId);

            return inventory?.Quantity;
        }

        /// <summary>
        /// Gets or creates the product.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sellableItem">The XC sellable item.</param>
        /// <returns>The OC <see cref="Product"/>.</returns>
        protected async Task<string> GetFirstProductImage(CommercePipelineExecutionContext context, SellableItem sellableItem)
        {
            var imageUrl = string.Empty;
            var imagesComponent = sellableItem.GetComponent<ImagesComponent>();
            var sitecoreConnectionPolicy = context.GetPolicy<SitecoreConnectionPolicy>();
            var storageService = context.CommerceContext.GetObject<CloudStorageService>();

            foreach (var imageId in imagesComponent.Images)
            {
                var itemModel = await Commander.Pipeline<IGetItemByIdPipeline>().RunAsync(new ItemModelArgument(imageId), context).ConfigureAwait(false);
                if (itemModel == null)
                {
                    Result.ProductImages.ItemsErrored++;
                    context.Logger.LogError($"{Name}: Processing image '{imageId}' for sellable item '{sellableItem.Id}' failed.");
                }

                //var url = $"{sitecoreConnectionPolicy.MediaLibraryUrl}/{itemModel[ItemModel.MedialUrl].ToString()}";
                //var startIndex = url.IndexOf("habitat");
                //var endIndex = url.IndexOf("?");
                //url = endIndex == -1 ? url : url.Substring(0, endIndex);
                //var path = url.Substring(startIndex);
                //string fullPath = $"C:\\Projects\\Images\\{path}";

                //var fileName = Path.GetFileNameWithoutExtension(fullPath);

                imageUrl = $"{storageService.GetBaseUrl()}{storageService.Container.Name}/{sellableItem.FriendlyId}/{itemModel[ItemModel.ItemName]}";
                break;
            }

            return imageUrl;
        }

        protected async Task CreateCustomProductXpMappings(CommercePipelineExecutionContext context, SellableItem sellableItem, DiscoverProduct product)
        {
            product.Manufacturer = sellableItem.Manufacturer;
            product.Inventory = await GetInventoryFromDefaultInventorySet(context, sellableItem);

            await Task.FromResult(product);
        }

        ///// <summary>
        ///// Creates variants for a product.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="sellableItem">The XC sellable item used to enrich OC variant data and disable invalid variants.</param>
        ///// <param name="product">The OC product the variants will be created for.</param>
        ///// <returns>The list of OC <see cref="Variant"/>s created/</returns>
        //protected async Task<List<Variant>> GetOrCreateVariants(CommercePipelineExecutionContext context, SellableItem sellableItem, Product product)
        //{
        //    try
        //    {
        //        var variationsSummary = sellableItem.GetVariationsSummary();

        //        // 1. Create unique specs for product
        //        var specs = await ConstructAndSaveSpecs(context, product, variationsSummary);

        //        // 2. Create spec options
        //        await ConstructAndSaveSpecOptions(context, specs, variationsSummary);

        //        // 3. Create spec product assignments
        //        await ConstructAndSaveSpecProductAssignments(context, product, specs);

        //        // 4. Generate variants
        //        context.Logger.LogInformation($"Generating variants; Product ID: {product.ID}");
        //        await Client.Products.GenerateVariantsAsync(product.ID, true);

        //        // 5. Update generated variants
        //        var variantList = await UpdateVariants(context, sellableItem, product, variationsSummary);
                
        //        return variantList;
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Variants.ItemsErrored++;

        //        context.Abort(
        //            await context.CommerceContext.AddMessage(
        //                context.GetPolicy<KnownResultCodes>().Error,
        //                DiscoverConstants.Errors.CreateVariantsFailed,
        //                new object[]
        //                {
        //                    Name,
        //                    product.ID,
        //                    ex.Message,
        //                    ex
        //                },
        //                $"{Name}: Ok| Creating variants '{product.ID}' failed.\n{ex.Message}\n{ex}").ConfigureAwait(false),
        //            context);

        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Creates or updates product specs.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="product">The product that will be used for the Spec ID prefix.</param>
        ///// <param name="variationsSummary">The XC variations summary that will be converted to specs.</param>
        ///// <returns>The list of <see cref="Spec"/>s.</returns>
        //protected async Task<List<Spec>> ConstructAndSaveSpecs(CommercePipelineExecutionContext context, Product product, VariationsSummary variationsSummary)
        //{
        //    var specs = new List<Spec>();
        //    var distinctVariationProperties = variationsSummary.UniqueProperties.Distinct();

        //    foreach (var variationProperty in distinctVariationProperties)
        //    {
        //        var spec = new Spec
        //        {
        //            ID = $"{product.ID}_{variationProperty}",
        //            Name = variationProperty,
        //            Required = true,
        //            DefinesVariant = true
        //        };

        //        Result.Specs.ItemsProcessed++;

        //        context.Logger.LogInformation($"Saving spec; Spec ID: {spec.ID}");
        //        spec = await Client.Specs.SaveAsync(spec.ID, spec);
        //        Result.Specs.ItemsUpdated++;

        //        specs.Add(spec);
        //    }

        //    // await Throttler.RunAsync(specs, 100, 20, spec => client.Specs.SaveAsync(spec.ID, spec));

        //    return specs;
        //}

        ///// <summary>
        ///// Creates or updates product spec options.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="specs">The list of specs that will have options created.</param>
        ///// <param name="variationsSummary">The XC variations summary that will be converted to specs.</param>
        ///// <returns></returns>
        //protected async Task ConstructAndSaveSpecOptions(CommercePipelineExecutionContext context, List<Spec> specs, VariationsSummary variationsSummary)
        //{
        //    foreach (var spec in specs)
        //    {
        //        var specVariationProperties = variationsSummary.GetDistinctValues(spec.Name);

        //        foreach (var propertyValue in specVariationProperties)
        //        {
        //            var option = new SpecOption
        //            {
        //                ID = propertyValue.ToValidOrderCloudId(),
        //                Value = propertyValue
        //            };

        //            Result.SpecOptions.ItemsProcessed++;

        //            context.Logger.LogInformation($"Saving spec option; Spec ID: {spec.ID}, Option ID: {option.ID}");
        //            await Client.Specs.SaveOptionAsync(spec.ID, option.ID, option);
        //            Result.SpecOptions.ItemsUpdated++;
        //        }

        //        // await Throttler.RunAsync(specOptions, 100, 20, spec => client.Specs.SaveOptionAsync(spec.ID, specOption.ID, specOption));
        //    }
        //}

        ///// <summary>
        ///// Creates or updates product spec options.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="product">The product to be associated to the specs.</param>
        ///// <param name="specs">The list of specs that will have options created.</param>
        ///// <returns></returns>
        //protected async Task ConstructAndSaveSpecProductAssignments(CommercePipelineExecutionContext context, Product product, List<Spec> specs)
        //{
        //    foreach (var spec in specs)
        //    {
        //        var specProductAssignment = new SpecProductAssignment
        //        {
        //            SpecID = spec.ID,
        //            ProductID = product.ID
        //        };

        //        Result.SpecProductAssignments.ItemsProcessed++;

        //        context.Logger.LogInformation($"Saving spec product assignment; Spec ID: {specProductAssignment.SpecID}, Product ID: {specProductAssignment.ProductID}");
        //        await Client.Specs.SaveProductAssignmentAsync(specProductAssignment);
        //        Result.SpecProductAssignments.ItemsUpdated++;
        //    }

        //    // await Throttler.RunAsync(specOptions, 100, 20, specProductAssignments => client.Specs.SaveProductAssignmentAsync(specProductAssignment));
        //}

        ///// <summary>
        ///// Updates variants with inventory, pricing, etc.
        ///// </summary>
        ///// <param name="context">The context.</param>
        ///// <param name="client">The <see cref="OrderCloudClient"/>.</param>
        ///// <param name="sellableItem"></param>
        ///// <param name="product">The product to be associated to the specs.</param>
        ///// <param name="variationsSummary"></param>
        ///// <param name="productSettings"></param>
        ///// <param name="exportResult"></param>
        ///// <returns></returns>
        //protected async Task<List<Variant>> UpdateVariants(CommercePipelineExecutionContext context, SellableItem sellableItem, Product product, VariationsSummary variationsSummary)
        //{
        //    var variantList = new List<Variant>();
        //    var page = 1;
        //    ListPage<Variant> pagedVariants;

        //    do
        //    {
        //        pagedVariants = await Client.Products.ListVariantsAsync(product.ID, page: page++);

        //        foreach (var variant in pagedVariants.Items)
        //        {
        //            Result.Variants.ItemsProcessed++;

        //            var matchingVariant = GetVariationSummary(variationsSummary, variant);
        //            if (matchingVariant != null)
        //            {
        //                variantList.Add(variant);

        //                var xcVariant = sellableItem.GetVariation(matchingVariant.Id);
        //                var displayProperties = xcVariant.GetChildComponent<DisplayPropertiesComponent>();

        //                var updatedVariant = new PartialVariant
        //                {
        //                    ID = matchingVariant.Id,
        //                    Active = !xcVariant.Disabled,
        //                    Description = displayProperties.DisambiguatingDescription
        //                };

        //                updatedVariant.xp = new ExpandoObject();
        //                updatedVariant.xp.Tags = xcVariant.Tags.Select(t => t.Name);

        //                if (xcVariant.HasChildComponent<ItemSpecificationsComponent>())
        //                {
        //                    var specifications = xcVariant.GetChildComponent<ItemSpecificationsComponent>();

        //                    updatedVariant.ShipWeight = Convert.ToDecimal(specifications.Weight);
        //                    updatedVariant.ShipHeight = Convert.ToDecimal(specifications.Height);
        //                    updatedVariant.ShipWidth = Convert.ToDecimal(specifications.Width);
        //                    updatedVariant.ShipLength = Convert.ToDecimal(specifications.Length);
        //                }

        //                // 5a. Update variant inventory
        //                if (!ProductSettings.MultiInventory)
        //                {
        //                    var inventory = await GetInventoryInformation(context, sellableItem, matchingVariant.Id, ProductSettings.InventorySetId);
        //                    if (inventory != null)
        //                    {
        //                        updatedVariant.Inventory = new VariantInventory
        //                        {
        //                            QuantityAvailable = inventory.Quantity
        //                        };
        //                    }
        //                }

        //                // 5b. Update variant pricing
        //                updatedVariant.xp.PriceSchedules = null;
        //                if (xcVariant.HasPolicy<ListPricingPolicy>())
        //                {
        //                    // TODO: create price markups

        //                    // How would we apply variant specific pricing for multi-currency?
        //                    // It appears price markups can only support single currency as different currencies may require different results.
        //                    // Use xp to track the priceschedules?

        //                    // This is a sample workaround solution and probably not the best solution.
        //                    var priceSchedules = await CreateOrUpdatePriceSchedules(context, matchingVariant.Id, xcVariant.GetPolicy<ListPricingPolicy>());
        //                    updatedVariant.xp.PriceSchedules = priceSchedules.Select(p => p.ID).ToList();
        //                }

        //                await CreateCustomVariantXpMappings(sellableItem, updatedVariant);

        //                context.Logger.LogInformation($"Patching variant; Updating inventory and pricing; Product ID: {product.ID}, Variant ID: {variant.ID}");
        //                await Client.Products.PatchVariantAsync(product.ID, variant.ID, updatedVariant);
        //                Result.Variants.ItemsPatched++;
        //            }
        //            else
        //            {
        //                // 5c. Disable invalid variants
        //                var updatedVariant = new PartialVariant
        //                {
        //                    ID = variant.ID,
        //                    Active = false
        //                };
        //                updatedVariant.xp = new ExpandoObject();
        //                updatedVariant.xp.Tags = null;
        //                updatedVariant.xp.PriceSchedules = null;

        //                context.Logger.LogInformation($"Patching variant; Disabling invalid variants; Product ID: {product.ID}, Variant ID: {variant.ID}");
        //                await Client.Products.PatchVariantAsync(product.ID, variant.ID, updatedVariant);
        //                Result.Variants.ItemsPatched++;
        //            }

        //            // await Throttler.RunAsync(updatedVariants, 100, 20, updatedVariant => client.Products.PatchVariantAsync(product.ID, updatedVariant.ID, updatedVariant));
        //        }
        //    } while (pagedVariants != null && pagedVariants.Meta.Page < pagedVariants.Meta.TotalPages);

        //    return variantList;
        //}

        //protected async Task CreateCustomVariantXpMappings(SellableItem sellableItem, PartialVariant variant)
        //{
        //    var xp = variant.xp;

        //    await Task.FromResult(variant);
        //}

        /// <summary>
        /// Gets the Inventory Information for the sellable item or variant, if variationId is provided.
        /// </summary>
        /// <param name="sellableItem"></param>
        /// <param name="variationId"></param>
        /// <param name="inventorySetId"></param>
        /// <param name="context"></param>
        /// <returns>The <see cref="InventoryInformation"/> entry for the sellable item / variant.</returns>
        protected async Task<InventoryInformation> GetInventoryInformation(CommercePipelineExecutionContext context, SellableItem sellableItem, string variationId, string inventorySetId)
        {
            if (sellableItem == null || !sellableItem.HasComponent<InventoryComponent>(variationId))
            {
                return null;
            }

            var inventoryComponent = sellableItem.GetComponent<InventoryComponent>(variationId);

            var inventoryAssociation =
                inventoryComponent.InventoryAssociations.FirstOrDefault(x =>
                    x.InventorySet.EntityTarget.Equals(
                        inventorySetId.EnsurePrefix(CommerceEntity.IdPrefix<InventorySet>()),
                        StringComparison.OrdinalIgnoreCase));

            var inventoryInformation =
                await Commander.Pipeline<IFindEntityPipeline>()
                    .RunAsync(
                        new FindEntityArgument(typeof(InventoryInformation),
                            inventoryAssociation.InventoryInformation.EntityTarget,
                            inventoryAssociation.InventoryInformation.EntityTargetUniqueId),
                        context.CommerceContext.PipelineContextOptions)
                    .ConfigureAwait(false) as InventoryInformation;

            return inventoryInformation;
        }

        ///// <summary>
        ///// Get the variation summary for a the variant.
        ///// </summary>
        ///// <param name="variationsSummary"></param>
        ///// <param name="variant"></param>
        ///// <returns></returns>
        //protected VariationSummary GetVariationSummary(VariationsSummary variationsSummary, Variant variant)
        //{
        //    var found = true;
        //    foreach (var variation in variationsSummary.Variations)
        //    {
        //        found = true;
        //        foreach (var property in variation.VariationProperties)
        //        {
        //            if (variant.Specs.FirstOrDefault(s => s.Name == property.Name && s.Value == property.Value) == null)
        //            {
        //                found = false;
        //                break;
        //            }
        //        }

        //        if (found)
        //        {
        //            return variation;
        //        }
        //    }

        //    return null;
        //}

    }
}