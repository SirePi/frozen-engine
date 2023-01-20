using Frozen.Drawing;

namespace Frozen.ECS.Components
{
	public class SpriteAnimator : Component
	{
		[RequiredComponent]
		public SpriteRenderer Renderer { get; protected set; }

		private Frame[] frames;

		private Frame currentFrame;

		private int currentFrameIndex;

		private float time;

		private string currentAnimation;

		public void SetAnimation(string animation)
		{
			if (this.currentAnimation == animation)
				return;

			this.currentAnimation = animation;
			this.frames = this.Renderer.Material.SpriteSheet.Atlas.GetAnimationChain(this.currentAnimation);
			this.UpdateFrame(0);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.time += Time.FrameSeconds;

			if (this.currentFrame.Duration <= 0)
				return;

			while (this.time > this.currentFrame.Duration)
			{
				this.time -= this.currentFrame.Duration;
				this.UpdateFrame((this.currentFrameIndex + 1) % this.frames.Length);
			}
		}

		private void UpdateFrame(int index)
		{
			this.currentFrameIndex = index;
			this.currentFrame = this.frames[this.currentFrameIndex];
			this.Renderer.SpriteIndex = this.currentFrame.SpriteIndex;
		}
	}
}
