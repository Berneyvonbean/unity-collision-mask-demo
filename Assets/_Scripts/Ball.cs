using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	public float moveForce;
	public float jumpForce;

	private Rigidbody2D rb;

	private void Start () {
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update () {
		if (Input.GetKeyDown("space")) {
			rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}
	}

	private void FixedUpdate () {

		if (Input.GetKey("a")) {
			rb.AddForce(Vector2.right * -moveForce);
		}
		if (Input.GetKey("d")) {
			rb.AddForce(Vector2.right * moveForce);
		}
	}
}