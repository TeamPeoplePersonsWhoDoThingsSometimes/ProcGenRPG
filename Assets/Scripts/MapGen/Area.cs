using UnityEngine;
using System.Collections;

public class Area {

	public AreaData Data;

	//private TileSet tileSet;
	private MapGenerator generator;

	private Area left, right, up, down;

	public Area(TileSet tiles) {
		Data = new AreaData(-1, "Landy Land", tiles.generatorType.ToString(), -1);
		this.generator = MapGenerator.getMapGenerator(this,tiles);
		this.generator.SetArea(this);
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

	public void setLeft(Area left) {
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
	}

	public Area getLeft() {
		return this.left;
	}

	public Area getRight() {
		return this.right;
	}

	public Area getUp() {
		return this.up;
	}

	public Area getDown() {
		return this.down;
	}

	public bool IsDeadEnd() {
		return (up == null && left == null && right == null && down != null)
			|| (up == null && left == null && right != null && down == null)
				|| (up == null && left != null && right == null && down == null)
				|| (up != null && left == null && right == null && down == null);
	}

	public bool IsIsland() {
		return up == null && left == null && right == null && down == null;
	}

	public bool HasUp() {
		return up != null;
	}

	public bool HasDown() {
		return down != null;
	}

	public bool HasRight() {
		return right != null;
	}

	public bool HasLeft() {
		return left != null;
	}

}
