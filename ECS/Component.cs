using System.Reflection;

namespace Frozen.ECS
{
	public abstract class Component
	{
		private bool _isActive = true;

		public Entity Entity { get; internal set; }

		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (value != _isActive)
				{
					_isActive = value;
					if (_isActive) OnActivate();
					else OnDeactivate();
				}
			}
		}

		protected Component()
		{
			IsActive = true;
		}

		protected virtual void OnActivate()
		{ }

		protected virtual void OnDeactivate()
		{ }

		protected virtual void OnUpdate()
		{ }

		internal void UpdateRequirements()
		{
			foreach (PropertyInfo pi in Engine.RequiredComponentsCache[GetType()])
			{
				object component = Entity.Get(pi.PropertyType);

				if (component == null)
					throw new RequiredComponentNotFoundException(Entity.Name, pi.PropertyType.Name);

				pi.SetValue(this, component);
			}
		}

		public void Update(bool force = false)
		{
			if (IsActive || force)
				OnUpdate();
		}
	}
}
