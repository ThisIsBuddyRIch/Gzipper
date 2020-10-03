using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gzipper
{
	public class BlockingDictionary<TKey, TValue> : IDisposable where TValue : class where TKey : notnull
	{
		private readonly int _maxSize;
		private readonly IDictionary<TKey, TValue> _dictionary;
		private readonly object _locker = new object();
		private bool _isClose;
		private bool _isDisposed;
		private readonly SemaphoreSlim _freeNodes;
		private readonly SemaphoreSlim _occupiedNodes;
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly AutoResetEvent _event = new AutoResetEvent(true);

		public BlockingDictionary(int maxSize, IDictionary<TKey, TValue> dictionary)
		{
			_maxSize = maxSize;
			_dictionary = dictionary;
			_freeNodes = new SemaphoreSlim(maxSize, maxSize);
			_occupiedNodes = new SemaphoreSlim(0, maxSize);
		}

		public void Add(TKey key, TValue value)
		{
			EnsureNotDisposed();
			if (_isClose)
			{
				throw new InvalidOperationException("Cant produce to closed pipe");
			}

			_freeNodes.Wait();
			lock (_locker)
			{
				_dictionary.Add(key, value);
				_occupiedNodes.Release();
				_event.Set();
			}
		}

		public (TKey, TValue) GetFirstItem()
		{
			return Get(GetFirstItem);
		}

		public TValue GetByKey(TKey key)
		{
			var (_, value) = Get(dict => GetByKey(dict, key));
			return value;
		}

		private (TKey, TValue) GetByKey(IDictionary<TKey, TValue> dictionary, TKey key)
		{
			while (true)
			{
				if (dictionary.TryGetValue(key, out TValue result))
				{
					dictionary.Remove(key);
					return (key, result);
				}

				if (result == null)
				{
					if (dictionary.Count == _maxSize)
					{
						throw new ApplicationException($"Can't find item by key {key} and dict is full! " +
						                               $"Extend max size {_maxSize}");
					}

					Monitor.Exit(_locker);
					_event.WaitOne(100);
					Monitor.Enter(_locker);
				}
			}
		}

		private (TKey, TValue) Get(Func< IDictionary<TKey, TValue>, (TKey, TValue)> getter)
		{
			EnsureNotDisposed();
			if (!_cancellationTokenSource.IsCancellationRequested)
			{
				try
				{
					_occupiedNodes.Wait(_cancellationTokenSource.Token);
				}
				catch (OperationCanceledException)
				{
				}
			}

			lock (_locker)
			{
				if (_isClose)
				{
					if (_dictionary.Count == 0)
					{
						throw new InvalidOperationException("Pipe is empty");
					}

					return getter(_dictionary);
				}

				var result =  getter(_dictionary);
				_freeNodes.Release();
				return result;
			}
		}

		private (TKey, TValue) GetFirstItem(IDictionary<TKey, TValue> dictionary)
		{
			var (key, value) = dictionary.First();
			dictionary.Remove(key);
			return (key, value);
		}

		public int Size()
		{
			EnsureNotDisposed();
			lock (_locker)
			{
				return _dictionary.Count;
			}
		}

		public bool IsComplete()
		{
			EnsureNotDisposed();
			lock (_locker)
			{
				return _dictionary.Count == 0 && _isClose;
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
			_event.Dispose();
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