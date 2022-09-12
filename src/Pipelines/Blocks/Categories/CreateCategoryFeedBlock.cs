﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateCategoryFeedBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Ajsuth.Sample.Discover.Engine.Service;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Blocks
{
    /// <summary>Defines the asynchronous executing CreateCategoryFeed pipeline block</summary>
    /// <seealso cref="AsyncPipelineBlock{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.Blocks.CreateCategoryFeed)]
    public class CreateCategoryFeedBlock : AsyncPipelineBlock<ExportToFeedsArgument, ExportToFeedsArgument, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commander.</summary>
        /// <value>The commander.</value>
        protected CommerceCommander Commander { get; set; }

        /// <summary>Initializes a new instance of the <see cref="CreateCategoryFeedBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public CreateCategoryFeedBlock(CommerceCommander commander)
        {
            this.Commander = commander;
        }

        /// <summary>Executes the pipeline block's code logic.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="ExportToFeedsArgument"/>.</returns>
        public override async Task<ExportToFeedsArgument> RunAsync(ExportToFeedsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");

            if (!arg.ProcessSettings.ProcessCategories)
            {
                context.Logger.LogInformation($"Skipping category export - not enabled.");
                return arg;
            }

            FeedService.CreateFeedFile<DiscoverCategory>(arg.CategorySettings.CategoryFeedFilePath);
            context.Logger.LogInformation($"Category feed created at '{arg.CategorySettings.CategoryFeedFilePath}'");

            long itemsProcessed = 0;

            var listName = DiscoverConstants.Lists.Categories;

            var items = await GetListIds<Category>(context, listName, int.MaxValue).ConfigureAwait(false);
            var listCount = items.List.TotalItemCount;

            context.Logger.LogInformation($"{Name}-Reviewing List:{listName}|Count:{listCount}|Environment:{context.CommerceContext.Environment.Name}");

            if (listCount == 0)
            {
                return arg;
            }

            itemsProcessed += listCount;

            foreach (var entityId in items.EntityReferences.Select(e => e.EntityId))
            {
                var error = false;

                var newContext = new CommercePipelineExecutionContextOptions(new CommerceContext(context.CommerceContext.Logger, context.CommerceContext.TelemetryClient)
                {
                    Environment = context.CommerceContext.Environment,
                    Headers = context.CommerceContext.Headers,
                },
                onError: x => error = true,
                onAbort: x =>
                {
                    if (!x.Contains("Ok|", StringComparison.OrdinalIgnoreCase))
                    {
                        error = true;
                    }
                });

                newContext.CommerceContext.AddObject(context.CommerceContext.GetObject<FeedResult>());
                newContext.CommerceContext.AddObject(context.CommerceContext.GetObject<ProblemObjects>());

                context.Logger.LogDebug($"{Name}-Exporting category: '{entityId}'. Environment: {context.CommerceContext.Environment.Name}");
                await Commander.Pipeline<AppendCategoryToFeedPipeline>()
                    .RunAsync(
                        new FeedEntitiesArgument(entityId, arg),
                        newContext)
                    .ConfigureAwait(false);

                if (error)
                {
                    context.Abort(
                        await context.CommerceContext.AddMessage(
                            context.GetPolicy<KnownResultCodes>().Error,
                            DiscoverConstants.Errors.ExportCategoriesFailed,
                            new object[] { Name },
                            $"{Name}: Export categories failed.").ConfigureAwait(false),
                        context);
                }
            }

            context.Logger.LogInformation($"{Name}-Exporting categories Completed: {(int)itemsProcessed}. Environment: {context.CommerceContext.Environment.Name}");
            return arg;
        }

        protected virtual async Task<FindEntitiesInListArgument> GetListIds<T>(CommercePipelineExecutionContext context, string listName, int take, int skip = 0)
        {
            var arg = new FindEntitiesInListArgument(typeof(T), listName, skip, take)
            {
                LoadEntities = false,
                LoadTotalItemCount = true
            };
            var result = await Commander.Pipeline<FindEntitiesInListPipeline>().RunAsync(arg, context.CommerceContext.PipelineContextOptions).ConfigureAwait(false);

            return result;
        }
    }
}