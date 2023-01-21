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
}
