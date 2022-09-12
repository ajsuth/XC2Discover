// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateDiscoverFeedsCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Ajsuth.Sample.Discover.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Commands
{
    /// <summary>Defines the CreateDiscoverFeeds command.</summary>
    public class CreateDiscoverFeedsCommand : CommerceCommand
    {
        /// <summary>Gets or sets the commander.</summary>
        /// <value>The commander.</value>
        protected CommerceCommander Commander { get; set; }

        /// <summary>Initializes a new instance of the <see cref="CreateDiscoverFeedsCommand" /> class.</summary>
        /// <param name="commander">The <see cref="CommerceCommander"/>.</param>
        /// <param name="serviceProvider">The service provider</param>
        public CreateDiscoverFeedsCommand(CommerceCommander commander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Commander = commander;
        }

        /// <summary>The process of the command.</summary>
        /// <param name="commerceContext">The commerce context.</param>
        /// <param name="exportSettings">The export settings.</param>
        /// <param name="siteSettings">The list of site settings.</param>
        /// <param name="categorySettings">The category settings.</param>
        /// <param name="productSettings">The product settings.</param>
        /// <param name="cloudStorageSettings">The azure cloud storage settings.</param>
        /// <returns>The <see cref="FeedResult"/>.</returns>
        public virtual async Task<FeedResult> Process(
            CommerceContext commerceContext,
            FeedSettings exportSettings,
            List<SitePolicy> siteSettings,
            CategoryFeedPolicy categorySettings,
            SellableItemFeedPolicy productSettings,
            CloudStoragePolicy cloudStorageSettings)
        {
            var context = commerceContext.CreatePartialClone();
            using (var activity = CommandActivity.Start(context, this))
            {
                var arg = new ExportToFeedsArgument(exportSettings, siteSettings, categorySettings, productSettings, cloudStorageSettings);
                await Commander.Pipeline<ICreateDiscoverFeedsPipeline>().RunAsync(arg, context.PipelineContextOptions).ConfigureAwait(false);
                
                return context.GetObject<FeedResult>();
            }
        }
    }
}