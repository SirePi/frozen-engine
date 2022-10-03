﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frozen.Drawing
{
	public class Atlas
	{
		public static Atlas SingleSprite()
		{
			Atlas result = new Atlas();
			result.sprites.Add(UVRect.One);
			return result;
		}

		public static Atlas FromGrid(int rows, int columns)
		{
			float x = 1f / columns;
			float y = 1f / rows;

			Atlas result = new Atlas();

			for (int r = 0; r < rows; r++)
				for (int c = 0; c < columns; c++)
					result.sprites.Add(new UVRect(x * c, y * r, x, y));
			
			return result;
		}

		private readonly List<UVRect> sprites = new List<UVRect>();
		private readonly Dictionary<string, UVRect> atlas = new Dictionary<string, UVRect>();
		private readonly Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();

		private Atlas() { }

		public UVRect this[int spriteIndex] => this.sprites[spriteIndex];

		public UVRect this[string spriteName] => this.atlas[spriteName];

		public int SpriteIndexOf(string name)
		{
			return this.sprites.IndexOf(this[name]);
		}

		public Frame[] GetAnimationChain(string animation)
		{
			return this.animations[animation];
		}

		public void AddAnimationChain(string name, int fromSpriteIndex, int toSpriteIndex, float duration)
		{
			float[] durations = Enumerable.Range(fromSpriteIndex, toSpriteIndex - fromSpriteIndex + 1).Select(_ => duration).ToArray();
			this.AddAnimationChain(name, fromSpriteIndex, toSpriteIndex, durations);
		}

		public void AddAnimationChain(string name, int fromSpriteIndex, int toSpriteIndex, float[] durations)
		{
			int[] spriteIndexes = Enumerable.Range(fromSpriteIndex, toSpriteIndex - fromSpriteIndex + 1).ToArray();
			this.AddAnimationChain(name, spriteIndexes, durations);
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
	}
}