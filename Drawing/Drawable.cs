using Frozen.ECS.Components;
using Frozen.ECS.Systems;
using Frozen.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Frozen.Drawing
{
	public abstract class Drawable
	{
		public abstract void Draw(GraphicsDevice device, Camera camera);
		public abstract void Clean();
	}

	public abstract class PrimitiveDrawable : Drawable
	{ 
		protected ExpandingArray<VertexPositionColorTexture> vertices;
		protected ExpandingArray<int> indices;

		public abstract Material Material { get; protected set; }
		protected abstract Func<int, bool> ValidateVertices { get; }
		protected abstract int VerticesPerPrimitive { get; }

		public PrimitiveType PrimitiveType { get; private set; }

		protected PrimitiveDrawable(PrimitiveType pType)
		{
			this.PrimitiveType = pType;
			this.vertices = new ExpandingArray<VertexPositionColorTexture>();
			this.indices = new ExpandingArray<int>();
		}

		public override void Clean()
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

		public override void Draw(GraphicsDevice device, Camera camera)
		{
			int primitivesCount = this.indices.Count / this.VerticesPerPrimitive;

			if (primitivesCount > 0)
			{
				this.Material.EffectParameters["WorldViewProj"].SetValue(Matrix.Identity * camera.View * camera.Projection);

				foreach (EffectPass pass in this.Material.Effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					device.DrawUserIndexedPrimitives(this.PrimitiveType, this.vertices.Data, 0, this.vertices.Data.Length, this.indices.Data, 0, primitivesCount);
				}
			}
		}
	}

	internal class TriangleList : PrimitiveDrawable
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

	internal class LinesList : PrimitiveDrawable
	{
		public override Material Material { get; protected set; } = Material.FlatColor;
		protected override Func<int, bool> ValidateVertices => CoreMath.IsMultipleOf2;
		protected override int VerticesPerPrimitive => 2;

		public LinesList() : base(PrimitiveType.LineList)
		{ }
	}

	internal class CameraBoundDrawable : Drawable
	{
		private Func<Camera, IEnumerable<PrimitiveDrawable>> drawingFunc;

		public override void Draw(GraphicsDevice device, Camera camera)
		{
			foreach (PrimitiveDrawable sd in this.drawingFunc(camera))
				sd.Draw(device, camera);
		}

		public override void Clean()
		{ }

		public void Reset(Func<Camera, IEnumerable<PrimitiveDrawable>> drawingFunc)
		{
			this.drawingFunc = drawingFunc;
		}
	}
}
