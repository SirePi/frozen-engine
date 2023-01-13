using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frozen.Utilities;
using Microsoft.Xna.Framework.Audio;
using NAudio.Vorbis;
using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class AudioSource
	{
		private readonly Pool<AudioInstance> pool;

		protected AudioSource()
		{
			this.pool = new Pool<AudioInstance>(() => this.CreateNewInstance());
		}

		protected abstract AudioInstance InternalCreateNewInstance();

		public AudioInstance CreateNewInstance()
		{
			AudioInstance instance = this.InternalCreateNewInstance();
			instance.OnStateChanged += this.Instance_OnStateChanged;
			return instance;
		}

		private void Instance_OnStateChanged(AudioInstance arg1, SoundState arg2)
		{
			if (arg2 == SoundState.Stopped)
				this.pool.ReturnOne(arg1);
		}

		public AudioInstance GetAudioInstance()
		{
			return this.pool.GetOne();
		}

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
		private readonly byte[] source;
		private readonly WaveFormat waveFormat;

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
					wave = new MediaFoundationReader(filename);
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

			this.source = new byte[wave.Length];
			wave.Read(this.source, 0, this.source.Length);

			this.waveFormat = wave.WaveFormat;
		}

		protected override AudioInstance InternalCreateNewInstance()
		{
			AudioProvider provider = new FileAudioProvider(new RawSourceWaveStream(this.source, 0, this.source.Length, this.waveFormat));
			return new AudioInstance(provider);
		}
	}

	public class GeneratedAudioSource : AudioSource
	{
		private readonly WaveGenerator source;
		internal GeneratedAudioSource(WaveGenerator provider)
		{
			this.source = provider;
		}

		protected override AudioInstance InternalCreateNewInstance()
		{
			AudioProvider provider = new GeneratedAudioProvider(this.source.Clone());
			return new AudioInstance(provider);
		}
	}
}
