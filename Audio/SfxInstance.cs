using System;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Enums;
using Microsoft.Xna.Framework;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Frozen.Audio
{
	public class SfxInstance : AudioInstance
	{
		private SoundEmitter _emitter;
		private SoundListener _listener;
		private PanningSampleProvider _pan;
		private SmbPitchShiftingSampleProvider _pitch;
		private bool _updatePan;
		private bool _updatePitch;
		private bool _updateVolume;

		public float Pan
		{
			get
			{
				if (_pan != null)
					return _pan.Pan;
				else return 0;
			}
			set
			{
				if (_pan != null)
					_pan.Pan = MathHelper.Clamp(value, -1, 1);
			}
		}

		public float Pitch { get => _pitch.PitchFactor; set => _pitch.PitchFactor = value; }

		internal SfxInstance(AudioProvider provider) : base(provider)
		{ }

		protected override ISampleProvider SetupPipeline()
		{
			_pitch = new SmbPitchShiftingSampleProvider(_source.SampleProvider);
			if (_pitch.WaveFormat.Channels == 1)
			{
				_pan = new PanningSampleProvider(_pitch);
				return _pan;
			}
			else
				return _pitch;
		}

		internal override void Update()
		{
			if (_source.IsActive && _emitter != null && _listener != null)
			{
				Vector3 distance = _emitter.Transform.WorldPosition - _listener.Transform.WorldPosition;
				float length = distance.Length();

				if (_updatePitch)
				{
					// https://gamedev.stackexchange.com/q/23583
					float vrr = Vector3.Dot(_listener.Transform.Velocity, distance) / length;
					float vsr = Vector3.Dot(_emitter.Transform.Velocity, distance) / length;
					float pitch = (AudioSystem.SpeedOfSound + vrr) / (AudioSystem.SpeedOfSound + vsr);
					Pitch = pitch;
				}

				if (_updatePan)
				{
					Vector3 relativePosition = Vector3.Transform(_emitter.Transform.WorldPosition, _listener.Camera.View * _listener.Camera.Projection);
					float angle = MathF.Atan2(relativePosition.Z, relativePosition.X) - MathHelper.PiOver2;
					Pan = -MathF.Sin(angle);
				}

				if (_updateVolume)
				{
					Volume = MathHelper.Clamp(1 - ((length - _listener.FullVolumeDistance) / _listener.CutOffDistance), 0, 1) * _emitter.Volume;
				}
			}
			else
			{
				_emitter = null;
				_listener = null;
			}
		}

		public void Apply3D(SoundEmitter emitter, SoundListener listener, ThreeDSoundChange change = ThreeDSoundChange.Default)
		{
			_emitter = emitter;
			_listener = listener;
			_updatePitch = change.HasFlag(ThreeDSoundChange.Pitch);
			_updatePan = change.HasFlag(ThreeDSoundChange.Pan);
			_updateVolume = change.HasFlag(ThreeDSoundChange.Volume);

			Update();
		}
	}
}
