// <copyright file="CatalogItemBaseExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajsuth.Sample.Discover.Engine.FrameworkExtensions
{
    /// <summary>
    /// Defines extensions for <see cref="CatalogItemBase"/>
    /// </summary>
    public static class CatalogItemBaseExtensions
    {
        public static List<string> GetParentCategoriesSitecoreIds(this CatalogItemBase category)
        {
            return category.ParentCategoryList?.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
        }
    }
}
