using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace server.Code.GlobalUtils
{
	public static class MainThread
	{
		private static ConcurrentQueue<Action> Tasks = new();
		public static readonly Thread Thread;
		
		static MainThread()
		{
			Thread = Thread.CurrentThread;
		}
		
		public static void Run(Action task)
		{
			Tasks.Enqueue(task);
		}

		public static void Pulse()
		{
			while (true)
			{
				var result = Tasks.TryDequeue(out var action);
				if (!result) break;
				action?.Invoke();
			}
		}
		
		private static readonly int _mainThreadId = Environment.CurrentManagedThreadId;
		
		public static bool IsMainThread => Environment.CurrentManagedThreadId == _mainThreadId;

		public static void Assert()
		{
			Debug.Assert(IsMainThread, "Not in main thread");
		}
	}
}