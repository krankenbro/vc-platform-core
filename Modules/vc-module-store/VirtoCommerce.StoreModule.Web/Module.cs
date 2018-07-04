using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Services;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.ChangeLog;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Security.Events;
using VirtoCommerce.StoreModule.Core;
using VirtoCommerce.StoreModule.Core.Events;
using VirtoCommerce.StoreModule.Core.Services;
using VirtoCommerce.StoreModule.Data.Handlers;
using VirtoCommerce.StoreModule.Data.Repositories;
using VirtoCommerce.StoreModule.Data.Services;
using VirtoCommerce.StoreModule.Web.JsonConverters;

namespace VirtoCommerce.StoreModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }
        public void Initialize(IServiceCollection serviceCollection)
        {
            var snapshot = serviceCollection.BuildServiceProvider();
            var configuration = snapshot.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("VirtoCommerce.Store") ?? configuration.GetConnectionString("VirtoCommerce");
            serviceCollection.AddDbContext<StoreDbContext>(options => options.UseSqlServer(connectionString));
            serviceCollection.AddTransient<IStoreRepository, StoreRepositoryImpl>();
            serviceCollection.AddSingleton<Func<IStoreRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IStoreRepository>());
            serviceCollection.AddSingleton<IStoreService, StoreServiceImpl>();
            serviceCollection.AddSingleton<IStoreSearchService, StoreSearchServiceImpl>();

            serviceCollection.AddSingleton<StoreChangedEventHandler>();

            
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            ModuleInfo.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Store|General",
                Settings = ModuleConstants.Settings.General.AllSettings.ToArray()
            });
            ModuleInfo.Settings.Add(new ModuleSettingsGroup
            {
                Name = "Store|SEO",
                Settings = ModuleConstants.Settings.SEO.AllSettings.ToArray()
            });

            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IKnownPermissionsProvider>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission()
                {
                    GroupName = "Store",
                    ModuleId = ModuleInfo.Id,
                    Name = x
                }).ToArray());

            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<StoreDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }

            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcJsonOptions>>();
            var paymentMethodsRegistrar = appBuilder.ApplicationServices.GetService<IPaymentMethodsRegistrar>();
            var shippingMethodsRegistrar = appBuilder.ApplicationServices.GetService<IShippingMethodsRegistrar>();
            var taxRegistrar = appBuilder.ApplicationServices.GetService<ITaxRegistrar>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicStoreJsonConverter(paymentMethodsRegistrar, shippingMethodsRegistrar, taxRegistrar));

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<StoreChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<StoreChangedEventHandler>().Handle(message));
        }

        public void Uninstall()
        {
        }
    }
}
