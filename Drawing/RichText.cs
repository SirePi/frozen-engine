using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Frozen.Drawing;
using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Frozen.Drawing.Font;

namespace Frozen.Drawing
{
	internal struct Offsets
	{
		public int Vertex { get; set; }
		public int Index { get; set; }
	}

	internal class TextPart
	{
		public string Text { get; set; }
		public float Width { get; set; }
	}


	internal class RichTextSegment
	{
		public Font Font { get; set; }
		public Color Color { get; set; }
		public string Text { get; set; }
		public float Width { get; set; }
	}

	public class RichText
	{
		private static readonly Regex ControlRegex = new Regex(@"^\[(?<type>t|c)=(?<value>.+)\]");
		private const string LineSeparator = "\n";

		private bool _dirtyText;
		private bool _dirtyWidth;
		private bool _dirtyAlignment;

		private string _rawText;
		private Font _font;
		private Color _color;
		private int _maxWidth;
		private Alignment _alignment;

		private List<RichTextSegment> _parts = new List<RichTextSegment>();
		private float _lineHeight = 0;

		private VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[4 * 64];
		private int[] _indexes = new int[6 * 64];

		public string RawText
		{
			get { return _rawText; }
			set
			{
				if (_rawText != value)
				{
					_rawText = value.Replace("\r", string.Empty);

					while (_vertices.Length < _rawText.Length * 4)
					{
						_vertices = new VertexPositionColorTexture[_vertices.Length * 2];
						_indexes = new int[_indexes.Length * 2];
					}

					_dirtyText = true;
				}
			}
		}

		public Font Font
		{
			get { return _font; }
			set
			{
				if (_font != value)
				{
					_font = value;
					_dirtyText = true;
				}
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				if (_color != value)
				{
					_color = value;
					_dirtyText = true;
				}
			}
		}

		public int MaxWidth
		{
			get { return _maxWidth; }
			set
			{
				if (_maxWidth != value)
				{
					_maxWidth = value;
					_dirtyWidth = true;
				}
			}
		}
		public Alignment Alignment
		{
			get { return _alignment; }
			set
			{
				if (_alignment != value)
				{
					_alignment = value;
					_dirtyAlignment = true;
				}
			}
		}

		public RichText(string text, int? maxWidth = null, Alignment? alignment = null)
		{
			_color = Color.White;
			_maxWidth = maxWidth ?? int.MaxValue;
			_alignment = alignment ?? Alignment.TopLeft;
			RawText = text;
		}

		private void AddRichTextSegment(RichTextSegment segment)
		{
			if (segment != null)
			{
				_parts.Add(segment);
				segment.Width = segment.Font.XnaFont.MeasureString(segment.Text).X;
				_lineHeight = MathF.Max(_lineHeight, segment.Font.LineHeight);
			}
		}

		private void CheckDirtyText()
		{
			_parts.Clear();

			if (_dirtyText)
			{
				if (string.IsNullOrWhiteSpace(_rawText))
					return;

				string text = _rawText;

				Color color = Color;
				Font font = Font;
				RichTextSegment currentSegment = null;
				_lineHeight = 0;

				while (text.Length > 0)
				{
					Match ctrl = ControlRegex.Match(text);
					if (ctrl.Success)
					{
						AddRichTextSegment(currentSegment);
						currentSegment = null;

						string type = ctrl.Groups["type"].Value.ToLowerInvariant();
						string value = ctrl.Groups["value"].Value.ToLowerInvariant();
						switch (type)
						{
							case "c":
								color = string.IsNullOrWhiteSpace(value) ? Color : ColorExtensions.FromHex(value);
								break;

							case "f":
								font = string.IsNullOrWhiteSpace(value) ? Font : Font;
								break;
						}

						text = text.Substring(ctrl.Length);
					}
					else
					{
						currentSegment ??= new RichTextSegment { Font = font, Color = color };

						int nextControl = text.IndexOf('[');
						if (nextControl == -1)
							currentSegment.Text = text;
						else
							currentSegment.Text = text.Substring(0, nextControl);

						text = text.Substring(currentSegment.Text.Length);
					}
				}

				AddRichTextSegment(currentSegment);

				_dirtyText = false;
			}
		}

