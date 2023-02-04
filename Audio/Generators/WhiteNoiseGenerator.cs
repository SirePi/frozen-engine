using System;
using FontStashSharp;
using Frozen.ECS.Systems;

namespace Frozen.Audio
{
	public class WhiteNoiseGenerator : AudioGenerator
	{
		public WhiteNoiseGenerator(float amplitude = 1, int sampleRate = AudioSystem.SampleRate)
			: base(amplitude, sampleRate)
		{ }

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			for (int n = 0; n < sampleCount; n++)
			{
				float r1 = Engine.Random.NextFloat();
				float r2 = Engine.Random.NextFloat();

				buffer[n + offset] = MathF.Sqrt(-2.0f * MathF.Log(r1)) * MathF.Cos(FrozenMath.TWO_PI * r2);
			}
			return sampleCount;
		}
	}
}
