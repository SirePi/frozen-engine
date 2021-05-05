﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace FrozenEngine.ECS.Components
{
	public sealed class Transform : Component
	{
		private Vector3 position;
		private float rotation;
		private float scale = 1;
		private Matrix transformMatrix;

		public new bool IsActive => true;

		public Vector3 Position
		{
			get => this.position;
			set
			{
				this.position = value;
				this.UpdateMatrix();
			}
		}
		public float Rotation
		{
			get => this.rotation;
			set
			{
				this.rotation = value;
				this.UpdateMatrix();
			}
		}

		public float Scale 
		{
			get => this.scale;
			set
			{
				this.scale = value;
				this.UpdateMatrix();
			}
		}

		private void UpdateMatrix()
		{
			this.transformMatrix = Matrix.CreateTranslation(this.position) * Matrix.CreateRotationZ(this.rotation) * Matrix.CreateScale(this.scale);
		}

		public void MoveBy(Vector3 movement)
		{
			this.Position += movement;
		}

		public void MoveBy(float x = 0, float y = 0, float z = 0)
		{
			this.MoveBy(new Vector3(x, y, z));
		}

		public void MoveTo(Vector3 position)
		{
			this.Position = position;
		}

		public Matrix FullTransformMatrix
		{
			get
			{
				Matrix result = this.transformMatrix;

				Entity entity = this.Entity;
				while (entity.Parent != null)
				{
					if (entity.Parent.Get<Transform>(out Transform t))
						result = t.transformMatrix * result;

					entity = entity.Parent;
				}

				return result;
			}
		}
	}
}
