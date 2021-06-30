using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;

namespace FrozenEngine.Utilities
{
	public class Log
	{
		private const string DefaultLayout = @"${date:format=HH\:mm\:ss.fff}|${level}|${message}";

		/// <summary>
		/// The default Core logger; should be used for engine-related logs
		/// </summary>
		public Logger Core { get; private set; }
		/// <summary>
		/// The default Game logger; should be used for gameplay-related logs
		/// </summary>
		public Logger Game { get; private set; }

		internal Log()
		{
			LogManager.Configuration = new NLog.Config.LoggingConfiguration();

			this.Core = this.CustomLog("Core", "core.txt");
			this.Game = this.CustomLog("Game", "game.txt");
		}

		/// <summary>
		/// Returns a new NLog Logger object
		/// </summary>
		/// <param name="name">The name of the logger</param>
		/// <param name="fileName">The name of the file written by the logger; if a logger with the same name already exists, it is returned instead</param>
		/// <returns></returns>
		public Logger CustomLog(string name, string fileName = null)
		{
			if (name == null)
				throw new ArgumentNullException(name);

			if (LogManager.Configuration.FindTargetByName(name) == null)
			{
				if (fileName == null)
					throw new ArgumentNullException(fileName);

				NLog.Targets.FileTarget target = new NLog.Targets.FileTarget(name)
				{
					FileName = fileName,
					Layout = DefaultLayout,
					MaxArchiveFiles = 0,
					AutoFlush = true,
					DeleteOldFileOnStartup = true
				};

				LogManager.Configuration.AddTarget(target);
				LogManager.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, target, name);
				LogManager.Configuration.Reload();
			}

			return LogManager.GetLogger(name);
		}
	}
}
