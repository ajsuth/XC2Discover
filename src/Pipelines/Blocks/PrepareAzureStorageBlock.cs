// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrepareAzureStorageBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Ajsuth.Sample.Discover.Engine.Pipelines.Arguments;
using Ajsuth.Sample.Discover.Engine.Service;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Pipelines.Blocks
{
    /// <summary>Defines the asynchronous executing PrepareExport pipeline block</summary>
    /// <seealso cref="AsyncPipelineBlock{TInput, TOutput, TContext}" />
    [PipelineDisplayName(DiscoverConstants.Pipelines.Blocks.PrepareExport)]
    public class PrepareAzureStorageBlock : AsyncPipelineBlock<ExportToFeedsArgument, ExportToFeedsArgument, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commander.</summary>
        /// <value>The commander.</value>
        protected CommerceCommander Commander { get; set; }

        /// <summary>Initializes a new instance of the <see cref="PrepareExportBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public PrepareAzureStorageBlock(CommerceCommander commander)
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

            if (!arg.CloudStorageSettings.IsValid())
            {
                return await Task.FromResult(arg);
            }

            context.CommerceContext.AddObject(new CloudStorageService(arg.CloudStorageSettings));

            return await Task.FromResult(arg);
        }
    }
}