using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Frozen.Audio
{
	public abstract class AudioEffect : ISampleProvider
	{
		protected ISampleProvider _source;
		public WaveFormat WaveFormat => _source.WaveFormat;

		internal virtual void ApplyTo(ISampleProvider source)
		{
			_source = source;
		}

		public virtual int Read(float[] buffer, int offset, int count)
		{
			return _source.Read(buffer, offset, count);
		}
	}
}
