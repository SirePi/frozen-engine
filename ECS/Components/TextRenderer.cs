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
		private class GlyphInfo
		{
			public SpriteFont.Glyph Glyph { get; set; }

			public float UVBottom { get; set; }
			public float UVLeft { get; set; }
			public float UVRight { get; set; }
			public float UVTop { get; set; }
		}

		private class TextPart
		{
			public string Text { get; set; }
			public float Width { get; set; }
		}

		private static readonly string[] LineSeparators = new[]
		{
			"\n\r",
			"\n",
		};

		private bool _dirtyText;
		private SpriteFont _font;
		private Dictionary<char, GlyphInfo> _glyphs;
		private int[] _indexes;
		private float _lineHeight;
		private Material _material;
		private float _maxWidth;
		private float _spaceWidth;
		private string _text;

		private VertexPositionColorTexture[] _vertices;
		public override Rectangle Bounds => Rect;
		public Color ColorTint { get; set; } = Color.White;

		public SpriteFont Font
		{
			get => _font;
			set
			{
				_dirtyText = _font != value;
				_font = value ?? throw new ArgumentNullException(nameof(value));

				Vector2 texelSize = Vector2.One / _font.Texture.Bounds.Size.ToVector2();
				_glyphs = _font.GetGlyphs().ToDictionary(
					k => k.Key,
					v =>
					{
						Vector2 topLeft = v.Value.BoundsInTexture.Location.ToVector2() * texelSize;
						Vector2 bottomRight = (v.Value.BoundsInTexture.Location + v.Value.BoundsInTexture.Size).ToVector2() * texelSize;
						return new GlyphInfo
						{
							Glyph = v.Value,
							UVTop = topLeft.Y,
							UVBottom = bottomRight.Y,
							UVLeft = topLeft.X,
							UVRight = bottomRight.X
						};
					});

				_lineHeight = _font.MeasureString("Wq").Y;
				_spaceWidth = _glyphs[' '].Glyph.BoundsInTexture.Width + _glyphs[' '].Glyph.RightSideBearing;
				_material = Material.AlphaBlendedSprite(new Sprite(_font.Texture));
			}
		}

		public float MaxWidth
		{
			get => _maxWidth;
			set
			{
				_dirtyText = _maxWidth != value;
				if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
				float mWidth = _font.MeasureString("M").X;
				if (mWidth > value) throw new ArgumentOutOfRangeException(nameof(value));
				_maxWidth = value;
			}
		}

		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public override long RendererSortedHash => (long)Transform.Position.Z << 32 + Font.GetHashCode();

		public string Text
		{
			get => _text;
			set
			{
				_dirtyText = _text != value;
				_text = value;
			}
		}

		public TextRenderer()
		{
			Font = Frozen.Engine.ContentProvider.DefaultContent.Get<SpriteFont>("Arial.xnb");
		}

		public override void Draw(DrawingSystem drawing)
		{
			if (string.IsNullOrWhiteSpace(_text))
				return;

			drawing.DrawTexturedTriangles(_material, _vertices, _indexes);
		}

		public override void UpdateRenderer()
		{
			if (_dirtyText && !string.IsNullOrWhiteSpace(_text))
			{
				Matrix matrix = Transform.FullTransformMatrix;

				string[] srcRows = _text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
				List<TextPart> rows = new List<TextPart>();

				foreach (string srcRow in srcRows)
				{
					TextPart[] parts = srcRow
						.Split(' ')
						.Select(word => new TextPart { Text = word, Width = _font.MeasureString(word).X })
						.ToArray();

					rows.Add(new TextPart { Text = parts[0].Text, Width = parts[0].Width });

					for (int i = 1; i < parts.Length; i++)
					{
						if (rows[rows.Count - 1].Width + _spaceWidth + parts[i].Width > _maxWidth)
						{
							rows.Add(new TextPart { Text = parts[i].Text, Width = parts[i].Width });
						}
						else if (parts[i].Width > _maxWidth)
						{
							// special case.. try to cut
							float cutoff = _maxWidth / parts[i].Width;
							int cutPosition = (int)(parts[i].Width * cutoff);

							string cutString = parts[i].Text.Substring(0, cutPosition);
							float cutWidth = _font.MeasureString(cutString).X;
							rows.Add(new TextPart { Text = cutString, Width = cutWidth });
							rows.Add(new TextPart { Text = parts[i].Text.Substring(cutPosition), Width = parts[i].Width - cutWidth });
						}
						else
						{
							rows[rows.Count - 1].Text = string.Format("{0} {1}", rows[rows.Count - 1].Text, parts[i].Text);
							rows[rows.Count - 1].Width += _spaceWidth + parts[i].Width;
						}
					}
				}

				Vector2 textArea = _font.MeasureString(string.Join(Environment.NewLine, rows.Select(t => t.Text)));
				Vector2 start = -textArea * .5f;
				float startX = start.X;

				_vertices = new VertexPositionColorTexture[_text.Length * 4];
				_indexes = new int[_text.Length * 6];

				int o = 0;
				int vOffset, iOffset;
				for (int i = 0; i < rows.Count; i++)
				{
					foreach (char c in rows[i].Text)
					{
						if (!_glyphs.TryGetValue(c, out GlyphInfo glyph))
						{
							if (_font.DefaultCharacter.HasValue)
								glyph = _glyphs[_font.DefaultCharacter.Value];
							else
								continue;
						}

						vOffset = o * 4;
						float x = start.X + glyph.Glyph.Cropping.X;
						float y = start.Y + glyph.Glyph.Cropping.Y;

						_vertices[vOffset + 0].Color = ColorTint;
						_vertices[vOffset + 0].Position = Vector3.Transform(new Vector3(x, y, 0), matrix);
						_vertices[vOffset + 0].TextureCoordinate.X = glyph.UVLeft;
						_vertices[vOffset + 0].TextureCoordinate.Y = glyph.UVTop;
						_vertices[vOffset + 1].Color = ColorTint;
						_vertices[vOffset + 1].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y, 0), matrix);
						_vertices[vOffset + 1].TextureCoordinate.X = glyph.UVRight;
						_vertices[vOffset + 1].TextureCoordinate.Y = glyph.UVTop;
						_vertices[vOffset + 2].Color = ColorTint;
						_vertices[vOffset + 2].Position = Vector3.Transform(new Vector3(x, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
						_vertices[vOffset + 2].TextureCoordinate.X = glyph.UVLeft;
						_vertices[vOffset + 2].TextureCoordinate.Y = glyph.UVBottom;
						_vertices[vOffset + 3].Color = ColorTint;
						_vertices[vOffset + 3].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
						_vertices[vOffset + 3].TextureCoordinate.X = glyph.UVRight;
						_vertices[vOffset + 3].TextureCoordinate.Y = glyph.UVBottom;

						iOffset = o * 6;
						_indexes[iOffset + 0] = Renderer.QUAD_INDICES[0] + vOffset;
						_indexes[iOffset + 1] = Renderer.QUAD_INDICES[1] + vOffset;
						_indexes[iOffset + 2] = Renderer.QUAD_INDICES[2] + vOffset;
						_indexes[iOffset + 3] = Renderer.QUAD_INDICES[3] + vOffset;
						_indexes[iOffset + 4] = Renderer.QUAD_INDICES[4] + vOffset;
						_indexes[iOffset + 5] = Renderer.QUAD_INDICES[5] + vOffset;

						start.X += glyph.Glyph.Width + glyph.Glyph.RightSideBearing;

						o++;
					}

					start.X = startX;
					start.Y += _lineHeight;
				}
			}
		}
	}
}
