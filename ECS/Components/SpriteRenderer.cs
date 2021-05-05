using FrozenEngine.Drawing;
using FrozenEngine.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public class SpriteRenderer : Renderer
	{
		private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
		private static readonly int[] indices = new int[] { 0, 1, 2, 1, 3, 2 };
		private Material material;

		public Material Material
		{
			get { return this.material; }
			set
			{
				this.material = value;
				if (this.Rect == Rectangle.Empty)
					this.Rect = this.material.SpriteSheet.Texture.Bounds.Transform2D(-this.material.SpriteSheet.Texture.Bounds.Size.ToVector2() / 2);
			}
		}
		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public Color ColorTint { get; set; } = Color.White;

		public override long RendererSortedHash => this.material.DefaultSortingHash(this.Transform.Position.Z);
		public override Rectangle Bounds => this.Rect;
		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawTexturedTriangles(this.Material, this.vertices, SpriteRenderer.indices);
		}

		public override void UpdateRenderer(GameTime gameTime)
		{
			Matrix matrix = this.Transform.FullTransformMatrix;

			this.vertices[0].Color = this.ColorTint;
			this.vertices[0].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Top, 0), matrix);
			this.vertices[0].TextureCoordinate = Vector2.Zero;
			this.vertices[1].Color = this.ColorTint;
			this.vertices[1].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Top, 0), matrix);
			this.vertices[1].TextureCoordinate = Vector2.UnitX;
			this.vertices[2].Color = this.ColorTint;
			this.vertices[2].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Bottom, 0), matrix);
			this.vertices[2].TextureCoordinate = Vector2.UnitY;
			this.vertices[3].Color = this.ColorTint;
			this.vertices[3].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Bottom, 0), matrix);
			this.vertices[3].TextureCoordinate = Vector2.One;
		}
	}
}
