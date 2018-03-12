using System.Collections.Generic;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    /// <summary>
    /// Represents one outline element: catalog, category or product.
    /// </summary>
    public class OutlineItem : Entity, ISeoSupport
    {
        #region ISeoSupport Members
     
        /// <summary>
        /// Object type
        /// </summary>
        public string SeoObjectType { get; set; }

        /// <summary>
        /// All SEO records for the object
        /// </summary>
        public IList<SeoInfo> SeoInfos { get; set; }

        #endregion

        /// <summary>
        /// True when this object is linked to the virtual parent.
        /// </summary>
        public bool HasVirtualParent { get; set; }

        public override string ToString()
        {
            return (HasVirtualParent ? "*" : "") + Id;
        }
    }
}
