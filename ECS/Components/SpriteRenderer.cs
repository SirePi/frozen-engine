using Frozen.Drawing;
using Frozen.ECS.Systems;
using Frozen.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.ECS.Components
{
	public class SpriteRenderer : Renderer
	{
		private readonly VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4];
		private Material _material;
		private int _spriteIndex;

		public override Rectangle Bounds => Rect;

		public Color ColorTint { get; set; } = Color.White;

		public Material Material
		{
			get => _material;
			set
			{
				if (_material != value)
				{
					_material = value;
					UpdateRect();
				}
			}
		}

		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public override long RendererSortedHash => _material.DefaultSortingHash(Transform.Position.Z);

		public int SpriteIndex
		{
			get => _spriteIndex;
			set
			{
				if (_spriteIndex != value)
				{
					_spriteIndex = value;
					UpdateRect();
				}
			}
		}

		private void UpdateRect()
		{
			Rectangle rect = _material.SpriteSheet[_spriteIndex];
			Rect = rect.Transform2D(-rect.Size.ToVector2() * .5f);
		}

		public override void Draw(DrawingSystem drawing)
		{
			drawing.DrawTexturedTriangles(Material, _vertices, Renderer.QUAD_INDICES);
		}

		public override void UpdateRenderer()
		{
			Matrix matrix = Transform.FullTransformMatrix;
			UVRect uv = _material.SpriteSheet.Atlas[_spriteIndex];

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
		}
	}
}
