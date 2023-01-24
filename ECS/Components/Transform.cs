using Microsoft.Xna.Framework;

namespace Frozen.ECS.Components
{
	public sealed class Transform : Component
	{
		private Vector3 _lastPosition;
		private Vector3 _position;
		private float _rotation;
		private float _scale = 1;
		private Matrix _transformMatrix = Matrix.Identity;

		public Matrix FullTransformMatrix
		{
			get
			{
				Matrix result = _transformMatrix;

				Entity entity = Entity;
				while (entity.Parent != null)
				{
					if (entity.Parent.Get<Transform>(out Transform t))
						result *= t._transformMatrix;

					entity = entity.Parent;
				}

				return result;
			}
		}

		public new bool IsActive => true;

		public Vector3 Position
		{
			get => _position;
			set
			{
				_position = value;
				UpdateMatrix();
			}
		}

		public float Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				UpdateMatrix();
			}
		}

		public float Scale
		{
			get => _scale;
			set
			{
				_scale = value;
				UpdateMatrix();
			}
		}

		public Vector3 Velocity { get; private set; }

		public Vector3 WorldPosition
		{
			get => Vector3.Transform(Vector3.Zero, FullTransformMatrix);
		}

		private void UpdateMatrix()
		{
			_transformMatrix = Matrix.CreateScale(_scale) * Matrix.CreateRotationZ(_rotation) * Matrix.CreateTranslation(_position);
		}

		protected override void OnUpdate()
		{
			Velocity = (_position - _lastPosition) / Time.FrameSeconds;
			_lastPosition = _position;
		}

		public void MoveBy(Vector2 movement)
		{
			MoveBy(new Vector3(movement, 0));
		}

		public void MoveBy(Vector3 movement)
		{
			Position += movement;
		}

		public void MoveBy(float x = 0, float y = 0, float z = 0)
		{
			MoveBy(new Vector3(x, y, z));
		}

		public void MoveTo(Vector3 position)
		{
			Position = position;
		}
	}
}
