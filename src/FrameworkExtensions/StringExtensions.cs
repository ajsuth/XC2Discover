// <copyright file="StringExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Text.RegularExpressions;

namespace Ajsuth.Sample.Discover.Engine.FrameworkExtensions
{
    /// <summary>
    /// Defines extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex NonAlphaNumericRegex = new Regex("[^\\w]", RegexOptions.Compiled);

        /// <summary>
        /// Sanitizes the identifier input to a valid Discover identifier.
        /// </summary>
        /// <param name="identifier">The identifier to be sanitized.</param>
        /// <returns>A valid Discover identifier</returns>
        public static string ToValidDiscoverId(this string identifier)
        {
            return identifier;
            // TODO: update with valid transformation rules.
            // return NonAlphaNumericRegex.Replace(identifier, "_");
        }

        public static string ToUTF8(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
        }
    }
}
