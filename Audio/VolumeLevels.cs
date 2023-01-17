using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frozen.Enums;

namespace Frozen.Audio
{
	public static class VolumeLevels
	{
		public static float Master { get; set; }

		public static Dictionary<AudioCategory, float> Levels { get; private set; } = Enum.GetValues(typeof(AudioCategory)).Cast<AudioCategory>().ToDictionary(k => k, v => 1f);
	}
}
