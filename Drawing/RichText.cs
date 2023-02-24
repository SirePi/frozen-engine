using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FontStashSharp;
using FontStashSharp.RichText;
using Frozen.Drawing;
using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class RichText
	{
		private bool _dirty;
		private Color _color;
		private Alignment _alignment;

		private RichTextLayout _rtl;
		private RenderTarget2D _renderTarget;

		public string RawText
		{
			get { return _rtl.Text; }
			set
			{
				_dirty = _rtl.Text != value;
				_rtl.Text = value;
			}
		}

		public SpriteFontBase Font
		{
			get { return _rtl.Font; }
			set
			{
				_dirty = _rtl.Font != value;
				_rtl.Font = value;
			}
		}

		public Color Color
		{
			get { return _color; }
			set
			{
				_dirty = _color != value;
				_color = value;
			}
		}

		public int? MaxWidth
		{
			get { return _rtl.Width; }
			set
			{
				_dirty = _rtl.Width != value;
				_rtl.Width = value;
			}
		}

		public Alignment Alignment
		{
			get { return _alignment; }
			set
			{
				_dirty = _alignment != value;
				_alignment = value;
			}
		}

		internal Material Material { get; private set; }
		internal UVRect UVRect { get; private set; }
		internal Point Size { get { return _rtl.Size; } }

		public RichText(string text, int? maxWidth = null, Alignment? alignment = null)
		{
			_color = Color.White;
			_alignment = alignment ?? Alignment.TopLeft;
			_renderTarget = new RenderTarget2D(Engine.Game.GraphicsDevice, 256, 128);
			Material = Material.FromSprite(new Sprite(_renderTarget));

			_rtl = new RichTextLayout()
			{
				Text = text,
				Width = maxWidth,
			};

			_dirty = true;
		}

		private int CalculateTextureSize(int minSize)
		{
			return (int)MathF.Pow(2, MathF.Ceiling(MathF.Log(minSize, 2)));
		}

		public void Update(SpriteBatch batch)
		{
			if (_dirty)
			{
				int textureWidth = CalculateTextureSize(_rtl.Size.X);
				int textureHeigth = CalculateTextureSize(_rtl.Size.Y);

				if (_renderTarget.Width < textureWidth || _renderTarget.Height < textureHeigth)
				{ 
					_renderTarget = new RenderTarget2D(Engine.Game.GraphicsDevice, textureWidth, textureHeigth);
					Material.SpriteSheet = new Sprite(_renderTarget);
				}

				UVRect = new UVRect(Vector2.Zero, _rtl.Size.ToVector2() / _renderTarget.Bounds.Size.ToVector2());

				Engine.Game.GraphicsDevice.SetRenderTarget(_renderTarget);
				batch.Begin(SpriteSortMode.Immediate);
				batch.GraphicsDevice.Clear(Color.Transparent);
				_rtl.Draw(batch, Vector2.Zero, Color.White, horizontalAlignment: _alignment.ToFontStashSharpAlignment());
				batch.End();
				Engine.Game.GraphicsDevice.SetRenderTarget(null);

				_dirty = false;
			}
		}
	}
}
