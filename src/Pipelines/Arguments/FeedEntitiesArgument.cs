// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedEntitiesArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Arguments
{
    /// <summary>Defines the FeedEntities pipeline argument.</summary>
    /// <seealso cref="PipelineArgument" />
    public class FeedEntitiesArgument : ExportToFeedsArgument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedEntitiesArgument"/> class.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        public FeedEntitiesArgument(string entityId, ExportToFeedsArgument exportArgument)
            : base(exportArgument?.ProcessSettings, exportArgument?.SiteSettings, exportArgument?.CategorySettings, exportArgument?.ProductSettings, exportArgument?.CloudStorageSettings)
        {
            Condition.Requires(entityId, nameof(entityId)).IsNotNull();

            EntityId = entityId;
        }

        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public string EntityId { get; set; }
    }
}