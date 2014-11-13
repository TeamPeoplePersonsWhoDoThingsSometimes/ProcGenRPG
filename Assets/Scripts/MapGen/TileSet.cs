using UnityEngine;
using System.Collections.Generic;

/**
 * A small class intended to hold a complete tileset for a specific generator
 */
public class TileSet : MonoBehaviour {
	public GeneratorTypes generatorType;
	public List<Tile> tiles;
	public List<Enemy> enemyTypes;
}
