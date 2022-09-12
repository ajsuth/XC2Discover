// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppendSellableItemToFeedBlock.cs" company="Sitecore Corporation">
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
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Inventory;
using Sitecore.Commerce.Plugin.Management;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>The feed result model.</summary>
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
                await CreateSKURows(context, sellableItem, product);
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
                    SKU = productId, // Handles flattenning standalone products, and SKU products will override this value.
                    CcIds = await GetCategoryIds(context, sellableItem),
                    Price = GetListPrice(sellableItem) ?? 0,
                    //SalePrice = GetSalePrice(sellableItem),
                    IsActive = true,
                    ProductType = "Product",
                    SearchKeywords = string.Join('|', sellableItem.Tags.Select(t => t.Name)),
                    Brand = sellableItem.Brand,
                    StockQuantity = await GetInventoryFromDefaultInventorySet(context, sellableItem),
                };
                product.SalePrice = product.Price;

                await CreateCustomProductAttributesMappings(context, sellableItem, product);

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

            FeedService.AppendToFeedFile(ProductSettings.ProductFeedFilePath, product);

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

        protected virtual float? GetListPrice(SellableItem sellableItem, string variantId = null)
        {
            var listPricePolicy = sellableItem.GetPolicy<ListPricingPolicy>(variantId, false);
            var price = listPricePolicy?.Prices.FirstOrDefault(p => p.CurrencyCode == ProductSettings.DefaultCurrency);
            return (float)price?.Amount;
        }

        protected virtual float? GetSalePrice(SellableItem sellableItem)
        {
            return null;
        }

        protected virtual async Task<int?> GetInventoryFromDefaultInventorySet(CommercePipelineExecutionContext context, SellableItem sellableItem, string variantId = null)
        {
            var inventory = await GetInventoryInformation(context, sellableItem, variantId, ProductSettings.InventorySetId);

            return inventory?.Quantity;
        }

        /// <summary>
        /// Gets the first image associated to the sellable item.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sellableItem">The XC sellable item.</param>
        /// <returns>The OC <see cref="Product"/>.</returns>
        protected async Task<string> GetFirstProductImage(CommercePipelineExecutionContext context, SellableItem sellableItem, string variantId = null)
        {
            var imageUrl = string.Empty;
            var imagesComponent = sellableItem.GetComponent<ImagesComponent>(variantId);
            var storageService = context.CommerceContext.GetObject<CloudStorageService>();

            foreach (var imageId in imagesComponent.Images)
            {
                var itemModel = await Commander.Pipeline<IGetItemByIdPipeline>().RunAsync(new ItemModelArgument(imageId), context).ConfigureAwait(false);
                if (itemModel == null)
                {
                    Result.ProductImages.ItemsErrored++;
                    context.Logger.LogError($"{Name}: Processing image '{imageId}' for sellable item '{sellableItem.Id}' failed.");
                }

                imageUrl = $"{storageService.GetBaseUrl()}{storageService.Container.Name}/{sellableItem.FriendlyId}/{itemModel[ItemModel.ItemName]}";
                break;
            }

            return imageUrl;
        }

        protected async Task CreateCustomProductAttributesMappings(CommercePipelineExecutionContext context, SellableItem sellableItem, DiscoverProduct product)
        {
            product.Manufacturer = sellableItem.Manufacturer;

            await Task.FromResult(product);
        }

        protected async Task CreateSKURows(CommercePipelineExecutionContext context, SellableItem sellableItem, DiscoverProduct product)
        {
            try
            {
                var variationsComponent = sellableItem.GetComponent<ItemVariationsComponent>();
                var variations = variationsComponent.GetChildComponents<ItemVariationComponent>();
                foreach (var variation in variations)
                {
                    var skuId = variation.Id.ToValidDiscoverId();
                    var displayProperties = variation.GetChildComponent<DisplayPropertiesComponent>();

                    var skuProduct = new DiscoverProduct(product)
                    {
                        SkuName = variation.DisplayName.ToUTF8(),
                        //SkuUrl = ConstructProductRelativeUrl(skuId),
                        SkuImageUrl = ProductSettings.IncludeImages ? await GetFirstProductImage(context, sellableItem, variation.Id) : "",
                        SkuDescription = variation.Description,
                        SKU = skuId,
                        ProductType = "SKU",
                        OverridePrice = GetListPrice(sellableItem, variation.Id),
                        //OverrideSalePrice = GetSalePrice(sellableItem),
                        OverrideStockQuantity = await GetInventoryFromDefaultInventorySet(context, sellableItem, variation.Id),
                        Color = displayProperties?.Color,
                        Size = displayProperties?.Size,
                    };
                    skuProduct.OverrideSalePrice = skuProduct.OverridePrice;

                    await CreateCustomSkuAttributesMappings(context, sellableItem, skuProduct);

                    context.Logger.LogInformation($"Appending product sku; Product ID: {product.Id}, SKU: {variation.Id}");

                    FeedService.AppendToFeedFile(ProductSettings.ProductFeedFilePath, skuProduct);

                    Result.SKUs.ItemsAppended++;
                }
            }
            catch (Exception ex)
            {
                Result.SKUs.ItemsErrored++;

                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        DiscoverConstants.Errors.CreateVariantsFailed,
                        new object[]
                        {
                                Name,
                                product.Id,
                                ex.Message,
                                ex
                        },
                        $"{Name}: Ok| Creating variants '{product.Id}' failed.\n{ex.Message}\n{ex}").ConfigureAwait(false),
                    context);
            }
        }

        protected async Task CreateCustomSkuAttributesMappings(CommercePipelineExecutionContext context, SellableItem sellableItem, DiscoverProduct skuProduct)
        {

            await Task.FromResult(skuProduct);
        }

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
    }
}