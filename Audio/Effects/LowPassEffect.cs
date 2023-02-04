using Frozen.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest
{
    public class LowPassEffect : AudioEffect
    {
        private readonly float _cutoffFrequency;
        private float _lastSample;

        public LowPassEffect(float cutoffFrequency)
        {
            _cutoffFrequency = cutoffFrequency;
        }

		public override int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                _lastSample = buffer[offset + i] = buffer[offset + i] * _cutoffFrequency + _lastSample * (1 - _cutoffFrequency);
            }

            return samplesRead;
        }
    }
}
