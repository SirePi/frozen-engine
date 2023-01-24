using Frozen.ECS.Systems;
using NAudio.Wave;

namespace Frozen.Audio
{
	internal class SilenceGenerator : WaveProvider32
	{
		private static SilenceGenerator _instance;

		public static SilenceGenerator Instance
		{
			get
			{
				if (_instance == null)
					_instance = new SilenceGenerator();

				return _instance;
			}
		}

		private SilenceGenerator()
		{
			SetWaveFormat(AudioSystem.SampleRate, 2);
		}

		public override int Read(float[] buffer, int offset, int sampleCount)
		{
			for (int i = 0; i < sampleCount; i++)
				buffer[i + offset] = 0;

			return sampleCount;
		}
	}
}
