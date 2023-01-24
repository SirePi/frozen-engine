using System;
using System.Collections.Generic;
using System.Linq;
using Frozen.ECS.Components;
using Frozen.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Frozen.Drawing
{
	internal class CameraBoundItem : DrawItem
	{
		private Func<Camera, IEnumerable<PrimitiveItem>> _drawingFunc;

		public override void Clean()
		{ }

		public override void Draw(GraphicsDevice device, Camera camera)
		{
			foreach (PrimitiveItem sd in _drawingFunc(camera))
				sd.Draw(device, camera);
		}

		public void Reset(Func<Camera, IEnumerable<PrimitiveItem>> drawingFunc)
		{
			_drawingFunc = drawingFunc;
		}
	}

	internal class LinesList : PrimitiveItem
	{
		protected override Func<int, bool> ValidateVertices => FrozenMath.IsMultipleOf2;
		protected override int VerticesPerPrimitive => 2;
		public override Material Material { get; protected set; } = Material.FlatColor;

		public LinesList() : base(PrimitiveType.LineList)
		{ }
	}

	internal class TriangleList : PrimitiveItem
	{
		protected override Func<int, bool> ValidateVertices => FrozenMath.IsMultipleOf3;
		protected override int VerticesPerPrimitive => 3;
		public override Material Material { get; protected set; }

		public TriangleList() : base(PrimitiveType.TriangleList)
		{ }

		public void Reset(Material material)
		{
			Material = material;
			Clean();
		}
	}

	public abstract class DrawItem
	{
		public abstract void Clean();

		public abstract void Draw(GraphicsDevice device, Camera camera);
	}

	public abstract class PrimitiveItem : DrawItem
	{
		protected ExpandingArray<int> _indexes;
		protected ExpandingArray<VertexPositionColorTexture> _vertices;

		protected abstract Func<int, bool> ValidateVertices { get; }
		protected abstract int VerticesPerPrimitive { get; }
		public abstract Material Material { get; protected set; }
		public PrimitiveType PrimitiveType { get; private set; }

		protected PrimitiveItem(PrimitiveType pType)
		{
			PrimitiveType = pType;
			_vertices = new ExpandingArray<VertexPositionColorTexture>();
			_indexes = new ExpandingArray<int>();
		}

		public void AppendVertices(VertexPositionColorTexture[] vertices, int[] indexes)
		{
			if (!ValidateVertices(indexes.Length))
				throw new ArgumentException("Not enough vertices");

			// Offsetting indexes to match the inserted vertices
			_indexes.AddRange(indexes.Select(i => i + _vertices.Count).ToArray());
			_vertices.AddRange(vertices);
		}

		public override void Clean()
		{
			_vertices.Clear();
			_indexes.Clear();
		}

		public override void Draw(GraphicsDevice device, Camera camera)
		{
			int primitivesCount = _indexes.Count / VerticesPerPrimitive;

			if (primitivesCount > 0)
			{
				device.BlendState = Material.BlendState;
				Material.SetShaderParameters(camera.View, camera.Projection);

				foreach (EffectPass pass in Material.Effect.CurrentTechnique.Passes)
				{
					pass.Apply();
					device.DrawUserIndexedPrimitives(PrimitiveType, _vertices.Data, 0, _vertices.Data.Length, _indexes.Data, 0, primitivesCount);
				}
			}
		}
	}
}
