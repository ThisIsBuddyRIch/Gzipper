using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class BlockingDictionary_Test
	{
		[Test]
		[Repeat(100)]
		[TestCase(2)]
		[TestCase(4)]
		[TestCase(8)]
		public void ProduceTest_WhenMaxProduceWithConsume_ShouldReturnAllItems(int consumersCount)
		{
			var maxSize = 10;
			var actual = new BlockingDictionary<int, string>(
				maxSize,
				new SortedDictionary<int, string>(
					new Dictionary<int, string>(1000)));

			var produceThread = new Thread(() => Produce(actual, Enumerable.Range(0, 1000).ToArray()));
			var consumeThreads = new List<Thread>();

			consumeThreads
				.AddRange(Enumerable.Range(0, consumersCount)
					.Select(_ => new Thread(() => Consume(actual))));

			produceThread.Start();
			foreach (var thread in consumeThreads) thread.Start();

			produceThread.Join();
			actual.Close();
			foreach (var thread in consumeThreads) thread.Join();

			Assert.That(actual.Size(), Is.EqualTo(0));
		}

		[Test]
		[Repeat(1000)]
		public void ProduceTest_WhenReadByKeys_ShouldReturnAll()
		{
			var maxSize = 3001;
			var blockingDictionary = new BlockingDictionary<int, string>(maxSize, new Dictionary<int, string>(maxSize));

			var expectedData = Enumerable.Range(0, 4000).ToArray();
			var produceThreads = new List<Thread>
			{
				new Thread(() => Produce(blockingDictionary, expectedData[..1000])),
				new Thread(() => Produce(blockingDictionary, expectedData[1000..2000])),
				new Thread(() => Produce(blockingDictionary, expectedData[2000..3000])),
				new Thread(() => Produce(blockingDictionary, expectedData[3000..4000]))
			};
			var result = new List<string>();
			var consumeThread = new Thread(() => ConsumeByKey(blockingDictionary, result));

			consumeThread.Start();
			foreach (var thread in produceThreads) thread.Start();

			foreach (var thread in produceThreads) thread.Join();
			blockingDictionary.Close();
			consumeThread.Join();
			foreach (var i in expectedData) i.ToString().Should().Be(result[i]);
		}

		private void Produce(BlockingDictionary<int, string> blockingDictionary, int[] data)
		{
			foreach (var i in data) blockingDictionary.Add(i, i.ToString());
		}

		private void ConsumeByKey(BlockingDictionary<int, string> blockingDictionary, List<string> result)
		{
			var key = 0;
			while (!blockingDictionary.IsComplete())
				try
				{
					result.Add(blockingDictionary.GetByKey(key));
					key++;
				}
				catch (InvalidOperationException)
				{
					break;
				}
		}

		private void Consume(BlockingDictionary<int, string> blockingPipe)
		{
			while (!blockingPipe.IsComplete())
				try
				{
					blockingPipe.GetFirstItem();
				}
				catch (InvalidOperationException)
				{
					break;
				}
		}
	}
}