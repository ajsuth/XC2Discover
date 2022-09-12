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
            /// The create admin address failed error name.
            /// </summary>
            public const string CreateAdminAddressFailed = nameof(CreateAdminAddressFailed);

            /// <summary>
            /// The create api client failed error name.
            /// </summary>
            public const string CreateApiClientFailed = nameof(CreateApiClientFailed);
            
            /// <summary>
            /// The create buyer failed error name.
            /// </summary>
            public const string CreateBuyerFailed = nameof(CreateBuyerFailed);

            /// <summary>
            /// The create catalog failed error name.
            /// </summary>
            public const string CreateCatalogFailed = nameof(CreateCatalogFailed);

            /// <summary>
            /// The create catalog product assignment failed error name.
            /// </summary>
            public const string CreateCatalogProductAssignmentFailed = nameof(CreateCatalogProductAssignmentFailed);

            /// <summary>
            /// The create category failed error name.
            /// </summary>
            public const string CreateCategoryFailed = nameof(CreateCategoryFailed);

            /// <summary>
            /// The create category product assignment failed error name.
            /// </summary>
            public const string CreateCategoryProductAssignmentFailed = nameof(CreateCategoryProductAssignmentFailed);
            
            /// <summary>
            /// The create product failed error name.
            /// </summary>
            public const string CreateProductFailed = nameof(CreateProductFailed);

            /// <summary>
            /// The create variants failed error name.
            /// </summary>
            public const string CreateVariantsFailed = nameof(CreateVariantsFailed);
            
            /// <summary>
            /// The export all catalog assignments failed error name.
            /// </summary>
            public const string ExportAllCatalogAssignmentsFailed = nameof(ExportAllCatalogAssignmentsFailed);

            /// <summary>
            /// The export all category assignments failed error name.
            /// </summary>
            public const string ExportAllCategoryAssignmentsFailed = nameof(ExportAllCategoryAssignmentsFailed);

            /// <summary>
            /// The export buyers failed error name.
            /// </summary>
            public const string ExportBuyersFailed = nameof(ExportBuyersFailed);

            /// <summary>
            /// The export buyers extended failed error name.
            /// </summary>
            public const string ExportBuyersExtendedFailed = nameof(ExportBuyersExtendedFailed);

            /// <summary>
            /// The export catalogs failed error name.
            /// </summary>
            public const string ExportCatalogsFailed = nameof(ExportCatalogsFailed);

            /// <summary>
            /// The export categories failed error name.
            /// </summary>
            public const string ExportCategoriesFailed = nameof(ExportCategoriesFailed);

            /// <summary>
            /// The export customers failed error name.
            /// </summary>
            public const string ExportCustomersFailed = nameof(ExportCustomersFailed);

            /// <summary>
            /// The export sellable items failed error name.
            /// </summary>
            public const string ExportSellableItemsFailed = nameof(ExportSellableItemsFailed);
            
            /// <summary>
            /// The get admin address failed error name.
            /// </summary>
            public const string GetAdminAddressFailed = nameof(GetAdminAddressFailed);

            /// <summary>
            /// The get buyer failed error name.
            /// </summary>
            public const string GetBuyerFailed = nameof(GetBuyerFailed);

            /// <summary>
            /// The get catalog failed error name.
            /// </summary>
            public const string GetCatalogFailed = nameof(GetCatalogFailed);

            /// <summary>
            /// The get category failed error name.
            /// </summary>
            public const string GetCategoryFailed = nameof(GetCategoryFailed);

            /// <summary>
            /// The get product failed error name.
            /// </summary>
            public const string GetProductFailed = nameof(GetProductFailed);

            /// <summary>
            /// The invalid OrderCloud client policy error name.
            /// </summary>
            public const string InvalidOrderCloudClientPolicy = nameof(InvalidOrderCloudClientPolicy);

            /// <summary>
            /// The storefront not found error name.
            /// </summary>
            public const string StorefrontNotFound = nameof(StorefrontNotFound);

            /// <summary>
            /// The update buyer user failed error name.
            /// </summary>
            public const string UpdateBuyerUserFailed = nameof(UpdateBuyerUserFailed);

            /// <summary>
            /// The update category parent id failed error name.
            /// </summary>
            public const string UpdateCategoryParentIdFailed = nameof(UpdateCategoryParentIdFailed);
        }

        /// <summary>
        /// The names of the lists.
        /// </summary>
        public static class Lists
        {
            /// <summary>
            /// The catalogs list name.
            /// </summary>
            public const string Catalogs = nameof(Catalogs);

            /// <summary>
            /// The categories list name.
            /// </summary>
            public const string Categories = nameof(Categories);

            /// <summary>
            /// The customers list name.
            /// </summary>
            public const string Customers = nameof(Customers);

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
            /// The export catalogs pipeline name.
            /// </summary>
            public const string ExportCatalogs = "Discover.Pipeline.ExportCatalogs";

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
                /// The export catalogs pipeline block name.
                /// </summary>
                public const string ExportCatalogs = "Discover.Block.ExportCatalogs";

                /// <summary>
                /// The export catalog pipeline block name.
                /// </summary>
                public const string ExportCatalog = "Discover.Block.ExportCatalog";

                /// <summary>
                /// The prepare export pipeline block name.
                /// </summary>
                public const string PrepareExport = "Discover.Block.PrepareExport";

                /// <summary>
                /// The validate catalog pipeline block name.
                /// </summary>
                public const string ValidateCatalog = "Discover.Block.ValidateCatalog";

                /// <summary>
                /// The validate category pipeline block name.
                /// </summary>
                public const string ValidateCategory = "Discover.Block.ValidateCategory";

                /// <summary>
                /// The validate domain pipeline block name.
                /// </summary>
                public const string ValidateDomain = "Discover.Block.ValidateDomain";

                /// <summary>
                /// The validate sellable item pipeline block name.
                /// </summary>
                public const string ValidateSellableItem = "Discover.Block.ValidateSellableItem";

                /// <summary>
                /// The validate storefront pipeline block name.
                /// </summary>
                public const string ValidateStorefront = "Discover.Block.ValidateStorefront";
            }
        }
    }
}