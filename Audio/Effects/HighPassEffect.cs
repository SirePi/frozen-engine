using Frozen.Audio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioTest
{
    public class HighPassEffect : AudioEffect
    {
        private readonly float _cutoffFrequency;
        private float _lastSample;

        public HighPassEffect(float cutoffFrequency)
        {
            _cutoffFrequency = cutoffFrequency;
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                float currentSample = buffer[offset + i];
                buffer[offset + i] = currentSample - _lastSample + _cutoffFrequency * (buffer[offset + i] - currentSample);
                _lastSample = currentSample;
            }

            return samplesRead;
        }
    }
}
