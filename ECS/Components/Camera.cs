using Frozen.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Frozen.ECS.Components
{
	public class Camera : Component
	{
		private readonly static float FovOver2 = MathHelper.PiOver4 / 2;
		private readonly static float FovOver2Tan = MathF.Tan(FovOver2);

		public static Camera CreateCamera(CameraType type, CameraViewportSize size, Alignment alignment, Point? margin = null)
		{
			return new Camera(type, size)
			{
				Alignment = alignment,
				Margin = margin ?? Point.Zero
			};
		}

		public static Camera CreateCamera(CameraType type, Vector2 size, bool isAbsoluteSize, Alignment alignment, Point? margin = null)
		{
			return CreateCamera(type, new CameraViewportSize(size, isAbsoluteSize), alignment, margin);
		}

		public static Camera CreateFullScreen(CameraType type)
		{
			return CreateCamera(type, Vector2.One, false, Alignment.None);
		}

		public static IEnumerable<Camera> CreateSplitScreen(CameraType type, SplitScreen split)
		{
			Vector2 cameraSize;

			switch (split)
			{
				case SplitScreen.TwoVertical:
					cameraSize = new Vector2(.5f, 1f);
					yield return CreateCamera(type, cameraSize, false, Alignment.Left);
					yield return CreateCamera(type, cameraSize, false, Alignment.Right);
					break;
				case SplitScreen.TwoHorizontal:
					cameraSize = new Vector2(1f, .5f);
					yield return CreateCamera(type, cameraSize, false, Alignment.Top);
					yield return CreateCamera(type, cameraSize, false, Alignment.Bottom);
					break;
				case SplitScreen.FourWays:
					cameraSize = new Vector2(.5f, .5f);
					yield return CreateCamera(type, cameraSize, false, Alignment.TopLeft);
					yield return CreateCamera(type, cameraSize, false, Alignment.TopRight);
					yield return CreateCamera(type, cameraSize, false, Alignment.BottomLeft);
					yield return CreateCamera(type, cameraSize, false, Alignment.BottomRight);
					break;
			}
		}

		private readonly CameraType type;
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
		public Matrix View { get; protected set; }
		public Matrix Projection { get; protected set; }
		public Alignment Alignment { get; set; } = Alignment.None;
		public Point Margin { get; set; } = Point.Zero;
		public Viewport Viewport { get; private set; }
		public Color ClearColor { get; set; } = Color.Black;
		public RenderTarget2D RenderTarget { get; private set; }

		/// <summary>
		/// Returns the Z coordinate of the plane where 1 distance unit equals 1 pixel
		/// </summary>
		public float PixelPerfectPlane
		{
			get
			{
				switch(this.type)
				{
					case CameraType.Perspective: return this.Transform.WorldPosition.Z + (this.Viewport.Height / 2) / FovOver2Tan;
					case CameraType.Orthogonal: return this.Transform.WorldPosition.Z + this.nearPlane;
					default: return this.Transform.WorldPosition.Z;
				}
			}
		}

		protected Camera(CameraType type, CameraViewportSize size)
		{
			this.type = type;
			this.size = size;
			this.nearPlane = 1;
			this.farPlane = 10000f;
			this.dirtyProjection = true;
			this.UpdateViewport();
		}

		private void UpdateViewport()
		{
			GraphicsDevice device = Engine.Game.GraphicsDevice;
			int width = (int)this.size.Size.X;
			int height = (int)this.size.Size.Y;

			if (!this.size.IsAbsoluteSize)
			{
				width = (int)(device.Viewport.Width * this.size.Size.X);
				height = (int)(device.Viewport.Height * this.size.Size.Y);
			}

			this.Viewport = new Viewport(0, 0, width, height);
			this.RenderTarget = new RenderTarget2D(device, width, height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
		}

		protected override void OnUpdate()
		{
			GraphicsDevice device = Engine.Game.GraphicsDevice;
			if (this.windowAspectRatio != device.Viewport.AspectRatio && !this.size.IsAbsoluteSize)
			{
				this.UpdateViewport();
				
				this.windowAspectRatio = device.Viewport.AspectRatio;
				this.dirtyProjection = true;
			}

			this.UpdateMatrices();
			base.OnUpdate();
		}

		private void UpdateMatrices()
		{
			if (this.dirtyProjection)
			{
				float aspectRatio = (float)this.Viewport.Width / this.Viewport.Height;
				switch (this.type)
				{
					case CameraType.Perspective:
						this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, this.nearPlane, this.farPlane);
						break;
					case CameraType.Orthogonal:
						this.Projection = Matrix.CreateOrthographic(this.Viewport.Width, this.Viewport.Height, this.nearPlane, this.farPlane);
						break;
					case CameraType.Isometric:
						throw new NotImplementedException();
				}

				this.dirtyProjection = false;
			}

			if (this.type == CameraType.Isometric)
				this.View = Matrix.CreateLookAt(this.Transform.Position, this.Transform.Position + Vector3.UnitZ, -Vector3.UnitY) * Matrix.CreateRotationZ(this.Transform.Rotation);
			else
				this.View = Matrix.CreateLookAt(this.Transform.Position, this.Transform.Position + Vector3.UnitZ, -Vector3.UnitY) * Matrix.CreateRotationZ(this.Transform.Rotation);
		}

		/// <summary>
		/// Converts a position from world to screen space; Z can be ignored
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Vector3 WorldToScreen(Vector3 position)
		{
			return this.Viewport.Project(position, this.Projection, this.View, Matrix.Identity);
		}

		/// <summary>
		/// Converts a position from screen to world space at the specified Z
		/// </summary>
		/// <param name="position"></param>
		/// <param name="targetZ"></param>
		/// <returns></returns>
		public Vector3 ScreenToWorld(Vector2 position, float targetZ)
		{
			Vector3 near = this.Viewport.Unproject(new Vector3(position, 0), this.Projection, this.View, Matrix.Identity);
			Vector3 far = this.Viewport.Unproject(new Vector3(position, 1), this.Projection, this.View, Matrix.Identity);

			float delta = (targetZ - near.Z) / (far.Z - near.Z);

			return near + (far - near) * delta;
		}

		/// <summary>
		/// Returns the scale of the graphics at the specified Z
		/// </summary>
		/// <param name="z"></param>
		/// <returns></returns>
		public float ScaleAtZ(float z)
		{
			return this.Viewport.Height / this.HeightAtZ(z);
		}

		/// <summary>
		/// Returns the visible height of the world at the specified Z
		/// </summary>
		/// <param name="z"></param>
		/// <see cref="https://discourse.threejs.org/t/functions-to-calculate-the-visible-width-height-at-a-given-z-depth-from-a-perspective-camera/269"/>
		/// <returns></returns>
		public float HeightAtZ(float z)
		{
			return Math.Abs(this.Transform.WorldPosition.Z - z) * FovOver2Tan * 2;
		}
	}
}
