﻿using System;
using System.Collections.Generic;
using System.Text;
using Frozen.Audio;
using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Frozen.ECS.Systems
{
	public class AudioSystem
	{
		private readonly List<Sound3D> threeDsounds;
		public event Action<MediaState> OnMediaStateChanged;

		internal AudioSystem()
		{
			this.threeDsounds = new List<Sound3D>();
			MediaPlayer.MediaStateChanged += this.MediaPlayer_MediaStateChanged;
		}

		private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
		{
			this.OnMediaStateChanged?.Invoke(MediaPlayer.State);
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

		public SoundEffectInstance PlaySoundEffect(SoundEffect sfx)
		{
			SoundEffectInstance instance = sfx.CreateInstance();
			instance.Play();
			return instance;
		}

		public SoundEffectInstance PlaySoundEffect3D(SoundEffect sfx, SoundEmitter emitter, SoundListener listener)
		{
			SoundEffectInstance instance = sfx.CreateInstance();
			bool added = false;
			
			for(int i = 0; i <this.threeDsounds.Count; i++)
			{
				if (!this.threeDsounds[i].IsActive)
				{
					this.threeDsounds[i].Setup(instance, emitter, listener);
					added = true;
				}
			}

			if(!added)
			{
				Sound3D s3D = new Sound3D();
				s3D.Setup(instance, emitter, listener);
				this.threeDsounds.Add(s3D);
			}

			instance.Play();
			return instance;
		}

		public void Update()
		{
			foreach(Sound3D s in this.threeDsounds)
			{
				if(s.IsActive)
					s.Update();
			}
		}
	}
}
