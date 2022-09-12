using CsvHelper.Configuration.Attributes;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    public class DiscoverProduct
    {
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

        /// <summary>
        /// Comma-separated list of customer category identifiers
        /// </summary>
        [Name("price")]
        public float Price { get; set; }

        [Name("sale_price")]
        public float? SalePrice { get; set; }

        /// <summary>
        /// int-driven boolean value for active/enabled flag
        /// </summary>
        [Name("is_active")]
        public int IsActive { get; set; }

        [Name("product_type")]
        public string ProductType { get; set; }

        [Name("search_keywords")]
        public string SearchKeywords { get; set; }

        [Name("brand")]
        public string Brand { get; set; }

        // Custom property
        [Name("manufacturer")]
        public string Manufacturer { get; set; }

        [Name("inventory")]
        public int? Inventory { get; set; }
    }
}
