using System;
using System.Reflection;

namespace MmImageLoading.MAUI.Helpers
{
	internal static class Dependency
	{
		public static void Register(Type type, Type renderer)
		{
			var assembly = typeof(Image).GetTypeInfo().Assembly;

			var types = assembly.GetTypes().ToList();
			var selected = types.Where(x => x.FullName.Contains("Registrar"));
			var registrarType = types.FirstOrDefault(x => x.FullName == "Microsoft.Maui.Controls.Internals.Registrar");

			var registrarProperty = registrarType.GetRuntimeProperty("Registered");

			var registrar = registrarProperty.GetValue(registrarType, null);
			var registerMethod = registrar.GetType().GetRuntimeMethod("Register", new[] { typeof(Type), typeof(Type) });
			registerMethod.Invoke(registrar, new[] { type, renderer });
		}
	}
}
