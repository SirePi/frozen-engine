using System;
using NAudio.Wave;

namespace Frozen.Audio
{
	internal class FileAudioProvider : AudioProvider
	{
		private WaveStream _stream;

		public override bool IsActive => _stream.Position < _stream.Length;

		public override TimeSpan TimeLeft => _stream.TotalTime - _stream.CurrentTime;

		public FileAudioProvider(WaveStream source) : base(source.ToSampleProvider(), source.WaveFormat)
		{
			_stream = source;
		}

		internal override void Reset()
		{
			_stream.Position = 0;
		}
	}

	internal class GeneratedAudioProvider : AudioProvider
	{
		internal WaveGenerator Source { get; private set; }

		public override bool IsActive => true;

		public override TimeSpan TimeLeft => TimeSpan.Zero;

		public GeneratedAudioProvider(WaveGenerator source) : base(source, source.WaveFormat)
		{
			Source = source;
		}

		internal override void Reset()
		{
			Source.Reset();
		}
	}

	public abstract class AudioProvider
	{
		internal ISampleProvider SampleProvider { get; private set; }

		internal WaveFormat WaveFormat { get; private set; }

		public abstract bool IsActive { get; }

		public abstract TimeSpan TimeLeft { get; }

		protected AudioProvider(ISampleProvider provider, WaveFormat waveFormat)
		{
			SampleProvider = provider;
			WaveFormat = waveFormat;
		}

		internal abstract void Reset();
	}
}
