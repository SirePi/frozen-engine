using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class WaveGenerator : AudioGenerator
	{
		public int Frequency { get; set; }

		internal WaveGenerator(int frequency, float amplitude, int sampleRate)
			: base(amplitude, sampleRate)
		{
			Frequency = frequency;
		}
	}
}
