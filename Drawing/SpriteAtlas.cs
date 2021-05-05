using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FrozenEngine.Drawing
{
	public class SpriteAtlas
	{
		private readonly List<Rectangle> sprites = new List<Rectangle>();
		private readonly Dictionary<string, Rectangle> atlas = new Dictionary<string, Rectangle>();
		private readonly Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();

		public Rectangle this[int spriteIndex]
		{
			get { return this.sprites[spriteIndex]; }
		}

		public Rectangle this[string spriteName]
		{
			get { return this.atlas[spriteName]; }
		}

		public int SpriteIndexOf(string name)
		{
			return this.sprites.IndexOf(this[name]);
		}

		public void AddAnimationChain(string name, int[] spriteIndexes, float[] durations)
		{
			if (spriteIndexes.Length != durations.Length)
				throw new ArgumentException("Provided arrays differ in length.");

			this.AddAnimationChain(name, spriteIndexes.Zip(durations, (s, d) => new Frame(s, d)).ToArray());
		}

		public void AddAnimationChain(string name, string[] spriteNames, float[] durations)
		{
			if (spriteNames.Length != durations.Length)
				throw new ArgumentException("Provided arrays differ in length.");

			this.AddAnimationChain(name, spriteNames.Zip(durations, (s, d) => new Frame(this.SpriteIndexOf(s), d)).ToArray());
		}

		public void AddAnimationChain(string name, params Frame[] frames)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if (this.animations.ContainsKey(name))
				throw new ArgumentException($"An animationChain with the name {name} already exists.");

			this.animations[name] = frames;
		}

		public Frame[] GetAnimationChain(string animation)
		{
			return this.animations[animation];
		}
	}
}
