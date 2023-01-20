using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frozen.Audio
{
	internal class SineWaveGenerator : WaveGenerator
	{
		public SineWaveGenerator(int frequency, float amplitude, int sampleRate)
			: base(frequency, amplitude, sampleRate)
		{ }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = this.WaveFormat.SampleRate;
			for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = this.Amplitude * MathF.Sin(FrozenMath.TWO_PI * this.sample * this.Frequency / sampleRate);

				this.sample++;
                if (this.sample >= sampleRate) 
					this.sample = 0;
            }
            return sampleCount;
        }
	}
}
