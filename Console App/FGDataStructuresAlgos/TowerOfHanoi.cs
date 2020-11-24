using System;

namespace FGDataStructuresAlgos
{
	public static class TowerOfHanoi
	{
		public static void TOH(int amountOfDiscs, int origin = 1, int via = 2, int destination = 3)
		{
			if (amountOfDiscs == 1)
			{
				Console.WriteLine($"Move one disc from {origin}, to {destination}");
			}
			else if (amountOfDiscs == 2)
			{
				TOH(1, origin, destination, via);
				TOH(1, origin, via, destination);
				TOH(1, via, origin, destination);
			}
			else
			{
				TOH(amountOfDiscs - 1, origin, destination, via);
				TOH(1, origin, via, destination);
				TOH(amountOfDiscs - 1, via, origin, destination);
			}
		}
	}
}