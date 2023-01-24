using System;
using System.Linq;
using Frozen.Utilities;
using Microsoft.Xna.Framework.Audio;
using NAudio.Vorbis;
using NAudio.Wave;
using NLayer.NAudioSupport;

namespace Frozen.Audio
{
	public abstract class AudioSource
	{
		private readonly DerivedObjectsPool<AudioInstance> _pool;

		protected AudioSource()
		{
			_pool = new DerivedObjectsPool<AudioInstance>();
		}

		private void Instance_OnStateChanged(AudioInstance arg1, SoundState arg2)
		{
			if (arg2 == SoundState.Stopped)
				_pool.ReturnOne(arg1);
		}

		internal BgmInstance GetMusicInstance()
		{
			BgmInstance instance = _pool.GetOne(() => new BgmInstance(ToAudioProvider()));
			instance.OnStateChanged += Instance_OnStateChanged;
			return instance;
		}

		internal SfxInstance GetSoundEffectInstance()
		{
			SfxInstance instance = _pool.GetOne(() => new SfxInstance(ToAudioProvider()));
			instance.OnStateChanged += Instance_OnStateChanged;
			return instance;
		}

		internal abstract AudioProvider ToAudioProvider();

		public static GeneratedAudioSource SineWave(int frequency = 400, float amplitude = .25f, int sampleRate = 48000)
		{
			return new GeneratedAudioSource(new SineWaveGenerator(frequency, amplitude, sampleRate));
		}

		public static GeneratedAudioSource SquareWave(int frequency = 400, float amplitude = .25f, int sampleRate = 48000)
		{
			return new GeneratedAudioSource(new SquareWaveGenerator(frequency, amplitude, sampleRate));
		}
	}

	public class FileAudioSource : AudioSource
	{
		private readonly byte[] _source;
		private readonly WaveFormat _waveFormat;

		internal FileAudioSource(string filename) : base()
		{
			string extension = filename.Split('.').Last().ToLowerInvariant();

			WaveStream wave;

			switch (extension)
			{
				case "ogg":
					wave = new VorbisWaveReader(filename);
					break;

				case "mp3":
					Mp3FileReaderBase.FrameDecompressorBuilder builder = new Mp3FileReaderBase.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
					wave = new Mp3FileReaderBase(filename, builder);
					break;

				case "aiff":
					wave = new AiffFileReader(filename);
					break;

				case "wav":
					wave = new WaveFileReader(filename);
					break;

				default:
					throw new Exception("Unknown file format");
			}

			if (wave.WaveFormat.Channels < 1 || wave.WaveFormat.Channels > 2)
				throw new Exception("Invalid number of channels");

			_source = new byte[wave.Length];
			wave.Read(_source, 0, _source.Length);

			_waveFormat = wave.WaveFormat;
		}

		internal override AudioProvider ToAudioProvider()
		{
			return new FileAudioProvider(new RawSourceWaveStream(_source, 0, _source.Length, _waveFormat));
		}
	}

	public class GeneratedAudioSource : AudioSource
	{
		public WaveGenerator Generator { get; private set; }

		internal GeneratedAudioSource(WaveGenerator generator)
		{
			Generator = generator;
		}

		internal override AudioProvider ToAudioProvider()
		{
			return new GeneratedAudioProvider(Generator);
		}
	}
}
