// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppendCategoryToFeedPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Framework.Pipelines;

namespace Ajsuth.Sample.Discover.Engine.Pipelines
{
    /// <summary>Defines the AppendCategoryToFeed pipeline interface</summary>
    /// <seealso cref="IPipeline{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.AppendCategoryToFeed)]
    public interface IAppendCategoryToFeedPipeline : IPipeline<FeedEntitiesArgument, Category, CommercePipelineExecutionContext>
    {
    }
}
