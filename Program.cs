using System;

namespace FGDataStructuresAlgos
{
	class Program
	{
		static void Main(string[] args)
		{
			LinkedListDemo();
		}

		static void LinkedListDemo()
		{
			int[] ints = new int[] {5, 64, 36, 75, 446, 35};
			LinkedList<int> linkList = new LinkedList<int>(ints);
			LinkedList<int>.LinkedListNode<int> node = linkList.Last;
			while (node != null)
			{
				Console.WriteLine(node.Value);
				node = node.Previous;
			}
			
			Console.WriteLine(linkList.Contains(34));
		}
	}
}