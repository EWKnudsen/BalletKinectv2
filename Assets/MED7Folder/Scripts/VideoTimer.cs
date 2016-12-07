using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoTimer : MonoBehaviour {

	//public Text uiText;
	[HideInInspector]
	public PlayVideo videoScript;
	[HideInInspector]
	public Image timer;
	[HideInInspector]
	public GameObject background;

	float duration;
	float timeLeft;

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Timer").GetComponent<Image> ();
		background = GameObject.Find ("Background");
		videoScript = GameObject.Find ("Video").GetComponent<PlayVideo> ();
		duration = videoScript.videoDuration;
	}

	// Update is called once per frame
	void Update () {
		// For MainScene to display timer
		if (timer != null && videoScript != null) {
			timeLeft = Time.timeSinceLevelLoad / duration;
			timer.fillAmount = timeLeft;
		}
	}

}

