using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Kaleidoscope : MonoBehaviour {

	public GameObject mask;

	[Header("Properties")]
	public float diameter;
	public float radius;

	private float delay = 0.0f;
	private bool frozen;

	private GameObject blockContainer;
	private LinkedList<PartialBlock> inactiveBlocks = new LinkedList<PartialBlock>();
	private float[] squareEdges = new float[] {0.5f, -0.5f, -0.5f, 0.5f};

	// Start is called before the first frame update
	void Start () {
		Game.instance.kaleidoscope = this;

		mask.transform.localScale = Vector3.zero;
		radius = diameter / 2.0f;

		FillInactiveList();
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update () {
		if (delay > 0) {
			delay -= Time.deltaTime;

			return;
		}

		if (mask.transform.localScale.x != diameter) {
			mask.transform.localScale = Vector3.MoveTowards(mask.transform.localScale, new Vector3(diameter, diameter, 1), 20 * Time.deltaTime);
		}

		ProcessInput();

		UpdateTileColliders();
	}


	// Handle input that controls the kaleidoscope, such as mouse movement
	private void ProcessInput () {
		if (Input.GetKeyDown("t")) {
			frozen = !frozen;
		}

		if (frozen) {
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, 100)) {
			transform.localPosition = new Vector3(hit.point.x, hit.point.y, 0);
		}
	}


	// Update the colliders of tiles within range of the kaleidoscope
	private void UpdateTileColliders () {
		Level level = Game.instance.level;

		if (level == null) {
			return;
		}

		Vector3Int tilePos = level.grid.WorldToCell(transform.localPosition);

		for (int x = tilePos.x - (int)Mathf.Ceil(radius); x <= tilePos.x + (int)Mathf.Ceil(radius); x++) {
			for (int y = tilePos.y - (int)Mathf.Ceil(radius); y <= tilePos.y + (int)Mathf.Ceil(radius); y++) {
				Tile tempMixedTile = level.mixedMap.GetTile<Tile>(new Vector3Int(x, y, 0));
				Tile tempColorTile = level.colorMap.GetTile<Tile>(new Vector3Int(x, y, 0));

				// Check for full overlap
				if (tempMixedTile != null && tempColorTile != null) {
					if (tempMixedTile.colliderType == Tile.ColliderType.Grid && tempColorTile.colliderType == Tile.ColliderType.Grid) {
						continue;
					}
				}

				float[] edges;

				// Check for grey world colliders
				if (tempMixedTile != null) {
					edges = GetTileEdges(tempMixedTile);

					if (IsWithinRange(x + 0.5f, y + 0.5f, edges)) {
						level.mixedMap.SetTile(new Vector3Int(x, y, 0), null);
						PartialBlock newBlock = SetBlockActive(new Vector3(x + 0.5f, y + 0.5f, 0));
						newBlock.SetEdges(edges);
						newBlock.reversed = false;
						newBlock.Update();
					}
				}

				// Check for color world colliders
				if (tempColorTile != null) {
					edges = GetTileEdges(tempColorTile);

					if (IsWithinRange(x + 0.5f, y + 0.5f, edges)) {
						if (tempColorTile.colliderType != Tile.ColliderType.None && !level.CheckForColorSquare(x, y)) {
							PartialBlock newBlock = SetBlockActive(new Vector3(x + 0.5f, y + 0.5f, 0));
							newBlock.SetEdges(edges);
							newBlock.reversed = true;
							level.SetColorSquare(x, y, true);
							newBlock.Update();
						}
					}
				}
			}
		}
	}

	// Check if a tile is within range of the kaleidoscope
	private bool IsWithinRange (float xPos, float yPos, float[] edges) {
		Vector2 kPos = new Vector2(transform.localPosition.x, transform.localPosition.y);

		bool bottomLeft = Vector2.Distance(new Vector2(xPos + edges[2], yPos + edges[1]), kPos) <= radius;
		bool bottomRight = Vector2.Distance(new Vector2(xPos + edges[3], yPos + edges[1]), kPos) <= radius;
		bool topLeft = Vector2.Distance(new Vector2(xPos + edges[2], yPos + edges[0]), kPos) <= radius;
		bool topRight = Vector2.Distance(new Vector2(xPos + edges[3], yPos + edges[0]), kPos) <= radius;

		return (bottomLeft || bottomRight || topLeft || topRight);
	}

	// Returns the four edges of a tile's collider
	private float[] GetTileEdges (Tile tile) {
		if (tile.colliderType == Tile.ColliderType.Grid) {
			return squareEdges;
		}

		List<Vector2> shape = new List<Vector2>();
		tile.sprite.GetPhysicsShape(0, shape);

		float[] xList = new float[] {shape[0].x, shape[1].x, shape[2].x, shape[3].x};
		float[] yList = new float[] {shape[0].y, shape[1].y, shape[2].y, shape[3].y};
		float[] edges = new float[4];

		edges[0] = Mathf.Max(yList);
		edges[1] = Mathf.Min(yList);
		edges[2] = Mathf.Min(xList);
		edges[3] = Mathf.Max(xList);

		return edges;
	}


	// Sets a PartialBlock as inactive
	public void SetBlockInactive (PartialBlock block) {
		block.gameObject.SetActive(false);

		inactiveBlocks.AddLast(block);
	}

	// Sets an inactive PartialBlock as active and returns the PartialBlock
	public PartialBlock SetBlockActive (Vector3 pos) {
		PartialBlock block = inactiveBlocks.First.Value;
		inactiveBlocks.RemoveFirst();

		block.tilePos = Game.instance.level.grid.WorldToCell(pos);
		block.transform.localPosition = pos;
		block.gameObject.SetActive(true);

		return block;
	}

	// Checks approximately whether or not the gameobject is considered inside the kaleidoscope.
	public bool IsInKaleidoscope (GameObject testObject) {
		float dist = Vector3.Distance(new Vector3(testObject.transform.position.x, testObject.transform.position.y, 0), transform.position);

		return dist < radius;
	}

	// Returns a point on the edge of the kaleidoscope that lies on the given X
	// Coordinates returned will be relative to the given pos.
	public Vector2 GetPointOnX (float xPos, Vector3 pos) {
		float xPosRel = xPos - transform.localPosition.x;
		float yPosRel = Mathf.Sqrt((radius * radius) - (xPosRel * xPosRel));

		if (pos.y < transform.localPosition.y) {
			yPosRel *= -1;
		}

		xPosRel += transform.localPosition.x;
		yPosRel += transform.localPosition.y;

		xPosRel -= pos.x;
		yPosRel -= pos.y;

		return new Vector2(xPosRel, yPosRel);
	}

	// Returns a point on the edge of the kaleidoscope that lies on the given Y.
	// Coordinates returned will be relative to the given pos.
	public Vector2 GetPointOnY (float yPos, Vector3 pos) {
		float yPosRel = yPos - transform.localPosition.y;
		float xPosRel = Mathf.Sqrt((radius*radius) - (yPosRel*yPosRel));

		if (pos.x < transform.localPosition.x) {
			xPosRel *= -1;
		}

		xPosRel += transform.localPosition.x;
		yPosRel += transform.localPosition.y;

		xPosRel -= pos.x;
		yPosRel -= pos.y;

		return new Vector2(xPosRel, yPosRel);
	}


	// Fills the inactive list with PartialBlocks
	private void FillInactiveList () {
		if (blockContainer != null) {
			Destroy(blockContainer);
		}

		blockContainer = new GameObject();
		blockContainer.name = "Partial Blocks";
		blockContainer.transform.position = Vector3.zero;

		inactiveBlocks.Clear();

		int amount = (int)Mathf.Ceil((diameter + 1) * (diameter + 1));

		for (int i = 0; i < 49; i++) {
			inactiveBlocks.AddFirst(GeneratePartialBlock(Vector3.zero));
			inactiveBlocks.First.Value.transform.SetParent(blockContainer.transform);
			inactiveBlocks.First.Value.gameObject.SetActive(false);
		}
	}

	// Generates a new partial block at the given location.
	// Returns the newly generated PartialBlock instance
	private PartialBlock GeneratePartialBlock (Vector3 pos) {
		// PartialBlock newBlock = Instantiate<PartialBlock>(Game.instance.partialBlock, pos, Quaternion.Euler(0, 0, 0));

		return Instantiate<PartialBlock>(Game.instance.partialBlock, pos, Quaternion.Euler(0, 0, 0));
	}
}
