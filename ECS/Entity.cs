using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Frozen.DelegatesAndEvents;

namespace Frozen.ECS
{
	public class Entity : IEnumerable<Component>, IDisposable
	{
		private static readonly Component[] NoComponents = new Component[0];

		private readonly HashSet<Entity> _children = new HashSet<Entity>();
		private readonly Dictionary<Type, Component> _components = new Dictionary<Type, Component>();
		private bool _dirtyComponents;
		private bool _disposedValue;
		private Entity _parent;
		private Component[] _updateOrderedComponents;
		public IEnumerable<Entity> Children => _children;

		public bool IsActive { get; set; } = true;

		public string Name { get; private set; }

		public Entity Parent
		{
			get { return _parent; }
			set
			{
				Scene oldScene = Scene;
				Scene newScene = value?.Scene;

				if (oldScene != newScene)
				{
					oldScene?.Remove(this);
					newScene?.Add(this);
				}

				if (_parent != value)
				{
					_parent?._children.Remove(this);
					_parent = value;
					_parent?._children.Add(this);

					OnParentChanged?.Invoke(this);
				}
			}
		}

		public Scene Scene { get; internal set; }

		public object Tag { get; set; }

		public event EntityComponentEvent OnComponentAdded;

		public event EntityComponentEvent OnComponentRemoved;

		public event EntityEvent OnParentChanged;

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
			Name = name;

			foreach (Component cmp in componentsList)
				Add(cmp);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_components.Clear();
					_children.Clear();
				}
				_disposedValue = true;
			}
		}

		public void Add(Component component)
		{
			Type t = component.GetType();

			if (_components.ContainsKey(t))
				throw new InvalidOperationException($"Tried to add a component of type {component.GetType()} to Entity {Name}, but another component of the same type is already present.");
			else
			{
				component.Entity = this;
				_components.Add(t, component);
				OnComponentAdded?.Invoke(this, component);
				_dirtyComponents = true;
			}
		}

		public void Clear()
		{
			while (_components.Any())
				Remove(_components.First().Value);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public bool Get<T>(out T component) where T : Component
		{
			component = Get<T>();
			return component != null;
		}

		public T Get<T>() where T : Component
		{
			return Get(typeof(T)) as T;
		}

		public bool Get(Type componentType, out Component component)
		{
			component = Get(componentType);
			return component != null;
		}

		public Component Get(Type componentType)
		{
			foreach (KeyValuePair<Type, Component> kvp in _components)
			{
				if (kvp.Key == componentType || kvp.Key.IsSubclassOf(componentType))
					return kvp.Value;
			}
			return null;
		}

		public IEnumerable<T> GetAll<T>() where T : Component
		{
			Type componentType = typeof(T);
			foreach (KeyValuePair<Type, Component> kvp in _components)
			{
				if (kvp.Key == componentType || kvp.Key.IsSubclassOf(componentType))
					yield return kvp.Value as T;
			}
		}

		public IEnumerator<Component> GetEnumerator()
		{
			return _components.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _components.Values.GetEnumerator();
		}

		public void RefreshComponents()
		{
			foreach (Component component in _components.Values)
				component.UpdateRequirements();

			_updateOrderedComponents = _components
				.OrderBy(kvp => Engine.ComponentsUpdateOrder[kvp.Key])
				.Select(kvp => kvp.Value)
				.ToArray();
		}

		public void Remove(Component component)
		{
			Type t = component.GetType();

			if (_components[t] != component)
				throw new InvalidOperationException($"Tried to remove a component of type {component.GetType()} to Entity {Name}, but it was not the one currently attached.");
			else
			{
				_components.Remove(t);
				component.Entity = null;
				OnComponentRemoved?.Invoke(this, component);
				_dirtyComponents = true;
			}
		}

		public void Remove<T>() where T : Component
		{
			Remove(_components[typeof(T)]);
		}

		public override string ToString()
		{
			return $"{{{Name}}}";
		}

		public void Update(bool force = false)
		{
			if (IsActive || force)
			{
				if (_dirtyComponents)
					RefreshComponents();

				foreach (Component component in _updateOrderedComponents)
					component.Update(force);

				foreach (Entity child in _children)
					child.Update(force);

				_dirtyComponents = false;
			}
		}
	}
}
