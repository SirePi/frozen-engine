using System;
using System.Collections.Generic;
using System.Text;
using Frozen.Audio;
using Frozen.ECS.Components;
using Frozen.Enums;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.ECS.Systems
{
	public class AudioSystem
	{
		internal const int SampleRate = 48000;
		internal const float SpeedOfSound = 3400;

		private readonly List<ThreeDSound> threeDsounds;
		public SoundMixer Master { get; private set; }
		public SoundMixer Music { get; private set; }
		public SoundMixer SoundEffects { get; private set; }
		public SoundMixer Voice { get; private set; }

		private byte[] buffer;
		private IWaveProvider pcm16; 
		private DynamicSoundEffectInstance soundOutput;

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

			this.threeDsounds = new List<ThreeDSound>();

			// just instantiate it once to make it faster later on
			this.soundOutput = new DynamicSoundEffectInstance(this.Master.WaveFormat.SampleRate, AudioChannels.Stereo);
			this.soundOutput.BufferNeeded += this.SoundOutput_BufferNeeded;
			this.soundOutput.Play();
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

		public AudioInstance PlayMusic(AudioSource song, float volume = 1, float fadeInMilliseconds = 0)
		{
			AudioInstance instance = song.GetAudioInstance();
			this.Music.AddMixerInput(instance);
			// this.bgm.Play(, "ya");

			instance.Play();
			return instance;
		}

		/*
		public void PlaySongLooping(Song song)
		{
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;
		}
		*/

		public AudioInstance PlaySoundEffect(AudioSource sfx, float volume = 1, float pan = 0, float pitch = 1)
		{
			AudioInstance instance = sfx.GetAudioInstance();
			instance.Volume = volume;
			instance.Pan = pan;
			instance.Pitch = pitch;

			this.SoundEffects.AddMixerInput(instance);

			instance.Play();
			return instance;
		}

		public AudioInstance PlaySoundEffect3D(AudioSource sfx, SoundEmitter emitter, SoundListener listener, ThreeDSoundChange type = ThreeDSoundChange.Default)
		{
			AudioInstance instance = this.PlaySoundEffect(sfx);

			for (int i = 0; i < this.threeDsounds.Count; i++)
			{
				if (!this.threeDsounds[i].IsActive)
				{
					this.threeDsounds[i].Setup(instance, emitter, listener, type);
					return instance;
				}
			}

			ThreeDSound sound = new ThreeDSound();
			sound.Setup(instance, emitter, listener, type);
			this.threeDsounds.Add(sound);

			return instance;
		}

		public void Update()
		{
			// this.bgm.Update();
			foreach (ThreeDSound s3D in this.threeDsounds)
				s3D.Update();
		}
	}
}