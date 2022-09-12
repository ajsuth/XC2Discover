// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverConstants.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Sample.Discover.Engine
{
    /// <summary>The Discover constants.</summary>
    public class DiscoverConstants
    {
        /// <summary>
        /// The names of the errors.
        /// </summary>
        public static class Errors
        {
            /// <summary>
            /// The create category failed error name.
            /// </summary>
            public const string CreateCategoryFailed = nameof(CreateCategoryFailed);

            /// <summary>
            /// The create product failed error name.
            /// </summary>
            public const string CreateProductFailed = nameof(CreateProductFailed);

            /// <summary>
            /// The create variants failed error name.
            /// </summary>
            public const string CreateVariantsFailed = nameof(CreateVariantsFailed);
            
            /// <summary>
            /// The export categories failed error name.
            /// </summary>
            public const string ExportCategoriesFailed = nameof(ExportCategoriesFailed);

            /// <summary>
            /// The export sellable items failed error name.
            /// </summary>
            public const string ExportSellableItemsFailed = nameof(ExportSellableItemsFailed);
            
            /// <summary>
            /// The get category failed error name.
            /// </summary>
            public const string GetCategoryFailed = nameof(GetCategoryFailed);

            /// <summary>
            /// The get product failed error name.
            /// </summary>
            public const string GetProductFailed = nameof(GetProductFailed);

            /// <summary>
            /// The storefront not found error name.
            /// </summary>
            public const string StorefrontNotFound = nameof(StorefrontNotFound);
        }

        /// <summary>
        /// The names of the lists.
        /// </summary>
        public static class Lists
        {
            /// <summary>
            /// The categories list name.
            /// </summary>
            public const string Categories = nameof(Categories);

            /// <summary>
            /// The sellable items list name.
            /// </summary>
            public const string SellableItems = nameof(SellableItems);
        }

        /// <summary>
        /// The names of the pipelines.
        /// </summary>
        public static class Pipelines
        {
            /// <summary>
            /// The append category to feed pipeline name.
            /// </summary>
            public const string AppendCategoryToFeed = "Discover.Pipeline.AppendCategoryToFeed";

            /// <summary>
            /// The append sellable item to feed pipeline name.
            /// </summary>
            public const string AppendSellableItemToFeed = "Discover.Pipeline.AppendSellableItemToFeed";
            
            /// <summary>
            /// The create discover feeds pipeline name.
            /// </summary>
            public const string CreateDiscoverFeeds = "Discover.Pipeline.CreateDiscoverFeeds";

            /// <summary>
            /// The names of the pipeline blocks.
            /// </summary>
            public static class Blocks
            {
                /// <summary>
                /// The append category to feed pipeline block name.
                /// </summary>
                public const string AppendCategoryToFeed = "Discover.Block.AppendCategoryToFeed";

                /// <summary>
                /// The append sellable item to feed pipeline block name.
                /// </summary>
                public const string AppendSellableItemToFeed = "Discover.Block.AppendSellableItemToFeed";

                /// <summary>
                /// The configure ops service api pipeline block name.
                /// </summary>
                public const string ConfigureOpsServiceApi = "Discover.Block.ConfigureOpsServiceApi";

                /// <summary>
                /// The create category feed pipeline block name.
                /// </summary>
                public const string CreateCategoryFeed = "Discover.Block.CreateCategoryFeed";

                /// <summary>
                /// The create product feed pipeline block name.
                /// </summary>
                public const string CreateProductFeed = "Discover.Block.CreateProductFeed";

                /// <summary>
                /// The prepare export pipeline block name.
                /// </summary>
                public const string PrepareExport = "Discover.Block.PrepareExport";

                /// <summary>
                /// The validate category pipeline block name.
                /// </summary>
                public const string ValidateCategory = "Discover.Block.ValidateCategory";

                /// <summary>
                /// The validate sellable item pipeline block name.
                /// </summary>
                public const string ValidateSellableItem = "Discover.Block.ValidateSellableItem";
            }
        }
    }
}