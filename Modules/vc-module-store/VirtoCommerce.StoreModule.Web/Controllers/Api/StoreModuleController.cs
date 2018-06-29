using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Web.Model;

namespace VirtoCommerce.StoreModule.Web.Controllers.Api
{
    [Route("api/stores")]
    public class StoreModuleController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IShippingMethodsService _shippingService;
        private readonly IPaymentMethodsService _paymentService;
        private readonly ITaxService _taxService;
        //private readonly ISecurityService _securityService;
        //private readonly IPermissionScopeService _permissionScopeService;
        //private readonly INotificationManager _notificationManager;

        public StoreModuleController(IStoreService storeService, IShippingMethodsService shippingService, IPaymentMethodsService paymentService, ITaxService taxService
            //                         , ISecurityService securityService, IPermissionScopeService permissionScopeService, INotificationManager notificationManager
            )
        {
            _storeService = storeService;
            _shippingService = shippingService;
            _paymentService = paymentService;
            _taxService = taxService;
            //_securityService = securityService;
            //_permissionScopeService = permissionScopeService;
            //_notificationManager = notificationManager;
        }

        /// <summary>
        /// Search stores
        /// </summary>
        [HttpPost]
        [Route("search")]
        [ProducesResponseType(typeof(StoreSearchResult), 200)]
        public async Task<StoreSearchResult> SearchStores([FromBody]StoreSearchCriteria criteria)
        {
            var retVal = new StoreSearchResult();
            //Filter resulting stores correspond to current user permissions
            //first check global permission
            //TODO
            //if (!_securityService.UserHasAnyPermission(User.Identity.Name, null, StorePredefinedPermissions.Read))
            //{
            //    //Get user 'read' permission scopes
            //    criteria.StoreIds = _securityService.GetUserPermissions(User.Identity.Name)
            //                                          .Where(x => x.Id.StartsWith(StorePredefinedPermissions.Read))
            //                                          .SelectMany(x => x.AssignedScopes)
            //                                          .OfType<StoreSelectedScope>()
            //                                          .Select(x => x.Scope)
            //                                          .ToArray();
            //    //Do not return all stores if user don't have corresponding permission
            //    if (criteria.StoreIds.IsNullOrEmpty())
            //    {
            //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
            //    }
            //}

            var result = await _storeService.SearchStoresAsync(criteria);
            retVal.TotalCount = result.TotalCount;
            retVal.Stores = result.Stores.ToArray();
            return retVal;
        }

        /// <summary>
        /// Get all stores
        /// </summary>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(Store[]), 200)]
        public async Task<IActionResult> GetStores()
        {
            var criteria = new StoreSearchCriteria
            {
                Skip = 0,
                Take = int.MaxValue
            };
            var storesSearchResult = await SearchStores(criteria);
            return Ok(storesSearchResult.Stores);
        }

        /// <summary>
        /// Get store by id
        /// </summary>
        /// <param name="id">Store id</param>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Store), 200)]
        public async Task<IActionResult> GetStoreById(string id)
        {
            var result = await _storeService.GetByIdAsync(id);
            //TODO
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Read, result);
            //result.Scopes = _permissionScopeService.GetObjectPermissionScopeStrings(result).ToArray();
            return Ok(result);
        }


        /// <summary>
        /// Create store
        /// </summary>
        /// <param name="store">Store</param>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Store), 200)]
        [Authorize(ModuleConstants.Security.Permissions.Create)]
        public async Task<IActionResult> Create([FromBody]Store store)
        {
            var retVal = await _storeService.CreateAsync(store);
            return Ok(retVal);
        }

        /// <summary>
        /// Update store
        /// </summary>
        /// <param name="store">Store</param>
        [HttpPut]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Update([FromBody]Store store)
        {
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Update, store);
            await _storeService.UpdateAsync(new[] { store });
            return Ok();
        }

        /// <summary>
        /// Delete stores
        /// </summary>
        /// <param name="ids">Ids of store that needed to delete</param>
        [HttpDelete]
        [Route("")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            //var stores = await _storeService.GetByIdsAsync(ids);
            //CheckCurrentUserHasPermissionForObjects(StorePredefinedPermissions.Delete, stores);
            await _storeService.DeleteAsync(ids);
            return Ok();
        }

        /// <summary>
        /// Send dynamic notification (contains custom list of properties) to store or administrator email 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("send/dynamicnotification")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> SendDynamicNotificationAnStoreEmail(SendDynamicNotificationRequest request)
        {
            var store = await _storeService.GetByIdAsync(request.StoreId);

            if (store == null)
                throw new InvalidOperationException(string.Concat("Store not found. StoreId: ", request.StoreId));

            if (string.IsNullOrEmpty(store.Email) && string.IsNullOrEmpty(store.AdminEmail))
                throw new InvalidOperationException(string.Concat("Both store email and admin email are empty. StoreId: ", request.StoreId));

            throw new NotImplementedException();
            //var notification = _notificationManager.GetNewNotification<StoreDynamicEmailNotification>(request.StoreId, "Store", request.Language);

            //notification.Recipient = !string.IsNullOrEmpty(store.Email) ? store.Email : store.AdminEmail;
            //notification.Sender = notification.Recipient;
            //notification.IsActive = true;
            //notification.FormType = request.Type;
            //notification.Fields = request.Fields;

            //_notificationManager.ScheduleSendNotification(notification);

            return Ok();
        }

        /// <summary>
        /// Check if given contact has login on behalf permission
        /// </summary>
        /// <param name="storeId">Store ID</param>
        /// <param name="id">Contact ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{storeId}/accounts/{id}/loginonbehalf")]
        [ProducesResponseType(typeof(LoginOnBehalfInfo), 200)]
        public IActionResult GetLoginOnBehalfInfo(string storeId, string id)
        {
            var result = new LoginOnBehalfInfo
            {
                UserName = id
            };

            //TODO
            //var user = await _securityService.FindByIdAsync(id, UserDetails.Reduced);

            //if (user != null)
            //{
            //    //TODO: Check if requested user has permission to login on behalf for given store
            //    result.CanLoginOnBehalf = _securityService.UserHasAnyPermission(user.UserName, null, StorePredefinedPermissions.LoginOnBehalf);
            //}

            return Ok(result);
        }

        /// <summary>
        /// Returns list of stores which user can sign in
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("allowed/{userId}")]
        [ProducesResponseType(typeof(Store[]), 200)]
        public IActionResult GetUserAllowedStores(string userId)
        {
            var retVal = new List<Store>();
            //TODO
            //var user = await _securityService.FindByIdAsync(userId, UserDetails.Reduced);
            //if (user != null)
            //{
            //    var storeIds = _storeService.GetUserAllowedStoreIds(user);
            //    retVal.AddRange(_storeService.GetByIds(storeIds.ToArray()));
            //}
            return Ok(retVal.ToArray());
        }

        //TODO
        //private void CheckCurrentUserHasPermissionForObjects(string permission, params coreModel.Store[] objects)
        //{
        //    //Scope bound security check
        //    var scopes = objects.SelectMany(x => _permissionScopeService.GetObjectPermissionScopeStrings(x)).Distinct().ToArray();
        //    if (!_securityService.UserHasAnyPermission(User.Identity.Name, scopes, permission))
        //    {
        //        throw new HttpResponseException(HttpStatusCode.Unauthorized);
        //    }
        //}
    }
}
