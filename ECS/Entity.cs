using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.ECS
{
	public class Entity : IEnumerable<Component>, IDisposable
	{
		private static readonly Component[] NoComponents = new Component[0];
		private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();
		private readonly HashSet<Entity> children = new HashSet<Entity>();

		private Component[] updateOrderedComponents;
		private bool disposedValue;
		private bool dirtyComponents;

		private Entity parent;

		public event Action<Entity, Component> OnComponentAdded;
		public event Action<Entity, Component> OnComponentRemoved;
		public event Action<Entity> OnParentChanged;

		public string Name { get; private set; }
		public object Tag { get; set; }
		public bool IsActive { get; set; } = true;
		public IEnumerable<Entity> Children => this.children;

		public Entity Parent
		{
			get { return this.parent; }
			set
			{
				Scene oldScene = this.Scene;
				Scene newScene = value?.Scene;

				if(oldScene != newScene)
				{
					oldScene?.Remove(this);
					newScene?.Add(this);
				}

				this.parent?.children.Remove(this);
				this.parent = value;
				this.parent?.children.Add(this);
			}
		}

		public Scene Scene { get; internal set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public Entity(string name) : this(name, NoComponents)
		{ }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="componentsList"></param>
		public Entity(string name, IEnumerable<Component> componentsList)
		{
			this.Name = name;

			foreach (Component cmp in componentsList)
				this.Add(cmp);
		}

		public void Add(Component component)
		{
			Type t = component.GetType();

			if (this.components.ContainsKey(t))
				throw new InvalidOperationException($"Tried to add a component of type {component.GetType()} to Entity {this.Name}, but another component of the same type is already present.");
			else
			{
				component.Entity = this;
				this.components.Add(t, component);
				OnComponentAdded?.Invoke(this, component);
				this.dirtyComponents = true;
			}
		}

		public void Remove(Component component)
		{
			Type t = component.GetType();

			if (this.components[t] != component)
				throw new InvalidOperationException($"Tried to remove a component of type {component.GetType()} to Entity {this.Name}, but it was not the one currently attached.");
			else
			{
				this.components.Remove(t);
				component.Entity = null;
				OnComponentRemoved?.Invoke(this, component);
				this.dirtyComponents = true;
			}
		}

		public void Remove<T>() where T : Component
		{
			this.Remove(this.components[typeof(T)]);
		}

		public void Clear()
		{
			while (this.components.Any())
				this.Remove(this.components.First().Value);
		}

		public bool Get<T>(out T component) where T : Component
		{
			component = this.Get<T>();
			return component != null;
		}

		public T Get<T>() where T : Component
		{
			return this.Get(typeof(T)) as T;
		}

		public bool Get(Type componentType, out Component component)
		{
			component = this.Get(componentType);
			return component != null;
		}

		public Component Get(Type componentType)
		{
			foreach(KeyValuePair<Type, Component> kvp in this.components)
			{
				if (kvp.Key == componentType || kvp.Key.IsSubclassOf(componentType))
					return kvp.Value;
			}
			return null;
		}

		public IEnumerable<T> GetAll<T>() where T: Component
		{
			Type componentType = typeof(T);
			foreach (KeyValuePair<Type, Component> kvp in this.components)
			{
				if (kvp.Key == componentType || kvp.Key.IsSubclassOf(componentType))
					yield return kvp.Value as T;
			}
		}

		public void Update(bool force = false)
		{
			if (this.IsActive || force)
			{
				if (this.dirtyComponents)
					this.RefreshComponents();

				foreach (Component component in this.updateOrderedComponents)
					component.Update(force);

				foreach (Entity child in this.children)
					child.Update(force);

				this.dirtyComponents = false;
			}
		}

		public void RefreshComponents()
		{
			foreach (Component component in this.components.Values)
				component.UpdateRequirements();

			this.updateOrderedComponents = this.components
				.OrderBy(kvp => Engine.ComponentsUpdateOrder[kvp.Key])
				.Select(kvp => kvp.Value)
				.ToArray();
		}

		public IEnumerator<Component> GetEnumerator()
		{
			return this.components.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.components.Values.GetEnumerator();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					this.components.Clear();
					this.children.Clear();
				}
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			this.Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public override string ToString()
		{
			return $"{{{this.Name}}}";
		}
	}
}
