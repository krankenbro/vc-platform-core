using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Model
{
    /// <summary>
    /// Used for mark types which can be inherited form parent object (Product -> Variations  example)
    /// </summary>
    public interface IInheritable
    {
        bool IsInherited { get; }
        void TryInheritFrom(IEntity parent);
    }
}