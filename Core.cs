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
	public static class Core
	{
		internal static Dictionary<Type, PropertyInfo[]> RequiredComponentsCache { get; private set; }
		internal static Dictionary<Type, int> ComponentsUpdateOrder { get; private set; }
		internal static Game Game { get; private set; }
		internal static DrawingSystem Drawing { get; private set; }
		internal static DefaultContent DefaultContent { get; private set; }

		public static Rectangle Viewport { get; private set; }
		public static KeyboardManager Keyboard { get; private set; }
		public static IReadOnlyDictionary<PlayerIndex, GamePadManager> GamePad { get; private set; }

		public static void Initialize(Game game)
		{
			Game = game;
			DefaultContent = new DefaultContent(game.Content);

			Drawing = new DrawingSystem(game.GraphicsDevice);

			Type[] components = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(
					asm => asm.GetTypes().Where(t => t.IsSubclassOf(typeof(Component)) && !t.IsAbstract)
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

			Keyboard = new KeyboardManager();
			GamePad = Enum.GetValues(typeof(PlayerIndex)).Cast<PlayerIndex>().ToDictionary(k => k, v => new GamePadManager(v));
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
