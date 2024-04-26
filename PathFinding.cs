using System;
using System.Collections.Generic;

namespace Dungeon_Escape
{
	class PathFinding
	{
		private static List<char> Direction_List = new();
		private static bool directionsFound;

		/// <summary>
		/// Finds the shortest path from the source cell to the destination cell via recursion.
		/// </summary>
		/// <param name="mat">The 2D int array holding the board layout.</param>
		/// <param name="i">The y coordinate of the source cell.</param>
		/// <param name="j">The x coordinate of the source cell.</param>
		/// <param name="y">The y coordinate of the destination cell.</param>
		/// <param name="x">The y coordinate of the destination cell.</param>
		/// <returns>A queue holding the directions (N, E, S, W) to get to the desired destination.</returns>
		public static List<char> FindPath(int[,] mat, int i, int j, int x, int y)
		{
			Direction_List.Clear();
			directionsFound = false;

			// if invalid input, 'dirList' is returned as empty //
			if (mat[i, j] == 1 || mat[x, y] == 1 || mat[i, j] == 3 || mat[x, y] == 3)
			{
				return Direction_List;
			}

			int min_dist;

			// creates a bool array of the same size of the board layout array //
			bool[,] visited = new bool[mat.GetLength(0), mat.GetLength(1)];

			// shortest path is recursively found //
			min_dist = FindShortestPath(mat, visited, i, j, x, y, int.MaxValue, 0);

			// if shortest path successfully found, return 'dirList', otherwise return empty //
			if (min_dist != int.MaxValue)
			{
				return Direction_List;
			}

			return Direction_List;
		}

		private static int FindShortestPath(int[,] mat, bool[,] visited, int i, int j, int x, int y, int min_dist, int dist)
		{
			// if the destination is found, update 'min_dist' //
			if (i == x && j == y)
			{
				directionsFound = true;
				return Math.Min(dist, min_dist);
			}

			// set (i, j) cell as visited //
			visited[i, j] = true;

			// go to the bottom cell //
			if (IsSafe(mat, visited, i + 1, j) && !directionsFound)
			{
				Direction_List.Add('S');
				min_dist = FindShortestPath(mat, visited, i + 1, j, x, y, min_dist, dist + 1);
			}

			// go to the right cell //
			if (IsSafe(mat, visited, i, j + 1) && !directionsFound)
			{
				Direction_List.Add('E');
				min_dist = FindShortestPath(mat, visited, i, j + 1, x, y, min_dist, dist + 1);
			}

			// go to the top cell //
			if (IsSafe(mat, visited, i - 1, j) && !directionsFound)
			{
				Direction_List.Add('N');
				min_dist = FindShortestPath(mat, visited, i - 1, j, x, y, min_dist, dist + 1);
			}

			// go to the left cell //
			if (IsSafe(mat, visited, i, j - 1) && !directionsFound)
			{
				Direction_List.Add('W');
				min_dist = FindShortestPath(mat, visited, i, j - 1, x, y, min_dist, dist + 1);
			}

			// if no moves can be taken, remove (i, j) from the visited matrix, dequeue 'CurrentPath', and return to previous cell //

			if (!directionsFound)
			{
				Direction_List.RemoveAt(Direction_List.Count - 1);
			}

			visited[i, j] = false;

			return min_dist;
		}

		/// <summary>
		/// Checks if it is possible to go to (x, y) from the current position.
		/// </summary>
		/// <param name="mat">The 2D int array holding the map layout.</param>
		/// <param name="visited">The 2D bool array holding which cells have been visited.</param>
		/// <param name="x">The x coordinate being checked.</param>
		/// <param name="y">The y coordinate being checked.</param>
		/// <returns>A bool signifying validity of movement.</returns>
		private static bool IsSafe(int[,] mat, bool[,] visited, int x, int y)
			// x and y coordinated are within bounds of the array //
			=> x >= 0 && x < mat.GetLength(0)
			&& y >= 0 && y < mat.GetLength(1)
			// cell is empty and unvisited //
			&& mat[x, y] != 1 && mat[x, y] != 3
			&& !visited[x, y];
	}
}