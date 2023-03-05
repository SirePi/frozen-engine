using System;
using System.Collections.Generic;
using System.Linq;
using Frozen.Drawing;
using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Components
{
	public class TextRenderer : Renderer
	{
		public RichText Text { get; set; }

		private VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];
		public override Rectangle Bounds => Rect;
		public Color ColorTint { get; set; } = Color.White;

		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public override long RendererSortedHash => (long)Transform.Position.Z << 32 + Text.GetHashCode();

		public override void Draw(DrawingSystem drawing)
		{
			bool updated = Text.UpdateIfNeeded(drawing.Batch);

			if (updated)
			{
				switch (Text.Alignment)
				{
					case Alignment.TopLeft: Rect = new Rectangle(0 , 0, Text.Size.X, Text.Size.Y); break;
					case Alignment.Top: Rect = new Rectangle(-Text.Size.X / 2, 0, Text.Size.X, Text.Size.Y); break;
					case Alignment.TopRight: Rect = new Rectangle(-Text.Size.X, 0, Text.Size.X, Text.Size.Y); break;
					case Alignment.Left: Rect = new Rectangle(0, -Text.Size.Y / 2, Text.Size.X, Text.Size.Y); break;
					case Alignment.Center: Rect = new Rectangle(-Text.Size.X / 2, -Text.Size.Y / 2, Text.Size.X, Text.Size.Y); break;
					case Alignment.Right: Rect = new Rectangle(-Text.Size.X, -Text.Size.Y / 2, Text.Size.X, Text.Size.Y); break;
					case Alignment.BottomLeft: Rect = new Rectangle(0, -Text.Size.Y, Text.Size.X, Text.Size.Y); break;
					case Alignment.Bottom: Rect = new Rectangle(-Text.Size.X / 2, -Text.Size.Y, Text.Size.X, Text.Size.Y); break;
					case Alignment.BottomRight: Rect = new Rectangle(-Text.Size.X, -Text.Size.Y, Text.Size.X, Text.Size.Y); break;
				}
			}

			Matrix matrix = Transform.FullTransformMatrix;
			UVRect uv = Text.UVRect;

			_vertices[0].Color = ColorTint;
			_vertices[0].Position = Vector3.Transform(new Vector3(Rect.Left, Rect.Top, 0), matrix);
			_vertices[0].TextureCoordinate = uv.TopLeft;
			_vertices[1].Color = ColorTint;
			_vertices[1].Position = Vector3.Transform(new Vector3(Rect.Right, Rect.Top, 0), matrix);
			_vertices[1].TextureCoordinate = uv.TopRight;
			_vertices[2].Color = ColorTint;
			_vertices[2].Position = Vector3.Transform(new Vector3(Rect.Left, Rect.Bottom, 0), matrix);
			_vertices[2].TextureCoordinate = uv.BottomLeft;
			_vertices[3].Color = ColorTint;
			_vertices[3].Position = Vector3.Transform(new Vector3(Rect.Right, Rect.Bottom, 0), matrix);
			_vertices[3].TextureCoordinate = uv.BottomRight;

			drawing.DrawTexturedTriangles(Text.Material, _vertices, Renderer.QUAD_INDICES);
		}

		public override void UpdateRenderer()
		{
			// doing everything inside Draw method due to the need to temporarily use the SpriteBatch
		}
	}
}
