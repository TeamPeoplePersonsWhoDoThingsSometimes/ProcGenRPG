using UnityEngine;
using System.Collections.Generic;

/**
 * City generator
 * 
 * TileSet definitions:
 * 0-grass
 * 1-road
 * 2-block (for houses proabably, or parks)
 * 3-townCenter plot
 * 4-town center
 */
public class CityGenerator : MapGenerator {

	private static int WIDTH = 1600;//both width and height of the area
	private static float BRANCH_CHANCE = 0.5f;
	private static int BLOCK_RADIUS = 20;
	private static int MAX_DEPTH = 5;

	private Vector2 townCenterPlot; // tile identified by its top left corner
	private Vector2 townCenterDirection; //direction the town center should face
	private Dictionary<Vector2, Vector2> blocks; //tiles on top of which houses or parks can go, maps position and direction
	private List<Vector2> roads; // roads (by top left corner)
	private float minX, minY, maxX, maxY; //extrema for city roads
	private Vector2 center;
	private float radius;

	public CityGenerator(Area a, TileSet tiles) : base(a,tiles) {
		blocks = new Dictionary<Vector2, Vector2> ();
		roads = new List<Vector2> ();
	}

	/**
	 * Length determines city radius
	 */
	protected override void generateGround (int length)
	{
		center = new Vector2 (WIDTH / 2, WIDTH / 2);

		//step 1: generate main entrance and exit area roads
		//TODO handle not having exits on all sides
		Vector2 bottomVertical = Vector2.zero, topVertical = Vector2.zero, leftHorizontal = Vector2.zero, rightHorizontal = Vector2.zero;//extrema of main roads in city limits
		int mainTileCount = WIDTH / (int)tileSet.tiles[1].size;
		for (int i = 0; i <= mainTileCount; i++) {
			placeRoad(new Vector2(WIDTH/2, i*tileSet.tiles[1].size));
			placeRoad(new Vector2(i*tileSet.tiles[1].size, WIDTH/2));
			if (mainTileCount/2 + BLOCK_RADIUS == i) {
				topVertical = new Vector2(WIDTH/2,i*tileSet.tiles[1].size);
				rightHorizontal = new Vector2(i*tileSet.tiles[1].size,WIDTH/2);
				maxX = i*tileSet.tiles[1].size;
			} else if (mainTileCount/2 - BLOCK_RADIUS == i) {
				bottomVertical = new Vector2(WIDTH/2,i*tileSet.tiles[1].size);
				leftHorizontal = new Vector2(i*tileSet.tiles[1].size,WIDTH/2);
				minX = i*tileSet.tiles[1].size;
			}
		}

		maxY = maxX;
		minY = minX;
		
		//step 2: generate city radius
		radius = Random.Range (50, WIDTH/2 - 50);

		//step 3: place town center area
		//town center will always be on one of the four corners of the intersection
		//of the two primary roads or at the end of either road that dead-ends at the center of town
		//TODO handle dead ends (after handling not having exits on all sides
		int corner = Random.Range (1, 4);
		corner = 4;
		if (corner == 1) { // top right
			//big formula, but simple, it just cuts the width in half and adds back half a road to put the town center on the edge of a road at the center of the map
			townCenterPlot = new Vector2 (((float)WIDTH) / 2.0f + ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 + (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f);
			townCenterDirection = -Vector2.up;
		} else if (corner == 2) { //bottom right
			townCenterPlot = new Vector2 (((float)WIDTH) / 2.0f + ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 - (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f);
			townCenterDirection = -Vector2.right;
		} else if (corner == 3) { //bottom left
			townCenterPlot = new Vector2 (((float)WIDTH) / 2.0f - ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 - (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f);
			townCenterDirection = Vector2.up;
		} else if (corner == 4) { //top left
			townCenterPlot = new Vector2 (((float)WIDTH) / 2.0f - ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 + (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f);
			townCenterDirection = Vector2.right;
		}

		//step 4: recursive city radius fill step
		//randomly place roads along the main roads in
		//the city radius, with a higher chance of placing
		//the closer to the center of the city you are or when a road was placed on the other side
		//then do this exact same thing on the placed road,
		//but with decreased chances.  Stop when chances are negligible
		//or a potential branch cannot have a length that is larger than a block

		branchFromRoad (leftHorizontal, rightHorizontal, 0);
		branchFromRoad (bottomVertical, topVertical, 0);


		//step 5: place base tiles for buildings
		//basically just go through the list of roads and place blockss where possible
		foreach (Vector2 vec in roads) {
			if((vec-center).magnitude <= radius && Random.value > 0.5) { // only place in radius
				//try to place block on any side of the road

				Vector2 up = right (vec, Vector2.up);
				Vector2 rVec = right (vec, Vector2.right);
				Vector2 lVec = left (vec, Vector2.right);
				Vector2 down = left (vec, Vector2.up);

				tryPlaceBlock(up, Vector2.up);
				tryPlaceBlock(rVec, Vector2.right);
				tryPlaceBlock(lVec, -Vector2.right);
				tryPlaceBlock(down, -Vector2.up);
			}

		}

		//step 6: City outskirt generation
		// just go through all the area outside the city limits
		// and randomly place grass tiles PLACE OUTSKIRTS BUILDING HERE
		
		for (float i = 0; i < WIDTH; i += tileSet.tiles[0].size) {
			bool inCityWide = false;
			
			for(float j = 0; j < WIDTH; j += tileSet.tiles[0].size) {
				//if(inCityWide && j > WIDTH/2 - radius && j < WIDTH/2 + radius) continue;//skip when in the city
				if(!blocks.ContainsKey(new Vector2(i,j))) {
					SpawnTile(i,j,0);
				}
			}
		}


		//step 7: determine plazas
		//TODO change the roads list to a dictionary mapping ints to lists
		// in order to do this effeciently
		/*foreach (Vector2 vec in roads) {
			int check = 0;
		}*/

	}

	protected override void generateStructures (List<Tile> ground, bool up, bool down, bool right, bool left)
	{
		//place town center
		SpawnTileInDirection (townCenterPlot.x, townCenterPlot.y, 3, townCenterDirection);

		//place houses
		foreach (KeyValuePair<Vector2, Vector2> plot in blocks) {
			SpawnTileInDirection(plot.Key.x, plot.Key.y, 2, plot.Value);
		}
	}

	/**
	 * Generate a road branch off the given vector-defined road path (the vectors mark the top left of a road tile)
	 */
	private void branchFromRoad(Vector2 end1, Vector2 end2, int depth) {
		if (depth > MAX_DEPTH) {
			return;
		} // limit branching

		Vector2 dir = (end2 - end1).normalized;
		int lestBranch = 0;//makes the loop skip branches so that roads don't just branch side by side

		//branch loop
		for (Vector2 branchPoint = end1; (branchPoint - end1).magnitude < (end2 - end1).magnitude; branchPoint += dir * tileSet.tiles[1].size ) {
			if(lestBranch != 0) {
				lestBranch--;
				continue;
			}

			if(Random.value < BRANCH_CHANCE) {

				//branch dirs
				Vector2 d1 = new Vector2(dir.y, -dir.x);
				Vector2 d2 = new Vector2(-dir.y, dir.x);

				//randomize a branch in both dirs
				Vector2 dest1 = findFirstTile(branchPoint + d1 * tileSet.tiles[1].size, d1, Random.Range(0,WIDTH/(int)(2*tileSet.tiles[1].size))); //because with main roads, width/2 is largest possible road
				Vector2 dest2 = findFirstTile(branchPoint + d2 * tileSet.tiles[1].size, d2, Random.Range(0,WIDTH/(int)(2*tileSet.tiles[1].size)));

				//spawn tiles for the branches
				for (Vector2 dVec = branchPoint; (dVec - branchPoint).magnitude < (dest1 - branchPoint).magnitude; dVec += d1 * tileSet.tiles[1].size) {
					placeRoad(dVec);
				}

				for (Vector2 dVec = branchPoint + d2 * tileSet.tiles[1].size; (dVec - branchPoint).magnitude < (dest2 - branchPoint).magnitude; dVec += d2*tileSet.tiles[1].size) {
					placeRoad(dVec);
				}

				if(dest1 != branchPoint + d1 * tileSet.tiles[1].size && dest1 != end2) {
					branchFromRoad(branchPoint, dest1, depth + 1);
				}

				if(dest2 != branchPoint + d2 * tileSet.tiles[1].size && dest2 != end2) {
					branchFromRoad(branchPoint, dest2, depth + 1);
				}

				lestBranch = 2;
			}
		}
	}

	/**
	 * Finds the upper left point of the first tile in the given direction starting at the given point or edge of road bounds
	 * within the given distance. Farthest tile location with the given distance if no tiles are in the way.
	 * 
	 * Note:  This method also may create new branches to facilitate "magic" glueing of branches together as the method
	 * finds the end of this branch
	 */
	private Vector2 findFirstTile(Vector2 start, Vector2 dir, int dist) {
		Vector2 init = start;
		Vector2 pDir = new Vector2 (-dir.y, dir.x);

		//first check to make sure this is even a valid branch
		Vector2 lVec = left (start, pDir);
		Vector2 leftLeft = left (lVec, pDir);
		Vector2 rVec = right (start, pDir);
		Vector2 rightRight = right (rVec, pDir);
		Vector2 frontFront = start + 2 * dir * tileSet.tiles [1].size;
		if (TileExists (lVec.x, lVec.y) || TileExists (leftLeft.x, leftLeft.y) || TileExists (rVec.x, rVec.y) || TileExists (rightRight.x, rightRight.y)) {
			return init;
		}

		bool returnFromGlue = false;// if true, then the current loop has glued this road to another road and thus should be returned

		for (int i = 0; i < dist; i++) {
			lVec = left (start, pDir);
			leftLeft = left (lVec, pDir);
			rVec = right (start, pDir);
			rightRight = right (rVec, pDir);
			frontFront = start + 2 * dir * tileSet.tiles [1].size;
			if (TileExists(start.x, start.y) || TileExists(lVec.x, lVec.y) || TileExists (rVec.x, rVec.y) || (start - center).magnitude > radius) { //okay, we hit a boundary , so now we need to return our target
				if (i < 2) {//if within 2, don't branch, because it is wierd if we have a road length 1 that was forced to length 1
					return init;
				}

				return start - dir * tileSet.tiles[1].size * 3;
			}

			if(i >= 2) { // don't glue until 2 away from the source road
				if (TileExists(leftLeft.x, leftLeft.y)) { //glue together roads that move close enough together, then return
					for (Vector2 dVec = start; (dVec - start).magnitude < (start - leftLeft).magnitude; dVec -= pDir * tileSet.tiles[1].size) {
						placeRoad(dVec);
					}
					returnFromGlue = true;
				}

				if(TileExists(rightRight.x, rightRight.y)) {
					for (Vector2 dVec = start; (dVec - start).magnitude < (start - rightRight).magnitude; dVec += pDir * tileSet.tiles[1].size) {
						placeRoad(dVec);
					}
					returnFromGlue = true;
				}

				if(TileExists(frontFront.x, frontFront.y)) {
					for (Vector2 dVec = start; (dVec - start).magnitude < (start - frontFront).magnitude; dVec += dir * tileSet.tiles[1].size) {
						placeRoad(dVec);
					}
					returnFromGlue = true;
				}
			}

			if(returnFromGlue) {
				return start;
			}

			start += dir * tileSet.tiles[1].size;
		}
		return start;
	}

	/**
	 * place a road at the given position
	 */
	private void placeRoad(Vector2 pos) {
		SpawnTile (pos.x, pos.y, 1);
		roads.Add (pos);
	}

	/**
	 * Place a block at the given position facing the given direction
	 * if possible
	 */
	private void tryPlaceBlock(Vector2 pos, Vector2 front) {
		if(!TileExists(pos.x, pos.y) && !blocks.ContainsKey(pos)) {
			//SpawnTileInDirection (pos.x, pos.y, 2, front);
			blocks.Add (pos, front);
		}
	}

	/**
	 * Returns the point to the "left" (negative perpendicular)
	 * given a point and its positive perpundicular
	 */
	private Vector2 left(Vector2 point, Vector2 pDir) {
		return point - pDir * tileSet.tiles[1].size;
	}

	/**
	 * Returns the point to the "right" (negative perpendicular)
	 * given a point and its positive perpundicular
	 */
	private Vector2 right(Vector2 point, Vector2 pDir) {
		return point + pDir * tileSet.tiles[1].size;
	}
}