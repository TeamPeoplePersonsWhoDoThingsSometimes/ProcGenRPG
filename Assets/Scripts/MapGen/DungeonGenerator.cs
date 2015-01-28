using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DungeonGenerator : MapGenerator {

	public DungeonGenerator(Area a, TileSet tiles) : base(a,tiles) {
	}

	protected override void generateGround(int length) {
		length /= 4;
		Vector2 cursor = Vector2.zero;
		int dir = Random.Range(0, 4);//0:-x, 1: +y, 2: +x, 3: -y
		int width = Random.Range(1,3);
		generateHallway(length, width, dir, cursor);
	}

	int leftDir(int dir) {
		//wierd conditional handles the fact that -1%4 returns -1
		return (dir-1<0?3:dir-1);
	}

	int rightDir(int dir) {
		return (dir + 1) % 4;
	}

	int backDir(int dir) {
		return (dir + 2) % 4;
	}

	Vector2 moveCursor(int dir, Vector2 cursor) {
		if(dir == 0) {
			cursor += -Vector2.right.normalized*tileSet.tiles[0].size;
		} else if(dir == 1) {
			cursor += Vector2.up.normalized*tileSet.tiles[0].size;
		} else if(dir == 2) {
			cursor += Vector2.right.normalized*tileSet.tiles[0].size;
		} else if(dir == 3) {
			cursor += -Vector2.up.normalized*tileSet.tiles[0].size;
		}
		return cursor;
	}

	Vector2 moveCursor(int dir, Vector2 cursor, float dist) {
		if(dir == 0) {
			cursor += -Vector2.right.normalized*dist;
		} else if(dir == 1) {
			cursor += Vector2.up.normalized*dist;
		} else if(dir == 2) {
			cursor += Vector2.right.normalized*dist;
		} else if(dir == 3) {
			cursor += -Vector2.up.normalized*dist;
		}
		return cursor;
	}

	//if not left, then right
	Vector2 moveCursorToSide(int dir, Vector2 cursor) {
		do {
			cursor = moveCursor(dir, cursor);
		}while(TileExists(cursor.x, cursor.y));
		return cursor;
	}

	/**
	 * A straight line, used in other generators
	 */
	Vector2 generateLine(int n, int dir, Vector2 cursor) {
		for(int i = 0; i < n; i++) {
			SpawnTile(cursor.x, cursor.y, 0);
			cursor = moveCursor(dir, cursor);
		}
		return cursor;
	}

	Vector2 generateHallway(int n, int w, int dir, Vector2 cursor) {
		for(int count = 0; count < n; count++) {
			cursor = generateLine(w, dir, cursor);
			if(count % 2 == 0) {
				float action = Random.value;
				if(action > 0.8f) {//fork
					bool left = Random.value > 0.5f;
					bool up = Random.value > 0.5f;
					bool dirSwitch = (dir == 0 || dir == 2) ? Random.value > 0.9f : Random.value > 0.1f;

//					int num = 0 + (left?1:0) + (right?1:0) + (up?1:0);

					if(left && dirSwitch) {
//						Debug.LogError("Left");
						generateHallway(n-count, 1, 0, cursor);
					}
					if(!left && dirSwitch) {
//						Debug.LogError("Right");
						generateHallway(n-count, 1, 2, cursor);
					}
					if(up && !dirSwitch) {
//						Debug.LogError("Up");
						generateHallway(n-count, 1, 1, cursor);
					}
					if(!up && !dirSwitch) {
//						Debug.LogError("Down");
						generateHallway(n-count, 1, 3, cursor);
					}
					return cursor;
				} else if(action > 0.6f) {//generate room
					int height = Random.Range(1,4);
					int width = Random.Range(1,4);
					bool left = Random.value > 0.5f;
					bool right = Random.value > 0.5f;
					
					if(!left && !right) {
						left = Random.value > 0.5f;
						right = !left;
					}
					
					if(left && canRoomFit(width, height, leftDir(dir), cursor)) {
						generateRoom(width, height, leftDir(dir), cursor);
					}
					if(right && canRoomFit(width, height, rightDir(dir), cursor)) {
						generateRoom(width, height, rightDir(dir),  cursor);
					}
				}
//				else if(action > 0.79f) {//branch
//					int branch = Random.Range(1,4);//1: left, 2: right, 3: both
//					int width = Random.Range(1,1);
//					if(branch == 1) {
//						generateBranch(Random.Range(5,20), width, leftDir(dir), dir, moveCursorToSide(leftDir(dir), cursor));
//					} else if(branch == 2) {
//						generateBranch(Random.Range(5,20), width, rightDir(dir), dir, moveCursorToSide(rightDir(dir), cursor));
//					} else {
//						generateBranch(Random.Range(5,20), width, leftDir(dir), dir, moveCursorToSide(leftDir(dir), cursor));
//						generateBranch(Random.Range(5,20), width, rightDir(dir), dir, moveCursorToSide(rightDir(dir), cursor));
//					}
//				}
			}
		}
		return cursor;
	}

	/**
	 * Like a hallway, except this will not spawn forks, grand rooms, or other branches
	 */
	Vector2 generateBranch(int n, int w, int dir, int lineDir,Vector2 cursor) {
		for(int count = 0; count < n; count++) {
			//float action = Random.value;
			generateLine(w, lineDir, cursor);
			cursor = moveCursor(dir, cursor);
		}
		return cursor;
	}

	void generateRoom(float w, float h, int dir, Vector2 cursor) {
		w *= tileSet.tiles[0].size;
		h *= tileSet.tiles[0].size;
		Vector2 localCursor = cursor;
		SpawnTile(localCursor.x, localCursor.y, 0);
		localCursor = moveCursor(dir, localCursor);

		for(float i = 0; i <= w; i+= tileSet.tiles[0].size) {
			for(float j = 0; j <= h; j+= tileSet.tiles[0].size) {
				SpawnTile(localCursor.x + i, localCursor.y + j, 0);
				for(int k = 0; k < tileSet.enemyTypeChances.Count; k++) {
					if (Random.value < tileSet.enemyTypeChances[k]) {
						SpawnEnemy(k, localCursor.x + i, localCursor.y + j);
					}
				}
			}
		}
	}
	
	bool canRoomFit(float w, float h, int dir, Vector2 cursor) {
		w *= tileSet.tiles[0].size;
		h *= tileSet.tiles[0].size;
		Vector2 localCursor = cursor;
		if(dir == 0) {
			w++;
			localCursor.x -= w;
			localCursor.y -= h/2;
		} else if(dir == 1) {
			h++;
			localCursor.x -= w/2;
		} else if(dir == 2) {
			w++;
			localCursor.y -= h/2;
		} else if(dir == 3) {
			h++;
			localCursor.y -= h;
			localCursor.x -= w/2;
		}

		for(float i = 0; i <= w; i += tileSet.tiles[0].size) {
			if(TileExists(localCursor.x + i, localCursor.y)) 
				return false;
			else if(TileExists(localCursor.x + i, localCursor.y + h)) 
				return false;
		}
		for(float i = 0; i <= h; i += tileSet.tiles[0].size) {
	    	if(TileExists(localCursor.x, localCursor.y + i))
			   return false;
			else if(TileExists(localCursor.x + w, localCursor.y + i))
			   return false;
	   	}
	
	   	return true;
	}

	protected override void generateStructures(List<Tile> ground, bool up, bool down, bool right, bool left) {
		bool hasDoneUp = !up;
		bool hasDoneDown = !down;
		bool hasDoneLeft = !left;
		bool hasDoneRight = !right;
		bool placeLight = false;
		float placeLightCounter = 5;
		foreach(Tile t in ground) {
			if(placeLightCounter > 4) {
				placeLight = true;
				placeLightCounter = 0;
			} else {
				placeLightCounter++;
				placeLight = false;
			}
			SpawnTileInDirection(t.X, t.Z - t.size, placeLight ? 2 : 1, Vector2.right);
			SpawnTileInDirection(t.X, t.Z + t.size, placeLight ? 2 : 1, -Vector2.right);
			SpawnTileInDirection(t.X + t.size, t.Z, placeLight ? 2 : 1, Vector2.up);
			SpawnTileInDirection(t.X - t.size, t.Z, placeLight ? 2 : 1, -Vector2.up);
		}

		foreach (Tile t in ground) {
			SpawnTile(t.X + t.size, t.Z + t.size, 1);
			SpawnTile(t.X + t.size, t.Z - t.size, 1);
			SpawnTile(t.X - t.size, t.Z - t.size, 1);
			SpawnTile(t.X - t.size, t.Z + t.size, 1);
		}
	}
}