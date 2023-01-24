using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	public class DrawTechnique
	{
		internal Effect Effect { get; private set; }
		public static DrawTechnique FlatColor { get; private set; } = new DrawTechnique(new BasicEffect(Engine.Game.GraphicsDevice) { TextureEnabled = false, VertexColorEnabled = true });

		public DrawTechnique(Effect baseEffect)
		{
			Effect = baseEffect;
		}
	}
}
