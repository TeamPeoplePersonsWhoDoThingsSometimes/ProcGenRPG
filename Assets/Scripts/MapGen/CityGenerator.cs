using UnityEngine;
using System.Collections.Generic;

/**
 * City generator
 * 
 * TileSet definitions:
 * 0-grass
 * 1-road
 * 2-block (for houses proabably, or parks)
 * 3-townCenter
 * 
 */
public class CityGenerator : MapGenerator {

	private static int WIDTH = 800;//both width and height of the area

	private Vector2 townCenterPlot; // tile identified by its top left corner
	private List<Vector2> blocks; //tiles (by top left corner) on top of which houses or parks can go

	public CityGenerator(Area a, TileSet tiles) : base(a,tiles) {
		blocks = new List<Vector2> ();
	}

	/**
	 * Length determines city radius
	 */
	protected override void generateGround (int length)
	{
		//step 1: generate main entrance and exit area roads
		//TODO handle not having exits on all sides
		for (int i = 0; i <= WIDTH/tileSet.tiles[1].size; i++) {
			SpawnTile(WIDTH/2,i*tileSet.tiles[1].size,1);
			SpawnTile(i*tileSet.tiles[1].size,WIDTH/2,1);
		}

		//step 2: generate city radius
		int radius = Random.Range (50, WIDTH - 50);

		//step 3: place town center area
		//town center will always be on one of the four corners of the intersection
		//of the two primary roads or at the end of either road that dead-ends at the center of town
		//TODO handle dead ends (after handling not having exits on all sides
		int corner = Random.Range (1, 4);
		//if (corner == 1) { // top right
			SpawnTile (((float)WIDTH) / 2.0f + ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 + (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f, 3);
		/*} else if (corner == 2) {
			SpawnTile (((float)WIDTH) / 2.0f + ((float)(tileSet.tiles [3].size + tileSet.tiles [1].size)) * .5f, WIDTH / 2 + (tileSet.tiles [3].size + tileSet.tiles [1].size) * .5f, 3);		
		}*/

		//step 4: recursive city radius fill step
		//randomly place roads along the main roads in
		//the city radius, with a higher chance of placing
		//the closer to the center of the city you are or when a road was placed on the other side
		//then do this exact same thing on the placed road,
		//but with decreased chances.  Stop when chances are negligible
		//or a potential branch cannot have a length that is larger than a block


	}

	private void branchFromRoad(Vector2 end1, Vector2 end2) {
		Vector2 dir = (end1 - end2).normalized;
		/*for (Vector2 branchPoint = end1; branchPoint != end2; branchPoint += dir * tileSet.tiles[1].size ) {

		}*/
	}

}
