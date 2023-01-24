namespace Frozen.Drawing
{
	public struct Frame
	{
		public float Duration { get; private set; }
		public int SpriteIndex { get; private set; }

		public Frame(int spriteIndex, float duration)
		{
			SpriteIndex = spriteIndex;
			Duration = duration;
		}
	}
}
