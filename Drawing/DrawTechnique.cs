using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class DrawTechnique
	{
		public static DrawTechnique FlatColor { get; private set; } = new DrawTechnique(new BasicEffect(Engine.Game.GraphicsDevice) { TextureEnabled = false, VertexColorEnabled = true });

		internal Effect Effect { get; private set; }

		public DrawTechnique(Effect baseEffect)
		{
			this.Effect = baseEffect;
		}
	}
}
