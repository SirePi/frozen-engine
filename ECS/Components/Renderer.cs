﻿using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.ECS.Components
{
	public abstract class Renderer : Component
	{
		[RequiredComponent]
		public Transform Transform { get; protected set; }

		public bool IgnoreCamera { get; set; }
		public abstract Rectangle Bounds { get; }
		public abstract long RendererSortedHash { get; }
		public abstract void Draw(DrawingSystem drawing);
		public abstract void UpdateRenderer();
		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateRenderer();
		}
	}
}
