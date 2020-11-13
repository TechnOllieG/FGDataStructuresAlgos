using System;

namespace FGDataStructuresAlgos
{
	public class IntBinaryTree
	{
		public IntBinaryTreeNode Root { get; set; } = null;

		public void Add(int value)
		{
			if (Root == null)
			{
				Root = new IntBinaryTreeNode(value);
			}

			var currentLoc = Root;
			var branch = Root.Left;
			while (branch != null)
			{
				if (value > branch.Value)
				{
					
				}
				else
				{
					
				}
			}
		}

		public bool BreadthContains(int value)
		{
			int iterations = 0;
			iterations++;
			
			Console.WriteLine($"Amount of iterations: {iterations}");
			return false;
		}

		public bool DepthContains(int value)
		{
			int iterations = 0;
			iterations++;
			
			Console.WriteLine($"Amount of iterations: {iterations}");
			return false;
		}
	}

	public class IntBinaryTreeNode
	{
		public int Value { get; set; } = default;
		public IntBinaryTreeNode Left { get; set; } = null;
		public IntBinaryTreeNode Right { get; set; } = null;

		public IntBinaryTreeNode(int value, IntBinaryTreeNode left = null, IntBinaryTreeNode right = null)
		{
			Value = value;
			Left = left;
			Right = right;
		}
	}
}