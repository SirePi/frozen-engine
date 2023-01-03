using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public class AudioInstance
	{
		private readonly WaveStream source;

		private readonly PanningSampleProvider pan;
		private readonly SmbPitchShiftingSampleProvider pitch;
		private readonly VolumeSampleProvider volume;
		private readonly IWaveProvider pcm16;

		private byte[] buffer = new byte[4096];

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
		public bool IsActive => this.source.CurrentTime < this.source.TotalTime;

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

		internal AudioInstance(WaveStream source)
		{
			this.source = source;
			ISampleProvider src = this.source.ToSampleProvider();

			if (source.WaveFormat.Channels == 1)
			{
				this.pan = new PanningSampleProvider(src);
				src = this.pan;
			}

			this.pitch = new SmbPitchShiftingSampleProvider(src);
			this.volume = new VolumeSampleProvider(this.pitch);
			this.pcm16 = this.volume.ToWaveProvider16();
		}

		public void Play()
		{
			this.source.Position = 0;
			DynamicSoundEffectInstance sfx = new DynamicSoundEffectInstance(this.source.WaveFormat.SampleRate, AudioChannels.Stereo);
			sfx.BufferNeeded += (sender, args) => this.ReadBuffer(sender as DynamicSoundEffectInstance);
			sfx.Play();
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

		private void ReadBuffer(DynamicSoundEffectInstance sfx)
		{
			int bufferSize = (int)MathF.Floor(this.pcm16.WaveFormat.AverageBytesPerSecond * Time.FrameSeconds);
			bufferSize = (int)(MathF.Ceiling(bufferSize / this.pcm16.WaveFormat.BlockAlign)) * this.pcm16.WaveFormat.BlockAlign;

			while (bufferSize > this.buffer.Length)
				this.buffer = new byte[this.buffer.Length * 2];

			while (sfx.PendingBufferCount < 2 && this.State == SoundState.Playing)
			{
				int read = this.pcm16.Read(this.buffer, 0, bufferSize);

				if (read > 0)
					sfx.SubmitBuffer(this.buffer, 0, read);
				else
					this.State = SoundState.Stopped;
			}
		}
	}
}
