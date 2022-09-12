// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloudStoragePolicy.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;

namespace Ajsuth.Sample.Discover.Engine.Policies
{
    /// <summary>Defines the CloudStorage policy.</summary>
    /// <seealso cref="Policy" />
    public class CloudStoragePolicy : Policy
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public string Container { get; set; }

        /// <summary>
        /// Returns true if all properties have values.
        /// </summary>
        /// <returns>Returns true if all properties have values.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ConnectionString)
                && !string.IsNullOrEmpty(BaseUrl)
                && !string.IsNullOrEmpty(Container);
        }
    }
}
