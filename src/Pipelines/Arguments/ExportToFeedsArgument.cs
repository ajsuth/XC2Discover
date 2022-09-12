// <copyright file="ExportToFeedsArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using System.Collections.Generic;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Arguments
{
    /// <summary>Defines the ExportToFeeds pipeline argument.</summary>
    /// <seealso cref="PipelineArgument" />
    public class ExportToFeedsArgument : PipelineArgument
    {
        public ExportToFeedsArgument(
            FeedSettings processSettings,
            List<SitePolicy> siteSettings,
            CategoryFeedPolicy categorySettings,
            SellableItemFeedPolicy productSettings,
            CloudStoragePolicy cloudStorageSettings)
        {
            Condition.Requires(processSettings, nameof(processSettings)).IsNotNull();

            ProcessSettings = processSettings;
            SiteSettings = siteSettings ?? new List<SitePolicy>();
            CategorySettings = categorySettings;
            ProductSettings = productSettings;
            CloudStorageSettings = cloudStorageSettings;
        }

        /// <summary>
        /// The process settings
        /// </summary>
        public FeedSettings ProcessSettings { get; set; }

        /// <summary>
        /// The site settings
        /// </summary>
        public List<SitePolicy> SiteSettings { get; set; }

        /// <summary>
        /// The category settings
        /// </summary>
        public CategoryFeedPolicy CategorySettings { get; set; }

        /// <summary>
        /// The product settings
        /// </summary>
        public SellableItemFeedPolicy ProductSettings { get; set; }

        /// <summary>
        /// The cloud storage settings
        /// </summary>
        public CloudStoragePolicy CloudStorageSettings { get; set; }
    }
}
