// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedSettings.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    /// <summary>Defines the feed settings model.</summary>
    /// <seealso cref="Model" />
    public class FeedSettings : Model
    {
        /// <summary>
        /// Flag to export category data to Discover feed
        /// </summary>
        public bool ProcessCategories { get; set; } = false;

        /// <summary>
        /// Flag to export product data to Discover feed
        /// </summary>
        public bool ProcessProducts { get; set; } = false;
    }
}