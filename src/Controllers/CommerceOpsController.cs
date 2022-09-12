// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceOpsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Ajsuth.Sample.Discover.Engine.Commands;
using Ajsuth.Sample.Discover.Engine.Models;
using Ajsuth.Sample.Discover.Engine.Policies;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Controllers
{
    /// <summary>Defines the commerce ops controller</summary>
    /// <seealso cref="CommerceODataController" />
    public class CommerceOpsController : CommerceODataController
    {
        /// <summary>Initializes a new instance of the <see cref="CommerceOpsController" /> class.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommerceOpsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Create Sitecore Discover feeds.
        /// </summary>
        /// <param name="value">The action parameters.</param>
        /// <returns>The action result.</returns>
        [HttpPost]
        [ODataRoute("CreateDiscoverFeeds", RouteName = CoreConstants.CommerceOpsApi)]
        public async Task<IActionResult> CreateDiscoverFeeds([FromBody] ODataActionParameters value)
        {
            Condition.Requires(value, nameof(value)).IsNotNull();

            if (!ModelState.IsValid || value == null)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (!value.ContainsKey("processSettings") || !(value["processSettings"] is FeedSettings processSettings))
            {
                return new BadRequestObjectResult(value);
            }

            var command = Command<CreateDiscoverFeedsCommand>();
            var result = await command.Process(
                CurrentContext,
                processSettings,
                (value["siteSettings"] as IEnumerable<SitePolicy>)?.ToList(),
                value["categorySettings"] as CategoryFeedPolicy,
                value["productSettings"] as SellableItemFeedPolicy,
                value["cloudStorageSettings"] as CloudStoragePolicy).ConfigureAwait(false);

            return new ObjectResult(result);
        }
    }
}