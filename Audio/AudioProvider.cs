using System;
using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class AudioProvider
	{
		internal ISampleProvider SampleProvider { get; private set; }

		internal WaveFormat WaveFormat { get; private set; }

		internal abstract void Reset();

		public abstract TimeSpan TimeLeft { get; }

		public abstract bool IsActive { get; }

		protected AudioProvider(ISampleProvider provider, WaveFormat waveFormat)
		{
			this.SampleProvider = provider;
			this.WaveFormat = waveFormat;
		}
	}

	internal class FileAudioProvider : AudioProvider
	{
		private WaveStream stream;

		public FileAudioProvider(WaveStream source) : base(source.ToSampleProvider(), source.WaveFormat)
		{
			this.stream = source;
		}

		public override bool IsActive => this.stream.Position < this.stream.Length;

		public override TimeSpan TimeLeft => this.stream.TotalTime - this.stream.CurrentTime;

		internal override void Reset()
		{
			this.stream.Position = 0;
		}
	}

	internal class GeneratedAudioProvider : AudioProvider
	{
		internal WaveGenerator Source { get; private set; }

		public GeneratedAudioProvider(WaveGenerator source) : base(source, source.WaveFormat)
		{
			this.Source = source;
		}

		public override bool IsActive => true;

		public override TimeSpan TimeLeft => TimeSpan.Zero;

		internal override void Reset()
		{
			this.Source.Reset();
		}
	}
}
