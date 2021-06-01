using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace FrozenEngine.ECS.Components
{
	public class SoundEmitter : Component
	{
		[RequiredComponent]
		internal Transform Transform { get; set; }

		public float Volume { get; set; } = 1;
	}
}
