using System;
using System.Collections.Generic;
using Frozen.Drawing;
using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Systems
{
	public class DrawingSystem
	{
		internal SpriteBatch Batch { get; private set; }

		private readonly GraphicsDevice _device;
		private readonly List<DrawItem> _drawables = new List<DrawItem>();

		private int _drawablesUsed;

		internal DrawingSystem(GraphicsDevice device)
		{
			_device = device;
			_device.BlendState = BlendState.AlphaBlend;
			_device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

			Batch = new SpriteBatch(_device);
		}

		private void ClearDrawables()
		{
			for (int i = 0; i < _drawables.Count; i++)
				_drawables[i].Clean();

			_drawablesUsed = 0;
		}

		private T GetFreeDrawable<T>() where T : DrawItem, new()
		{
			for (int i = _drawablesUsed; i < _drawables.Count; i++)
			{
				if (_drawables[i] is T t)
				{
					if (i != _drawablesUsed)
					{
						// Swap
						DrawItem tmp = _drawables[_drawablesUsed];
						_drawables[_drawablesUsed] = t;
						_drawables[i] = tmp;
					}

					return t;
				}
			}

			T result = new T();
			_drawables.Add(result);

			return result;
		}

		internal void DrawScene(Scene scene)
		{
			ClearDrawables();

			foreach (Renderer renderer in scene.GetSortedRenderers())
				renderer.Draw(this);

			_device.SetRenderTarget(null);
			_device.Clear(Color.Black);

			foreach (Camera camera in scene.GetCameras())
			{
				_device.SetRenderTarget(camera.RenderTarget);

				_device.Clear(camera.ClearColor);
				_device.BlendState = BlendState.AlphaBlend;
				_device.DepthStencilState = DepthStencilState.Default;
				_device.SamplerStates[0] = SamplerState.AnisotropicWrap;
				_device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

				for (int i = 0; i < _drawablesUsed; i++)
					_drawables[i].Draw(_device, camera);
			}

			_device.SetRenderTarget(null);

			Batch.Begin();
			foreach (Camera camera in scene.GetCameras())
			{
				Vector2 location = Vector2.Zero;

				if (camera.Alignment.HasFlag(Alignment.Top))
					location.Y = camera.Margin.Y;
				if (camera.Alignment.HasFlag(Alignment.Bottom))
					location.Y = _device.Viewport.Height - camera.RenderTarget.Height - camera.Margin.Y;
				if (camera.Alignment.HasFlag(Alignment.Left))
					location.X = camera.Margin.X;
				if (camera.Alignment.HasFlag(Alignment.Right))
					location.X = _device.Viewport.Width - camera.RenderTarget.Width - camera.Margin.X;

				Batch.Draw(camera.RenderTarget, location, Color.White);
			}
			Batch.End();

			foreach (UI ui in scene.GetActiveComponents<UI>())
				ui.Draw();
		}

		public void DrawAxisAlignedRectangle(Vector3 position, Vector2 size, Color color)
		{
			VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[]
			{
				new VertexPositionColorTexture{ Color = color, Position = position },
				new VertexPositionColorTexture{ Color = color, Position = position + new Vector3(size.X, 0, 0) },
				new VertexPositionColorTexture{ Color = color, Position = position + new Vector3(size.X, size.Y, 0) },
				new VertexPositionColorTexture{ Color = color, Position = position + new Vector3(0, size.Y, 0) }
			};

			DrawLines(vertices, new int[] { 0, 1, 1, 2, 2, 3, 3, 0 });
		}

		public void DrawCameraBoundPrimitives(Func<Camera, IEnumerable<PrimitiveItem>> drawingFunc)
		{
			CameraBoundItem draw = GetFreeDrawable<CameraBoundItem>();
			draw.Reset(drawingFunc);

			_drawablesUsed++;
		}

		/// <summary>
		/// Adds indexed triangles to be rendered as flat colored texture. The TextureCoordinates information in the <paramref name="vertices"/> array is discarded.
		/// </summary>
		/// <param name="vertices"></param>
		/// <param name="indices"></param>
		public void DrawFlatTriangles(VertexPositionColorTexture[] vertices, int[] indices)
		{
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].TextureCoordinate = Vector2.Zero;

			DrawTexturedTriangles(Material.FlatColor, vertices, indices);
		}

		public void DrawLines(VertexPositionColorTexture[] vertices, int[] indices)
		{
			if (_drawablesUsed > 0 && _drawables[_drawablesUsed - 1] is LinesList ll)
			{
				ll.AppendVertices(vertices, indices);
			}
			else
			{
				LinesList lList = GetFreeDrawable<LinesList>();
				lList.AppendVertices(vertices, indices);

				_drawablesUsed++;
			}
		}

		public void DrawRichText(Vector3 position, RichText text)
		{
			text.Update(Batch);
			
		}

		public void DrawTexturedTriangles(Material material, VertexPositionColorTexture[] vertices, int[] indices)
		{
			if (_drawablesUsed > 0 && _drawables[_drawablesUsed - 1] is TriangleList tl && tl.Material == material)
			{
				tl.AppendVertices(vertices, indices);
			}
			else
			{
				TriangleList tList = GetFreeDrawable<TriangleList>();
				tList.Reset(material);
				tList.AppendVertices(vertices, indices);

				_drawablesUsed++;
			}
		}
	}
}
