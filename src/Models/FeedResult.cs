// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedResult.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Ajsuth.Sample.Discover.Engine.Models
{
    /// <summary>Defines the FeedResult model.</summary>
    /// <seealso cref="Model" />
    public class FeedResult : Model
    {
        public FeedObject Catalogs { get; set; } = new FeedObject();
        public FeedObject Categories { get; set; } = new FeedObject();
        public FeedObject ProductImages { get; set; } = new FeedObject();
        public FeedObject Products { get; set; } = new FeedObject();
        public FeedObject Variants { get; set; } = new FeedObject();
    }
}