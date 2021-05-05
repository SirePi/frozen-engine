using FrozenEngine.Drawing;
using FrozenEngine.ECS.Components;
using FrozenEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS.Systems
{
	public class DrawingSystem
	{
		private readonly List<TexturedTris> texturedTris = new List<TexturedTris>();
		private readonly ColoredLines coloredLines = new ColoredLines();
		private int trisCount;
		private int trisUsed;

		private readonly SpriteBatch batch;
		private readonly GraphicsDevice device;
		private readonly BasicEffect lineEffect;

		internal DrawingSystem(GraphicsDevice device)
		{
			this.device = device;
			this.device.BlendState = BlendState.AlphaBlend;
			this.device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

			this.batch = new SpriteBatch(device);

			this.lineEffect = new BasicEffect(device);
			this.lineEffect.TextureEnabled = false;
			this.lineEffect.VertexColorEnabled = true;
		}

		internal void DrawScene(Scene scene, GameTime gameTime)
		{
			this.Clear();

			foreach (Renderer renderer in scene.GetSortedRenderers())
				renderer.Draw(this);

			foreach (Camera camera in scene.GetCameras())
			{
				this.device.SetRenderTarget(camera.RenderTarget);
				this.device.BlendState = BlendState.AlphaBlend;
				this.device.DepthStencilState = DepthStencilState.None;
				this.device.SamplerStates[0] = SamplerState.AnisotropicClamp;
				this.device.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };

				for (int i = 0; i < this.trisUsed; i++)
				{
					TexturedTris tt = this.texturedTris[i];
					if (tt.PrimitivesCount > 0)
					{
						if (!tt.Material.IsCompiled)
							tt.Material.Compile(this.device);

						tt.Material.Effect.Projection = camera.Projection;
						tt.Material.Effect.View = camera.View;
						tt.Material.Effect.World = Matrix.Identity;

						foreach (EffectPass pass in tt.Material.Effect.CurrentTechnique.Passes)
						{
							pass.Apply();
							// this.device.DrawUserPrimitives(PrimitiveType.TriangleList, tt.Vertices, 0, tt.PrimitivesCount);
							this.device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, tt.Vertices, 0, tt.Vertices.Length, tt.Indices, 0, tt.PrimitivesCount);
						}
					}
				}

				if (this.coloredLines.PrimitivesCount > 0)
				{
					this.lineEffect.Projection = camera.Projection;
					this.lineEffect.View = camera.View;
					this.lineEffect.World = Matrix.Identity;

					foreach (EffectPass pass in this.lineEffect.CurrentTechnique.Passes)
					{
						pass.Apply();
						this.device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.coloredLines.Vertices, 0, this.coloredLines.Vertices.Length, this.coloredLines.Indices, 0, this.coloredLines.PrimitivesCount);
					}
				}
			}

			this.device.SetRenderTarget(null);
			this.device.Clear(Color.Black);

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
				ui.Draw(gameTime);
		}

		private void Clear()
		{
			this.coloredLines.Clean();

			for (int i = 0; i < this.trisCount; i++)
				this.texturedTris[i].Clean();

			this.trisUsed = 0;
		}

		public void DrawTexturedTriangles(Material material, VertexPositionColorTexture[] vertices, int[] indices)
		{
			if (this.trisUsed > 0 && this.texturedTris[this.trisUsed - 1].Material == material)
			{
				this.texturedTris[this.trisUsed - 1].AppendVertices(vertices, indices);
			}
			else
			{
				TexturedTris tris;
				if (this.trisUsed < this.trisCount)
				{
					tris = this.texturedTris[this.trisUsed];
					tris.Reset(material);
				}
				else
				{
					tris = new TexturedTris(material);
					this.texturedTris.Add(tris);
					this.trisCount++;
				}

				tris.AppendVertices(vertices, indices);

				this.trisUsed++;
			}
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

			this.DrawTexturedTriangles(Material.White, vertices, indices);
		}

		public void DrawLines(VertexPositionColor[] vertices, int[] indices)
		{
			this.coloredLines.AppendVertices(vertices, indices);
		}

		public void DrawAxisAlignedRectangle(Vector3 position, Vector2 size, Color color)
		{
			VertexPositionColor[] vertices = new VertexPositionColor[]
			{
				new VertexPositionColor{ Color = color, Position = position },
				new VertexPositionColor{ Color = color, Position = position + new Vector3(size.X, 0, 0) },
				new VertexPositionColor{ Color = color, Position = position + new Vector3(size.X, size.Y, 0) },
				new VertexPositionColor{ Color = color, Position = position + new Vector3(0, size.Y, 0) }
			};

			this.coloredLines.AppendVertices(vertices, new int[] { 0, 1, 1, 2, 2, 3, 3, 0 });
		}
	}
}
