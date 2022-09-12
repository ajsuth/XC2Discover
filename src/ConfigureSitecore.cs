// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Ajsuth.Sample.Discover.Engine.Pipelines;
using Ajsuth.Sample.Discover.Engine.Pipelines.Blocks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using Sitecore.Framework.Rules;
using System.Reflection;

namespace Ajsuth.Sample.Discover.Engine
{
    /// <summary>The configure sitecore class.</summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>The configure services.</summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.RegisterAllPipelineBlocks(assembly);
            services.RegisterAllCommands(assembly);

            services.Sitecore().Rules(config => config.Registry(registry => registry.RegisterAssembly(assembly)));

            services.Sitecore().Pipelines(builder => builder

                .ConfigurePipeline<IConfigureOpsServiceApiPipeline>(pipeline => pipeline
                    .Add<Pipelines.Blocks.ConfigureOpsServiceApiBlock>()
                )

                //.AddPipeline<IExportCatalogsPipeline, ExportCatalogsPipeline>(pipeline => pipeline
                //    .Add<ValidateCatalogBlock>()
                //    //.Add<ExportCatalogBlock>()
                //)

                //.AddPipeline<IExportCatalogAssignmentsPipeline, ExportCatalogAssignmentsPipeline>(pipeline => pipeline
                //    .Add<ValidateCatalogBlock>()
                //    //.Add<ExportCatalogAssignmentsBlock>()
                //    .Add<ExportCatalogAssignmentsForHeadstartBlock>()
                //)

                .AddPipeline<IAppendCategoryToFeedPipeline, AppendCategoryToFeedPipeline>(pipeline => pipeline
                    .Add<ValidateCategoryBlock>()
                    .Add<AppendCategoryToFeedBlock>()
                )

                .AddPipeline<IAppendSellableItemToFeedPipeline, AppendSellableItemToFeedPipeline>(pipeline => pipeline
                    .Add<ValidateSellableItemBlock>()
                    .Add<AppendSellableItemToFeedBlock>()
                )

                .AddPipeline<ICreateDiscoverFeedsPipeline, CreateDiscoverFeedsPipeline>(pipeline => pipeline
                    .Add<PrepareExportBlock>()
                    .Add<PrepareAzureStorageBlock>()
                    //.Add<ExportCatalogsBlock>()
                    .Add<CreateCategoryFeedBlock>()
                    .Add<CreateProductFeedBlock>()
                    //.Add<ExportAllCatalogAssignmentsBlock>()
                )

            );
        }
    }
}
