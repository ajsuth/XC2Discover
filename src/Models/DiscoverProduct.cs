// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverProduct.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CsvHelper.Configuration.Attributes;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    public class DiscoverProduct
    {
        public DiscoverProduct()
        {
        }

        public DiscoverProduct(DiscoverProduct product)
        {
            Id = product.Id;
            Name = product.Name;
            Url = product.Url;
            ImageUrl = product.ImageUrl;
            Description = product.Description;
            SKU = product.SKU;
            CcIds = product.CcIds;
            Price = product.Price;
            SalePrice = product.SalePrice;
            IsActive = product.IsActive;
            ProductType = product.ProductType;
            SearchKeywords = product.SearchKeywords;
            Brand = product.Brand;
            Manufacturer = product.Manufacturer;
            StockQuantity = product.StockQuantity;
        }

        // required
        [Name("product_id")]
        public string Id { get; set; }

        // required
        [Name("name")]
        public string Name { get; set; }

        // required
        [Name("product_url")]
        public string Url { get; set; }

        [Name("image_url")]
        public string ImageUrl { get; set; }

        [Name("description")]
        public string Description { get; set; }

        [Name("sku")]
        public string SKU { get; set; }

        /// <summary>
        /// Comma-separated list of customer category identifiers
        /// </summary>
        [Name("ccids")]
        public string CcIds { get; set; }

        [Name("price")]
        public float Price { get; set; }

        [Name("sale_price")]
        public float? SalePrice { get; set; }

        [Name("is_active")]
        public bool IsActive { get; set; }

        [Name("product_type")]
        public string ProductType { get; set; }

        [Name("search_keywords")]
        public string SearchKeywords { get; set; }

        [Name("brand")]
        public string Brand { get; set; }

        // Custom product-level property
        [Name("manufacturer")]
        public string Manufacturer { get; set; }

        // SKU-level Properties

        [Name("stock_quantity")]
        public int? StockQuantity { get; set; }
        [Name("sku_name")]
        public string SkuName { get; set; }

        [Name("sku_description")]
        public string SkuDescription { get; set; }

        [Name("sku_url")]
        public string SkuUrl { get; set; }

        [Name("sku_image_url")]
        public string SkuImageUrl { get; set; }

        [Name("override_price")]
        public float? OverridePrice { get; set; }

        [Name("override_sale_price")]
        public float? OverrideSalePrice { get; set; }

        [Name("override_stock_quantity")]
        public int? OverrideStockQuantity { get; set; }

        [Name("color")]
        public string Color { get; set; }

        [Name("size")]
        public string Size { get; set; }
    }
}
