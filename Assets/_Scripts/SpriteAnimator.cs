using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour {

	[Header("References")]
	[SerializeField] private SpriteRenderer greySprite;
	[SerializeField] private SpriteRenderer colorSprite;

	[SerializeField] private Sprite[] greySprites;
	[SerializeField] private Sprite[] colorSprites;

	// Animation Variables
	private bool animating;
	private bool repeat;
	private bool flipX;
	private int[] sequence;
	private int currentFrame;
	private float frameDuration;
	private float currentFrameTime;

	// Pause Variables
	private bool paused;
	private float pauseDuration;

	// Start is called before the first frame update
	private void Start () {
		animating = false;
		paused = false;
	}

	// Update is called once per frame
	private void Update () {
		if (!animating) {
			return;
		}

		if (pauseDuration > 0) {
			pauseDuration -= Time.deltaTime;

			if (pauseDuration <= 0) {
				UnpauseAnimation();
			}
		}

		if (paused) {
			return;
		}

		currentFrameTime += Time.deltaTime;

		if (currentFrameTime >= frameDuration) {
			IncrementFrame();
		}
	}

	// Starts a new animation
	// - The "sequence" is an array of integers, each one being the index of a sprite.
	// - The "frameDuration" is how long (in seconds) each frame will be displayed for.
	// - The boolean "repeat" represents whether or not the animation should repeat.
	// - The boolean "flipX" represents whether the sprites should be flipped horizontally.
	public void StartAnimation (int[] sequence, float frameDuration, bool repeat, bool flipX) {
		if (animating && sequence == this.sequence && frameDuration == this.frameDuration && flipX == this.flipX) {
			return;
		}
		if (sequence == null) {
			return;
		}

		this.sequence = sequence;
		this.frameDuration = frameDuration;
		this.repeat = repeat;

		currentFrame = 0;
		currentFrameTime = 0;

		SetSprite(sequence[currentFrame]);

		this.flipX = flipX;
		greySprite.flipX = flipX;
		colorSprite.flipX = flipX;

		animating = true;
	}

	// Stops the current animation on whatever frame is currently displayed.
	public void StopAnimation () {
		animating = false;
		frameDuration = 0;
		currentFrameTime = 0;

		paused = false;
		pauseDuration = 0;
	}

	// Stops the current animation and sets the sprite based on the given index.
	public void StopAnimation (int stopSprite) {
		StopAnimation();

		SetSprite(stopSprite);
	}

	// Stops the current animation and sets the sprite to the first frame of the current sequence.
	public void StopAnimationOnFirst () {
		if (!animating) {
			return;
		}

		StopAnimation();

		SetSprite(sequence[0]);
	}

	// Stops the current animation and sets the sprite to the last frame of the current sequence.
	public void StopAnimationOnLast () {
		if (!animating) {
			return;
		}

		StopAnimation();

		SetSprite(sequence[sequence.Length - 1]);
	}

	// Pauses the current animation
	public void PauseAnimation () {
		if (!animating) {
			return;
		}

		paused = true;
		pauseDuration = 0;
	}

	// Pauses the current animation for the given amount of time (in seconds).
	public void PauseAnimation (float duration) {
		if (!animating) {
			return;
		}

		paused = true;
		pauseDuration = duration;
	}

	// Unpauses the currently paused animation. Does nothing if no animation was paused.
	public void UnpauseAnimation () {
		paused = false;
		pauseDuration = 0;
	}

	// Sets the frame duration of the current animation to a different amount.
	public void SetFrameDuration (float duration) {
		frameDuration = duration;
	}


	// Sets the current animation frame to the next frame in the sequence.
	private void IncrementFrame () {
		currentFrame++;

		if (currentFrame >= sequence.Length) {
			if (repeat) {
				currentFrame = 0;
			}
			else {
				StopAnimation();

				return;
			}
		}

		SetSprite(sequence[currentFrame]);

		currentFrameTime = 0;
	}


	// Sets the sprite of both the grey and colored SpriteRenderer to the given index.
	private void SetSprite (int spriteIndex) {
		if (sequence == null) {
			return;
		}
		if (spriteIndex < 0 || spriteIndex >= greySprites.Length) {
			return;
		}

		greySprite.sprite = greySprites[spriteIndex];
		colorSprite.sprite = colorSprites[spriteIndex];
	}
}





















