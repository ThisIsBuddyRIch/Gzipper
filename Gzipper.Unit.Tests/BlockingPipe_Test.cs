using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class BlockingPipe_Test
	{

		[Test]
		[Repeat(100)]
		[TestCase(2)]
		[TestCase(4)]
		[TestCase(8)]
		public void ProduceTest_WhenMaxProduceWithConsume_ShouldReturnAllItems(int consumersCount)
		{
			var maxSize = 10;
			var actual = new BlockingPipe<int>(maxSize);

			var actualThreads = new List<Thread>();

			actualThreads.Add(
					new Thread(() => Produce(actual, Enumerable.Range(0, 1000).ToArray())));

			actualThreads
				.AddRange(Enumerable.Range(0, consumersCount)
					.Select(_ => new Thread(() => Consume(actual))));

			foreach (var thread in actualThreads)
			{
				thread.Start();
			}

			foreach (var thread in actualThreads)
			{
				thread.Join();
			}

			Assert.That(actual.Size(), Is.EqualTo(0));
		}

		private void Produce(BlockingPipe<int> blockingPipe, int[] data)
		{
			foreach (var i in data)
			{
				blockingPipe.Produce(i);
			}

			blockingPipe.Close();
		}

		private void Consume(BlockingPipe<int> blockingPipe)
		{
			while (!blockingPipe.IsComplete())
			{
				try
				{
					blockingPipe.Consume();
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}
	}


}