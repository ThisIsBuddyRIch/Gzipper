using System;
using System.Collections.Generic;
using System.Linq;

namespace Gzipper.ContentModels
{
	public class ContentTable
	{
		private readonly List<ContentTableItem> _items;
		private readonly object _locker = new object();

		public ContentTable()
		{
			_items = new List<ContentTableItem>();
		}

		public ContentTable(byte[] serializedTable)
		{
			if (serializedTable.Length % ContentTableItem.SerializedLength != 0)
			{
				throw new ArgumentException($"Corrupted table, invalid length {serializedTable.Length}");
			}

			var tableItems = new List<ContentTableItem>();

			for (var i = 0; i < serializedTable.Length/ContentTableItem.SerializedLength; i++)
			{
				tableItems.Add(
					new ContentTableItem(
						serializedTable[(i*ContentTableItem.SerializedLength)..((i+1)*ContentTableItem.SerializedLength)]));
			}

			_items = tableItems;
		}

		public void Add(ContentTableItem item)
		{
			lock (_locker)
			{
				_items.Add(item);
			}
		}

		public ContentTableItem[] GetItems()
		{
			lock (_locker)
			{
				return _items.ToArray();
			}
		}

		public byte[] Serialize()
		{
			lock (_locker)
			{
				return _items
					.OrderBy(x => x.CompressedContentPosition)
					.SelectMany(x => x.Serialize())
					.ToArray();
			}
		}
	}
}