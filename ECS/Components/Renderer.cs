using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;

namespace Frozen.ECS.Components
{
	public abstract class Renderer : Component
	{
		public static readonly int[] QUAD_INDICES = new int[] { 0, 1, 2, 1, 3, 2 };

		public abstract Rectangle Bounds { get; }

		public bool IgnoreCamera { get; set; }

		public abstract long RendererSortedHash { get; }

		[RequiredComponent]
		public Transform Transform { get; protected set; }

		protected override void OnUpdate()
		{
			base.OnUpdate();
			UpdateRenderer();
		}

		public abstract void Draw(DrawingSystem drawing);

		public abstract void UpdateRenderer();
	}
}
