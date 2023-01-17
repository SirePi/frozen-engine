using System;
using System.Collections.Generic;
using System.Text;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Enums;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frozen.Audio
{
	class ThreeDSound
	{
		private AudioInstance sfx;
		private SoundEmitter emitter;
		private SoundListener listener;
		private bool updatePitch;
		private bool updatePan;
		private bool updateVolume;

		public bool IsActive { get; private set; }

		internal ThreeDSound()
		{ }

		public void Setup(AudioInstance sfx, SoundEmitter emitter, SoundListener listener, ThreeDSoundChange type)
		{
			this.sfx = sfx;
			this.emitter = emitter;
			this.listener = listener;
			this.updatePitch = type.HasFlag(ThreeDSoundChange.Pitch);
			this.updatePan = type.HasFlag(ThreeDSoundChange.Pan);
			this.updateVolume = type.HasFlag(ThreeDSoundChange.Volume);

			this.IsActive = true;
			this.Update();
		}

		public void Update()
		{
			if(this.IsActive)
			{
				Vector3 distance = this.emitter.Transform.WorldPosition - this.listener.Transform.WorldPosition;
				float length = distance.Length();

				if (this.updatePitch)
				{
					// https://gamedev.stackexchange.com/q/23583
					float vrr = Vector3.Dot(this.listener.Transform.Velocity, distance) / length;
					float vsr = Vector3.Dot(this.emitter.Transform.Velocity, distance) / length;
					float pitch = (AudioSystem.SpeedOfSound + vrr) / (AudioSystem.SpeedOfSound + vsr);
					this.sfx.Pitch = pitch;
				}

				if (this.updatePan)
				{
					Vector3 relativePosition = Vector3.Transform(this.emitter.Transform.WorldPosition, this.listener.Camera.View * this.listener.Camera.Projection);
					float angle = MathF.Atan2(relativePosition.Z, relativePosition.X) - MathHelper.PiOver2;
					this.sfx.Pan = -MathF.Sin(angle);
				}

				if (this.updateVolume)
				{
					this.sfx.Volume = MathHelper.Clamp(1 - ((length - this.listener.FullVolumeDistance) / this.listener.CutOffDistance), 0, 1) * this.emitter.Volume;
				}

				this.IsActive = this.sfx.IsActive;
			}
		}
	}
}
