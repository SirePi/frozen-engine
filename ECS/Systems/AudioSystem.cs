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
		public const float SpeedOfSound = 3400;
		private readonly List<ThreeDSound> threeDsounds;
		private BGMPlayer bgm;

		internal AudioSystem()
		{
			this.bgm = new BGMPlayer();
			this.threeDsounds = new List<ThreeDSound>();

			// just instantiate it once to make it faster later on
			using DynamicSoundEffectInstance dummy = new DynamicSoundEffectInstance(48000, AudioChannels.Stereo);
		}

		public void PlayBGM(AudioSource song, float volume = 1, float fadeInMilliseconds = 0)
		{
			this.bgm.Play(song.ToAudioProvider(), "ya");
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
			instance.Play(volume, pan, pitch);
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
			this.bgm.Update();
			foreach (ThreeDSound s3D in this.threeDsounds)
				s3D.Update();
		}
	}
}