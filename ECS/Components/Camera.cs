using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public abstract class Camera : Component
	{
		public static T CreateCamera<T>(CameraViewportSize size, Alignment alignment, Vector2? margin = null) where T : Camera
		{
			T camera = typeof(T)
				.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
				.Invoke(new object[] { size }) as T;
			camera.Alignment = alignment;
			camera.Margin = margin ?? Vector2.Zero;
			return camera;
		}

		public static T CreateCamera<T>(Vector2 size, bool isAbsoluteSize, Alignment alignment, Vector2? margin = null) where T : Camera
		{
			return CreateCamera<T>(new CameraViewportSize(size, isAbsoluteSize), alignment, margin);
		}

		public static T CreateFullScreen<T>() where T : Camera
		{
			return CreateCamera<T>(Vector2.One, false, Alignment.None);
		}

		public static IEnumerable<T> CreateSplitScreen<T>(SplitScreen split) where T : Camera
		{
			Vector2 cameraSize;

			switch (split)
			{
				case SplitScreen.TwoVertical:
					cameraSize = new Vector2(.5f, 1f);
					yield return CreateCamera<T>(cameraSize, false, Alignment.Left);
					yield return CreateCamera<T>(cameraSize, false, Alignment.Right);
					break;
				case SplitScreen.TwoHorizontal:
					cameraSize = new Vector2(1f, .5f);
					yield return CreateCamera<T>(cameraSize, false, Alignment.Top);
					yield return CreateCamera<T>(cameraSize, false, Alignment.Bottom);
					break;
				case SplitScreen.FourWays:
					cameraSize = new Vector2(.5f, .5f);
					yield return CreateCamera<T>(cameraSize, false, Alignment.TopLeft);
					yield return CreateCamera<T>(cameraSize, false, Alignment.TopRight);
					yield return CreateCamera<T>(cameraSize, false, Alignment.BottomLeft);
					yield return CreateCamera<T>(cameraSize, false, Alignment.BottomRight);
					break;
			}
		}

		[RequiredComponent]
		public Transform Transform { get; protected set; }
		public Matrix View { get; protected set; }
		public Matrix Projection { get; protected set; }
		public Color ClearColor { get; set; } = Color.Black;
		public Alignment Alignment { get; set; } = Alignment.None;
		public Vector2 Margin { get; set; } = Vector2.Zero;

		private readonly CameraViewportSize size;
		private float windowAspectRatio;
		public RenderTarget2D RenderTarget { get; private set; }

		protected Camera(CameraViewportSize size)
		{
			this.size = size;
			this.UpdateRenderTarget();
		}

		private void UpdateRenderTarget()
		{
			GraphicsDevice device = Core.Game.GraphicsDevice;
			float width = this.size.Size.X;
			float height = this.size.Size.Y;

			if (!this.size.IsAbsoluteSize)
			{
				width = device.Viewport.Width * this.size.Size.X;
				height = device.Viewport.Height * this.size.Size.Y;
			}
			this.RenderTarget = new RenderTarget2D(Core.Game.GraphicsDevice, (int)width, (int)height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			GraphicsDevice device = Core.Game.GraphicsDevice;
			if (this.windowAspectRatio != device.Viewport.AspectRatio && !this.size.IsAbsoluteSize)
			{
				this.UpdateRenderTarget();
				float aspectRatio = (float)this.RenderTarget.Width / this.RenderTarget.Height;
				this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, .1f, 100000f);

				this.windowAspectRatio = device.Viewport.AspectRatio;
			}

			base.OnUpdate(gameTime);
		}
	}

	public class TwoPointFiveDCamera : Camera
	{
		internal TwoPointFiveDCamera(CameraViewportSize size) : base(size)
		{ }

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			this.View = Matrix.CreateLookAt(this.Transform.Position, this.Transform.Position + Vector3.UnitZ, -Vector3.UnitY) * Matrix.CreateRotationZ(this.Transform.Rotation);
		}
	}

	public class TargetCamera : Camera
	{
		public Vector3 Target { get; set; }

		public TargetCamera(CameraViewportSize size) : base(size)
		{ }

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);

			Vector3 forward = this.Target - this.Transform.Position;
			Vector3 side = Vector3.Cross(forward, Vector3.Up);
			Vector3 up = Vector3.Cross(forward, side);

			this.View = Matrix.CreateLookAt(this.Transform.Position, this.Target, up) * Matrix.CreateRotationZ(this.Transform.Rotation);
		}
	}

	public class FreeCamera : Camera
	{
		public float Yaw { get; set; }
		public float Pitch { get; set; }

		private Vector3 translation;

		public FreeCamera(CameraViewportSize size) : base(size)
		{ }

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);

			Matrix rotation = Matrix.CreateFromYawPitchRoll(this.Yaw, this.Pitch, 0);
			this.translation = Vector3.Transform(this.translation, rotation);
			this.Transform.MoveBy(this.translation);

			Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
			Vector3 target = this.Transform.Position + forward;

			Vector3 up = Vector3.Transform(Vector3.Up, rotation);

			this.View = Matrix.CreateLookAt(this.Transform.Position, target, up);

			this.translation = Vector3.Zero;
		}

		public void Rotate(float yawChange, float pitchChange)
		{
			this.Yaw += yawChange;
			this.Pitch += pitchChange;
		}
	}
}
