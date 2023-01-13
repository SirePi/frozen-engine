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
	public class AudioContent
	{
		internal ISampleProvider Source { get; private set; }
		internal WaveFormat WaveFormat { get; private set; }

		private readonly Pool<AudioInstance> pool;

		public AudioContent(string filename) : this()
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

			this.Source = wave.ToSampleProvider();
			this.Sour.
			this.Source = new RawSourceWaveStream
			this.Source = new System.IO.MemoryStream();
			wave.CopyTo(this.Source);

			this.WaveFormat = wave.WaveFormat;
		}

		public AudioContent(SoundEffect sfx) : this()
		{
			
		}

		private AudioContent() 
		{
			this.pool = new Pool<AudioInstance>(() => this.CreateNewInstance());
		}

		private AudioInstance CreateNewInstance()
		{
			AudioInstance instance = new AudioInstance(this);
			instance.OnStateChanged += this.Instance_OnStateChanged;

			return instance;
		}

		private void Instance_OnStateChanged(AudioInstance arg1, SoundState arg2)
		{
			if(arg2 == SoundState.Stopped)
				this.pool.ReturnOne(arg1);
		}

		public AudioInstance GetAudioInstance()
		{
			return this.pool.GetOne();
		}

		public static AudioContent SineWave(int frequency, float amplitude = .5f, int sampleRate = 48000)
		{
			AudioContent content = new AudioContent();
			content.Source = new SineWaveGenerator { Frequency = frequency, Amplitude = amplitude, SampleRate = sampleRate };
			content.WaveFormat = new WaveFormat(sampleRate, 16, 1);
			return content;
		}

		public static AudioContent SquareWave(int frequency, float amplitude = .5f, int sampleRate = 48000)
		{
			AudioContent content = new AudioContent();
			content.Source = new SquareWaveGenerator { Frequency = frequency, Amplitude = amplitude, SampleRate = sampleRate };
			content.WaveFormat = new WaveFormat(sampleRate, 1);
			return content;
		}
	}
}
