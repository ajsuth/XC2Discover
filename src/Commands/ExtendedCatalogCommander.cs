using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Threading.Tasks;

namespace Ajsuth.Sample.Discover.Engine.Commands
{
    public class ExtendedCatalogCommander : CatalogCommander
    {
        public ExtendedCatalogCommander(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public virtual async Task<CatalogItemBase> GetParentCatalogItem(string id, CommercePipelineExecutionContext context)
        {
            if (!id.IsEntityId<Catalog>())
            {
                var dataSet = await Command<GetMappingsForIdFromDbCommand>().Process(context.CommerceContext, context.CommerceContext.Environment.Name, id).ConfigureAwait(false);
                id = dataSet.Tables[0]?.Rows[0]?["EntityId"]?.ToString();
                if (id == null)
                {
                    return null;
                }
            }

            if (id.IsEntityId<SellableItem>())
            {
                return await GetEntity<SellableItem>(context.CommerceContext, id).ConfigureAwait(false);
            }
            else if (id.IsEntityId<Category>())
            {
                return await GetEntity<Category>(context.CommerceContext, id).ConfigureAwait(false);
            }
            else if (id.IsEntityId<Catalog>())
            {
                return await GetEntity<Catalog>(context.CommerceContext, id).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }
    }
}
