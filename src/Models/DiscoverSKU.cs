using CsvHelper.Configuration.Attributes;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    public class DiscoverSKU: DiscoverProduct
    {
        //[Name("sku")]
        //public string SKU { get; set; }

        [Name("sku_description")]
        public string SkuDescription { get; set; }

        [Name("sku_url")]
        public string SkuUrl { get; set; }

        [Name("sku_image_url")]
        public string SkuImageUrl { get; set; }

        [Name("override_price")]
        public float OverridePrice { get; set; }

        [Name("override_sale_price")]
        public float? OverrideSalePrice { get; set; }

        [Name("override_stock_quantity")]
        public int OverrideStockQuantity { get; set; }

        /// <summary>
        /// int-driven boolean value for active/enabled flag
        /// </summary>
        //[Name("is_active")]
        //public int IsActive { get; set; }
    }
}
