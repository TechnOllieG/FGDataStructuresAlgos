using System;

namespace FGDataStructuresAlgos
{
	class Program
	{
		struct Test
		{
			public int hello;
			public int yo;
		}
		
		static void Main(string[] args)
		{
			int[] ints = new int[] {5, 64, 36, 75, 446, 35};
			LinkedList<int> linkList = new LinkedList<int>(ints);
			LinkedList<int>.LinkedListNode<int> node = linkList.Last;
			while (node != null)
			{
				Console.WriteLine(node.Value);
				node = node.Previous;
			}

			Test test = new Test();
			test.hello = 5;
			test.yo = 2;
			
			Test test2 = new Test();
			test2.hello = 5;
			test2.yo = 2;
			
			if (test.GetHashCode() == test2.GetHashCode())
			{
				Console.WriteLine("HashCode worked");
			}

			if (test.Equals(test2))
			{
				Console.WriteLine("Equals worked");
			}
			Console.WriteLine(linkList.Contains(34));
		}
	}
}