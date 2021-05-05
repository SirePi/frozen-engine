using FrozenEngine.ECS.Systems;
using FrozenEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FrozenEngine.Drawing
{
	internal abstract class Drawable<T> where T : struct, IVertexType
	{
		protected ExpandingArray<T> vertices;
		protected ExpandingArray<int> indices;
		protected abstract Func<int, bool> ValidateVertices { get; }
		protected abstract int VerticesPerPrimitive { get; }

		public int PrimitivesCount => this.indices.Count / this.VerticesPerPrimitive;
		public PrimitiveType PrimitiveType { get; private set; }
		public T[] Vertices => this.vertices.Data;
		public int[] Indices => this.indices.Data;

		protected Drawable(PrimitiveType pType)
		{
			this.PrimitiveType = pType;
			this.vertices = new ExpandingArray<T>();
			this.indices = new ExpandingArray<int>();
		}

		public virtual void Clean()
		{
			this.vertices.Clear();
			this.indices.Clear();
		}

		public void AppendVertices(T[] vertices, int[] indices)
		{
			if(!this.ValidateVertices(indices.Length))
				throw new ArgumentException("Not enough vertices");

			// Offsetting indices to match the inserted vertices
			this.indices.AddRange(indices.Select(i => i + this.vertices.Count).ToArray());
			this.vertices.AddRange(vertices);
		}
	}

	internal class TexturedTris : Drawable<VertexPositionColorTexture>
	{
		public Material Material { get; private set; }
		protected override Func<int, bool> ValidateVertices => CoreMath.IsMultipleOf3;
		protected override int VerticesPerPrimitive => 3;

		public TexturedTris(Material material) : base(PrimitiveType.TriangleList)
		{
			this.Material = material;
		}

		public void Reset(Material material)
		{
			this.Material = material;
			this.Clean();
		}
	}

	internal class ColoredLines : Drawable<VertexPositionColor>
	{
		protected override Func<int, bool> ValidateVertices => CoreMath.IsMultipleOf2;
		protected override int VerticesPerPrimitive => 2;

		public ColoredLines() : base(PrimitiveType.LineList)
		{ }
	}
}
