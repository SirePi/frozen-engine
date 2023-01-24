using Frozen.Audio;
using Frozen.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frozen
{
	public static class DelegatesAndEvents
	{
		public delegate void AudioStateChanged(AudioInstance instance, SoundState newState);

		public delegate void EntityComponentEvent(Entity entity, Component component);

		public delegate void EntityEvent(Entity entity);

		public delegate void LifetimeEvent(GameTime gametime);

		public delegate void SceneChanged(Scene scene);

		public delegate void SceneChanging(Scene current, Scene next);
	}
}
