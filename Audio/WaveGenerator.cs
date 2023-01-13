using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class WaveGenerator : WaveProvider32
	{
		protected int sample;

		public int Frequency { get; set; }
		public float Amplitude { get; set; }

		internal WaveGenerator(int frequency, float amplitude, int sampleRate)
		{
			this.Frequency = frequency;
			this.Amplitude = amplitude;
			this.SetWaveFormat(sampleRate, 1);
			this.sample = 0;
		}

		public abstract WaveGenerator Clone();
		public void Reset()
		{
			this.sample = 0;
		}
	}
}
