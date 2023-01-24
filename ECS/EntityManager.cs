using System;
using System.Collections.Generic;
using System.Linq;
using Frozen.ECS.Components;
using static Frozen.DelegatesAndEvents;

namespace Frozen.ECS
{
	internal class EntityManager
	{
		private readonly HashSet<Entity> _entities = new HashSet<Entity>();
		private readonly HashSet<Entity> _pendingEntities = new HashSet<Entity>();
		private readonly HashSet<Entity> _removedEntities = new HashSet<Entity>();

		public event EntityComponentEvent OnComponentAdded;
		public event EntityComponentEvent OnComponentRemoved;
		public event EntityEvent OnEntityAdded;
		public event EntityEvent OnEntityRemoved;

		internal EntityManager()
		{ }

		private void Entity_OnComponentAdded(Entity entity, Component component)
		{
			OnComponentAdded?.Invoke(entity, component);
		}

		private void Entity_OnComponentRemoved(Entity entity, Component component)
		{
			OnComponentRemoved?.Invoke(entity, component);
		}

		internal bool AddEntity(Entity entity)
		{
			bool toAdd = !_entities.Contains(entity);
			if (toAdd)
			{
				entity.OnComponentAdded += Entity_OnComponentAdded;
				entity.OnComponentRemoved += Entity_OnComponentRemoved;

				_pendingEntities.Add(entity);
				OnEntityAdded?.Invoke(entity);
			}
			return toAdd;
		}

		internal IEnumerable<T> GetActiveComponents<T>() where T : Component
		{
			return GetActiveEntities().SelectMany(e => e.GetAll<T>());
		}

		internal IEnumerable<T> GetActiveComponents<T>(Func<T, bool> predicate) where T : Component
		{
			return GetActiveEntities().SelectMany(e => e.GetAll<T>()).Where(predicate);
		}

		internal IEnumerable<Entity> GetActiveEntities()
		{
			return _entities.Where(e => e.IsActive);
		}

		internal IEnumerable<Entity> GetActiveEntities(Func<Entity, bool> predicate)
		{
			return _entities.Where(e => e.IsActive && predicate(e));
		}

		internal IEnumerable<Entity> GetActiveEntities<T>() where T : Component
		{
			return GetEntities<T>().Where(e => e.IsActive);
		}

		internal IEnumerable<Entity> GetActiveEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return GetEntities<T>().Where(e => e.IsActive && predicate(e));
		}

		internal IEnumerable<T> GetComponents<T>() where T : Component
		{
			return _entities.SelectMany(e => e.GetAll<T>());
		}

		internal IEnumerable<Entity> GetEntities()
		{
			return _entities;
		}

		internal IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate)
		{
			return _entities.Where(predicate);
		}

		internal IEnumerable<Entity> GetEntities<T>() where T : Component
		{
			return GetComponents<T>().Select(c => c.Entity);
		}

		internal IEnumerable<Entity> GetEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return GetComponents<T>().Select(c => c.Entity).Where(predicate);
		}

		internal Entity GetEntityByName(string name)
		{
			return _entities.FirstOrDefault(e => e.Name == name);
		}

		internal IEnumerable<Entity> GetRootEntities()
		{
			return _entities.Where(e => e.Parent == null);
		}

		internal IEnumerable<Renderer> GetSortedRenderers()
		{
			return GetComponents<Renderer>().OrderByDescending(r => r.RendererSortedHash);
		}

		internal bool RemoveEntity(Entity entity)
		{
			bool toRemove = _entities.Contains(entity);
			if (toRemove)
			{
				entity.OnComponentAdded -= Entity_OnComponentAdded;
				entity.OnComponentRemoved -= Entity_OnComponentRemoved;

				_removedEntities.Add(entity);
				OnEntityRemoved?.Invoke(entity);
			}
			return toRemove;
		}

		internal void Update()
		{
			foreach (Entity e in _removedEntities)
				_entities.Remove(e);
			_removedEntities.Clear();

			foreach (Entity e in _pendingEntities)
				_entities.Add(e);

			_pendingEntities.Clear();

			foreach (Entity e in _entities.Where(e => e.Parent == null && e.IsActive))
				e.Update(false);
		}

		public void Clear()
		{
			_entities.Clear();
			_pendingEntities.Clear();
			_removedEntities.Clear();
		}
	}
}
