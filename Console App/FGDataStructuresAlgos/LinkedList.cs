using System;

namespace FGDataStructuresAlgos
{
	[Serializable]
	public class LinkedList<T>
	{
		public int Count { get; private set; } = 0;
		public LinkedListNode<T> First { get; private set; } = null;
		public LinkedListNode<T> Last { get; private set; } = null;
		public bool IsListEmpty => First == null && Last == null && Count == 0;
		
		public LinkedList(T[] values)
		{
			foreach (T value in values)
			{
				AddLast(value);
			}
		}

		public void Clear()
		{
			First = null;
			Last = null;
			Count = 0;
		}

		public void AddFirst(T value)
		{
			if (IsListEmpty)
			{
				CreateFirstElement(value);
			}
			
			var current = new LinkedListNode<T>(this, First, null, value);
			First.Previous = current;
			First = current;
			Count++;
		}

		public void AddLast(T value)
		{
			if (IsListEmpty)
			{
				CreateFirstElement(value);
				return;
			}
			
			var current = new LinkedListNode<T>(this, null, Last, value);
			Last.Next = current;
			Last = current;
			Count++;
		}

		public void AddBefore(LinkedListNode<T> node, T value)
		{
			if (IsListEmpty)
			{
				CreateFirstElement(value);
			}

			var current = new LinkedListNode<T>(this, node, node.Previous, value);
			if (node.Previous != null)
				node.Previous.Next = current;
			node.Previous = current;
			Count++;
		}
		
		public void AddAfter(LinkedListNode<T> node, T value)
		{
			if (IsListEmpty)
			{
				CreateFirstElement(value);
			}
			
			var current = new LinkedListNode<T>(this, node.Next, node, value);
			if(node.Next != null)
				node.Next.Previous = current;
			node.Next = current;
			Count++;
		}
		
		public bool Contains(T value)
		{
			LinkedListNode<T> pointer = First;
			while (pointer != null)
			{
				if (value.Equals(pointer.Value))
				{
					return true;
				}

				pointer = pointer.Next;
			}
			return false;
		}

		private void CreateFirstElement(T value)
		{
			First = Last = new LinkedListNode<T>(this, null, null, value);
		}
		
		[Serializable]
		public class LinkedListNode<T>
		{
			public LinkedList<T> List { get; set; }
			public LinkedListNode<T> Next { get; set; }
			public LinkedListNode<T> Previous { get; set; }
			public T Value { get; set; }
			public LinkedListNode<T> ValueRef => this;

			public LinkedListNode(LinkedList<T> list = null, LinkedListNode<T> next = null, LinkedListNode<T> previous = null, T value = default)
			{
				List = list;
				Next = next;
				Previous = previous;
				Value = value;
			}
		}
	}
}