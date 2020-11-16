using System;

namespace FGDataStructuresAlgos
{
	class Program
	{
		static void Main(string[] args)
		{
			// LinkedListDemo();
			BinaryTreeDemo();
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

		static void BinaryTreeDemo()
		{
			BinaryTree<int> tree = new BinaryTree<int>();
			int[] ints = new int[] {5, 64, 36, 75, 446, 35};
			foreach (int integer in ints)
			{
				tree.Add(integer);
			}
			
			Console.WriteLine(tree.DepthContains(5)); // Should return true
			Console.WriteLine(tree.DepthContains(64)); // Should return true
			Console.WriteLine(tree.DepthContains(36)); // Should return true
			Console.WriteLine(tree.DepthContains(75)); // Should return true
			Console.WriteLine(tree.DepthContains(446)); // Should return true
			Console.WriteLine(tree.DepthContains(35)); // Should return true
			Console.WriteLine(tree.DepthContains(40)); // Should return false
		}

		static int Fibonacci(int n) => n > 1 ? Fibonacci(n - 1) + Fibonacci(n - 2) : 1;
	}
}