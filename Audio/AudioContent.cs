using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using NAudio.Vorbis;
using NAudio.Wave;

namespace Frozen.Audio
{
	public class AudioContent
	{
		internal WaveStream WaveStream { get; private set; }
		
		public AudioContent(string filename)
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
				throw new Exception("channels");

			this.WaveStream = wave;
		}

		public AudioContent(SoundEffect sfx)
		{
			
		}

		private AudioContent() { }

		public static AudioContent SineWave()
		{
			AudioContent content = new AudioContent();
			//content.WaveStream = new SineWaveProvider32();
			return content;
		}
	}
}
