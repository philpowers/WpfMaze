namespace MazeWpf.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AssemblyExtension
    {
        public static Assembly AssemblyFromType<T>()
        {
            return typeof(T).GetTypeInfo().Assembly;
        }

        public static Assembly AssemblyFromType(Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static IEnumerable<Type> GetConcreteTypesImplementingType(this Assembly assembly, Type type)
        {
            return assembly
                .GetTypes()
                .Where(t => IsConcreteType(t) && type.IsAssignableFrom(t));
        }

        public static IEnumerable<Type> GetConcreteTypesImplementing<T>(this Assembly assembly)
        {
            return GetConcreteTypesImplementingType(assembly, typeof(T));
        }

        public static IEnumerable<Type> GetConcreteTypesImplementingOpenGenericType(this Assembly assembly, Type openGenericType)
        {
            return assembly.GetTypes()
                .SelectMany(x => x.GetInterfaces(), (x, z) => new {x, z})
                .Select(t => new {t, y = t.x.BaseType})
                .Where(t =>
                    (t.y != null && t.y.IsGenericType &&
                     openGenericType.IsAssignableFrom(t.y.GetGenericTypeDefinition())) ||
                    (t.t.z.IsGenericType && openGenericType.IsAssignableFrom(t.t.z.GetGenericTypeDefinition())))
                .Where(t => !t.t.x.IsAbstract)
                .Select(t => t.t.x);
        }

        private static bool IsConcreteType(Type t)
        {
            var ti = t.GetTypeInfo();
            return ti.IsClass && !ti.IsAbstract && !ti.IsInterface;
        }
    }
}
