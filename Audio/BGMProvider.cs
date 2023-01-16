using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	internal enum Fade
	{
		None,
		FadingIn,
		FadingOut
	}

	internal class BGMProvider : ISampleProvider
	{
		private VolumeSampleProvider volume;

		private float targetVolume = 1;
		private float volumeDelta = 0;

		public float Volume { get => this.volume.Volume; set => this.volume.Volume = value; }

		internal BGMProvider(ISampleProvider provider, int sampleRate)
		{
			if (provider.WaveFormat.Channels == 1)
				provider = provider.ToStereo();

			this.volume = new VolumeSampleProvider(new WdlResamplingSampleProvider(provider, sampleRate));
		}

		public WaveFormat WaveFormat => this.volume.WaveFormat;

		internal void FadeIn(float fadeSeconds)
		{
			this.FadeTo(1, fadeSeconds);
		}

		internal void FadeOut(float fadeSeconds)
		{
			this.FadeTo(0, fadeSeconds);
		}

		internal void FadeTo(float targetVolume, float fadeSeconds)
		{
			this.targetVolume = targetVolume;
			this.volumeDelta = (targetVolume - this.Volume) / fadeSeconds;
		}

		internal void Update()
		{
			if (this.Volume != this.targetVolume)
			{
				float delta = this.volumeDelta * Time.FrameSeconds;
				if (this.targetVolume - this.Volume <= delta)
				{
					this.Volume = this.targetVolume;
					this.volumeDelta = 0;
				}
				else
					this.Volume += delta;
			}
		}


		public int Read(float[] buffer, int offset, int count)
		{
			return this.volume.Read(buffer, offset, count);
		}
	}
}
