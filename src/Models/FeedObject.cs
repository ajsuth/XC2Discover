// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedObject.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ajsuth.Sample.Discover.Engine.Models
{
    /// <summary>Defines the FeedObject model.</summary>
    public class FeedObject
    {
        public long ItemsProcessed { get; set; } = 0;
        public long ItemsAppended { get; set; } = 0;
        public long ItemsSkipped { get; set; } = 0;
        public long ItemsErrored { get; set; } = 0;

    }
}