using System;
using Frozen.Utilities;
using Microsoft.Xna.Framework.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using static Frozen.DelegatesAndEvents;

namespace Frozen.Audio
{
	public abstract class AudioInstance : IPoolable, ISampleProvider
	{
		private readonly VolumeSampleProvider _volume;
		private ISampleProvider _output;
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

		public event AudioStateChanged OnStateChanged;

		internal AudioInstance(AudioProvider provider)
		{
			_state = SoundState.Stopped;
			_source = provider;
			_volume = new VolumeSampleProvider(SetupPipeline());
			_output = _volume;
		}

		protected abstract ISampleProvider SetupPipeline();

		internal abstract void Update();

		public AudioInstance ApplyEffect(AudioEffect effect)
		{
			effect.ApplyTo(_volume);
			_output = effect;
			return this;
		}

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
					int read = _output.Read(buffer, offset, count);
					if (read < count)
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

		public void OnPickup()
		{
			_output = _volume;
		}

		public void OnReturn()
		{ }
	}
}
