using UnityEngine;
using System.Collections;

public class Area {

	public AreaData Data;

	private MapGenerator generator;

	private bool visited;

	/**
	 * true if new areas from this area have been ATTEMPTED to be made
	 */
	private bool isParent;

	private Area parent;
	private int xPos, yPos;

	public Area(TileSet tiles, Area parent, int x, int y) {
		Data = new AreaData(-1, "Landy Land", tiles.generatorType.ToString(), -1);
		this.generator = MapGenerator.getMapGenerator(this,tiles);
		this.generator.SetArea(this);
		visited = false;
		this.parent = parent;
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

	public void SetParent(Area parent) {
		this.parent = parent;
	}

	public Area GetParent() {
		return parent;
	}

	/**
	 * 0 is up, 1 is down, 2 is right, 3 is left
	 * returns -1 if no parent
	 */
	public int ParentDirection() {
		int direction = -1;
		if (parent.getY() > this.getY()) {
			direction = 0;
		} else if (parent.getY() < this.getY()) {
			direction = 1;
		} else if (parent.getX() > this.getX()) {
			direction = 2;
		} else if (parent.getX() < this.getX()) {
			direction = 3;
		}
		return direction;
	}

	public bool IsParent() {
		return isParent;
	}

	public void SetIsParent() {
		isParent = true;
	}
}