		public void UpdateRenderer(Alignment alignment)
		{
			CheckDirtyText();

			Offsets offsets = new Offsets();
			Vector2 location = Vector2.Zero;
			int currentWordStartIndex = 0;
			bool currentWordEnded = false;
			for(int i = 0; i < _parts.Count; i++)
			{
				RichTextSegment part = _parts[i];
				part.Font.TryGetGlyph(' ', out GlyphInfo spaceGlyph);
				float spaceWidth = spaceGlyph.Glyph.BoundsInTexture.Width + spaceGlyph.Glyph.RightSideBearing;

				Material material = Material.AlphaBlendedSprite(new Sprite(part.Font.XnaFont.Texture));

				for (int j = 0; j < part.Text.Length; j++)
				{
					char c = part.Text[j];

					switch (c)
					{
						case ' ':
							currentWordEnded = true;
							location.X += spaceWidth;
							break;

						case '\n':
							currentWordEnded = true;
							location.X = 0;
							location.Y += _lineHeight;
							break;

						default:
							if (!Font.TryGetGlyph(c, out GlyphInfo glyph))
								continue;

							if (currentWordEnded)
							{
								currentWordStartIndex = j;
								currentWordEnded = false;
							}

							float x = location.X + glyph.Glyph.Cropping.X;
							float y = location.Y + glyph.Glyph.Cropping.Y;

							_vertices[offsets.Vertex + 0].Color = Color;
							_vertices[offsets.Vertex + 0].Position = new Vector3(x, y, 0);
							_vertices[offsets.Vertex + 0].TextureCoordinate.X = glyph.UVLeft;
							_vertices[offsets.Vertex + 0].TextureCoordinate.Y = glyph.UVTop;
							_vertices[offsets.Vertex + 1].Color = Color;
							_vertices[offsets.Vertex + 1].Position = new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y, 0);
							_vertices[offsets.Vertex + 1].TextureCoordinate.X = glyph.UVRight;
							_vertices[offsets.Vertex + 1].TextureCoordinate.Y = glyph.UVTop;
							_vertices[offsets.Vertex + 2].Color = Color;
							_vertices[offsets.Vertex + 2].Position = new Vector3(x, y + glyph.Glyph.BoundsInTexture.Height, 0);
							_vertices[offsets.Vertex + 2].TextureCoordinate.X = glyph.UVLeft;
							_vertices[offsets.Vertex + 2].TextureCoordinate.Y = glyph.UVBottom;
							_vertices[offsets.Vertex + 3].Color = Color;
							_vertices[offsets.Vertex + 3].Position = new Vector3(x + glyph.Glyph.BoundsInTexture.Width, y + glyph.Glyph.BoundsInTexture.Height, 0);
							_vertices[offsets.Vertex + 3].TextureCoordinate.X = glyph.UVRight;
							_vertices[offsets.Vertex + 3].TextureCoordinate.Y = glyph.UVBottom;

							_indexes[offsets.Index + 0] = Renderer.QUAD_INDICES[0] + offsets.Vertex;
							_indexes[offsets.Index + 1] = Renderer.QUAD_INDICES[1] + offsets.Vertex;
							_indexes[offsets.Index + 2] = Renderer.QUAD_INDICES[2] + offsets.Vertex;
							_indexes[offsets.Index + 3] = Renderer.QUAD_INDICES[3] + offsets.Vertex;
							_indexes[offsets.Index + 4] = Renderer.QUAD_INDICES[4] + offsets.Vertex;
							_indexes[offsets.Index + 5] = Renderer.QUAD_INDICES[5] + offsets.Vertex;

							offsets.Vertex += 4;
							offsets.Index += 6;

							location.X += glyph.Glyph.Width + glyph.Glyph.RightSideBearing;
							break;
					}
				}
			}

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

		/*
		private void UpdateParts()
		{
			_rows.Clear();

			if (!string.IsNullOrWhiteSpace(_rawText))
			{
				string[] rawRows = _rawText.Split(LineSeparator);
				_rows.Clear();

				Color color = Color;
				Font font = Font;
				RichTextPart currentPart = null;

				for (int i = 0; i < rawRows.Length; i++)
				{
					string row = rawRows[i];
					_rows.Add(new List<RichTextPart>());

					while (row.Length > 0)
					{
						Match ctrl = ControlRegex.Match(row);
						if (ctrl.Success)
						{
							if (currentPart != null)
							{
								_rows[_rows.Count - 1].Add(currentPart);
								currentPart = null;
							}

							string type = ctrl.Groups["type"].Value.ToLowerInvariant();
							string value = ctrl.Groups["value"].Value.ToLowerInvariant();
							switch (type)
							{
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
								currentPart = new RichTextPart { Font = font, Color = color };

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
						_rows[_rows.Count - 1].Add(currentPart);
						currentPart = null;
					}
				}
			}
		}

		public int EmitVertices(Vector2 source, out VertexPositionColorTexture[] vertices, out int[] indexes, Alignment alignment = Alignment.TopLeft, int maxWidth = 0)
		{
			Offsets offsets = new Offsets();

			vertices = _vertices;
			indexes = _indexes;

			Vector2 origin = source;
			foreach (List<RichTextPart> row in _rows)
			{
				foreach (RichTextPart part in row)
				{
					Offsets newOffsets = part.FillVertices(ref _vertices, ref _indexes, offsets);



					offsets = newOffsets;
				}

				/*

				TextPart[] parts = srcRow
					.Split(' ')
					.Select(word => new TextPart { Text = word, Width = _font.MeasureString(word).X })
					.ToArray();

				_rows.Add(new TextPart { Text = parts[0].Text, Width = parts[0].Width });

				for (int i = 1; i < parts.Length; i++)
				{
					if (_rows[_rows.Count - 1].Width + _spaceWidth + parts[i].Width > _maxWidth)
					{
						_rows.Add(new TextPart { Text = parts[i].Text, Width = parts[i].Width });
					}
					else if (parts[i].Width > _maxWidth)
					{
						// special case.. try to cut
						float cutoff = _maxWidth / parts[i].Width;
						int cutPosition = (int)(parts[i].Width * cutoff);

						string cutString = parts[i].Text.Substring(0, cutPosition);
						float cutWidth = _font.MeasureString(cutString).X;
						_rows.Add(new TextPart { Text = cutString, Width = cutWidth });
						_rows.Add(new TextPart { Text = parts[i].Text.Substring(cutPosition), Width = parts[i].Width - cutWidth });
					}
					else
					{
						_rows[_rows.Count - 1].Text = string.Format("{0} {1}", _rows[_rows.Count - 1].Text, parts[i].Text);
						_rows[_rows.Count - 1].Width += _spaceWidth + parts[i].Width;
					}
				}
				*
			}

			return offsets.Vertex / 4;
		}
		*/
	}
}
