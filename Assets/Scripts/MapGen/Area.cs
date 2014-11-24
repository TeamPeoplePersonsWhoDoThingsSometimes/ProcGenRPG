using UnityEngine;
using System.Collections;

public class Area {

	public AreaData Data;

	//private TileSet tileSet;
	private MapGenerator generator;

	//private Area left, right, up, down;
	private bool visited;
	private int xPos, yPos;

	public Area(TileSet tiles, int x, int y) {
		Data = new AreaData(-1, "Landy Land", tiles.generatorType.ToString(), -1);
		this.generator = MapGenerator.getMapGenerator(this,tiles);
		this.generator.SetArea(this);
		visited = false;
		xPos = x;
		yPos = y;
	}

	public Area(AreaData data) {
		//saving integration, uncomment and modify to new API to properly integrate
		/*Data = data;
		this.generator = MapGenerator.getRandomMapgenerator(Data.generatorName);
		this.generator.SetArea(this);*/
	}

	// Use this for initialization
	public void Init() {
		if(Data.length != -1) {
			generator.InitWithData(Data);
		} else {
			Data.seed = Random.seed;
			generator.InitWithData(Data);
		}
	}

	public void Clear() {
		generator.Clear();
	}

	public void SetVisited() {
		visited = true;
	}

	public bool GetVisited() {
		return visited;
	}

	/*public void setLeft(Area left) {
		this.left = left;
		this.left.right = this;
	}

	public void setRight(Area right) {
		this.right = right;
		this.right.left = this;
	}

	public void setUp(Area up) {
		this.up = up;
		this.up.down = this;
	}

	public void setDown(Area down) {
		this.down = down;
		this.down.up = this;
	}*/

	public Area getLeft() {
		return Map.Get(xPos - 1, yPos);
	}

	public Area getRight() {
		return Map.Get(xPos + 1, yPos);
	}

	public Area getUp() {
		return Map.Get(xPos, yPos + 1);
	}

	public Area getDown() {
		return Map.Get(xPos, yPos - 1);
	}

	public bool IsDeadEnd() {
		return (getUp() == null && getLeft() == null && getRight() == null && getDown() != null)
			|| (getUp() == null && getLeft() == null && getRight() != null && getDown() == null)
				|| (getUp() == null && getLeft() != null && getRight() == null && getDown() == null)
				|| (getUp() != null && getLeft() == null && getRight() == null && getDown() == null);
	}

	public bool IsIsland() {
		return getUp() == null && getLeft() == null && getRight() == null && getDown() == null;
	}

	public bool HasUp() {
		return getUp() != null;
	}

	public bool HasDown() {
		return getDown() != null;
	}

	public bool HasRight() {
		return getRight() != null;
	}

	public bool HasLeft() {
		return getLeft() != null;
	}

	public int getX() {
		return xPos;
	}

	public int getY() {
		return yPos;
	}
}
