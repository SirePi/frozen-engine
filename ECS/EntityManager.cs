using FrozenEngine.ECS.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS
{
	internal class EntityManager
	{
		private readonly HashSet<Entity> entities = new HashSet<Entity>();
		private readonly HashSet<Entity> pendingEntities = new HashSet<Entity>();
		private readonly HashSet<Entity> disposedEntities = new HashSet<Entity>();
		private readonly HashSet<Entity> dirtyEntities = new HashSet<Entity>();

		public event Action<Entity> OnEntityAdded;
		public event Action<Entity> OnEntityRemoved;
		public event Action<Entity, Component> OnComponentAdded;
		public event Action<Entity, Component> OnComponentRemoved;

		internal EntityManager() { }

		internal void Update(GameTime gameTime)
		{
			foreach (Entity e in this.disposedEntities)
				this.entities.Remove(e);
			this.disposedEntities.Clear();

			foreach (Entity e in this.pendingEntities)
			{
				this.dirtyEntities.Add(e);
				this.entities.Add(e);
			}
			this.pendingEntities.Clear();

			foreach (Entity e in this.dirtyEntities)
				e.Refresh();
			this.dirtyEntities.Clear();

			foreach (Entity e in this.GetActiveRootEntities())
				e.Update(gameTime, false);
		}

		public bool AddEntity(Entity entity)
		{
			bool toAdd = !this.entities.Contains(entity);
			if (toAdd)
			{
				entity.OnComponentAdded += this.Entity_OnComponentAdded;
				entity.OnComponentRemoved += this.Entity_OnComponentRemoved;

				this.pendingEntities.Add(entity);
				this.OnEntityAdded?.Invoke(entity);
			}
			return toAdd;
		}

		public bool RemoveEntity(Entity entity)
		{
			bool toRemove = this.entities.Contains(entity);
			if (toRemove)
			{
				entity.OnComponentAdded -= this.Entity_OnComponentAdded;
				entity.OnComponentRemoved -= this.Entity_OnComponentRemoved;

				this.disposedEntities.Add(entity);
				this.OnEntityRemoved?.Invoke(entity);
			}
			return toRemove;
		}

		private void Entity_OnComponentAdded(Entity entity, Component component)
		{
			this.dirtyEntities.Add(entity);
			this.OnComponentAdded?.Invoke(entity, component);
		}

		private void Entity_OnComponentRemoved(Entity entity, Component component)
		{
			this.dirtyEntities.Add(entity);
			this.OnComponentRemoved?.Invoke(entity, component);
		}

		internal IEnumerable<T> GetComponents<T>() where T : Component
		{
			return this.entities.Select(e => e.Get<T>()).Where(c => c != null);
		}

		internal IEnumerable<T> GetActiveComponents<T>() where T : Component
		{
			return this.GetActiveEntities().Select(e => e.Get<T>()).Where(c => c != null);
		}

		internal IEnumerable<Entity> GetActiveEntities()
		{
			return this.entities.Where(e => e.IsActive);
		}

		internal IEnumerable<Entity> GetActiveEntities(Func<Entity, bool> predicate)
		{
			return this.entities.Where(e => e.IsActive && predicate(e));
		}

		internal IEnumerable<Entity> GetActiveEntities<T>() where T : Component
		{
			return this.GetEntities<T>().Where(e => e.IsActive);
		}

		internal IEnumerable<Entity> GetActiveEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return this.GetEntities<T>().Where(e => e.IsActive && predicate(e));
		}

		internal IEnumerable<Entity> GetEntities()
		{
			return this.entities;
		}

		internal IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate)
		{
			return this.entities.Where(predicate);
		}

		internal IEnumerable<Entity> GetEntities<T>() where T : Component
		{
			return this.GetComponents<T>().Select(c => c.Entity);
		}

		internal IEnumerable<Entity> GetEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return this.GetComponents<T>().Select(c => c.Entity).Where(predicate);
		}

		internal IEnumerable<Entity> GetRootEntities()
		{
			return this.entities.Where(e => e.Parent == null);
		}

		internal IEnumerable<Entity> GetActiveRootEntities()
		{
			return this.entities.Where(e => e.Parent == null && e.IsActive);
		}

		internal Entity GetEntityByName(string name)
		{
			return this.entities.FirstOrDefault(e => e.Name == name);
		}

		internal IEnumerable<Renderer> GetSortedRenderers()
		{
			return this.GetComponents<Renderer>().OrderByDescending(r => r.RendererSortedHash);
		}
	}
}
