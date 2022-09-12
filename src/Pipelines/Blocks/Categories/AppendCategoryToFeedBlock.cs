// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppendCategoryToFeedBlock.cs" company="Sitecore Corporation">
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
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Blocks
{
    /// <summary>Defines the asynchronous executing AppendCategoryToFeed pipeline block</summary>
    /// <seealso cref="AsyncPipelineBlock{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.Blocks.AppendCategoryToFeed)]
    public class AppendCategoryToFeedBlock : AsyncPipelineBlock<Category, Category, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commerce commander.</summary>
        protected ExtendedCatalogCommander Commander { get; set; }

        /// <summary>The export result model.</summary>
        protected FeedResult Result { get; set; }

        /// <summary>The problem objects model.</summary>
        protected ProblemObjects ProblemObjects { get; set; }

        /// <summary>The product settings.</summary>
        protected CategoryFeedPolicy CategorySettings { get; set; }

        /// <summary>Initializes a new instance of the <see cref="ExportCategoryBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public AppendCategoryToFeedBlock(ExtendedCatalogCommander commander)
        {
            this.Commander = commander;
        }

        /// <summary>Executes the pipeline block's code logic.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Category"/>.</returns>
        public override async Task<Category> RunAsync(Category category, CommercePipelineExecutionContext context)
        {
            Condition.Requires(category).IsNotNull($"{Name}: The customer can not be null");

            Result = context.CommerceContext.GetObject<FeedResult>();
            ProblemObjects = context.CommerceContext.GetObject<ProblemObjects>();

            var exportSettings = context.CommerceContext.GetObject<FeedEntitiesArgument>();
            CategorySettings = exportSettings.CategorySettings;

            var ocCategory = await CreateCategory(context, category);
            if (ocCategory == null)
            {
                return null;
            }

            return category;
        }

        protected async Task<DiscoverCategory> CreateCategory(CommercePipelineExecutionContext context, Category category)
        {
            var friendlyIdParts = category.FriendlyId.Split("-");
            var catalogId = friendlyIdParts[0].ToValidDiscoverId();
            var categoryId = friendlyIdParts[1].ToValidDiscoverId();

            try
            {
                Result.Categories.ItemsProcessed++;

                var parentCategoryList = category.GetParentCategoriesSitecoreIds();
                if (parentCategoryList.Count == 0)
                {
                    Result.Categories.ItemsSkipped++;
                    context.Logger.LogInformation($"Category skipped; Catalog ID: {catalogId}, Category ID: {categoryId}, Parent Category Count: {parentCategoryList.Count}");

                    return null;
                }
                else if (parentCategoryList.Count > 1)
                {
                    Result.Categories.ItemsErrored++;
                    ProblemObjects.Categories.Add(category.FriendlyId);
                    context.Logger.LogInformation($"Category has too many parent category assignments; Catalog ID: {catalogId}, Category ID: {categoryId}, Parent Categories: {category.ParentCategoryList}");

                    return null;
                }

                var parentItem = await Commander.GetParentCatalogItem(parentCategoryList[0], context);
                var parentCategoryId = parentItem is Category ? parentItem.Id.CategoryNameFromCategoryId().ToValidDiscoverId() : null;

                var dCategory = new DiscoverCategory
                {
                    CCId = categoryId,
                    Id = categoryId,
                    Name = category.DisplayName,
                    Url = ConstructProductRelativeUrl(categoryId),
                    ParentCategoryId = parentCategoryId,
                    Description = category.Description
                };

                // TODO: verify: Cannot set the category ParentID at this time as the parent category may not be created?

                context.Logger.LogInformation($"Appending category; Catalog ID: {catalogId}, Category ID: {categoryId}");

                FeedService.AppendToFeedFile<Category>(CategorySettings.CategoryFeedFilePath, dCategory);

                Result.Categories.ItemsAppended++;

                return dCategory;
            }
            catch (Exception e)
            {
                Result.Categories.ItemsErrored++;
                ProblemObjects.Categories.Add(category.FriendlyId);

                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        DiscoverConstants.Errors.CreateCategoryFailed,
                        new object[]
                        {
                                    Name,
                                    categoryId,
                                    e.Message,
                                    e
                        },
                        $"{Name}: Ok| Create category '{category.FriendlyId}' failed.\n{e.Message}\n{e}").ConfigureAwait(false),
                    context);

                return null;
            }
        }

        protected virtual string ConstructProductRelativeUrl(string categoryId)
        {
            var encodedCategoryId = HttpUtility.UrlEncode(categoryId);

            return $"products/{encodedCategoryId}";
        }
    }
}