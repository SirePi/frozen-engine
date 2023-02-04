using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frozen.Audio
{
    public class EchoEffect : AudioEffect
    {
		private readonly float _delayInSeconds;
		private readonly float _gain;

		private int _delayInSamples;
		private float[] _delayBuffer;
        private int _delayBufferIndex;
        private int _samplesSinceEnoughEnergy;

        public EchoEffect(float delayInSeconds, float gain)
        {
			_delayInSeconds = delayInSeconds;
            _gain = gain;
        }

		internal override void ApplyTo(ISampleProvider source)
		{
			base.ApplyTo(source);

			_delayInSamples = (int)(_delayInSeconds * source.WaveFormat.SampleRate);
			_delayBuffer = new float[_delayInSamples];
			_delayBufferIndex = 0;
		}

		public override int Read(float[] buffer, int offset, int count)
		{
			int samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < count; i++)
            {
                float sampleValue = i < samplesRead ? buffer[i + offset] : 0;
                sampleValue += _delayBuffer[_delayBufferIndex] * _gain;

                _delayBuffer[_delayBufferIndex] = sampleValue;

				_delayBufferIndex++;
                if (_delayBufferIndex >= _delayInSamples)
                    _delayBufferIndex = 0;

                _samplesSinceEnoughEnergy++;

                if (MathF.Abs(sampleValue) > 0.01f)
                    _samplesSinceEnoughEnergy = 0;

                buffer[i + offset] = sampleValue;
            }

            return samplesRead == 0 && _samplesSinceEnoughEnergy > _delayBuffer.Length ? 0 : count;
        }
    }
}
