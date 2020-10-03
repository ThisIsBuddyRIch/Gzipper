using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Gzipper.Infra
{
	public class TimerProvider
	{
		private readonly Dictionary<string, Stopwatch> _timers = new Dictionary<string, Stopwatch>();

		public Stopwatch CreateTimer(string name)
		{
			var timer = new Stopwatch();
			_timers.Add(name, timer);
			return timer;
		}

		public string GetReport()
		{
			var sb = new StringBuilder();
			foreach (var (name, timer) in _timers) sb.AppendLine($"Worker {name} worked {timer.Elapsed}");

			return sb.ToString();
		}
	}
}