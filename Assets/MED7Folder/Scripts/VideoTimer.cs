using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoTimer : MonoBehaviour {

	[HideInInspector]
	public PlayVideo videoScript;		// The video script
	[HideInInspector]
	public Image timer;					// The sprite image used for the timer
	[HideInInspector]
	public GameObject background;		// The background of the timer

	private float duration;				// The duration
	private float timeLeft;				// The time left

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Timer").GetComponent<Image> ();
		background = GameObject.Find ("Background");
		videoScript = GameObject.Find ("Video").GetComponent<PlayVideo> ();
		duration = videoScript.videoDuration; // Duration is set to the duration of the video
	}

	// Update is called once per frame
	void Update () {
		// When both the timer and the videoscript is not null
		if (timer != null && videoScript != null) {
			timeLeft = Time.timeSinceLevelLoad / duration;	// Calculation of the time elapsed in the video from 0 - 1
			timer.fillAmount = timeLeft;					// Fills the sprite image with the value above
		}
	}

}

