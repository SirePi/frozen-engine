using Frozen.Drawing;
using Frozen.ECS.Components;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.ECS.Systems
{
	public class DrawingSystem
	{
		private readonly List<Drawable> drawables = new List<Drawable>();
		private int drawablesUsed;

		private readonly SpriteBatch batch;
		private readonly GraphicsDevice device;

		internal DrawingSystem(GraphicsDevice device)
		{
			this.device = device;
			this.device.BlendState = BlendState.AlphaBlend;
			this.device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

			this.batch = new SpriteBatch(device);
		}

		internal void DrawScene(Scene scene)
		{
			this.ClearDrawables();

			foreach (Renderer renderer in scene.GetSortedRenderers())
				renderer.Draw(this);

			this.device.SetRenderTarget(null);
			this.device.Clear(Color.Black);
			
			foreach (Camera camera in scene.GetCameras())
			{
				this.device.SetRenderTarget(camera.RenderTarget);

				this.device.Clear(camera.ClearColor);
				this.device.BlendState = BlendState.AlphaBlend;
				this.device.DepthStencilState = DepthStencilState.Default;
				this.device.SamplerStates[0] = SamplerState.AnisotropicWrap;
				this.device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

				for (int i = 0; i < this.drawablesUsed; i++)
					this.drawables[i].Draw(this.device, camera);
			}

			this.device.SetRenderTarget(null);

			this.batch.Begin();
			foreach (Camera camera in scene.GetCameras())
			{
				Vector2 location = Vector2.Zero;

				if (camera.Alignment.HasFlag(Alignment.Top))
					location.Y = camera.Margin.Y;
				if (camera.Alignment.HasFlag(Alignment.Bottom))
					location.Y = this.device.Viewport.Height - camera.RenderTarget.Height - camera.Margin.Y;
				if (camera.Alignment.HasFlag(Alignment.Left))
					location.X = camera.Margin.X;
				if (camera.Alignment.HasFlag(Alignment.Right))
					location.X = this.device.Viewport.Width - camera.RenderTarget.Width - camera.Margin.X;

				this.batch.Draw(camera.RenderTarget, location, Color.White);
			}
			this.batch.End();

			foreach (UI ui in scene.GetActiveComponents<UI>())
				ui.Draw();
		}

		private void ClearDrawables()
		{
			for (int i = 0; i < this.drawables.Count; i++)
				this.drawables[i].Clean();

			this.drawablesUsed = 0;
		}

		public void DrawTexturedTriangles(Material material, VertexPositionColorTexture[] vertices, int[] indices)
		{
			if (this.drawablesUsed > 0 && this.drawables[this.drawablesUsed - 1] is TriangleList tl && tl.Material == material)
			{
				tl.AppendVertices(vertices, indices);
			}
			else
			{
				TriangleList tList = this.GetFreeDrawable<TriangleList>();
				tList.Reset(material);
				tList.AppendVertices(vertices, indices);

				this.drawablesUsed++;
			}
		}

		private T GetFreeDrawable<T>() where T : Drawable, new()
		{
			for (int i = this.drawablesUsed; i < this.drawables.Count; i++)
			{
				if (this.drawables[i] is T t)
				{
					if (i != this.drawablesUsed)
					{
						// Swap
						Drawable tmp = this.drawables[this.drawablesUsed];
						this.drawables[this.drawablesUsed] = t;
						this.drawables[i] = tmp;
					}

					return t;
				}
			}

			T result = new T();
			this.drawables.Add(result);

			return result;
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

			this.DrawTexturedTriangles(Material.FlatColor, vertices, indices);
		}

		public void DrawCameraBoundPrimitives(Func<Camera, IEnumerable<PrimitiveDrawable>> drawingFunc)
		{
			CameraBoundDrawable draw = this.GetFreeDrawable<CameraBoundDrawable>();
			draw.Reset(drawingFunc);

			this.drawablesUsed++;
		}

		public void DrawLines(VertexPositionColorTexture[] vertices, int[] indices)
		{
			if (this.drawablesUsed > 0 && this.drawables[this.drawablesUsed - 1] is LinesList ll)
			{
				ll.AppendVertices(vertices, indices);
			}
			else
			{
				LinesList lList = this.GetFreeDrawable<LinesList>();
				lList.AppendVertices(vertices, indices);

				this.drawablesUsed++;
			}
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

			this.DrawLines(vertices, new int[] { 0, 1, 1, 2, 2, 3, 3, 0 });
		}
	}
}
