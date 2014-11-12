using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DungeonGenerator : MapGenerator {

	public DungeonGenerator(Area a, TileSet tiles) : base(a,tiles) {
	}

	protected override void generateGround(int length) {
		Vector2 cursor = Vector2.zero;
		int dir = Random.Range(0, 4);//0:-x, 1: +y, 2: +x, 3: -y
		int width = Random.Range(1,3);
		generateHallway(length, width, dir, leftDir(dir), cursor);
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

	Vector2 generateHallway(int n, int w, int dir, int lineDir, Vector2 cursor) {
		//float adjustLeft = (((float)w-1)/2 + (w%2 == 0? 1/2 : 0)) * tiles[0].size;
		for(int count = 0; count < n; count++) {
			generateLine(w, lineDir, cursor);
			cursor = moveCursor(dir, cursor);
			if(count % 3 == 0) {
				float action = Random.value;
				if(action > 0.8f) {//fork
					bool left = Random.value > 0.6;
					bool right = Random.value > 0.6;
					bool up = Random.value > 0.6;

					//if none are selected, randomly select at least one direction to generate in
					if(!left && !right && !up) {
						int motion = Random.Range(1,4);
						if(motion == 1) left = true;
						else if(motion == 2) right = true;
						else if(motion == 3) up = true;
					}

					int num = 0 + (left?1:0) + (right?1:0) + (up?1:0);

					//if the fork goes left or right, then the current path must fill in to the far edge of the fork
					/*Vector2 tmpCursor = cursor;
					if(left || right) {
						for(int i = 0; i < w; i++) {
							generateLine(w, (dir + 1) % 4, tmpCursor);
							tmpCursor = moveCursor(dir, tmpCursor);
						}
					}*/

					if(left) {
						generateHallway((n-count)/num, w, leftDir(dir), backDir(dir), moveCursorToSide(leftDir(dir), cursor));
					}
					if(right) {
						generateHallway((n-count)/num, w, rightDir(dir), backDir(dir), moveCursorToSide(rightDir(dir), cursor));
					}
					if(up) {
						generateHallway((n-count)/num, w, dir, lineDir, cursor);
					}
					return cursor;

				} else if(action > 0.79f) {//branch
					int branch = Random.Range(1,4);//1: left, 2: right, 3: both
					int width = Random.Range(1,1);
					if(branch == 1) {
						generateBranch(Random.Range(5,20), width, leftDir(dir), dir, moveCursorToSide(leftDir(dir), cursor));
					} else if(branch == 2) {
						generateBranch(Random.Range(5,20), width, rightDir(dir), dir, moveCursorToSide(rightDir(dir), cursor));
					} else {
						generateBranch(Random.Range(5,20), width, leftDir(dir), dir, moveCursorToSide(leftDir(dir), cursor));
						generateBranch(Random.Range(5,20), width, rightDir(dir), dir, moveCursorToSide(rightDir(dir), cursor));
					}
				} else if(action > 0.6f) {//generate room
					int height = Random.Range(1,5);
					int width = Random.Range(1,5);
					bool left = Random.value > 0.5f;
					bool right = Random.value > 0.5f;

					if(!left && !right) {
						left = Random.value > 0.5f;
						right = !left;
					}

					if(left && canRoomFit(width, height, leftDir(dir), cursor)) {
						generateRoom(width, height, leftDir(dir), moveCursorToSide(leftDir(dir), cursor));
					}
					if(right && canRoomFit(width, height, rightDir(dir), cursor)) {
						generateRoom(width, height, rightDir(dir),  moveCursorToSide(rightDir(dir), cursor));
					}
				}
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
		
		//note that this is not a regular cursor movement, thus the movecursor method
		//is innefective
		if(dir == 0) {
			localCursor.x -= w;
			localCursor.y -= h/2;
		} else if(dir == 1) {
			localCursor.x -= w/2;
		} else if(dir == 2) {
			localCursor.y -= h/2;
		} else if(dir == 3) {
			localCursor.y -= h;
			localCursor.x -= w/2;
		}

		for(float i = 0; i <= w; i+= tileSet.tiles[0].size) {
			for(float j = 0; j <= h; j+= tileSet.tiles[0].size) {
				SpawnTile(localCursor.x + i, localCursor.y + j, 0);
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
		
	}
}