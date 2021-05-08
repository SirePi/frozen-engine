using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public class AudioListener : Component
	{
		[RequiredComponent]
		private Transform Transform { get; set; }
		[RequiredComponent]
		private SpriteRenderer Renderer { get; set; }
	}
}
