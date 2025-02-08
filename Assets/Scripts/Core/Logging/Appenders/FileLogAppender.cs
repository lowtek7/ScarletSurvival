using System;
using System.IO;
using Scarlet.Core.Logging.Interfaces;

namespace Scarlet.Core.Logging.Appenders
{
	public class FileLogAppender : LogAppenderBase, IDisposable
	{
		private readonly StreamWriter writer;
		private readonly object writeLock = new();
		private bool disposed;

		public FileLogAppender(LogConfiguration config, bool append = true)
			: base(config)
		{
			var filePath = config.LogFilePath;
			var directory = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var fileStream = new FileStream(filePath, append ? FileMode.Append : FileMode.Create,
				FileAccess.Write, FileShare.Read);
			writer = new StreamWriter(fileStream, Encoding.Default);
		}

		public override void Append(LogMessage message)
		{
			if (disposed) return;

			FormatMessage(message, StringBuilder);

			lock (writeLock)
			{
				writer.Write(StringBuilder.ToString());
			}
		}

		public override void Flush()
		{
			if (disposed) return;

			lock (writeLock)
			{
				writer.Flush();
			}
		}

		public void Dispose()
		{
			if (disposed) return;

			lock (writeLock)
			{
				writer.Dispose();
				disposed = true;
			}
		}
	}
}
