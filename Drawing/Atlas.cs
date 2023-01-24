using System;
using System.Collections.Generic;
using System.Linq;

namespace Frozen.Drawing
{
	public class Atlas
	{
		private readonly Dictionary<string, Frame[]> _animations = new Dictionary<string, Frame[]>();
		private readonly Dictionary<string, UVRect> _atlas = new Dictionary<string, UVRect>();
		private readonly List<UVRect> _sprites = new List<UVRect>();

		private Atlas()
		{ }

		public UVRect this[int spriteIndex] => _sprites[spriteIndex];

		public UVRect this[string spriteName] => _atlas[spriteName];

		public static Atlas FromGrid(int rows, int columns)
		{
			float x = 1f / columns;
			float y = 1f / rows;

			Atlas result = new Atlas();

			for (int r = 0; r < rows; r++)
				for (int c = 0; c < columns; c++)
					result._sprites.Add(new UVRect(x * c, y * r, x, y));

			return result;
		}

		public static Atlas SingleSprite()
		{
			Atlas result = new Atlas();
			result._sprites.Add(UVRect.One);
			return result;
		}

		public void AddAnimationChain(string name, int fromSpriteIndex, int toSpriteIndex, float duration)
		{
			float[] durations = Enumerable.Range(fromSpriteIndex, toSpriteIndex - fromSpriteIndex + 1).Select(_ => duration).ToArray();
			AddAnimationChain(name, fromSpriteIndex, toSpriteIndex, durations);
		}

		public void AddAnimationChain(string name, int fromSpriteIndex, int toSpriteIndex, float[] durations)
		{
			int[] spriteIndexes = Enumerable.Range(fromSpriteIndex, toSpriteIndex - fromSpriteIndex + 1).ToArray();
			AddAnimationChain(name, spriteIndexes, durations);
		}

		public void AddAnimationChain(string name, int[] spriteIndexes, float[] durations)
		{
			if (spriteIndexes.Length != durations.Length)
				throw new ArgumentException("Provided arrays differ in length.");

			AddAnimationChain(name, spriteIndexes.Zip(durations, (s, d) => new Frame(s, d)).ToArray());
		}

		public void AddAnimationChain(string name, string[] spriteNames, float[] durations)
		{
			if (spriteNames.Length != durations.Length)
				throw new ArgumentException("Provided arrays differ in length.");

			AddAnimationChain(name, spriteNames.Zip(durations, (s, d) => new Frame(SpriteIndexOf(s), d)).ToArray());
		}

		public void AddAnimationChain(string name, params Frame[] frames)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentNullException(nameof(name));

			if (_animations.ContainsKey(name))
				throw new ArgumentException($"An animationChain with the name {name} already exists.");

			_animations[name] = frames;
		}

		public Frame[] GetAnimationChain(string animation)
		{
			return _animations[animation];
		}

		public int SpriteIndexOf(string name)
		{
			return _sprites.IndexOf(this[name]);
		}
	}
}
