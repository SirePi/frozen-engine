using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS
{
	public class Entity : IEnumerable<Component>, IDisposable
	{
		private static readonly Component[] NoComponents = new Component[0];
		private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();
		private readonly HashSet<Entity> children = new HashSet<Entity>();
		private bool disposedValue;

		private Scene scene;

		public event Action<Entity, Component> OnComponentAdded;
		public event Action<Entity, Component> OnComponentRemoved;
		public event Action<Entity> OnParentChanged;

		public string Name { get; private set; }
		public object Tag { get; set; }
		public bool IsActive { get; set; } = true;
		public IEnumerable<Entity> Children => this.children;

		public Entity Parent { get; private set; }

		public Scene Scene
		{
			get { return this.Parent == null ? this.scene : this.Parent.Scene; }
			internal set
			{
				this.scene = value;
				foreach (Entity child in this.Children)
					child.Scene = value;
			}
		}

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

		private void AddChild(Entity child)
		{
			if (!this.children.Contains(child))
			{
				this.children.Add(child);
				this.Scene.AddEntity(child);
				child.Parent = this;
			}
		}

		private void RemoveChild(Entity child)
		{
			if (this.children.Contains(child))
			{
				this.children.Remove(child);
				child.Parent = null;
			}
		}

		public void AttachToParent(Entity parent)
		{
			if (parent == null)
				this.DetachFromParent();
			else if (this.Parent != parent)
			{
				this.Parent?.RemoveChild(this);
				parent.AddChild(this);
				OnParentChanged?.Invoke(this);
			}
		}

		public void DetachFromParent()
		{
			this.Parent?.RemoveChild(this);
			OnParentChanged?.Invoke(this);
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

		public void Update(GameTime gameTime, bool force = false)
		{
			if (this.IsActive || force)
			{
				foreach (Component component in this.components.Values)
					component.Update(gameTime, force);

				foreach (Entity child in this.children)
					child.Update(gameTime, force);
			}
		}

		public void Refresh()
		{
			foreach (Component component in this.components.Values)
				component.UpdateRequirements();
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
