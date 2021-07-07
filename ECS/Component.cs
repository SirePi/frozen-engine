using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Frozen.ECS
{
	public abstract class Component
	{
		private bool isActive = true;
		public bool IsActive
		{
			get { return this.isActive; }
			set
			{
				if (value != this.isActive)
				{
					this.isActive = value;
					if (this.isActive) this.OnActivate();
					else this.OnDeactivate();
				}
			}
		}
		public Entity Entity { get; internal set; }

		protected Component()
		{
			this.IsActive = true;
		}

		public virtual void OnActivate() { }
		public virtual void OnDeactivate() { }
		public void Update(bool force = false)
		{
			if (this.IsActive || force)
				this.OnUpdate();
		}

		protected virtual void OnUpdate() { }

		internal void UpdateRequirements()
		{
			foreach (PropertyInfo pi in Engine.RequiredComponentsCache[this.GetType()])
			{
				object component = this.Entity.Get(pi.PropertyType);

				if (component == null)
					throw new RequiredComponentNotFoundException(this.Entity.Name, pi.PropertyType.Name);

				pi.SetValue(this, component);
			}
		}
	}
}
