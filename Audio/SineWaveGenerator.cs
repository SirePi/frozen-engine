using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Frozen.Audio
{
	public class SineWaveProvider32 : WaveProvider32
	{
		int sample;

		public SineWaveProvider32()
		{
			this.Frequency = 1000;
			this.Amplitude = 1f; // let's not hurt our ears
		}

		public float Frequency { get; set; }
		public float Amplitude { get; set; }

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			int sampleRate = this.WaveFormat.SampleRate;
			for (int n = 0; n < sampleCount; n++)
			{
				buffer[n + offset] = (float)(this.Amplitude * Math.Sin((2 * Math.PI * this.sample * this.Frequency) / sampleRate));
				this.sample++;
				if (this.sample >= sampleRate) this.sample = 0;
			}
			return sampleCount;
		}
	}
}
