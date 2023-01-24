using System;
using System.Collections.Generic;

namespace Frozen.Coroutines
{
	/// <summary>
	/// A Coroutine's management class
	/// </summary>
	public sealed class Coroutine
	{
		private WaitUntil _currentCondition;
		private IEnumerator<WaitUntil> _enumerator = null;

		/// <summary>
		/// True if this Coroutine's Status is <see cref="CoroutineStatus.Paused"/> or <see cref="CoroutineStatus.Running"/>
		/// </summary>
		public bool IsAlive
		{
			get { return Status == CoroutineStatus.Paused || Status == CoroutineStatus.Running; }
		}

		/// <summary>
		/// The Exception encountered during the last frame
		/// </summary>
		public Exception LastException { get; private set; }

		/// <summary>
		/// The Coroutine's current <see cref="CoroutineStatus">execution status</see>
		/// </summary>
		public CoroutineStatus Status { get; private set; }

		internal void Setup(IEnumerable<WaitUntil> values)
		{
			Status = CoroutineStatus.Running;

			LastException = null;

			if (_enumerator != null)
				_enumerator.Dispose();

			_enumerator = values.GetEnumerator();
		}

		internal void Update()
		{
			if (Status != CoroutineStatus.Running) return;

			try
			{
				_currentCondition.Update();
				if (_currentCondition.IsComplete)
				{
					if (_enumerator.MoveNext())
						_currentCondition = _enumerator.Current;
					else
						Status = CoroutineStatus.Complete;
				}
			}
			catch (Exception e)
			{
				// Logs.Core.WriteError("An error occurred while processing Coroutine '{0}': {1}", enumerator.GetType().FullName, LogFormat.Exception(e));

				LastException = e;
				Status = CoroutineStatus.Error;
			}
		}

		/// <summary>
		/// Cancels the coroutine without executing any further code.
		/// </summary>
		public void Cancel()
		{
			_enumerator.Dispose();
			Status = CoroutineStatus.Cancelled;
		}

		/// <summary>
		/// Puts the coroutine on hold, to be resumed or cancelled later
		/// </summary>
		public void Pause()
		{
			if (Status == CoroutineStatus.Running)
				Status = CoroutineStatus.Paused;
		}

		/// <summary>
		/// Resumes a Paused coroutine
		/// </summary>
		public void Resume()
		{
			if (Status == CoroutineStatus.Paused)
				Status = CoroutineStatus.Running;
		}
	}
}
