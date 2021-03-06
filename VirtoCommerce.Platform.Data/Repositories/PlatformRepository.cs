using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Assets;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Model;

namespace VirtoCommerce.Platform.Data.Repositories
{
    public class PlatformRepository : DbContextRepositoryBase<PlatformDbContext>, IPlatformRepository
    {
        public PlatformRepository(PlatformDbContext dbContext)
            : base(dbContext)
        {
        }

        #region IPlatformRepository Members
        public virtual IQueryable<SettingEntity> Settings { get { return DbContext.Set<SettingEntity>(); } }

        public virtual IQueryable<DynamicPropertyEntity> DynamicProperties { get { return DbContext.Set<DynamicPropertyEntity>(); } }
        public virtual IQueryable<DynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get { return DbContext.Set<DynamicPropertyObjectValueEntity>(); } }
        public virtual IQueryable<DynamicPropertyDictionaryItemEntity> DynamicPropertyDictionaryItems { get { return DbContext.Set<DynamicPropertyDictionaryItemEntity>(); } }


        public virtual IQueryable<OperationLogEntity> OperationLogs { get { return DbContext.Set<OperationLogEntity>(); } }



        public virtual async Task<DynamicPropertyEntity[]> GetObjectDynamicPropertiesAsync(string[] objectTypeNames, string[] objectIds)
        {
            var properties = DynamicProperties.Include(x => x.DisplayNames)
                                              .OrderBy(x => x.Name)
                                              .Where(x => objectTypeNames.Contains(x.ObjectType)).ToArray();

            var propertyIds = properties.Select(x => x.Id).ToArray();
            var proprValues = await DynamicPropertyObjectValues.Include(x => x.DictionaryItem.DisplayNames)
                                                         .Where(x => propertyIds.Contains(x.PropertyId) && objectIds.Contains(x.ObjectId))
                                                         .ToArrayAsync();

            return properties;
        }

        public virtual async Task<DynamicPropertyDictionaryItemEntity[]> GetDynamicPropertyDictionaryItemByIdsAsync(string[] ids)
        {
            var retVal = await DynamicPropertyDictionaryItems.Include(x => x.DisplayNames)
                                     .Where(x => ids.Contains(x.Id))
                                     .ToArrayAsync();
            return retVal;
        }

        public virtual async Task<DynamicPropertyEntity[]> GetDynamicPropertiesByIdsAsync(string[] ids)
        {
            var retVal = await DynamicProperties.Include(x => x.DisplayNames)
                                          .Where(x => ids.Contains(x.Id))
                                          .OrderBy(x => x.Name)
                                          .ToArrayAsync();
            return retVal;
        }

        public virtual async Task<SettingEntity[]> GetObjectSettingsAsync(string objectType, string objectId)
        {
            var result = await Settings.Include(x => x.SettingValues)
                                 .Where(x => x.ObjectId == objectId && x.ObjectType == objectType)
                                 .OrderBy(x => x.Name)
                                 .ToArrayAsync();
            return result;
        }

        public IQueryable<AssetEntryEntity> AssetEntries => DbContext.Set<AssetEntryEntity>();

        public async Task<AssetEntryEntity[]> GetAssetsByIdsAsync(IEnumerable<string> ids)
        {
            return await AssetEntries.Where(x => ids.Contains(x.Id)).ToArrayAsync();
        }


        #endregion

    }
}
