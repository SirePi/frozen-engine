using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using NLog.Targets;

namespace Frozen.Utilities
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

			this.Core = this.CustomLog("core");
			this.Game = this.CustomLog("game");
		}

		/// <summary>
		/// Returns a new NLog Logger object
		/// </summary>
		/// <param name="name">The name of the logger</param>
		/// <returns></returns>
		public Logger CustomLog(string name)
		{
			if (name == null)
				throw new ArgumentNullException(name);

			if (LogManager.Configuration.FindRuleByName(name) == null)
			{
				FileTarget target = new FileTarget(name)
				{
					FileName = string.Format("{0}.txt", name),
					Layout = DefaultLayout,
					AutoFlush = true,
					KeepFileOpen = true,
					DeleteOldFileOnStartup = true
				};

				LogManager.Configuration.AddTarget(target);
				LogManager.Configuration.AddRule(LogLevel.Trace, LogLevel.Fatal, target, name);
				LogManager.ReconfigExistingLoggers();
			}

			return LogManager.GetLogger(name);
		}
	}
}
