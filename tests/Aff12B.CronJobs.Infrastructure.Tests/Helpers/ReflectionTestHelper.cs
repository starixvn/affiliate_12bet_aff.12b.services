using System.Collections.Concurrent;
using System.Reflection;

namespace Aff12B.CronJobs.Infrastructure.Tests.Helpers
{
    /// <summary>
    /// Helper class to optimize reflection usage in tests by caching MethodInfo instances
    /// This reduces the performance overhead of repeated reflection calls
    /// </summary>
    public static class ReflectionTestHelper
    {
        private static readonly ConcurrentDictionary<string, MethodInfo> _methodCache = new();
        private static readonly ConcurrentDictionary<string, PropertyInfo> _propertyCache = new();
        private static readonly ConcurrentDictionary<string, FieldInfo> _fieldCache = new();

        /// <summary>
        /// Gets a cached protected method to avoid repeated reflection overhead
        /// </summary>
        /// <param name="type">The type containing the method</param>
        /// <param name="methodName">Name of the method</param>
        /// <returns>Cached MethodInfo instance</returns>
        public static MethodInfo GetCachedProtectedMethod(Type type, string methodName)
        {
            var key = $"{type.FullName}.{methodName}";
            return _methodCache.GetOrAdd(key, _ =>
                type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Method {methodName} not found on {type.Name}"));
        }

        /// <summary>
        /// Invokes a cached protected method asynchronously
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="instance">Instance to invoke method on</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="parameters">Method parameters</param>
        /// <returns>Task result</returns>
        public static async Task<T> InvokeCachedProtectedMethodAsync<T>(object instance, string methodName, params object[] parameters)
        {
            var method = GetCachedProtectedMethod(instance.GetType(), methodName);
            var result = method.Invoke(instance, parameters);

            if (result is Task<T> taskResult)
            {
                return await taskResult;
            }

            throw new InvalidOperationException($"Method {methodName} did not return expected type Task<{typeof(T).Name}>");
        }

        /// <summary>
        /// Gets a cached protected property to avoid repeated reflection overhead
        /// </summary>
        /// <param name="type">The type containing the property</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Cached PropertyInfo instance</returns>
        public static PropertyInfo GetCachedProtectedProperty(Type type, string propertyName)
        {
            var key = $"{type.FullName}.{propertyName}";
            return _propertyCache.GetOrAdd(key, _ =>
                type.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException($"Property {propertyName} not found on {type.Name}"));
        }

        /// <summary>
        /// Gets a cached protected field to avoid repeated reflection overhead
        /// </summary>
        /// <param name="type">The type containing the field</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Cached FieldInfo instance</returns>
        public static FieldInfo GetCachedProtectedField(Type type, string fieldName)
        {
            var key = $"{type.FullName}.{fieldName}";
            return _fieldCache.GetOrAdd(key, _ =>
                type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Field {fieldName} not found on {type.Name}"));
        }

        /// <summary>
        /// Clears all caches (useful for testing or memory cleanup)
        /// </summary>
        public static void ClearCache()
        {
            _methodCache.Clear();
            _propertyCache.Clear();
            _fieldCache.Clear();
        }
    }
}
