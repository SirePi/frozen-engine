using System;

namespace Frozen.Audio
{
	internal class TriangleWaveGenerator : WaveGenerator
	{
		public TriangleWaveGenerator(int frequency, float amplitude, int sampleRate)
			: base(frequency, amplitude, sampleRate)
		{ }

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			int sampleRate = WaveFormat.SampleRate;
			for (int n = 0; n < sampleCount; n++)
			{
				buffer[n + offset] = Amplitude * MathF.Sign(MathF.Sin(FrozenMath.TWO_PI * _sample * Frequency / sampleRate));
				_sample++;
				if (_sample >= sampleRate)
					_sample = 0;
			}
			return sampleCount;
		}
	}
}
