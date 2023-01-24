using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Frozen.ECS;
using Frozen.ECS.Systems;
using Frozen.Input;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Myra;

namespace Frozen
{
	public static class Engine
	{
		internal static Dictionary<Type, int> ComponentsUpdateOrder { get; private set; }
		internal static DrawingSystem Drawing { get; private set; }
		internal static Dictionary<Type, PropertyInfo[]> RequiredComponentsCache { get; private set; }
		public static AudioSystem Audio { get; private set; }
		public static ContentProvider ContentProvider { get; private set; }
		public static Game Game { get; private set; }
		public static IReadOnlyDictionary<PlayerIndex, GamePadManager> GamePad { get; private set; }
		public static KeyboardManager Keyboard { get; private set; }
		public static Log Log { get; private set; }
		public static MouseManager Mouse { get; private set; }
		public static Random Random { get; private set; }

		private static IEnumerable<Type> GetRequiredComponentsFor(Type componentType)
		{
			foreach (PropertyInfo pi in RequiredComponentsCache[componentType])
			{
				yield return pi.PropertyType;
				foreach (Type t in GetRequiredComponentsFor(pi.PropertyType))
					yield return t;
			}
		}

		internal static void Initialize(Game game)
		{
			Game = game;

			Random = new Random((int)DateTime.Now.TimeOfDay.TotalMilliseconds);
			ContentProvider = new ContentProvider(game);
			Drawing = new DrawingSystem(game.GraphicsDevice);

			Log = new Log();
			Audio = new AudioSystem();
			Keyboard = new KeyboardManager();
			Mouse = new MouseManager();
			GamePad = Enum.GetValues(typeof(PlayerIndex)).Cast<PlayerIndex>().ToDictionary(k => k, v => new GamePadManager(v));

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

			// Setting up Myra
			MyraEnvironment.Game = game;
		}

		public static void ReseedRandom(int seed)
		{
			Random = new Random(seed);
		}
	}
}
