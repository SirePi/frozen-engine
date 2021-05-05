using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenCore.ECS.Components
{
    public class Camera : Component
    {
        [RequiredComponent]
        public Transform Transform { get; private set; }
        public float FocusDistance { get; set; }
        public float FarDistance { get; set; }
        public Color ClearColor { get; set; } = Color.CornflowerBlue;

        public void TransformRenderer(Renderer r, out Vector3 renderPosition, out float renderScale, out float batchDepth, out bool isInsideViewport)
        {
            float z = r.Transform.Position.Z - this.Transform.Position.Z;
            renderScale = this.FocusDistance / z;
            renderPosition = r.Transform.Position - this.Transform.Position;
            renderPosition *= renderScale;
            batchDepth = z / this.FarDistance;

            isInsideViewport = r.Bounds.Transform2D(renderPosition.XY() - r.Origin, renderScale).Intersects(Core.Viewport);
        }
    }
}