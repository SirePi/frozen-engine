using NAudio.Wave;

namespace Frozen.Audio
{
	public class BgmInstance : AudioInstance
	{
		private float _targetVolume;
		private float _volumeDelta;
		private float _volumeLimit;

		internal BgmInstance(AudioProvider provider) : base(provider)
		{ }

		protected override ISampleProvider SetupPipeline()
		{
			return _source.SampleProvider;
		}

		internal override void Update()
		{
			if (_volumeDelta != 0)
			{
				float delta = _volumeDelta * Time.FrameSeconds;
				Volume += delta;

				if (FrozenMath.IsBetween(Volume, _targetVolume, _volumeLimit))
				{
					Volume = _targetVolume;
					_volumeDelta = 0;
					_volumeLimit = 0;
				}
			}
		}

		public void FadeIn(float fadeSeconds)
		{
			FadeTo(1, fadeSeconds);
		}

		public void FadeOut(float fadeSeconds)
		{
			FadeTo(0, fadeSeconds);
		}

		public void FadeTo(float targetVolume, float fadeSeconds)
		{
			if (fadeSeconds == 0)
			{
				Volume = targetVolume;
			}
			else
			{
				_targetVolume = targetVolume;
				_volumeDelta = (targetVolume - Volume) / fadeSeconds;
				_volumeLimit = targetVolume + (_volumeDelta * 2);   // just to be sure
			}
		}
	}
}
