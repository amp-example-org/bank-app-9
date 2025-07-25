using System;
using Autofac;

namespace AcmeBankApp.Web.Helpers
{
    /// <summary>
    /// Legacy Service Locator anti-pattern - used alongside proper DI
    /// This was common in 2018 when teams were transitioning to DI but still had static dependencies
    /// </summary>
    public static class ServiceLocator
    {
        private static IContainer _container;

        public static void SetContainer(IContainer container)
        {
            _container = container;
            LogHelper.Info("Service Locator container set");
        }

        /// <summary>
        /// Legacy method to resolve services statically - anti-pattern
        /// </summary>
        public static T GetService<T>()
        {
            if (_container == null)
            {
                LogHelper.Error("Service Locator container not initialized");
                throw new InvalidOperationException("Service Locator container not initialized");
            }

            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, $"Failed to resolve service of type {typeof(T).Name}");
                throw;
            }
        }

        /// <summary>
        /// Legacy method to check if service is registered
        /// </summary>
        public static bool IsRegistered<T>()
        {
            return _container?.IsRegistered<T>() ?? false;
        }

        /// <summary>
        /// Legacy method used in static utility classes - this is why Service Locator was used
        /// </summary>
        public static T ResolveService<T>() where T : class
        {
            try
            {
                return GetService<T>();
            }
            catch
            {
                // Legacy pattern: Return null instead of throwing
                LogHelper.Warn($"Could not resolve service {typeof(T).Name}, returning null");
                return null;
            }
        }
    }
}
