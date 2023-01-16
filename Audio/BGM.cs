using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public class BGM
	{
		private readonly MixingSampleProvider mixer;
		private readonly VolumeSampleProvider volume;
		private readonly IWaveProvider pcm16;

		private readonly DynamicSoundEffectInstance bgm;

		private byte[] buffer = new byte[4096];
		private SoundState state;

		public event Action<TimeSpan> OnSongEnding;

		private Dictionary<string, BGMProvider> mixChannels;

		public float Volume { get => this.volume.Volume; set => this.volume.Volume = value; }

		private DateTime start;

		internal BGM()
		{
			this.mixChannels = new Dictionary<string, BGMProvider>();

			this.mixer = new MixingSampleProvider(new ISampleProvider[] { SilenceGenerator.Instance });
			this.volume = new VolumeSampleProvider(this.mixer);
			this.pcm16 = this.volume.ToWaveProvider16();

			this.bgm = new DynamicSoundEffectInstance(this.pcm16.WaveFormat.SampleRate, AudioChannels.Stereo);
			this.bgm.BufferNeeded += (sender, args) => this.ReadBuffer(sender as DynamicSoundEffectInstance);
		}

		internal void Play(AudioProvider provider, string songName, float fadeMilliseconds = 0)
		{
			this.mixChannels[songName] = new BGMProvider(provider.SampleProvider, this.mixer.WaveFormat.SampleRate);
			this.mixer.AddMixerInput(this.mixChannels[songName]);

			this.start = DateTime.Now;
			this.state = SoundState.Playing;

			this.mixChannels[songName].FadeIn(20);
			this.bgm.Play();
		}

		internal void Stop(float fadeMilliseconds = 0)
		{
			this.mixer.RemoveAllMixerInputs();
		}

		internal void Update()
		{
			foreach (BGMProvider provider in this.mixChannels.Values)
				provider.Update();
		}

		private void ReadBuffer(DynamicSoundEffectInstance sfx)
		{
			int bufferSize = (int)MathF.Ceiling(this.pcm16.WaveFormat.AverageBytesPerSecond * .05f);
			bufferSize = (int)MathF.Ceiling(bufferSize / this.pcm16.WaveFormat.BlockAlign) * this.pcm16.WaveFormat.BlockAlign;

			while (bufferSize > this.buffer.Length)
				this.buffer = new byte[this.buffer.Length * 2];

			while (sfx.PendingBufferCount < 2 && this.state == SoundState.Playing)
			{
				int read = this.pcm16.Read(this.buffer, 0, bufferSize);

				if (read > 0)
					sfx.SubmitBuffer(this.buffer, 0, read);
				else
					this.Stop();
			}
		}
	}
}
