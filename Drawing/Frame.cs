namespace Frozen.Drawing
{
	public struct Frame
	{
		public int SpriteIndex { get; private set; }

		public float Duration { get; private set; }

		public Frame(int spriteIndex, float duration)
		{
			this.SpriteIndex = spriteIndex;
			this.Duration = duration;
		}
	}
}
