using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine
{
	public struct CameraViewportSize
	{
		public Vector2 Size { get; private set; }
		public bool IsAbsoluteSize { get; private set; }

		public CameraViewportSize(Vector2 size, bool isAbsoluteSize)
		{
			this.Size = size;
			this.IsAbsoluteSize = isAbsoluteSize;
		}
	}
}
