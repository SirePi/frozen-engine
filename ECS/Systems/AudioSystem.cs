using System;
using Frozen.Audio;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;

namespace Frozen.ECS.Systems
{
	public class AudioSystem
	{
		private byte[] _buffer;
		private IWaveProvider _pcm16;
		private DynamicSoundEffectInstance _soundOutput;

		internal const int SampleRate = 48000;
		internal const float SpeedOfSound = 3400;

		public SoundMixer Master { get; private set; }

		public SoundMixer Music { get; private set; }

		public SoundMixer SoundEffects { get; private set; }

		public SoundMixer Voice { get; private set; }

		internal AudioSystem()
		{
			Music = new SoundMixer();
			SoundEffects = new SoundMixer();
			Voice = new SoundMixer();

			Master = new SoundMixer();
			Master.AddMixerInput(Music);
			Master.AddMixerInput(SoundEffects);
			Master.AddMixerInput(Voice);

			_pcm16 = Master.ToWaveProvider16();

			int bufferSize = (int)MathF.Ceiling(_pcm16.WaveFormat.AverageBytesPerSecond * .05f);
			bufferSize = (int)MathF.Ceiling(bufferSize / _pcm16.WaveFormat.BlockAlign) * _pcm16.WaveFormat.BlockAlign;

			_buffer = new byte[bufferSize];

			_soundOutput = new DynamicSoundEffectInstance(Master.WaveFormat.SampleRate, AudioChannels.Stereo);
			_soundOutput.BufferNeeded += SoundOutput_BufferNeeded;
			_soundOutput.Play();
		}

		private SfxInstance PlaySound(AudioSource source, float volume, float pan, float pitch)
		{
			SfxInstance instance = source.GetSoundEffectInstance();
			instance.Volume = volume;
			instance.Pan = pan;
			instance.Pitch = pitch;

			instance.Play();
			return instance;
		}

		private void SoundOutput_BufferNeeded(object sender, EventArgs e)
		{
			DynamicSoundEffectInstance sfx = sender as DynamicSoundEffectInstance;
			while (sfx.PendingBufferCount < 2)
			{
				int read = _pcm16.Read(_buffer, 0, _buffer.Length);
				sfx.SubmitBuffer(_buffer, 0, read);
			}
		}

		public BgmInstance PlayMusic(AudioSource song, float volume = 1)
		{
			BgmInstance instance = song.GetMusicInstance();
			instance.Volume = volume;
			instance.Play();

			Music.AddMixerInput(instance);
			return instance;
		}

		public SfxInstance PlaySoundEffect(AudioSource sfx, float volume = 1, float pan = 0, float pitch = 1)
		{
			SfxInstance instance = PlaySound(sfx, volume, pan, pitch);
			SoundEffects.AddMixerInput(instance);
			return instance;
		}

		public SfxInstance PlayVoice(AudioSource clip, float volume = 1, float pan = 0)
		{
			SfxInstance instance = PlaySound(clip, volume, pan, 1);
			Voice.AddMixerInput(instance);
			return instance;
		}
	}
}
