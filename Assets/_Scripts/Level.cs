using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour {

	public string levelName;

	[Header("Objects")]
	public Grid grid;

	public Tilemap greyMap;
	public Tilemap colorMap;
	public Tilemap mixedMap;

	public Tile blankTile;

	private bool[,] colorSquares = new bool[31, 31];

	// Start is called before the first frame update
	void Start () {
		Game.instance.level = this;

		GenerateMixedColliders();

		float ratio = Screen.height / (float)Screen.width;

		if (ratio > 9 / 16.0f) {
			Camera.main.orthographicSize = (80 / 9.0f) * ratio;
		}
	}

	// Reads collider data from the greyMap and copies it into the mixedMap
	private void GenerateMixedColliders () {
		for (int x = -12; x <= 12; x++) {
			for (int y = -12; y <= 12; y++) {
				SetColorSquare(x, y, false);

				Vector3Int pos = new Vector3Int(x, y, 0);

				Tile tempTile = greyMap.GetTile<Tile>(pos);

				if (tempTile == null) {
					mixedMap.SetTile(pos, null);

					continue;
				}

				if (tempTile.colliderType == Tile.ColliderType.None) {
					mixedMap.SetTile(pos, null);
				}
				else {
					PlaceColliderTile(pos);
				}
			}
		}
	}

	// Copies a tile from the grey layer to the mixed layer, for collision
	public void PlaceColliderTile (Vector3Int pos) {
		Tile tempTile = greyMap.GetTile<Tile>(pos);

		mixedMap.SetTile(pos, tempTile);
	}

	// Checks if a partial square for color walls exists at the given location
	public bool CheckForColorSquare (int xPos, int yPos) {
		return colorSquares[xPos + 15, yPos + 15];
	}

	// Sets partial square data for color walls to true or false at the given location
	public void SetColorSquare (int xPos, int yPos, bool newSetting) {
		colorSquares[xPos + 15, yPos + 15] = newSetting;
	}
}
