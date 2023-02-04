using System;
using System.Collections.Generic;
using Frozen.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Components
{
	public class Camera : Component
	{
		private static readonly float FovOver2 = MathHelper.PiOver4 / 2;

		private static readonly float FovOver2Tan = MathF.Tan(FovOver2);

		private readonly CameraViewportSize _size;
		private readonly CameraType _type;
		private bool _dirtyProjection;
		private float _farPlane;
		private float _nearPlane;
		private float _windowAspectRatio;

		public Alignment Alignment { get; set; } = Alignment.None;

		public Color ClearColor { get; set; } = Color.Black;

		public float FarPlaneDistance
		{
			get => _farPlane;
			set
			{
				_farPlane = value;
				_dirtyProjection = true;
			}
		}

		public Point Margin { get; set; } = Point.Zero;

		public float NearPlaneDistance
		{
			get => _nearPlane;
			set
			{
				_nearPlane = value;
				_dirtyProjection = true;
			}
		}

		/// <summary>
		/// Returns the Z coordinate of the plane where 1 distance unit equals 1 pixel
		/// </summary>
		public float PixelPerfectPlane
		{
			get
			{
				switch (_type)
				{
					case CameraType.Perspective: return Transform.WorldPosition.Z + (Viewport.Height / 2) / FovOver2Tan;
					case CameraType.Orthogonal: return Transform.WorldPosition.Z + _nearPlane;
					default: return Transform.WorldPosition.Z;
				}
			}
		}

		public Matrix Projection { get; protected set; }

		public RenderTarget2D RenderTarget { get; private set; }

		[RequiredComponent]
		public Transform Transform { get; protected set; }

		public Matrix View { get; protected set; }

		public Viewport Viewport { get; private set; }

		protected Camera(CameraType type, CameraViewportSize size)
		{
			_type = type;
			_size = size;
			_nearPlane = 1;
			_farPlane = 10000f;
			_dirtyProjection = true;
			UpdateViewport();
		}

		private void UpdateMatrices()
		{
			if (_dirtyProjection)
			{
				float aspectRatio = (float)Viewport.Width / Viewport.Height;
				switch (_type)
				{
					case CameraType.Perspective:
						Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, _nearPlane, _farPlane);
						break;

					case CameraType.Orthogonal:
						Projection = Matrix.CreateOrthographic(Viewport.Width, Viewport.Height, _nearPlane, _farPlane);
						break;

					case CameraType.Isometric:
						throw new NotImplementedException();
				}

				_dirtyProjection = false;
			}

			if (_type == CameraType.Isometric)
				View = Matrix.CreateLookAt(Transform.Position, Transform.Position + Vector3.UnitZ, -Vector3.UnitY) * Matrix.CreateRotationZ(Transform.Rotation);
			else
				View = Matrix.CreateLookAt(Transform.Position, Transform.Position + Vector3.UnitZ, -Vector3.UnitY) * Matrix.CreateRotationZ(Transform.Rotation);
		}

		private void UpdateViewport()
		{
			GraphicsDevice device = Engine.Game.GraphicsDevice;
			int width = (int)_size.Size.X;
			int height = (int)_size.Size.Y;

			if (!_size.IsAbsoluteSize)
			{
				width = (int)(device.Viewport.Width * _size.Size.X);
				height = (int)(device.Viewport.Height * _size.Size.Y);
			}

			Viewport = new Viewport(0, 0, width, height);
			RenderTarget = new RenderTarget2D(device, width, height, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
		}

		protected override void OnUpdate()
		{
			GraphicsDevice device = Engine.Game.GraphicsDevice;
			if (_windowAspectRatio != device.Viewport.AspectRatio && !_size.IsAbsoluteSize)
			{
				UpdateViewport();

				_windowAspectRatio = device.Viewport.AspectRatio;
				_dirtyProjection = true;
			}

			UpdateMatrices();
			base.OnUpdate();
		}

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

		/// <summary>
		/// Returns the visible height of the world at the specified Z
		/// </summary>
		/// <param name="z"></param>
		/// <see cref="https://discourse.threejs.org/t/functions-to-calculate-the-visible-width-height-at-a-given-z-depth-from-a-perspective-camera/269"/>
		/// <returns></returns>
		public float HeightAtZ(float z)
		{
			return Math.Abs(Transform.WorldPosition.Z - z) * FovOver2Tan * 2;
		}

		/// <summary>
		/// Returns the scale of the graphics at the specified Z
		/// </summary>
		/// <param name="z"></param>
		/// <returns></returns>
		public float ScaleAtZ(float z)
		{
			return Viewport.Height / HeightAtZ(z);
		}

		/// <summary>
		/// Converts a position from screen to world space at the specified Z
		/// </summary>
		/// <param name="position"></param>
		/// <param name="targetZ"></param>
		/// <returns></returns>
		public Vector3 ScreenToWorld(Vector2 position, float targetZ)
		{
			Vector3 near = Viewport.Unproject(new Vector3(position, 0), Projection, View, Matrix.Identity);
			Vector3 far = Viewport.Unproject(new Vector3(position, 1), Projection, View, Matrix.Identity);

			float delta = (targetZ - near.Z) / (far.Z - near.Z);

			return near + (far - near) * delta;
		}

		/// <summary>
		/// Converts a position from world to screen space; Z can be ignored
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public Vector3 WorldToScreen(Vector3 position)
		{
			return Viewport.Project(position, Projection, View, Matrix.Identity);
		}
	}
}
