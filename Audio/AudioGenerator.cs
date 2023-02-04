using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class AudioGenerator : WaveProvider32
	{
		protected int _sample;

		public float Amplitude { get; set; }

		internal AudioGenerator(float amplitude, int sampleRate)
		{
			Amplitude = amplitude;
			SetWaveFormat(sampleRate, 1);
			_sample = 0;
		}

		public void Reset()
		{
			_sample = 0;
		}

		public GeneratedAudioSource AsAudioSource()
		{
			return new GeneratedAudioSource(this);
		}
	}
}
