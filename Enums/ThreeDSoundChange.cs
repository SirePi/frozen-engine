using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.Enums
{
	[Flags]
	public enum ThreeDSoundChange 
	{
		None = 0,
		Pan = 1,
		Volume = 2,
		Pitch = 4,
		Default = Pan | Volume,
		All = Pan | Volume | Pitch
	}
}
