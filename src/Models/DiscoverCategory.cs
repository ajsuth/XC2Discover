// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverCategory.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using CsvHelper.Configuration.Attributes;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    public class DiscoverCategory
    {
        // Confirm property name
        [Name("id")]
        public string Id { get; set; }

        // required
        [Name("ccid")]
        public string CCId { get; set; }

        // required
        [Name("name")]
        public string Name { get; set; }

        // required
        [Name("url_path")]
        public string Url { get; set; }

        [Name("desc")]
        public string Description { get; set; }

        [Name("parent_ccid")]
        public string ParentCategoryId { get; set; }
        
    }
}
