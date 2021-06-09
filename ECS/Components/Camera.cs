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
		public static T CreateCamera<T>(CameraViewportSize size, Alignment alignment, Point? margin = null) where T : Camera
		{
			T camera = typeof(T)
				.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
				.Invoke(new object[] { size }) as T;
			camera.Alignment = alignment;
			camera.Margin = margin ?? Point.Zero;
			return camera;
		}

		public static T CreateCamera<T>(Vector2 size, bool isAbsoluteSize, Alignment alignment, Point? margin = null) where T : Camera
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

		private readonly CameraViewportSize size;
		private float windowAspectRatio;
		private float farPlane;
		private float nearPlane;
		private bool dirtyProjection;

		[RequiredComponent]
		public Transform Transform { get; protected set; }
		public float FarPlaneDistance 
		{
			get => this.farPlane;
			set
			{
				this.farPlane = value;
				this.dirtyProjection = true;
			}
		}
		public float NearPlaneDistance
		{
			get => this.nearPlane;
			set
			{
				this.nearPlane = value;
				this.dirtyProjection = true;
			}
		}
		public float PixelPerfectPlane
		{
			get
			{
				float x = this.Viewport.Bounds.Center.X;
				float y = this.Viewport.Bounds.Center.Y;

				Vector3 nearA = this.Viewport.Unproject(new Vector3(x, y, 0f), this.Projection, this.View, Matrix.Identity);
				Vector3 nearB = this.Viewport.Unproject(new Vector3(x + 1, y, 0f), this.Projection, this.View, Matrix.Identity);
				float nearDistance = (nearB - nearA).Length();

				Vector3 mid1A = this.Viewport.Unproject(new Vector3(x, y, .5f), this.Projection, this.View, Matrix.Identity);
				Vector3 mid1B = this.Viewport.Unproject(new Vector3(x + 1, y, .5f), this.Projection, this.View, Matrix.Identity);
				float mid1Distance = (mid1B - mid1A).Length();

				Vector3 mid2A = this.Viewport.Unproject(new Vector3(x, y, .75f), this.Projection, this.View, Matrix.Identity);
				Vector3 mid2B = this.Viewport.Unproject(new Vector3(x + 1, y, .75f), this.Projection, this.View, Matrix.Identity);
				float mid2Distance = (mid2B - mid2A).Length();

				Vector3 mid3A = this.Viewport.Unproject(new Vector3(x, y, .99f), this.Projection, this.View, Matrix.Identity);
				Vector3 mid3B = this.Viewport.Unproject(new Vector3(x + 1, y, .99f), this.Projection, this.View, Matrix.Identity);
				float mid3Distance = (mid3B - mid3A).Length();

				Vector3 farA = this.Viewport.Unproject(new Vector3(x, y, 1f), this.Projection, this.View, Matrix.Identity);
				Vector3 farB = this.Viewport.Unproject(new Vector3(x + 1, y, 1f), this.Projection, this.View, Matrix.Identity);
				float farDistance = (farB - farA).Length();

				float z = 1f / (farDistance - nearDistance);

				return this.Viewport.Unproject(new Vector3(0, 0, z), this.Projection, this.View, Matrix.Identity).Z;
			}
		}
		public Matrix View { get; protected set; }
		public Matrix Projection { get; protected set; }
		public Color ClearColor { get; set; } = Color.Black;
		public Alignment Alignment { get; set; } = Alignment.None;
		public Point Margin { get; set; } = Point.Zero;
		public Viewport Viewport { get; private set; }

		protected Camera(CameraViewportSize size)
		{
			this.size = size;
			this.nearPlane = .1f;
			this.farPlane = 100000f;
			this.UpdateViewport();
		}

		private void UpdateViewport()
		{
			GraphicsDevice device = Frozen.Game.GraphicsDevice;
			int width = (int)this.size.Size.X;
			int height = (int)this.size.Size.Y;

			if (!this.size.IsAbsoluteSize)
			{
				width = (int)(device.Viewport.Width * this.size.Size.X);
				height = (int)(device.Viewport.Height * this.size.Size.Y);
			}

			Point location = Point.Zero;

			if (this.Alignment.HasFlag(Alignment.Top))
				location.Y = this.Margin.Y;
			if (this.Alignment.HasFlag(Alignment.Bottom))
				location.Y = device.Viewport.Height - height - this.Margin.Y;
			if (this.Alignment.HasFlag(Alignment.Left))
				location.X = this.Margin.X;
			if (this.Alignment.HasFlag(Alignment.Right))
				location.X = device.Viewport.Width - width - this.Margin.X;

			this.Viewport = new Viewport(location.X, location.Y, (int)width, (int)height);
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			GraphicsDevice device = Frozen.Game.GraphicsDevice;
			if (this.windowAspectRatio != device.Viewport.AspectRatio && !this.size.IsAbsoluteSize)
			{
				this.UpdateViewport();
				
				this.windowAspectRatio = device.Viewport.AspectRatio;
				this.dirtyProjection = true;
			}

			if(this.dirtyProjection)
			{
				float aspectRatio = (float)this.Viewport.Width / this.Viewport.Height;
				this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, this.nearPlane, this.farPlane);
				this.dirtyProjection = false;
			}

			base.OnUpdate(gameTime);
		}

		public Vector3 WorldToScreen(Vector3 position)
		{
			return this.Viewport.Project(position, this.Projection, this.View, Matrix.Identity);
		}

		public Vector3 ScreenToWorld(Vector2 position, float targetZ)
		{
			Vector3 near = this.Viewport.Unproject(new Vector3(position, 0), this.Projection, this.View, Matrix.Identity);
			Vector3 far = this.Viewport.Unproject(new Vector3(position, 1), this.Projection, this.View, Matrix.Identity);

			float delta = (targetZ - near.Z) / (far.Z - near.Z);

			return near + (far - near) * delta;
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
