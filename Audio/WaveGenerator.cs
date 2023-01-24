using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class WaveGenerator : WaveProvider32
	{
		protected int _sample;

		public float Amplitude { get; set; }

		public int Frequency { get; set; }

		internal WaveGenerator(int frequency, float amplitude, int sampleRate)
		{
			Frequency = frequency;
			Amplitude = amplitude;
			SetWaveFormat(sampleRate, 1);
			_sample = 0;
		}

		public void Reset()
		{
			_sample = 0;
		}
	}
}
