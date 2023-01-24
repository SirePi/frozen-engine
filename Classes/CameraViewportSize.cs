using Microsoft.Xna.Framework;

namespace Frozen
{
	public struct CameraViewportSize
	{
		public bool IsAbsoluteSize { get; private set; }

		public Vector2 Size { get; private set; }

		public CameraViewportSize(Vector2 size, bool isAbsoluteSize)
		{
			Size = size;
			IsAbsoluteSize = isAbsoluteSize;
		}
	}
}
