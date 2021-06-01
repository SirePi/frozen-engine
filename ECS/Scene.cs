using FrozenEngine.Coroutines;
using FrozenEngine.ECS.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrozenEngine.ECS
{
	public class Scene
	{
		public static Scene Current => Core.Game.CurrentScene;
		public static void SwitchTo(Scene nextScene)
		{
			Core.Game.ChangeScene(nextScene);
		}

		private readonly EntityManager entityManager = new EntityManager();
		private readonly CoroutineManager coroutineManager = new CoroutineManager();
		private readonly HashSet<Renderer> renderers = new HashSet<Renderer>();
		private readonly HashSet<UI> uis = new HashSet<UI>();

		public int EntitiesCount => this.entityManager.GetEntities().Count();

		public Scene()
		{
			this.entityManager.OnEntityAdded += this.OnEntityAdded;
			this.entityManager.OnEntityRemoved += this.OnEntityRemoved;
			this.entityManager.OnComponentAdded += this.OnEntityComponentAdded;
			this.entityManager.OnComponentRemoved += this.OnEntityComponentRemoved;

			/*
			int i = 0;
			foreach (Camera c in Camera.CreateSplitScreen<TwoPointFiveDCamera>(SplitScreen.FourWays))
			{
				Entity e = new Entity($"Camera_{i}")
				{
					new Transform { Position = new Vector3(0, 0, -1000) },
					c
				};
				this.AddEntity(e);
			}
			*/
		}

		public void Update(GameTime gameTime)
		{
			this.entityManager.Update(gameTime);
			this.coroutineManager.Update(gameTime);
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

		public IEnumerable<T> GetComponents<T>() where T : Component
		{
			return this.entityManager.GetComponents<T>();
		}

		public IEnumerable<T> GetActiveComponents<T>() where T : Component
		{
			return this.entityManager.GetComponents<T>();
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
