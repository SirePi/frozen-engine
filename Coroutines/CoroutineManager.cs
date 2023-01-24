using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Frozen.Coroutines
{
	/// <summary>
	/// The Coroutine Manager
	/// </summary>
	public class CoroutineManager
	{
		private readonly Queue<Coroutine> _pool = new Queue<Coroutine>(64);
		private Queue<Coroutine> _currentCycle = new Queue<Coroutine>(256);
		private Queue<Coroutine> _nextCycle = new Queue<Coroutine>(256);

		/// <summary>
		/// Returns an IEnumerable of all currently active and scheduled Coroutines
		/// </summary>
		public IEnumerable<Coroutine> Coroutines
		{
			get { return _currentCycle.Concat(_nextCycle); }
		}

		/// <summary>
		/// Cancels all currently active and scheduled Coroutines
		/// </summary>
		public void Clear()
		{
			foreach (Coroutine c in _currentCycle)
			{
				c.Cancel();
				_pool.Enqueue(c);
			}
			_currentCycle.Clear();

			foreach (Coroutine c in _nextCycle)
			{
				c.Cancel();
				_pool.Enqueue(c);
			}
			_nextCycle.Clear();
		}

		/// <summary>
		/// Prepares a new Coroutine and places it in the scheduled queue, to be started in the next cycle
		/// </summary>
		/// <param name="enumerator">The Coroutine's execution body</param>
		/// <param name="name">The name of the Coroutine</param>
		/// <returns>The prepared Coroutine</returns>
		public Coroutine StartNew(IEnumerable<WaitUntil> enumerator)
		{
			Coroutine coroutine;
			if (_pool.Count > 0)
				coroutine = _pool.Dequeue();
			else
				coroutine = new Coroutine();

			coroutine.Setup(enumerator);
			coroutine.Update(); // run once as initialization phase, to get past the first Invalid (not yet set) Wait condition

			_nextCycle.Enqueue(coroutine);
			return coroutine;
		}

		/// <summary>
		/// The Coroutine's update cycle.
		/// </summary>
		public void Update()
		{
			// swap around the queues
			Queue<Coroutine> swap = _currentCycle;
			_currentCycle = _nextCycle;
			_nextCycle = swap;

			int count = _currentCycle.Count;
			for (int i = 0; i < count; i++)
			{
				Coroutine c = _currentCycle.Dequeue();
				c.Update();

				if (c.Status == CoroutineStatus.Running || c.Status == CoroutineStatus.Paused)
					_nextCycle.Enqueue(c);
				else
					_pool.Enqueue(c);
			}
		}
	}
}
