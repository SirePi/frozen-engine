using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using Frozen.ECS.Systems;
using System.Linq;

namespace Frozen.Audio
{
	public class SoundMixer : ISampleProvider
	{
		private static readonly ISampleProvider[] silence = new ISampleProvider[] { SilenceGenerator.Instance };

		private readonly MixingSampleProvider mixer;
		private readonly VolumeSampleProvider volume;
		private readonly Dictionary<ISampleProvider, ISampleProvider> resampledProviders;

		public float Volume { get => this.volume.Volume; set => this.volume.Volume = MathF.Max(value, 0); }

		internal SoundMixer()
		{
			this.resampledProviders = new Dictionary<ISampleProvider, ISampleProvider>();

			this.mixer = new MixingSampleProvider(silence);
			this.volume = new VolumeSampleProvider(this.mixer);
		}

		internal void AddMixerInput(ISampleProvider provider)
		{
			if (!this.resampledProviders.ContainsKey(provider))
			{
				if (provider.WaveFormat.SampleRate != this.mixer.WaveFormat.SampleRate)
					this.resampledProviders[provider] = new WdlResamplingSampleProvider(provider, this.mixer.WaveFormat.SampleRate);
				else
					this.resampledProviders[provider] = provider;

				this.mixer.AddMixerInput(this.resampledProviders[provider]);
			}
		}

		internal void RemoveMixerInput(ISampleProvider provider)
		{
			if(this.resampledProviders.ContainsKey(provider))
				this.mixer.RemoveMixerInput(this.resampledProviders[provider]);
		}

		public WaveFormat WaveFormat => this.volume.WaveFormat;

		public int Read(float[] buffer, int offset, int count)
		{
			foreach (ISampleProvider provider in this.resampledProviders.Keys)
				if (provider is AudioInstance ai)
					ai.Update();

			return this.volume.Read(buffer, offset, count);
		}
	}
}
