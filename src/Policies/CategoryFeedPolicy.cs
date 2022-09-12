// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryFeedPolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Ajsuth.Sample.Discover.Engine.Policies
{
    /// <summary>Defines the category feed policy.</summary>
    /// <seealso cref="Policy" />
    public class CategoryFeedPolicy : Policy
    {
        /// <summary>
        /// Gets or sets the category feed file path.
        /// </summary>
        /// <value>
        /// The category feed file path.
        /// </value>
        public string CategoryFeedFilePath { get; set; }
    }
}
