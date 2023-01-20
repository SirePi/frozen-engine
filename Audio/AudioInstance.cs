using System;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public abstract class AudioInstance : ISampleProvider
	{
		protected readonly AudioProvider source;

		private readonly VolumeSampleProvider volume;

		public event Action<AudioInstance, SoundState> OnStateChanged;

		public float Volume { get => this.volume.Volume; set => this.volume.Volume = MathF.Max(value, 0); }

		public TimeSpan TimeLeft => this.source.TimeLeft;

		private SoundState state;

		public SoundState State
		{
			get => this.state;
			private set
			{
				if (this.state != value)
				{
					this.state = value;
					this.OnStateChanged?.Invoke(this, this.state);
				}
			}
		}

		public WaveFormat WaveFormat => this.volume?.WaveFormat;

		internal AudioInstance(AudioProvider provider)
		{
			this.state = SoundState.Stopped;
			this.source = provider;
			this.volume = new VolumeSampleProvider(this.SetupPipeline());
		}

		protected abstract ISampleProvider SetupPipeline();

		internal abstract void Update();

		public void Play()
		{
			this.source.Reset();
			this.State = SoundState.Playing;
		}

		public void Pause()
		{
			if (this.State == SoundState.Playing)
				this.State = SoundState.Paused;
		}

		public void Resume()
		{
			if (this.State == SoundState.Paused)
				this.State = SoundState.Playing;
		}

		public void Stop()
		{
			this.State = SoundState.Stopped;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			switch (this.State)
			{
				case SoundState.Playing:
					int read = this.volume.Read(buffer, offset, count);
					if (!this.source.IsActive)
						this.Stop();

					return read;

				case SoundState.Paused:
					return SilenceGenerator.Instance.Read(buffer, offset, count);

				default:
					return 0;
			}
		}
	}

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

	public class BgmInstance : AudioInstance
	{
		private float targetVolume = 0;

		private float volumeDelta = 0;

		private float volumeLimit = 0;

		internal BgmInstance(AudioProvider provider) : base(provider)
		{ }

		protected override ISampleProvider SetupPipeline()
		{
			return this.source.SampleProvider;
		}

		public void FadeIn(float fadeSeconds)
		{
			this.FadeTo(1, fadeSeconds);
		}

		public void FadeOut(float fadeSeconds)
		{
			this.FadeTo(0, fadeSeconds);
		}

		public void FadeTo(float targetVolume, float fadeSeconds)
		{
			if (fadeSeconds == 0)
			{
				this.Volume = targetVolume;
			}
			else
			{
				this.targetVolume = targetVolume;
				this.volumeDelta = (targetVolume - this.Volume) / fadeSeconds;
				this.volumeLimit = targetVolume + (this.volumeDelta * 2);   // just to be sure
			}
		}

		internal override void Update()
		{
			if (this.volumeDelta != 0)
			{
				float delta = this.volumeDelta * Time.FrameSeconds;
				this.Volume += delta;

				if (FrozenMath.IsBetween(this.Volume, this.targetVolume, this.volumeLimit))
				{
					this.Volume = this.targetVolume;
					this.volumeDelta = 0;
					this.volumeLimit = 0;
				}
			}
		}
	}
}
