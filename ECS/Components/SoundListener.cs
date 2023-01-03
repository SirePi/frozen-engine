using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frozen.ECS.Components
{
	public class SoundListener : Component
	{
		[RequiredComponent]
		internal Transform Transform { get; set; }
		[RequiredComponent]
		internal Camera Camera { get; set; }

		public float CutOffDistance { get; set; } = 100000;
		public float FullVolumeDistance { get; set; } = 1000;
	}
}
