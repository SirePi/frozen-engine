using Frozen.Drawing;

namespace Frozen.ECS.Components
{
	public class SpriteAnimator : Component
	{
		private string _currentAnimation;
		private Frame _currentFrame;
		private int _currentFrameIndex;
		private Frame[] _frames;
		private float _time;

		[RequiredComponent]
		public SpriteRenderer Renderer { get; protected set; }

		private void UpdateFrame(int index)
		{
			_currentFrameIndex = index;
			_currentFrame = _frames[_currentFrameIndex];
			Renderer.SpriteIndex = _currentFrame.SpriteIndex;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			_time += Time.FrameSeconds;

			if (_currentFrame.Duration <= 0)
				return;

			while (_time > _currentFrame.Duration)
			{
				_time -= _currentFrame.Duration;
				UpdateFrame((_currentFrameIndex + 1) % _frames.Length);
			}
		}

		public void SetAnimation(string animation)
		{
			if (_currentAnimation == animation)
				return;

			_currentAnimation = animation;
			_frames = Renderer.Material.SpriteSheet.Atlas.GetAnimationChain(_currentAnimation);
			UpdateFrame(0);
		}
	}
}
