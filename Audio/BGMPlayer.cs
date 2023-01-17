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
	public class BGMPlayer
	{
		private readonly MixingSampleProvider mixer;
		private readonly VolumeSampleProvider volume;
		private readonly IWaveProvider pcm16;

		private readonly DynamicSoundEffectInstance bgm;

		private byte[] buffer = new byte[4096];

		public event Action<TimeSpan> OnSongEnding;

		private Dictionary<string, BGMInstance> bgms;

		internal BGMPlayer()
		{
			this.bgms = new Dictionary<string, BGMInstance>();

			this.mixer = new MixingSampleProvider(new ISampleProvider[] { SilenceGenerator.Instance });
			this.volume = new VolumeSampleProvider(this.mixer);
			this.pcm16 = this.volume.ToWaveProvider16();

			this.bgm = new DynamicSoundEffectInstance(this.pcm16.WaveFormat.SampleRate, AudioChannels.Stereo);
			this.bgm.BufferNeeded += (sender, args) => this.ReadBuffer(sender as DynamicSoundEffectInstance);
		}

		internal void Play(AudioProvider provider, string songName, float fadeSeconds = 0)
		{
			this.bgms[songName] = new BGMInstance(provider.SampleProvider, this.mixer.WaveFormat.SampleRate);
			this.mixer.AddMixerInput(this.bgms[songName]);

			this.bgms[songName].FadeIn(fadeSeconds);
			this.bgm.Play();
		}

		internal void Stop(float fadeSeconds = 0)
		{
			foreach (BGMInstance bgm in this.bgms.Values)
				this.mixer.RemoveMixerInput(bgm);

			this.bgms.Clear();
		}

		internal void Update()
		{
			foreach (BGMInstance provider in this.bgms.Values)
				provider.Update();
		}

		private void ReadBuffer(DynamicSoundEffectInstance sfx)
		{
			int bufferSize = (int)MathF.Ceiling(this.pcm16.WaveFormat.AverageBytesPerSecond * .05f);
			bufferSize = (int)MathF.Ceiling(bufferSize / this.pcm16.WaveFormat.BlockAlign) * this.pcm16.WaveFormat.BlockAlign;

			while (bufferSize > this.buffer.Length)
				this.buffer = new byte[this.buffer.Length * 2];

			while (sfx.PendingBufferCount < 2)
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
