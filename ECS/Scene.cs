using System;
using System.Collections.Generic;
using System.Linq;
using Frozen.Coroutines;
using Frozen.ECS.Components;

namespace Frozen.ECS
{
	public abstract class Scene
	{
		private static Dictionary<Type, Scene> _scenesDictionary = new Dictionary<Type, Scene>();

		private readonly CoroutineManager _coroutineManager = new CoroutineManager();
		private readonly EntityManager _entityManager = new EntityManager();
		private readonly HashSet<Renderer> _renderers = new HashSet<Renderer>();
		public static Scene Current => Engine.Game.CurrentScene;

		public int EntitiesCount => _entityManager.GetEntities().Count();

		protected Scene()
		{
			_entityManager.OnEntityAdded += OnEntityAdded;
			_entityManager.OnEntityRemoved += OnEntityRemoved;
			_entityManager.OnComponentAdded += OnEntityComponentAdded;
			_entityManager.OnComponentRemoved += OnEntityComponentRemoved;

			Build();
		}

		private void OnEntityAdded(Entity entity)
		{
			foreach (Component cmp in entity)
				OnEntityComponentAdded(entity, cmp);
		}

		private void OnEntityComponentAdded(Entity entity, Component component)
		{
			switch (component)
			{
				case Renderer r: _renderers.Add(r); break;
			}
		}

		private void OnEntityComponentRemoved(Entity entity, Component component)
		{
			switch (component)
			{
				case Renderer r: _renderers.Remove(r); break;
			}
		}

		private void OnEntityRemoved(Entity entity)
		{
			foreach (Component cmp in entity)
				OnEntityComponentRemoved(entity, cmp);
		}

		internal IEnumerable<Camera> GetCameras()
		{
			return _entityManager.GetActiveComponents<Camera>();
		}

		internal IEnumerable<Renderer> GetSortedRenderers()
		{
			return _entityManager.GetSortedRenderers();
		}

		public static void SwitchTo<T>() where T : Scene, new()
		{
			Type t = typeof(T);

			if (!_scenesDictionary.ContainsKey(t))
				_scenesDictionary.Add(t, new T());

			Engine.Game.ChangeScene(_scenesDictionary[t]);
		}

		public void Add(Entity entity)
		{
			if (entity.Scene != this)
			{
				entity.Scene?.Remove(entity);
				if (_entityManager.AddEntity(entity))
				{
					entity.Scene = this;
					foreach (Entity child in entity.Children)
						Add(child);
				}
			}
		}

		public virtual void AfterSwitchingFrom()
		{ }

		public virtual void AfterSwitchingTo()
		{ }

		public virtual void BeforeSwitchingFrom()
		{ }

		public virtual void BeforeSwitchingTo()
		{ }

		public abstract void Build();

		public void Clear()
		{
			_entityManager.Clear();
		}

		public IEnumerable<T> GetActiveComponents<T>() where T : Component
		{
			return _entityManager.GetActiveComponents<T>();
		}

		public IEnumerable<T> GetActiveComponents<T>(Func<T, bool> predicate) where T : Component
		{
			return _entityManager.GetActiveComponents(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities()
		{
			return _entityManager.GetActiveEntities();
		}

		public IEnumerable<Entity> GetActiveEntities(Func<Entity, bool> predicate)
		{
			return _entityManager.GetActiveEntities(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities<T>() where T : Component
		{
			return _entityManager.GetActiveEntities<T>();
		}

		public IEnumerable<Entity> GetActiveEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return _entityManager.GetActiveEntities<T>(predicate);
		}

		public IEnumerable<Entity> GetActiveEntities<T>(Func<T, bool> predicate) where T : Component
		{
			return _entityManager.GetActiveComponents<T>().Where(predicate).Select(t => t.Entity);
		}

		public IEnumerable<T> GetComponents<T>() where T : Component
		{
			return _entityManager.GetComponents<T>();
		}

		public IEnumerable<Entity> GetEntities()
		{
			return _entityManager.GetEntities();
		}

		public IEnumerable<Entity> GetEntities(Func<Entity, bool> predicate)
		{
			return _entityManager.GetEntities(predicate);
		}

		public IEnumerable<Entity> GetEntities<T>() where T : Component
		{
			return _entityManager.GetEntities<T>();
		}

		public IEnumerable<Entity> GetEntities<T>(Func<Entity, bool> predicate) where T : Component
		{
			return _entityManager.GetEntities<T>(predicate);
		}

		public Entity GetEntityByName(string name)
		{
			return _entityManager.GetEntityByName(name);
		}

		public void Remove(Entity entity)
		{
			if (entity.Scene == this)
			{
				if (_entityManager.RemoveEntity(entity))
					entity.Scene = null;

				foreach (Entity child in entity.Children)
					Remove(child);
			}
		}

		public void RemoveWhere(Func<Entity, bool> predicate)
		{
			Entity[] toRemove = _entityManager.GetEntities(predicate).ToArray();
			foreach (Entity e in toRemove)
				Remove(e);
		}

		public Coroutine StartCoroutine(IEnumerable<WaitUntil> coroutine)
		{
			return _coroutineManager.StartNew(coroutine);
		}

		public virtual void Update()
		{
			_entityManager.Update();
			_coroutineManager.Update();
		}
	}
}
