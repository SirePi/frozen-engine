using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog;
using NLog.Targets;
using Serilog;

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
		public Serilog.Core.Logger Game { get; private set; }

		internal Log()
		{
			NLog.Common.InternalLogger.LogLevel = LogLevel.Trace;
			NLog.Common.InternalLogger.LogToConsole = true;
			NLog.Common.InternalLogger.LogFile = "nlog-internal.txt";

			LogManager.Configuration = new NLog.Config.LoggingConfiguration();
			LogManager.ThrowExceptions = true;

			this.Core = this.CustomLog("core");
			this.Core.Info("Done!");

			this.Game = new Serilog.LoggerConfiguration()
				.WriteTo.File("game.txt")
				.CreateLogger();
			this.Game.Information("Yaaa {0}", 3);
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
