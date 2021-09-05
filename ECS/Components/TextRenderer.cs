using Frozen.Drawing;
using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.ECS.Components
{
	public class TextRenderer : Renderer
	{
		private static readonly string[] LineSeparators = new[]
		{
			"\n\r",
			"\n",
		};

		private class TextPart
		{
			public string Text { get; set; }
			public float Width { get; set; }
		}

		private class GlyphInfo
		{
			public SpriteFont.Glyph Glyph { get; set; }
			public float UVTop { get; set; }
			public float UVBottom { get; set; }
			public float UVLeft { get; set; }
			public float UVRight { get; set; }
		}

		public Rectangle Rect { get; set; } = Rectangle.Empty;
		public Color ColorTint { get; set; } = Color.White;
		public override long RendererSortedHash => (long)this.Transform.Position.Z << 32 + this.Font.GetHashCode();
		public override Rectangle Bounds => this.Rect;
		public float MaxWidth
		{
			get => this.maxWidth;
			set
			{
				this.dirtyText = this.maxWidth != value;
				if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
				float mWidth = this.font.MeasureString("M").X;
				if (mWidth > value) throw new ArgumentOutOfRangeException(nameof(value));
				this.maxWidth = value;
			}
		}
		public SpriteFont Font
		{
			get => this.font;
			set
			{
				this.dirtyText = this.font != value;
				this.font = value ?? throw new ArgumentNullException(nameof(value));

				Vector2 texelSize = Vector2.One / this.font.Texture.Bounds.Size.ToVector2();
				this.glyphs = this.font.GetGlyphs().ToDictionary(
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

				this.lineHeight = this.font.MeasureString("Wq").Y;
				this.spaceWidth = this.glyphs[' '].Glyph.BoundsInTexture.Width + this.glyphs[' '].Glyph.RightSideBearing;
				this.material = Material.AlphaBlendedSprite(new Sprite(this.font.Texture));
			}
		}
		public string Text
		{
			get => this.text;
			set
			{
				this.dirtyText = this.text != value;
				this.text = value;
			}
		}

		private VertexPositionColorTexture[] vertices;
		private int[] indices;
		private string text;
		private float maxWidth;
		private float spaceWidth;
		private float lineHeight;
		private bool dirtyText;
		private SpriteFont font;
		private Dictionary<char, GlyphInfo> glyphs;
		private Material material;

		public TextRenderer()
		{
			this.Font = Frozen.Engine.ContentProvider.DefaultContent.Get<SpriteFont>("Arial.xnb");
		}

		public override void Draw(DrawingSystem drawing)
		{
			if (string.IsNullOrWhiteSpace(this.text))
				return;

			drawing.DrawTexturedTriangles(this.material, this.vertices, this.indices);
		}

		public override void UpdateRenderer()
		{
			if (this.dirtyText && !string.IsNullOrWhiteSpace(this.text))
			{
				Matrix matrix = this.Transform.FullTransformMatrix;

				string[] srcRows = this.text.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
				List<TextPart> rows = new List<TextPart>();

				foreach (string srcRow in srcRows)
				{
					TextPart[] parts = srcRow
						.Split(' ')
						.Select(word => new TextPart { Text = word, Width = this.font.MeasureString(word).X })
						.ToArray();

					rows.Add(new TextPart { Text = parts[0].Text, Width = parts[0].Width });

					for (int i = 1; i < parts.Length; i++)
					{
						if (rows[rows.Count - 1].Width + this.spaceWidth + parts[i].Width > this.maxWidth)
						{
							rows.Add(new TextPart { Text = parts[i].Text, Width = parts[i].Width });
						}
						else if (parts[i].Width > this.maxWidth)
						{
							// special case.. try to cut
							float cutoff = this.maxWidth / parts[i].Width;
							int cutPosition = (int)(parts[i].Width * cutoff);

							string cutString = parts[i].Text.Substring(0, cutPosition);
							float cutWidth = this.font.MeasureString(cutString).X;
							rows.Add(new TextPart { Text = cutString, Width = cutWidth });
							rows.Add(new TextPart { Text = parts[i].Text.Substring(cutPosition), Width = parts[i].Width - cutWidth });
						}
						else
						{
							rows[rows.Count - 1].Text = string.Format("{0} {1}", rows[rows.Count - 1].Text, parts[i].Text);
							rows[rows.Count - 1].Width += this.spaceWidth + parts[i].Width;
						}
					}
				}

				Vector2 textArea = this.font.MeasureString(string.Join(Environment.NewLine, rows.Select(t => t.Text)));
				Vector2 start = -textArea * .5f;
				float startX = start.X;

				this.vertices = new VertexPositionColorTexture[this.text.Length * 4];
				this.indices = new int[this.text.Length * 6];

				int o = 0;
				int vOffset, iOffset;
				for (int i = 0; i < rows.Count; i++)
				{
					foreach (char c in rows[i].Text)
					{
						if (this.glyphs.TryGetValue(c, out GlyphInfo glyph))
						{
							vOffset = o * 4;
							float x = start.X + glyph.Glyph.Cropping.X;
							float y = start.Y + glyph.Glyph.Cropping.Y;

							this.vertices[vOffset + 0].Color = this.ColorTint;
							this.vertices[vOffset + 0].Position = Vector3.Transform(new Vector3(x, y, 0), matrix);
							this.vertices[vOffset + 0].TextureCoordinate.X = glyph.UVLeft;
							this.vertices[vOffset + 0].TextureCoordinate.Y = glyph.UVTop;
							this.vertices[vOffset + 1].Color = this.ColorTint;
							this.vertices[vOffset + 1].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y, 0), matrix);
							this.vertices[vOffset + 1].TextureCoordinate.X = glyph.UVRight;
							this.vertices[vOffset + 1].TextureCoordinate.Y = glyph.UVTop;
							this.vertices[vOffset + 2].Color = this.ColorTint;
							this.vertices[vOffset + 2].Position = Vector3.Transform(new Vector3(x, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
							this.vertices[vOffset + 2].TextureCoordinate.X = glyph.UVLeft;
							this.vertices[vOffset + 2].TextureCoordinate.Y = glyph.UVBottom;
							this.vertices[vOffset + 3].Color = this.ColorTint;
							this.vertices[vOffset + 3].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
							this.vertices[vOffset + 3].TextureCoordinate.X = glyph.UVRight;
							this.vertices[vOffset + 3].TextureCoordinate.Y = glyph.UVBottom;

							iOffset = o * 6;
							this.indices[iOffset + 0] = Renderer.QUAD_INDICES[0] + vOffset;
							this.indices[iOffset + 1] = Renderer.QUAD_INDICES[1] + vOffset;
							this.indices[iOffset + 2] = Renderer.QUAD_INDICES[2] + vOffset;
							this.indices[iOffset + 3] = Renderer.QUAD_INDICES[3] + vOffset;
							this.indices[iOffset + 4] = Renderer.QUAD_INDICES[4] + vOffset;
							this.indices[iOffset + 5] = Renderer.QUAD_INDICES[5] + vOffset;

							start.X += glyph.Glyph.Width + glyph.Glyph.RightSideBearing;
						}
						else
						{
							// missing glyph
						}

						o++;
					}

					start.X = startX;
					start.Y += this.lineHeight;
				}

				/*
				this.vertices = new VertexPositionColorTexture[4];
				this.indices = Renderer.QUAD_INDICES;

				int vOffset = 0;
				this.vertices[vOffset + 0].Color = this.ColorTint;
				this.vertices[vOffset + 0].Position = Vector3.Transform(new Vector3(0, 0, 0), matrix);
				this.vertices[vOffset + 0].TextureCoordinate.X = 0;
				this.vertices[vOffset + 0].TextureCoordinate.Y = 0;
				this.vertices[vOffset + 1].Color = this.ColorTint;
				this.vertices[vOffset + 1].Position = Vector3.Transform(new Vector3(0, this.font.Texture.Height, 0), matrix);
				this.vertices[vOffset + 1].TextureCoordinate.X = 0;
				this.vertices[vOffset + 1].TextureCoordinate.Y = 1;
				this.vertices[vOffset + 2].Color = this.ColorTint;
				this.vertices[vOffset + 2].Position = Vector3.Transform(new Vector3(this.font.Texture.Width, 0, 0), matrix);
				this.vertices[vOffset + 2].TextureCoordinate.X = 1;
				this.vertices[vOffset + 2].TextureCoordinate.Y = 0;
				this.vertices[vOffset + 3].Color = this.ColorTint;
				this.vertices[vOffset + 3].Position = Vector3.Transform(new Vector3(this.font.Texture.Width, this.font.Texture.Height, 0), matrix);
				this.vertices[vOffset + 3].TextureCoordinate.X = 1;
				this.vertices[vOffset + 3].TextureCoordinate.Y = 1;
				*/
			}
		}
	}
}
