using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public static Game instance;

	public Level level;
	public Kaleidoscope kaleidoscope;
	public Camera mainCam;

	[Header("Prefab References")]
	public PartialBlock partialBlock;


	// Start is called before the first frame update
	private void Start () {

		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Update () {
		if (mainCam == null) {
			mainCam = Camera.main;
		}
	}
}
