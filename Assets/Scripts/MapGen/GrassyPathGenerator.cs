using UnityEngine;
using System.Collections.Generic;

public class GrassyPathGenerator : MapGenerator {
	float cursorX = 0;
	float cursorZ = 0;

	public int maxPathSize = 30;
	public int minPathSize = 0;

	public int maxFieldSize = 6;
	public int minFieldSize = 4;

	public GrassyPathGenerator(Area a, TileSet tiles) : base(a,tiles) {
	}

	protected override void generateGround(int length) {
		int tempLength = (length % 4) + 3;
		bool left;
		bool up;

		bool justDidPath = false;
		bool justDidField = false;
		bool wasLeft = false;
		while(tempLength > 0) {
			left = Random.value > 0.5f;
			up = Random.value > 0.5f;
			//50% chance of path or field, if it is a path, make sure it doesn't go right
			//immediately after going left to avoid overlapping paths
			if(Random.value > 0.5f && !(justDidPath && wasLeft && !left && !up)) {
				spawnPath(Random.Range(maxPathSize, minPathSize), left, up);
				justDidPath = true;
				justDidField = false;
				wasLeft = left;
				tempLength--;
			} else if (!justDidField) {
				spawnField(Random.Range(maxFieldSize, minFieldSize), left, up);
				justDidField = true;
				justDidPath = false;
				tempLength--;
			}
		}
	}

	private void spawnPath(int distance, bool left, bool up) {
		// determines whether the path is going to move to the side or up
		bool direction = true;
		bool spawnedEnemy = false;
		while(distance > 0) {
			SpawnTile(cursorX, cursorZ, 0);
			if(spawnedEnemy) {
				SpawnEnemy(0, cursorX, cursorZ);
			}
			if (left && direction) {
				// spawns a tile beside the one going left, to make the path 2 blocks wide
				SpawnTile(cursorX, cursorZ + tileSet.tiles[0].size, 0);
				cursorX += tileSet.tiles[0].size;
			} else if (!left && direction) {
				// spawns a tile beside the one going right, to make the path 2 blocks wide
				SpawnTile(cursorX, cursorZ + tileSet.tiles[0].size, 0);
				cursorX -= tileSet.tiles[0].size;
			}
			if (up && !direction) {
				// spawns a tile beside the one going up, to make the path 2 blocks wide
				SpawnTile(cursorX + tileSet.tiles[0].size, cursorZ, 0);
				cursorZ += tileSet.tiles[0].size;
			}
			direction = Random.value < 0.5f;
			spawnedEnemy = Random.value < 0.25f;
			distance--;
		}
	}

	private void spawnField(int size, bool left, bool up) {
		float localX = cursorX;
		float localZ = cursorZ;
		if(left) {
			for(int i = 0; i < size; i++) {
				for(int j = 0; j < size; j++) {
					SpawnTile(localX, localZ, 0);
					localX += tileSet.tiles[0].size;
				}
				localZ += tileSet.tiles[0].size;
				localX = cursorX;
			}
			cursorX += (size - 1) * tileSet.tiles[0].size;
			cursorZ = localZ;
		} else {
			for(int i = 0; i < size; i++) {
				for(int j = 0; j < size; j++) {
					SpawnTile(localX, localZ, 0);
					localX -= tileSet.tiles[0].size;
				}
				localZ -= tileSet.tiles[0].size;
				localX = cursorX;
			}
			cursorX -= (size - 1) * tileSet.tiles[0].size;
			cursorZ = localZ;
		}
	}

	private void spawnOffShoot(int distance, int localX, int localZ) {
		for(int i = 0; i < distance; i++) {
			SpawnTile(localX, localZ, 0);
		}
	}

	protected override void generateStructures(List<Tile> ground, bool up, bool down, bool right, bool left) {
		bool hasDoneUp = !up;
		bool hasDoneDown = !down;
		bool hasDoneLeft = !left;
		bool hasDoneRight = !right;
		foreach(Tile t in ground) {
			SpawnTile(t.X + t.size, t.Z + t.size, 1);
			SpawnTile(t.X + t.size, t.Z - t.size, 1);
			SpawnTile(t.X - t.size, t.Z - t.size, 1);
			SpawnTile(t.X - t.size, t.Z + t.size, 1);
			SpawnTile(t.X, t.Z - t.size, 1);
			SpawnTile(t.X, t.Z + t.size, 1);
			SpawnTile(t.X + t.size, t.Z, 1);
			SpawnTile(t.X - t.size, t.Z, 1);
		}

		//Portal Generation Code
		foreach(Tile t in ground) {
			if(!hasDoneUp && ground.IndexOf(t) > ground.Count / Random.Range(2,5) && !TileExists(t.X, t.Z + t.size*2)){
				ForceTile(t.X, t.Z + t.size, 2);
				GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
				foreach(GameObject g in items) {
					if(g.GetComponent<Tile>().name.Equals("")) {
						g.GetComponent<Tile>().name = "UpPortal";
						break;
					}
				}
				hasDoneUp = true;
			}
			if(!hasDoneDown && ground.IndexOf(t) > ground.Count / Random.Range(2,5) && !TileExists(t.X, t.Z - t.size*2)){
				ForceTile(t.X, t.Z - t.size, 2);
				GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
				foreach(GameObject g in items) {
					if(g.GetComponent<Tile>().name.Equals("")) {
						g.GetComponent<Tile>().name = "DownPortal";
						break;
					}
				}
				hasDoneDown = true;
			}
			if(!hasDoneRight && ground.IndexOf(t) > ground.Count / Random.Range(2,5) && !TileExists(t.X + t.size*2, t.Z)){
				ForceTile(t.X + t.size, t.Z, 2);
				GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
				foreach(GameObject g in items) {
					if(g.GetComponent<Tile>().name.Equals("")) {
						g.GetComponent<Tile>().name = "RightPortal";
						break;
					}
				}
				hasDoneRight = true;
			}
			if(!hasDoneLeft && ground.IndexOf(t) > ground.Count / Random.Range(2,5) && !TileExists(t.X - t.size*2, t.Z)){
				ForceTile(t.X - t.size, t.Z, 2);
				GameObject[] items = GameObject.FindGameObjectsWithTag("Portal");
				foreach(GameObject g in items) {
					if(g.GetComponent<Tile>().name.Equals("")) {
						g.GetComponent<Tile>().name = "LeftPortal";
						break;
					}
				}
				hasDoneLeft = true;
			}
		}
	}
}
