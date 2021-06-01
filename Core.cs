using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using FrozenEngine.ECS;
using FrozenEngine.Input;
using FrozenEngine.ECS.Systems;

namespace FrozenEngine
{
	internal static class Core
	{
		internal static Dictionary<Type, PropertyInfo[]> RequiredComponentsCache { get; private set; }
		internal static Dictionary<Type, int> ComponentsUpdateOrder { get; private set; }
		internal static Game Game { get; private set; }
		internal static DrawingSystem Drawing { get; private set; }
		internal static DefaultContent DefaultContent { get; private set; }
		internal static AudioSystem Audio { get; private set; }

		public static void Initialize(Game game)
		{
			Game = game;
			DefaultContent = new DefaultContent(game.Content);

			Drawing = new DrawingSystem(game.GraphicsDevice);
			Audio = new AudioSystem();

			Type[] components = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(
					asm => asm.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)))
				).ToArray();

			// Building component cache
			RequiredComponentsCache = components.ToDictionary(
				k => k,
				v => v.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.Where(f => f.GetCustomAttribute<RequiredComponentAttribute>() != null)
					.ToArray()
			);

			// Building update order
			ComponentsUpdateOrder = components.ToDictionary(
				k => k,
				v => GetRequiredComponentsFor(v)
					.Distinct()
					.Count()
			);
		}

		private static IEnumerable<Type> GetRequiredComponentsFor(Type componentType)
		{
			foreach (PropertyInfo pi in RequiredComponentsCache[componentType])
			{
				yield return pi.PropertyType;
				foreach (Type t in GetRequiredComponentsFor(pi.PropertyType))
					yield return t;
			}
		}
	}
}
