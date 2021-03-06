using System;
using VirtoCommerce.Platform.Core.Exceptions;

namespace VirtoCommerce.CatalogModule.Core.Exceptions
{
    public class CatalogModuleException : PlatformException
    {
        public CatalogModuleException(string message) : base(message)
        {
        }

        public CatalogModuleException(string message, Exception innerException)
           : base(message, innerException)
        {
        }
    }
}
