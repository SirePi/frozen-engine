using System;
using System.Collections.Generic;
using System.Text;
using Frozen.ECS.Systems;
using NAudio.Wave;

namespace Frozen.Audio
{
	internal class SilenceGenerator : WaveProvider32
	{
		private static SilenceGenerator instance;
		public static SilenceGenerator Instance
		{
			get
			{
				if (instance == null)
					instance = new SilenceGenerator();

				return instance;
			}
		}

		internal static SilenceGenerator[] InstanceForMixer { get; private set; } = { Instance };

		private SilenceGenerator()
		{
			this.SetWaveFormat(AudioSystem.SampleRate, 2);
		}

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			for (int i = 0; i < sampleCount; i++)
				buffer[i + offset] = 0;

			return sampleCount;
		}
	}
}
