using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Frozen.Audio
{
	public class BgmInstance : AudioInstance
	{
		private float targetVolume;

		private float volumeDelta;

		private float volumeLimit;

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
