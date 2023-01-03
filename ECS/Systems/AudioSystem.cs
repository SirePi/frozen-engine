using System;
using System.Collections.Generic;
using System.Text;
using Frozen.Audio;
using Frozen.ECS.Components;
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

		private readonly Dictionary<AudioContent, Queue<AudioInstance>> audioInstances;
		private readonly List<Sound3D> threeDsounds;

		internal AudioSystem()
		{
			this.audioInstances = new Dictionary<AudioContent, Queue<AudioInstance>>();
			this.threeDsounds = new List<Sound3D>();
		}

		public void PlaySong(Song song, TimeSpan? startPosition = null)
		{
			MediaPlayer.Play(song, startPosition);
			MediaPlayer.IsRepeating = false;
		}

		public void PlaySongLooping(Song song)
		{
			MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;
		}

		public AudioInstance PlaySoundEffect(AudioContent sfx)
		{
			if(!this.audioInstances.ContainsKey(sfx))
				this.audioInstances[sfx] = new Queue<AudioInstance>();

			if (!this.audioInstances[sfx].TryDequeue(out AudioInstance instance))
			{
				instance = new AudioInstance(sfx.WaveStream);
				instance.OnStateChanged += this.OnAudioStateChanged;
			}

			instance.Play();
			
			return instance;

			/*
			SoundEffectInstance instance = sfx.SoundEffect.CreateInstance();
			instance.Play();
			return instance;
			*/
		}

		private void OnAudioStateChanged(AudioInstance instance, SoundState obj)
		{
			
		}

		public DynamicSoundEffectInstance PlayDynamicSoundEffect(AudioContent sfx, Action<DynamicSoundEffectInstance> onBufferNeeded)
		{
			/*
			DynamicSoundEffectInstance instance = sfx.GetDynamicSoundEffect();
			instance.Play();
			instance.BufferNeeded += (sender, args) => onBufferNeeded(sender as DynamicSoundEffectInstance);
			*/
			return null;
		}

		public AudioInstance PlaySoundEffect3D(AudioContent sfx, SoundEmitter emitter, SoundListener listener)
		{
			AudioInstance instance = this.PlaySoundEffect(sfx);

			for (int i = 0; i < this.threeDsounds.Count; i++)
			{
				if (!this.threeDsounds[i].IsActive)
				{
					this.threeDsounds[i].Setup(instance, emitter, listener);
					return instance;
				}
			}

			Sound3D s3D = new Sound3D();
			s3D.Setup(instance, emitter, listener);
			this.threeDsounds.Add(s3D);

			return instance;
		}

		public void Update()
		{
			foreach (Sound3D s3D in this.threeDsounds)
				s3D.Update();
		}
	}
}