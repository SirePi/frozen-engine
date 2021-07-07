using System;
using System.Collections.Generic;
using System.Text;
using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frozen.Audio
{
	class Sound3D
	{
		private SoundEffectInstance sfx;
		private SoundEmitter emitter;
		private SoundListener listener;
		public bool IsActive { get; private set; }

		internal Sound3D()
		{ }

		public void Setup(SoundEffectInstance sfx, SoundEmitter emitter, SoundListener listener)
		{
			this.sfx = sfx;
			this.emitter = emitter;
			this.listener = listener;
			this.IsActive = true;
		}

		public void Update()
		{
			if (this.sfx.State == SoundState.Stopped)
				this.IsActive = false;
			else if(this.emitter != null && this.listener != null)
			{
				Vector3 distance = this.emitter.Transform.WorldPosition - this.listener.Transform.WorldPosition;
				Vector3 relativePosition = Vector3.Transform(this.emitter.Transform.WorldPosition, this.listener.Camera.View * this.listener.Camera.Projection);

				float angle = MathF.Atan2(relativePosition.Z, relativePosition.X) - MathHelper.PiOver2;
				this.sfx.Pan = -MathF.Sin(angle);
				this.sfx.Volume = MathHelper.Clamp(1 - ((distance.LengthSquared() - this.listener.FullVolumeDistanceSquared) / this.listener.CutOffDistanceSquared), 0, 1) * this.emitter.Volume;
			}
		}
	}
}
