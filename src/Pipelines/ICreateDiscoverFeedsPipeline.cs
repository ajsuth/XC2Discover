// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateDiscoverFeedsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Ajsuth.Sample.Discover.Engine.Pipelines
{
    /// <summary>Defines the CreateDiscoverFeeds pipeline interface</summary>
    /// <seealso cref="IPipeline{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.CreateDiscoverFeeds)]
    public interface ICreateDiscoverFeedsPipeline : IPipeline<ExportToFeedsArgument, FeedResult, CommercePipelineExecutionContext>
    {
    }
}
