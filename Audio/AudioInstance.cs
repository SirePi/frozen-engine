using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public class AudioInstance : ISampleProvider
	{
		private readonly AudioProvider source;

		private readonly PanningSampleProvider pan;
		private readonly SmbPitchShiftingSampleProvider pitch;
		private readonly VolumeSampleProvider volume;

		public event Action<AudioInstance, SoundState> OnStateChanged;

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
					this.pan.Pan = value;
			}
		}

		public float Pitch { get => this.pitch.PitchFactor; set => this.pitch.PitchFactor = value; }
		public float Volume { get => this.volume.Volume; set => this.volume.Volume = value; }
		public bool IsActive => this.source.IsActive;

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
			ISampleProvider src = this.source.SampleProvider;

			this.pitch = new SmbPitchShiftingSampleProvider(src);
			if (this.pitch.WaveFormat.Channels == 1)
			{
				this.pan = new PanningSampleProvider(this.pitch);
				this.volume = new VolumeSampleProvider(this.pan);
			}
			else
				this.volume = new VolumeSampleProvider(this.pitch);
		}

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
			if (this.State == SoundState.Playing)
			{
				int read = this.volume.Read(buffer, offset, count);
				if (!this.source.IsActive)
					this.Stop();

				return read;
			}
			else
				return 0;
		}
	}
}
