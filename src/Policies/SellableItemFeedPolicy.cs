// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SellableItemFeedPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Ajsuth.Sample.Discover.Engine.Policies
{
    /// <summary>Defines the sellable item feed policy.</summary>
    /// <seealso cref="Policy" />
    public class SellableItemFeedPolicy : Policy
    {
        /// <summary>
        /// Gets or sets the product feed file path.
        /// </summary>
        /// <value>
        /// The product feed file path.
        /// </value>
        public string ProductFeedFilePath { get; set; }

        /// <summary>
        /// Gets or sets the include standalone products flag.
        /// </summary>
        /// <value>
        /// The include standalone products flag.
        /// </value>
        public bool IncludeStandaloneProducts { get; set; } = false;

        /// <summary>
        /// Gets or sets the include product with variants flag.
        /// </summary>
        /// <value>
        /// The include product with variants flag.
        /// </value>
        public bool IncludeProductsWithVariants { get; set; } = false;

        /// <summary>
        /// Gets or sets the inventory set identifier used for single inventory migrations.
        /// </summary>
        /// <value>
        /// The inventory set identifier.
        /// </value>
        public string InventorySetId { get; set; }

        /// <summary>
        /// Gets or sets the default currency for assigning DefaultPriceSchedules to products.
        /// </summary>
        /// <value>
        /// The default currency.
        /// </value>
        public string DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the include images flag to products.
        /// </summary>
        /// <value>
        /// The default currency.
        /// </value>
        public bool IncludeImages { get; set; } = false;

        /// <summary>
        /// Gets or sets the include images flag to products.
        /// </summary>
        /// <value>
        /// The default currency.
        /// </value>
        public bool UploadImages { get; set; } = false;
    }
}
