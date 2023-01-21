using System;
using System.Collections.Generic;
using System.Text;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Enums;
using Microsoft.Xna.Framework;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace Frozen.Audio
{
	public class SfxInstance : AudioInstance
	{
		private PanningSampleProvider pan;

		private SmbPitchShiftingSampleProvider pitch;

		private SoundEmitter emitter;

		private SoundListener listener;

		private bool updatePitch;

		private bool updatePan;

		private bool updateVolume;

		public float Pan
		{
			get
			{
				if (this.pan != null)
					return this.pan.Pan;
				else return 0;
			}
			set
			{
				if (this.pan != null)
					this.pan.Pan = MathHelper.Clamp(value, -1, 1);
			}
		}

		public float Pitch { get => this.pitch.PitchFactor; set => this.pitch.PitchFactor = value; }

		internal SfxInstance(AudioProvider provider) : base(provider)
		{ }

		protected override ISampleProvider SetupPipeline()
		{
			this.pitch = new SmbPitchShiftingSampleProvider(this.source.SampleProvider);
			if (this.pitch.WaveFormat.Channels == 1)
			{
				this.pan = new PanningSampleProvider(this.pitch);
				return this.pan;
			}
			else
				return this.pitch;
		}

		public void Apply3D(SoundEmitter emitter, SoundListener listener, ThreeDSoundChange change = ThreeDSoundChange.Default)
		{
			this.emitter = emitter;
			this.listener = listener;
			this.updatePitch = change.HasFlag(ThreeDSoundChange.Pitch);
			this.updatePan = change.HasFlag(ThreeDSoundChange.Pan);
			this.updateVolume = change.HasFlag(ThreeDSoundChange.Volume);

			this.Update();
		}

		internal override void Update()
		{
			if (this.source.IsActive && this.emitter != null && this.listener != null)
			{
				Vector3 distance = this.emitter.Transform.WorldPosition - this.listener.Transform.WorldPosition;
				float length = distance.Length();

				if (this.updatePitch)
				{
					// https://gamedev.stackexchange.com/q/23583
					float vrr = Vector3.Dot(this.listener.Transform.Velocity, distance) / length;
					float vsr = Vector3.Dot(this.emitter.Transform.Velocity, distance) / length;
					float pitch = (AudioSystem.SpeedOfSound + vrr) / (AudioSystem.SpeedOfSound + vsr);
					this.Pitch = pitch;
				}

				if (this.updatePan)
				{
					Vector3 relativePosition = Vector3.Transform(this.emitter.Transform.WorldPosition, this.listener.Camera.View * this.listener.Camera.Projection);
					float angle = MathF.Atan2(relativePosition.Z, relativePosition.X) - MathHelper.PiOver2;
					this.Pan = -MathF.Sin(angle);
				}

				if (this.updateVolume)
				{
					this.Volume = MathHelper.Clamp(1 - ((length - this.listener.FullVolumeDistance) / this.listener.CutOffDistance), 0, 1) * this.emitter.Volume;
				}
			}
			else
			{
				this.emitter = null;
				this.listener = null;
			}
		}
	}
}
