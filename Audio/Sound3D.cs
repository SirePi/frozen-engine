using System;
using System.Collections.Generic;
using System.Text;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frozen.Audio
{
	class Sound3D
	{
		private AudioInstance sfx;
		private SoundEmitter emitter;
		private SoundListener listener;

		public bool IsActive { get; private set; }

		internal Sound3D()
		{ }

		public void Setup(AudioInstance sfx, SoundEmitter emitter, SoundListener listener)
		{
			this.sfx = sfx;
			this.emitter = emitter;
			this.listener = listener;

			this.IsActive = true;
			this.Update();
		}

		public void Update()
		{
			if(this.emitter != null && this.listener != null && this.IsActive)
			{
				Vector3 distance = this.emitter.Transform.WorldPosition - this.listener.Transform.WorldPosition;
				float length = distance.Length();

				// https://gamedev.stackexchange.com/q/23583
				float vrr = Vector3.Dot(this.listener.Transform.Velocity, distance) / length;
				float vsr = Vector3.Dot(this.emitter.Transform.Velocity, distance) / length;
				float pitch = (AudioSystem.SpeedOfSound + vrr) / (AudioSystem.SpeedOfSound + vsr);
				this.sfx.Pitch = pitch;

				Vector3 relativePosition = Vector3.Transform(this.emitter.Transform.WorldPosition, this.listener.Camera.View * this.listener.Camera.Projection);
				float angle = MathF.Atan2(relativePosition.Z, relativePosition.X) - MathHelper.PiOver2;
				this.sfx.Pan = -MathF.Sin(angle);

				this.sfx.Volume = MathHelper.Clamp(1 - ((length - this.listener.FullVolumeDistance) / this.listener.CutOffDistance), 0, 1) * this.emitter.Volume;

				this.IsActive = this.sfx.IsActive;
			}
		}
	}
}
