using System;
using Frozen.Audio;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;

namespace Frozen.ECS.Systems
{
	public class AudioSystem
	{
		internal const int SampleRate = 48000;

		internal const float SpeedOfSound = 3400;

		private byte[] buffer;

		private IWaveProvider pcm16;

		private DynamicSoundEffectInstance soundOutput;

		public SoundMixer Master { get; private set; }

		public SoundMixer Music { get; private set; }

		public SoundMixer SoundEffects { get; private set; }

		public SoundMixer Voice { get; private set; }
		internal AudioSystem()
		{
			this.Music = new SoundMixer();
			this.SoundEffects = new SoundMixer();
			this.Voice = new SoundMixer();

			this.Master = new SoundMixer();
			this.Master.AddMixerInput(this.Music);
			this.Master.AddMixerInput(this.SoundEffects);
			this.Master.AddMixerInput(this.Voice);

			this.pcm16 = this.Master.ToWaveProvider16();

			int bufferSize = (int)MathF.Ceiling(this.pcm16.WaveFormat.AverageBytesPerSecond * .05f);
			bufferSize = (int)MathF.Ceiling(bufferSize / this.pcm16.WaveFormat.BlockAlign) * this.pcm16.WaveFormat.BlockAlign;

			this.buffer = new byte[bufferSize];

			this.soundOutput = new DynamicSoundEffectInstance(this.Master.WaveFormat.SampleRate, AudioChannels.Stereo);
			this.soundOutput.BufferNeeded += this.SoundOutput_BufferNeeded;
			this.soundOutput.Play();
		}

		public BgmInstance PlayMusic(AudioSource song, float volume = 1)
		{
			BgmInstance instance = song.GetMusicInstance();
			instance.Volume = volume;
			instance.Play();

			this.Music.AddMixerInput(instance);
			return instance;
		}

		public SfxInstance PlaySoundEffect(AudioSource sfx, float volume = 1, float pan = 0, float pitch = 1)
		{
			SfxInstance instance = this.PlaySound(sfx, volume, pan, pitch);
			this.SoundEffects.AddMixerInput(instance);
			return instance;
		}

		public SfxInstance PlayVoice(AudioSource clip, float volume = 1, float pan = 0)
		{
			SfxInstance instance = this.PlaySound(clip, volume, pan, 1);
			this.Voice.AddMixerInput(instance);
			return instance;
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
				int read = this.pcm16.Read(this.buffer, 0, this.buffer.Length);
				sfx.SubmitBuffer(this.buffer, 0, read);
			}
		}
	}
}
