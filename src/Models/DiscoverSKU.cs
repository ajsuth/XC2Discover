using CsvHelper.Configuration.Attributes;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    public class DiscoverSKU: DiscoverProduct
    {
        public DiscoverSKU()
        {
        }

        public DiscoverSKU(DiscoverProduct product)
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

        //[Name("sku")]
        //public string SKU { get; set; }

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

        /// <summary>
        /// int-driven boolean value for active/enabled flag
        /// </summary>
        //[Name("is_active")]
        //public int IsActive { get; set; }
    }
}
