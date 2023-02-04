using System;
using Frozen.ECS.Systems;
using NAudio.Dmo.Effect;
using NAudio.Wave;

namespace Frozen.Audio
{
	public class TriangleWaveGenerator : WaveGenerator
	{
		public TriangleWaveGenerator(int frequency, float amplitude = 1, int sampleRate = AudioSystem.SampleRate)
			: base(frequency, amplitude, sampleRate)
		{ }

		public override int Read(float[] buffer, int offset, int count)
		{
			int sampleRate = WaveFormat.SampleRate;
			int samplesPerPeriod = (int)(sampleRate / Frequency);

			for (int n = 0; n < count; n += 2)
			{
				float value = Amplitude * ((2.0f * (_sample++ % samplesPerPeriod) / samplesPerPeriod) - 1.0f);
				buffer[n + offset] = value;
				buffer[n + offset + 1] = -value;
			}

			return count;
		}
	}
}
