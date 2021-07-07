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

		private float fullVolumeDistance;
		private float cutOffDistance;

		public float CutOffDistance
		{
			get => this.cutOffDistance;
			set
			{
				this.cutOffDistance = value;
				this.CutOffDistanceSquared = value * value;
			}
		}

		public float FullVolumeDistance
		{
			get => this.fullVolumeDistance;
			set
			{
				this.fullVolumeDistance = value;
				this.FullVolumeDistanceSquared = value * value;
			}
		}

		internal float CutOffDistanceSquared { get; private set; }
		internal float FullVolumeDistanceSquared { get; private set; }

		public SoundListener()
		{
			this.CutOffDistance = 100000;
			this.FullVolumeDistance = 1000;
		}
	}
}
