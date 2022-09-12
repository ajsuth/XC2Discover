// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppendCategoryToFeedPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;

namespace Ajsuth.Sample.Discover.Engine.Pipelines
{
    /// <summary>Defines the ExportCategories pipeline.</summary>
    /// <seealso cref="CommercePipeline{TArg, TResult}" />
    /// <seealso cref="IAppendCategoryToFeedPipeline" />
    public class AppendCategoryToFeedPipeline : CommercePipeline<FeedEntitiesArgument, Category>, IAppendCategoryToFeedPipeline
    {
        /// <summary>Initializes a new instance of the <see cref="AppendCategoryToFeedPipeline" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AppendCategoryToFeedPipeline(IPipelineConfiguration<IAppendCategoryToFeedPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

