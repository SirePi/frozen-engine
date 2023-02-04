using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public class SoundMixer : ISampleProvider
	{
		private static readonly ISampleProvider[] _silence = new ISampleProvider[] { SilenceGenerator.Instance };

		private readonly MixingSampleProvider _mixer;
		private readonly Dictionary<ISampleProvider, ISampleProvider> _resampledProviders;
		private readonly VolumeSampleProvider _volume;
		private ISampleProvider _output;

		public float Volume { get => _volume.Volume; set => _volume.Volume = MathF.Max(value, 0); }

		public WaveFormat WaveFormat => _volume.WaveFormat;

		internal SoundMixer()
		{
			_resampledProviders = new Dictionary<ISampleProvider, ISampleProvider>();

			_mixer = new MixingSampleProvider(_silence);
			_volume = new VolumeSampleProvider(_mixer);

			_output = _volume;
		}

		internal void AddMixerInput(ISampleProvider provider)
		{
			if (!_resampledProviders.ContainsKey(provider))
			{
				if (provider.WaveFormat.SampleRate != _mixer.WaveFormat.SampleRate)
					_resampledProviders[provider] = new WdlResamplingSampleProvider(provider, _mixer.WaveFormat.SampleRate);
				else
					_resampledProviders[provider] = provider;

				_mixer.AddMixerInput(_resampledProviders[provider]);
			}
			else
			{
				// already known.. is it still playing?
				if (!_mixer.MixerInputs.Contains(_resampledProviders[provider]))
					_mixer.AddMixerInput(_resampledProviders[provider]); // not in list.. put it back
			}
		}

		internal void RemoveMixerInput(ISampleProvider provider)
		{
			if (_resampledProviders.ContainsKey(provider))
				_mixer.RemoveMixerInput(_resampledProviders[provider]);
		}

		public int Read(float[] buffer, int offset, int count)
		{
			foreach (ISampleProvider provider in _resampledProviders.Keys)
				if (provider is AudioInstance ai)
					ai.Update();

			return _output.Read(buffer, offset, count);
		}

		public void ApplyGlobalEffect(AudioEffect effect)
		{
			effect.ApplyTo(_volume);
			_output = effect;
		}

		public void RemoveGlobalEffect()
		{
			_output = _volume;
		}
	}
}
