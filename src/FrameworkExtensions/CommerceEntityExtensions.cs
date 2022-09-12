﻿// <copyright file="CommerceEntityExtensions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Composer;
using Sitecore.Commerce.Plugin.Views;
using System.Linq;

namespace Ajsuth.Sample.Discover.Engine.FrameworkExtensions
{
    /// <summary>
    /// Defines extensions for <see cref="CommerceEntity"/>
    /// </summary>
    public static class CommerceEntityExtensions
    {
        /// <summary>
        /// Gets the composer view.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="composerViewName">The name of the composer view to retrieve.</param>
        /// <returns>A <see cref="EntityView"/></returns>
        public static EntityView GetComposerViewFromName(this CommerceEntity entity, string composerViewName)
        {
            if (string.IsNullOrEmpty(composerViewName) || entity == null || !entity.HasComponent<ComposerTemplateViewsComponent>() || !entity.HasComponent<EntityViewComponent>())
            {
                return null;
            }

            var component = entity.GetComponent<ComposerTemplateViewsComponent>();
            var itemId = component.Views.FirstOrDefault(x => x.Value == composerViewName.ToEntityId<ComposerTemplate>()).Key;

            var viewComponent = entity.GetComponent<EntityViewComponent>();
            return viewComponent.ChildViewWithItemId(itemId);
        }
    }
}
