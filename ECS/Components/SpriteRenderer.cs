using Frozen.Drawing;
using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frozen.ECS.Components
{
	public class SpriteRenderer : Renderer
	{
		private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
		private Material material;
		private int spriteIndex;

		public Material Material
		{
			get => this.material;
			set
			{
				if (this.material != value)
				{
					this.material = value;
					this.UpdateRect();
				}
			}
		}
		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public Color ColorTint { get; set; } = Color.White;
		public int SpriteIndex 
		{
			get => this.spriteIndex;
			set
			{
				if(this.spriteIndex != value)
				{
					this.spriteIndex = value;
					this.UpdateRect();
				}
			}
		}

		public override long RendererSortedHash => this.material.DefaultSortingHash(this.Transform.Position.Z);
		public override Rectangle Bounds => this.Rect;

		private void UpdateRect()
		{
			Rectangle rect = this.material.SpriteSheet[this.spriteIndex];
			this.Rect = rect.Transform2D(-rect.Size.ToVector2() * .5f);
		}

		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawTexturedTriangles(this.Material, this.vertices, Renderer.QUAD_INDICES);
		}

		public override void UpdateRenderer()
		{
			Matrix matrix = this.Transform.FullTransformMatrix;
			UVRect uv = this.material.SpriteSheet.Atlas[this.spriteIndex];

			this.vertices[0].Color = this.ColorTint;
			this.vertices[0].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Top, 0), matrix);
			this.vertices[0].TextureCoordinate = uv.TopLeft;
			this.vertices[1].Color = this.ColorTint;
			this.vertices[1].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Top, 0), matrix);
			this.vertices[1].TextureCoordinate = uv.TopRight;
			this.vertices[2].Color = this.ColorTint;
			this.vertices[2].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Bottom, 0), matrix);
			this.vertices[2].TextureCoordinate = uv.BottomLeft;
			this.vertices[3].Color = this.ColorTint;
			this.vertices[3].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Bottom, 0), matrix);
			this.vertices[3].TextureCoordinate = uv.BottomRight;
		}
	}
}
