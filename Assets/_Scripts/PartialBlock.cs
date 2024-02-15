using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PartialBlock : MonoBehaviour {

	public EdgeCollider2D col;
	public bool reversed;
	public Vector3Int tilePos;

	private bool bottomLeft;
	private bool bottomRight;
	private bool topLeft;
	private bool topRight;

	private float top;
	private float bottom;
	private float left;
	private float right;

	private List<Vector2> points = new List<Vector2>();


	private void Start () {

	}


	// Update is called once per frame
	public void Update () {
		UpdateCollider();
	}


	// Update the collider for this frame by either disabling it or generating a new shape
	private void UpdateCollider () {
		int count = CountCorners(tilePos.x + 0.5f, tilePos.y + 0.5f);

		if (count == 0) {
			Disable();
		}
		else if (!reversed && count == 4) {
			col.enabled = false;
		}
		else {
			GetPoints(tilePos.x + 0.5f, tilePos.y + 0.5f);
			GenerateCollider();
		}
	}


	// Generate a new collider shape based on the calculated points
	private void GenerateCollider () {
		Vector2[] newPoints = new Vector2[points.Count + 1];
		newPoints[points.Count].Set(points[0].x, points[0].y);

		for (int i = 0; i < points.Count; i++) {
			newPoints[i].Set(points[i].x, points[i].y);
		}

		col.points = newPoints;

		DrawLines(col.points);

		col.enabled = true;
	}


	// Counts the number of tile corners that are within the kaleidoscope range
	private int CountCorners (float xPos, float yPos) {
		int count = 0;
		Kaleidoscope kaleidoscope = Game.instance.kaleidoscope;
		Vector2 kPos = new Vector2(kaleidoscope.transform.localPosition.x, kaleidoscope.transform.localPosition.y);

		bottomLeft = (Vector2.Distance(new Vector2(xPos + left, yPos + bottom), kPos) <= kaleidoscope.radius);
		bottomRight = (Vector2.Distance(new Vector2(xPos + right, yPos + bottom), kPos) <= kaleidoscope.radius);
		topLeft = (Vector2.Distance(new Vector2(xPos + left, yPos + top), kPos) <= kaleidoscope.radius);
		topRight = (Vector2.Distance(new Vector2(xPos + right, yPos + top), kPos) <= kaleidoscope.radius);

		if (bottomLeft) {count++;}
		if (bottomRight) {count++;}
		if (topLeft) {count++;}
		if (topRight) {count++;}

		return count;
	}


	// Fills the points list (in order clock-wise) with all points that should be included in the updated collider shape
	private void GetPoints (float xPos, float yPos) {
		points.Clear();
		Kaleidoscope kaleidoscope = Game.instance.kaleidoscope;

		// Top left corner:
		if (topLeft == reversed) {
			points.Add(new Vector2(left, top));
		}

		// Point on top edge:
		if ( (topLeft && !topRight) || (topRight && !topLeft) ) {
			points.Add( kaleidoscope.GetPointOnY(transform.position.y + top, transform.position) );
		}

		// Top right corner:
		if (topRight == reversed) {
			points.Add(new Vector2(right, top));
		}

		// Point on right edge:
		if ( (topRight && !bottomRight) || (bottomRight && !topRight) ) {
			points.Add( kaleidoscope.GetPointOnX(transform.position.x + right, transform.position) );
		}

		// Bottom right corner:
		if (bottomRight == reversed) {
			points.Add(new Vector2(right, bottom));
		}

		// Point on bottom edge:
		if ( (bottomLeft && !bottomRight) || (bottomRight && !bottomLeft) ) {
			points.Add( kaleidoscope.GetPointOnY(transform.position.y + bottom, transform.position) );
		}

		// Bottom left corner:
		if (bottomLeft == reversed) {
			points.Add(new Vector2(left, bottom));
		}

		// Point on left edge:
		if ( (topLeft && !bottomLeft) || (bottomLeft && !topLeft) ) {
			points.Add( kaleidoscope.GetPointOnX(transform.position.x + left, transform.position) );
		}
	}


	// Disables this PartialBlock
	private void Disable () {
		if (reversed) {
			Game.instance.level.SetColorSquare(tilePos.x, tilePos.y, false);
		}
		else {
			Game.instance.level.PlaceColliderTile(tilePos);
		}

		Game.instance.kaleidoscope.SetBlockInactive(this);
	}


	// Draws red outlines around a collider. Use this for debugging only
	// (Gizmos must be enabled in the window to see the red lines)
	private void DrawLines (Vector2[] points) {
		for (int i = 0; i < points.Length - 1; i++) {
			Vector3 pos1 = transform.position + new Vector3(points[i].x, points[i].y, -0.1f);
			Vector3 pos2 = transform.position + new Vector3(points[i + 1].x, points[i + 1].y, -0.1f);

			Debug.DrawLine(pos1, pos2, new Color(1, 0, 0, 1), 0.025f, false);
		}
	}

	// Sets the edge variables of this partial block
	public void SetEdges (float[] edges) {
		top = edges[0];
		bottom = edges[1];
		left = edges[2];
		right = edges[3];
	}
}
