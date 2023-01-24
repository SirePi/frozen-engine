﻿using System;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public abstract class AudioInstance : ISampleProvider
	{
		private readonly VolumeSampleProvider _volume;
		private SoundState _state;
		protected readonly AudioProvider _source;

		public SoundState State
		{
			get => _state;
			private set
			{
				if (_state != value)
				{
					_state = value;
					OnStateChanged?.Invoke(this, _state);
				}
			}
		}

		public TimeSpan TimeLeft => _source.TimeLeft;

		public float Volume { get => _volume.Volume; set => _volume.Volume = MathF.Max(value, 0); }

		public WaveFormat WaveFormat => _volume?.WaveFormat;

		public event Action<AudioInstance, SoundState> OnStateChanged;

		internal AudioInstance(AudioProvider provider)
		{
			_state = SoundState.Stopped;
			_source = provider;
			_volume = new VolumeSampleProvider(SetupPipeline());
		}

		protected abstract ISampleProvider SetupPipeline();

		internal abstract void Update();

		public void Pause()
		{
			if (State == SoundState.Playing)
				State = SoundState.Paused;
		}

		public void Play()
		{
			_source.Reset();
			State = SoundState.Playing;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			switch (State)
			{
				case SoundState.Playing:
					int read = _volume.Read(buffer, offset, count);
					if (!_source.IsActive)
						Stop();

					return read;

				case SoundState.Paused:
					return SilenceGenerator.Instance.Read(buffer, offset, count);

				default:
					return 0;
			}
		}

		public void Resume()
		{
			if (State == SoundState.Paused)
				State = SoundState.Playing;
		}

		public void Stop()
		{
			State = SoundState.Stopped;
		}
	}
}
