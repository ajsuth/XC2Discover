// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateDiscoverFeedsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Ajsuth.Sample.Discover.Engine.Pipelines
{
    /// <summary>Defines the CreateDiscoverFeeds pipeline.</summary>
    /// <seealso cref="CommercePipeline{TArg, TResult}" />
    /// <seealso cref="ICreateDiscoverFeedsPipeline" />
    public class CreateDiscoverFeedsPipeline : CommercePipeline<ExportToFeedsArgument, FeedResult>, ICreateDiscoverFeedsPipeline
    {
        /// <summary>Initializes a new instance of the <see cref="CreateDiscoverFeedsPipeline" /> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public CreateDiscoverFeedsPipeline(IPipelineConfiguration<ICreateDiscoverFeedsPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

