using UnityEngine;
using System.Collections;

/**
 * Used for storing Area object positions and getting them easily
 */
public class Map {

	/**
	 * storage for different quadrants of the Map
	 */
	private static Area[,] q1, q2, q3, q4;

	private static readonly int INITIAL_ARRAY_SIZE = 10;

	private static int areasFormed;
	private static int mostX, leastX, mostY, leastY;

	public static void Init() {
		q1 = new Area[INITIAL_ARRAY_SIZE, INITIAL_ARRAY_SIZE];
		q2 = new Area[INITIAL_ARRAY_SIZE, INITIAL_ARRAY_SIZE];
		q3 = new Area[INITIAL_ARRAY_SIZE, INITIAL_ARRAY_SIZE];
		q4 = new Area[INITIAL_ARRAY_SIZE, INITIAL_ARRAY_SIZE];
		areasFormed = 0;
	}

	/**
	 * Add an area to a position on the map.
	 * Returns false if spot is already filled or neighbors of possible new area have already been visited.
	 */
	public static bool Add(Area a) {
		int x = Mathf.Abs(a.getX());
		int y = Mathf.Abs(a.getY());
		while (x >= q1.GetLength(0) || y >= q1.GetLength(0)) {
			Resize();
		}

		if (a.getUp() != null && a.getUp().GetVisited()
		    || a.getDown() != null && a.getDown().GetVisited()
		    || a.getLeft() != null && a.getLeft().GetVisited()
		    || a.getRight() != null && a.getRight().GetVisited()) {
			return false;
		}

		if (Get(a.getX(), a.getY()) == null) {
			if (a.getX() >= 0) {
				if (a.getY() >= 0) {
					q1[x,y] = a;
				} else {
					q4[x,y] = a;
				}
			} else {
				if (a.getY() >= 0) {
					q2[x,y] = a;
				} else {
					q3[x,y] = a;
				}
			}
			areasFormed++;
			if (a.getX() > mostX)
				mostX = a.getX();
			if (a.getX() < leastX)
				leastX = a.getX();
			if (a.getY() > mostY)
				mostY = a.getY();
			if (a.getY() < leastY)
				leastY = a.getY();
			return true;
		} else {
			return false;
		}
	}

	/**
	 * Doubles the length of the array storage if need be
	 */
	private static void Resize() {
		int newLength = q1.GetLength(0) * 2;
		Area[,] new1 = new Area[newLength, newLength];
		Area[,] new2 = new Area[newLength, newLength];
		Area[,] new3 = new Area[newLength, newLength];
		Area[,] new4 = new Area[newLength, newLength];

		for (int x = 0; x < q1.GetLength(0); x++) {
			for (int y = 0; y < q1.GetLength(0); y++) {
				new1[x, y] = q1[x, y];
				new2[x, y] = q2[x, y];
				new3[x, y] = q3[x, y];
				new4[x, y] = q4[x, y];
			}
		}
		q1 = new1;
		q2 = new2;
		q3 = new3;
		q4 = new4;
	}

	/**
	 * Gets the Area in the given position of the map; null if empty
	 */
	public static Area Get(int x_Pos, int y_Pos) {
		Area area = null;
		int x = Mathf.Abs(x_Pos);
		int y = Mathf.Abs(y_Pos);
		if (x < q1.GetLength(0) && y < q1.GetLength(0)) {
			if (x_Pos >= 0) {
				if (y_Pos >= 0) {
					area = q1[x, y];
				} else {
					area = q4[x, y];
				}
			} else {
				if (y_Pos >= 0) {
					area = q2[x, y];
				} else {
					area = q3[x, y];
				}
			}
		}
		return area;
	}

	/**
	 * returns the number of areas that exist (are not null) in this map
	 */
	public static int GetSize() {
		return areasFormed;
	}

	/**
	 * return the x index of the most right area
	 */
	public static int GetMostX() {
		return mostX;
	}

	/**
	 * return the y index of the highest area
	 */
	public static int GetMostY() {
		return mostY;
	}

	/**
	 * return the x index of the most left area
	 */
	public static int GetLeastX() {
		return leastX;
	}

	/**
	 * return the y index of the lowest area
	 */
	public static int GetLeastY() {
		return leastY;
	}

	public static void PrintMap() {
		string map = "";
		for (int x = -1 * q1.GetLength(0) + 1; x < q1.GetLength(0); x++) {
			for (int y = -1 * q1.GetLength(0) + 1; y < q1.GetLength(0); y++) {
				if (x == 0 && y == 0) {
					map += "2 ";
				} else if (Get(x, y) != null) {
					map += "1 ";
				} else {
					map += "0 ";
				}
			}
			map += "\n";
		}
		Debug.Log(map);
	}
}
