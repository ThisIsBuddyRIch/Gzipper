using System;
using System.Collections.Generic;
using System.Threading;

namespace Gzipper
{
	public class BlockingPipe<T> : IDisposable
	{
		private readonly Queue<T> _queue = new Queue<T>();
		private readonly object _locker = new object();
		private bool _isClose;
		private bool _isDisposed;
		private readonly SemaphoreSlim _freeNodes;
		private readonly SemaphoreSlim _occupiedNodes;
		private readonly CancellationTokenSource  _cancellationTokenSource = new CancellationTokenSource();

		public BlockingPipe(int maxSize)
		{
			_freeNodes = new SemaphoreSlim(maxSize, maxSize);
			_occupiedNodes = new SemaphoreSlim(_queue.Count, maxSize);
		}

		public void Produce(T data)
		{
			EnsureNotDisposed();
			if (_isClose)
			{
				throw new InvalidOperationException("Cant produce to closed pipe");
			}

			_freeNodes.Wait();
			lock (_locker)
			{
				_queue.Enqueue(data);
				_occupiedNodes.Release();
			}
		}

		public T Consume()
		{
			EnsureNotDisposed();
			T result;
			if (!_cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					_occupiedNodes.Wait(_cancellationTokenSource.Token);
				}
				catch (OperationCanceledException ) { }
			}

			lock (_locker)
			{
				if (_isClose)
				{
					if (_queue.Count == 0)
					{
						throw new InvalidOperationException("Pipe is empty");
					}

					return _queue.Dequeue();
				}
				result = _queue.Dequeue();
				_freeNodes.Release();
			}

			return result;
		}

		public int Size()
		{
			EnsureNotDisposed();
			lock (_locker)
			{
				return _queue.Count;
			}
		}

		public bool IsComplete()
		{
			EnsureNotDisposed();
			lock (_locker)
			{
				return _queue.Count == 0 && _isClose;
			}
		}

		public void Close()
		{
			_isClose = true;
			_cancellationTokenSource.Cancel();
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				return;
			}

			_freeNodes.Dispose();
			_occupiedNodes.Dispose();
			_cancellationTokenSource.Dispose();
			_isDisposed = true;
		}

		private void EnsureNotDisposed()
		{
			if (_isDisposed)
			{
				throw new InvalidOperationException("Pipe has been disposed");
			}
		}
	}
}