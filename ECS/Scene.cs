using Frozen.Coroutines;
using Frozen.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frozen.ECS
{
	public abstract class Scene
	{
		private static Dictionary<Type, Scene> scenesDictionary = new Dictionary<Type, Scene>();
		public static Scene Current => Engine.Game.CurrentScene;
		public static void SwitchTo<T>() where T : Scene, new()
		{
			Type t = typeof(T);

			if (!scenesDictionary.ContainsKey(t))
				scenesDictionary.Add(t, new T());

			Engine.Game.ChangeScene(scenesDictionary[t]);
		}

		private readonly EntityManager entityManager = new EntityManager();
		private readonly CoroutineManager coroutineManager = new CoroutineManager();
		private readonly HashSet<Renderer> renderers = new HashSet<Renderer>();
		private readonly HashSet<UI> uis = new HashSet<UI>();

		public int EntitiesCount => this.entityManager.GetEntities().Count();

		protected Scene()
		{
			this.entityManager.OnEntityAdded += this.OnEntityAdded;
			this.entityManager.OnEntityRemoved += this.OnEntityRemoved;
			this.entityManager.OnComponentAdded += this.OnEntityComponentAdded;
			this.entityManager.OnComponentRemoved += this.OnEntityComponentRemoved;

			this.Build();
		}

		public abstract void Build();
		public virtual void AfterSwitchingFrom() { }
		public virtual void BeforeSwitchingFrom() { }
		public virtual void AfterSwitchingTo() { }
		public virtual void BeforeSwitchingTo() { }

		public virtual void Update()
		{
			this.entityManager.Update();
			this.coroutineManager.Update();
		}

		public void AddEntity(Entity entity)
		{
			entity.Scene?.RemoveEntity(entity);
			if (this.entityManager.AddEntity(entity))
				entity.Scene = this;
		}

		public void RemoveEntity(Entity entity)
		{
			if (this.entityManager.RemoveEntity(entity))
				entity.Scene = null;
		}

		public void RemoveWhere(Func<Entity, bool> predicate)
		{
			Entity[] toRemove = this.entityManager.GetEntities(predicate).ToArray();
			foreach (Entity e in toRemove)
				this.RemoveEntity(e);
		}

		public void Clear()
		{
			this.entityManager.Clear();
		}

		public IEnumerable<T> GetComponents<T>() where T : Component
		{
			return this.entityManager.GetComponents<T>();
		}

		public IEnumerable<T> GetActiveComponents<T>() where T : Component
		{
			return this.entityManager.GetActiveComponents<T>();
		}

		public IEnumerable<T> GetActiveComponents<T>(Func<T, bool> predicate) where T : Component
		{
			return this.entityManager.GetActiveComponents(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities()
		{
			return this.entityManager.GetActiveEntities();
		}

		public IEnumerable<Entity> GetActiveEntities(Func<Entity, bool> predicate)
		{
			return this.entityManager.GetActiveEntities(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities<T>() where T : Component
		{
			return this.entityManager.GetActiveEntities<T>();
		}

		public IEnumerable<Entity> GetActiveEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return this.entityManager.GetActiveEntities<T>(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities<T>(Func<T, bool> predicate) where T : Component
		{
			return this.entityManager.GetActiveComponents<T>().Where(predicate).Select(t => t.Entity);
		}

		public IEnumerable<Entity> GetEntities()
		{
			return this.entityManager.GetEntities();
		}

		public IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate)
		{
			return this.entityManager.GetEntities(predicate);
		}

		public IEnumerable<Entity> GetEntities<T>() where T : Component
		{
			return this.entityManager.GetEntities<T>();
		}

		public IEnumerable<Entity> GetEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return this.entityManager.GetEntities<T>(predicate);
		}

		public Entity GetEntityByName(string name)
		{
			return this.entityManager.GetEntityByName(name);
		}

		internal IEnumerable<Renderer> GetSortedRenderers()
		{
			return this.entityManager.GetSortedRenderers();
		}

		internal IEnumerable<Camera> GetCameras()
		{
			return this.entityManager.GetActiveComponents<Camera>();
		}

		private void OnEntityAdded(Entity entity)
		{
			foreach (Component cmp in entity)
				this.OnEntityComponentAdded(entity, cmp);
		}

		private void OnEntityRemoved(Entity entity)
		{
			foreach (Component cmp in entity)
				this.OnEntityComponentRemoved(entity, cmp);
		}

		private void OnEntityComponentAdded(Entity entity, Component component)
		{
			switch (component)
			{
				case Renderer r: this.renderers.Add(r); break;
				case UI ui: this.uis.Add(ui); break;
			}
		}

		private void OnEntityComponentRemoved(Entity entity, Component component)
		{
			switch (component)
			{
				case Renderer r: this.renderers.Remove(r); break;
				case UI ui: this.uis.Remove(ui); break;
			}
		}

		public Coroutine StartCoroutine(IEnumerable<WaitUntil> coroutine)
		{
			return this.coroutineManager.StartNew(coroutine);
		}
	}
}
