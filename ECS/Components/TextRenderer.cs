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
		private class TextPart
		{
			public string Text { get; set; }
			public float Width { get; set; }
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
				if (value <= 0) throw new ArgumentOutOfRangeException(nameof(this.MaxWidth));
				float mWidth = this.font.MeasureString("M").X;
				if (mWidth > value) throw new ArgumentOutOfRangeException(nameof(this.MaxWidth));
				this.maxWidth = value;
			}
		}
		public SpriteFont Font 
		{
			get => this.font;
			set
			{
				this.dirtyText = this.font != value;
				this.font = value ?? throw new ArgumentNullException(nameof(this.Font));
				this.glyphs = this.font.GetGlyphs();

				this.lineHeight = this.font.MeasureString("Wq").Y;
				this.spaceWidth = this.glyphs[' '].Width;
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

		private VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[400];
		private int[] indices = new int[600];
		private string text;
		private float maxWidth;
		private float spaceWidth;
		private float lineHeight;
		private bool dirtyText;
		private SpriteFont font;
		private Dictionary<char, SpriteFont.Glyph> glyphs;
		private Material material;

		public TextRenderer()
		{
			this.Font = Frozen.Engine.ContentProvider.DefaultContent.Get<SpriteFont>("Arial");
		}

		public override void Draw(DrawingSystem drawing)
		{
			if (string.IsNullOrWhiteSpace(this.text))
				return;

			drawing.DrawTexturedTriangles(this.material, this.vertices, Renderer.QUAD_INDICES);
		}

		public override void UpdateRenderer()
		{
			if (this.dirtyText && !string.IsNullOrWhiteSpace(this.text))
			{
				Matrix matrix = this.Transform.FullTransformMatrix;

				TextPart[] parts = this.text
					.Split(' ')
					.Select(word => new TextPart { Text = word, Width = this.font.MeasureString(word).X })
					.ToArray();

				List<TextPart> rows = new List<TextPart>();
				rows.Add(new TextPart { Text = parts[0].Text, Width = parts[0].Width });

				for(int i = 1; i < parts.Length; i++)
				{
					if(rows[rows.Count - 1].Width + this.spaceWidth + parts[i].Width > this.maxWidth)
					{
						rows.Add(new TextPart { Text = parts[i].Text, Width = parts[i].Width });
					}
					else if(parts[i].Width > this.maxWidth)
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
						rows[rows.Count - 1].Text += ' ' + parts[i].Text;
						rows[rows.Count - 1].Width += this.spaceWidth + parts[i].Width;
					}
				}

				Vector2 textArea = this.font.MeasureString(string.Join(Environment.NewLine, rows.Select(t => t.Text)));

				if (this.text.Length * 4 < this.vertices.Length)
				{
					this.vertices = new VertexPositionColorTexture[this.text.Length * 4 * 2];
					this.indices = new int[this.text.Length * 6 * 2];
				}

				for(int i = 0; i < rows.Count; i++)
				{
					foreach (char c in rows[i].Text)
					{
						if (this.glyphs.TryGetValue(this.text[i], out SpriteFont.Glyph glyph))
						{
							int offset = i * 4;
							this.vertices[offset + 0].Color = this.ColorTint;
							this.vertices[offset + 0].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Top, 0), matrix);
							this.vertices[offset + 0].TextureCoordinate = Vector2.Zero;
							this.vertices[offset + 1].Color = this.ColorTint;
							this.vertices[offset + 1].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Top, 0), matrix);
							this.vertices[offset + 1].TextureCoordinate = Vector2.UnitY;
							this.vertices[offset + 2].Color = this.ColorTint;
							this.vertices[offset + 2].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Bottom, 0), matrix);
							this.vertices[offset + 2].TextureCoordinate = Vector2.UnitX;
							this.vertices[offset + 3].Color = this.ColorTint;
							this.vertices[offset + 3].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Bottom, 0), matrix);
							this.vertices[offset + 3].TextureCoordinate = Vector2.One;
						}
					}
				}

				this.vertices[0].Color = this.ColorTint;
				this.vertices[0].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Top, 0), matrix);
				this.vertices[0].TextureCoordinate = Vector2.Zero;
				//this.vertices[0].TextureCoordinate = uv.TopLeft;
				this.vertices[1].Color = this.ColorTint;
				this.vertices[1].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Top, 0), matrix);
				this.vertices[1].TextureCoordinate = Vector2.UnitY;
				//this.vertices[1].TextureCoordinate = uv.TopRight;
				this.vertices[2].Color = this.ColorTint;
				this.vertices[2].Position = Vector3.Transform(new Vector3(this.Rect.Left, this.Rect.Bottom, 0), matrix);
				this.vertices[2].TextureCoordinate = Vector2.UnitX;
				//this.vertices[2].TextureCoordinate = uv.BottomLeft;
				this.vertices[3].Color = this.ColorTint;
				this.vertices[3].Position = Vector3.Transform(new Vector3(this.Rect.Right, this.Rect.Bottom, 0), matrix);
				this.vertices[3].TextureCoordinate = Vector2.One;

				if (this.text)
			}
		}
	}
}
