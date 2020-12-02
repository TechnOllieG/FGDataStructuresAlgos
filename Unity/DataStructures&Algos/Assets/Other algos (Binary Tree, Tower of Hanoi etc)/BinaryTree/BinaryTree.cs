using System;
using System.Collections.Generic;
using UnityEngine;

namespace FGDataStructuresAlgos
{
	public class BinaryTree<T> where T : IComparable
	{
		public BinaryTreeNode<T> Root { get; set; } = null;

		public void Add(T value)
		{
			if (Root == null)
			{
				Root = new BinaryTreeNode<T>(value);
				return;
			}

			var currentLoc = Root;
			
			while (true)
			{
				if (currentLoc.Value.CompareTo(value) < 0)
				{
					if (currentLoc.Right == null)
					{
						currentLoc.Right = new BinaryTreeNode<T>(value);
						return;
					}
					currentLoc = currentLoc.Right;
				}
				else
				{
					if (currentLoc.Left == null)
					{
						currentLoc.Left = new BinaryTreeNode<T>(value);
						return;
					}
					currentLoc = currentLoc.Left;
				}
			}
		}

		// Breadth Search here
		public BinaryTreeNode<T> BreadthSearch(T value)
		{
			if (Root == null)
			{
				Console.WriteLine("Tree contains no elements");
				return null;
			}
			
			BinaryTreeNode<T> returnValue = null;
			if (Root.Value.CompareTo(value) == 0)
				returnValue = Root;
			List<BinaryTreeNode<T>> nodes = new List<BinaryTreeNode<T>> {Root};
			int iterations = 1;

			while (true)
			{
				if (returnValue != null)
					break;
				List<BinaryTreeNode<T>> tempList = nodes;
				nodes = new List<BinaryTreeNode<T>>();

				foreach (BinaryTreeNode<T> node in tempList)
				{
					if (node.Left != null)
					{
						nodes.Add(node.Left);
						if (node.Left.Value.CompareTo(value) == 0)
							returnValue = node.Left;
					}
					
					if (node.Right != null)
					{
						nodes.Add(node.Right);
						if (node.Right.Value.CompareTo(value) == 0)
							returnValue = node.Right;
					}
					iterations++;
				}

				if (nodes.Count == 0)
					break;
			}
			
			Debug.Log($"Amount of iterations: {iterations}");
			return returnValue;
		}

		// Depth Search here
		public BinaryTreeNode<T> DepthSearch(T value)
		{
			if (Root == null)
			{
				Debug.Log("Tree contains no elements");
				return null;
			}
			
			int iterations = 0;
			BinaryTreeNode<T> currentNode = Root;
			BinaryTreeNode<T> returnValue = null;
			
			while (true)
			{
				iterations++;
				if (currentNode == null)
					break;

				if (value.CompareTo(currentNode.Value) == 0)
				{
					returnValue = currentNode;
					break;
				}

				if (currentNode.Value.CompareTo(value) < 0)
				{
					currentNode = currentNode.Right;
				}
				else
				{
					currentNode = currentNode.Left;
				}
			}

			Debug.Log($"Amount of iterations: {iterations}");
			return returnValue;
		}

		public bool BreadthContains(T value) => BreadthSearch(value) != null;
		public bool DepthContains(T value) => DepthSearch(value) != null;
	}

	public class BinaryTreeNode<T> where T : IComparable
	{
		public T Value { get; set; } = default;
		public BinaryTreeNode<T> Left { get; set; } = null;
		public BinaryTreeNode<T> Right { get; set; } = null;

		public BinaryTreeNode(T value, BinaryTreeNode<T> left = null, BinaryTreeNode<T> right = null)
		{
			Value = value;
			Left = left;
			Right = right;
		}
	}
}