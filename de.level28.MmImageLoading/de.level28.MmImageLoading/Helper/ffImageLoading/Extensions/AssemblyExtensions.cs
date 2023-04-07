using System;
using System.Reflection;

namespace MmImageLoading
{
    
    public static class AssemblyExtensions
    {
        public static string GetTypeAssemblyFullName(this Type type)
        {
            if (type == null)
                return null;

            var assembly = type.GetTypeInfo().Assembly;
            return assembly.FullName;
        }
    }
}
