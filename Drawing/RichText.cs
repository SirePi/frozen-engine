using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	internal class RichTextParagraph
	{
		public string RawText { get; set; }
		public List<RichTextPart> Parts { get; private set; } = new List<RichTextPart>();
	}

	internal class RichTextPart
	{
		public SpriteFont Font { get; set; }
		public Color Color { get; set; }
		public Color Background { get; set; }
		public string Text { get; set; }
		/*
		public void EmitVertices(Vector3 source, out VertexPositionColorTexture[] vertices, out int[] indexes, Alignment alignment = Alignment.Top, float? maxWidth = null)
		{

			Vector2 textArea = Font.MeasureString(string.Join(Environment.NewLine, rows.Select(t => t.Text)));
			Vector2 start = -textArea * .5f;
			float startX = start.X;

			vertices = new VertexPositionColorTexture[_text.Length * 4];
			indexes = new int[_text.Length * 6];

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

					vertices[vOffset + 0].Color = ColorTint;
					vertices[vOffset + 0].Position = Vector3.Transform(new Vector3(x, y, 0), matrix);
					vertices[vOffset + 0].TextureCoordinate.X = glyph.UVLeft;
					vertices[vOffset + 0].TextureCoordinate.Y = glyph.UVTop;
					vertices[vOffset + 1].Color = ColorTint;
					vertices[vOffset + 1].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y, 0), matrix);
					vertices[vOffset + 1].TextureCoordinate.X = glyph.UVRight;
					vertices[vOffset + 1].TextureCoordinate.Y = glyph.UVTop;
					vertices[vOffset + 2].Color = ColorTint;
					vertices[vOffset + 2].Position = Vector3.Transform(new Vector3(x, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
					vertices[vOffset + 2].TextureCoordinate.X = glyph.UVLeft;
					vertices[vOffset + 2].TextureCoordinate.Y = glyph.UVBottom;
					vertices[vOffset + 3].Color = ColorTint;
					vertices[vOffset + 3].Position = Vector3.Transform(new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y + glyph.Glyph.BoundsInTexture.Height, 0), matrix);
					vertices[vOffset + 3].TextureCoordinate.X = glyph.UVRight;
					vertices[vOffset + 3].TextureCoordinate.Y = glyph.UVBottom;

					iOffset = o * 6;
					indexes[iOffset + 0] = Renderer.QUAD_INDICES[0] + vOffset;
					indexes[iOffset + 1] = Renderer.QUAD_INDICES[1] + vOffset;
					indexes[iOffset + 2] = Renderer.QUAD_INDICES[2] + vOffset;
					indexes[iOffset + 3] = Renderer.QUAD_INDICES[3] + vOffset;
					indexes[iOffset + 4] = Renderer.QUAD_INDICES[4] + vOffset;
					indexes[iOffset + 5] = Renderer.QUAD_INDICES[5] + vOffset;

					start.X += glyph.Glyph.Width + glyph.Glyph.RightSideBearing;

					o++;
				}

				start.X = startX;
				start.Y += _lineHeight;
			}
		}
		*/
	}

	public class RichText
	{
		private static readonly Regex ControlRegex = new Regex(@"^\[(?<type>t|c|b)=(?<value>.+)\]");
		private static readonly string[] LineSeparators = new[]
		{
			"\n\r",
			"\n",
		};

		private string _rawText;
		private List<RichTextParagraph> _paragraphs;
		public string RawText
		{
			get
			{
				return _rawText;
			}
			set
			{
				if (_rawText != value)
				{
					_rawText = value;
					UpdateParagraphs();
				}
			}
		}

		public SpriteFont Font { get; set; }
		public Color Color { get; set; } = Color.White;
		public Color Background { get; set; } = Color.Transparent;

		public RichText(string text)
		{
			_paragraphs = new List<RichTextParagraph>();
			RawText = text;
		}

		private void UpdateParagraphs()
		{
			if (string.IsNullOrWhiteSpace(_rawText))
				_paragraphs = null;
			else
			{
				string[] rawRows = _rawText.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);

				Color color = Color;
				Color background = Background;
				SpriteFont font = Font;
				RichTextPart currentPart = null;
				int i = 0;

				for(i = 0; i < rawRows.Length; i++)
				{
					string row = rawRows[i];

					if (_paragraphs.Count > i)
					{
						if (_paragraphs[i].RawText == row)
							continue; // row is the same, can skip
					}
					else
						_paragraphs.Add(new RichTextParagraph { RawText = row });

					while(row.Length > 0)
					{
						Match ctrl = ControlRegex.Match(row);
						if(ctrl.Success)
						{
							if(currentPart != null)
							{
								_paragraphs[_paragraphs.Count - 1].Parts.Add(currentPart);
								currentPart = null;
							}

							string type = ctrl.Groups["type"].Value.ToLowerInvariant();
							string value = ctrl.Groups["value"].Value.ToLowerInvariant();
							switch (type)
							{
								case "b":
									background = string.IsNullOrWhiteSpace(value) ? Background : ColorExtensions.FromHex(value);
									break;

								case "c":
									color = string.IsNullOrWhiteSpace(value) ? Color : ColorExtensions.FromHex(value);
									break;

								case "f":
									font = string.IsNullOrWhiteSpace(value) ? Font : Font;
									break;
							}

							row = row.Substring(ctrl.Length);
						}
						else
						{
							if (currentPart == null)
								currentPart = new RichTextPart { Font = font, Color = color, Background = background };

							int nextControl = row.IndexOf('[');
							if (nextControl == -1)
								currentPart.Text = row;
							else
								currentPart.Text = row.Substring(0, nextControl);

							row = row.Substring(currentPart.Text.Length);
						}
					}

					if (currentPart != null)
					{
						_paragraphs[_paragraphs.Count - 1].Parts.Add(currentPart);
						currentPart = null;
					}
				}

				while (_paragraphs.Count > i)
					_paragraphs.Remove(_paragraphs[_paragraphs.Count - 1]);
			}
		}
		/*
		public void EmitVertices(Matrix transformMatrix, out VertexPositionColorTexture[] vertices, out int[] indexes)
		{
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
		}
		*/
	}
}
