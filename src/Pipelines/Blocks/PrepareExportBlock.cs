﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrepareExportBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Shops;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Blocks
{
    /// <summary>Defines the asynchronous executing PrepareExport pipeline block</summary>
    /// <seealso cref="AsyncPipelineBlock{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.Blocks.PrepareExport)]
    public class PrepareExportBlock : AsyncPipelineBlock<ExportToFeedsArgument, ExportToFeedsArgument, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commander.</summary>
        /// <value>The commander.</value>
        protected CommerceCommander Commander { get; set; }

        /// <summary>Initializes a new instance of the <see cref="PrepareExportBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public PrepareExportBlock(CommerceCommander commander)
        {
            this.Commander = commander;
        }

        /// <summary>Executes the pipeline block's code logic.</summary>
        /// <param name="arg">The pipeline argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="Catalog"/>.</returns>
        public override async Task<ExportToFeedsArgument> RunAsync(ExportToFeedsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");

            context.CommerceContext.AddObject(new FeedResult());
            context.CommerceContext.AddObject(new ProblemObjects());

            foreach (var siteSettings in arg.SiteSettings)
            {
                var storefront = await Commander.Command<GetShopCommand>().Process(context.CommerceContext, siteSettings.Storefront);

                if (storefront == null)
                {
                    context.Abort(
                        await context.CommerceContext.AddMessage(
                            context.GetPolicy<KnownResultCodes>().Error,
                            DiscoverConstants.Errors.StorefrontNotFound,
                            new object[]
                            {
                                Name,
                                siteSettings.Storefront
                            },
                            $"{Name}: Storefront '{siteSettings.Storefront}' not found.").ConfigureAwait(false),
                        context);

                    return null;
                }

                context.CommerceContext.AddUniqueEntity(storefront);
            }

            return await Task.FromResult(arg);
        }
    }
}