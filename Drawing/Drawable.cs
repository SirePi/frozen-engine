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
	internal abstract class Drawable
	{
		protected ExpandingArray<VertexPositionColorTexture> vertices;
		protected ExpandingArray<int> indices;

		public abstract Material Material { get; protected set; }
		protected abstract Func<int, bool> ValidateVertices { get; }
		protected abstract int VerticesPerPrimitive { get; }

		public int PrimitivesCount => this.indices.Count / this.VerticesPerPrimitive;
		public PrimitiveType PrimitiveType { get; private set; }
		public VertexPositionColorTexture[] Vertices => this.vertices.Data;
		public int[] Indices => this.indices.Data;

		protected Drawable(PrimitiveType pType)
		{
			this.PrimitiveType = pType;
			this.vertices = new ExpandingArray<VertexPositionColorTexture>();
			this.indices = new ExpandingArray<int>();
		}

		public virtual void Clean()
		{
			this.vertices.Clear();
			this.indices.Clear();
		}

		public void AppendVertices(VertexPositionColorTexture[] vertices, int[] indices)
		{
			if(!this.ValidateVertices(indices.Length))
				throw new ArgumentException("Not enough vertices");

			// Offsetting indices to match the inserted vertices
			this.indices.AddRange(indices.Select(i => i + this.vertices.Count).ToArray());
			this.vertices.AddRange(vertices);
		}
	}

	internal class TriangleList : Drawable
	{
		public override Material Material { get; protected set; }
		protected override Func<int, bool> ValidateVertices => CoreMath.IsMultipleOf3;
		protected override int VerticesPerPrimitive => 3;

		public TriangleList() : base(PrimitiveType.TriangleList)
		{ }

		public void Reset(Material material)
		{
			this.Material = material;
			this.Clean();
		}
	}

	internal class LinesList : Drawable
	{
		public override Material Material { get; protected set; } = Material.FlatColor;
		protected override Func<int, bool> ValidateVertices => CoreMath.IsMultipleOf2;
		protected override int VerticesPerPrimitive => 2;

		public LinesList() : base(PrimitiveType.LineList)
		{ }
	}
}
