using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

	//public Text uiText;
	public PlayVideo videoScript;
	public Image timer;
	float clipDuration;
	float timeLeft;

	// Use this for initialization
	void Start () {
		//uiText = FindObjectOfType<Text>();
		videoScript = GameObject.Find ("Video").GetComponent<PlayVideo> ();
		timer = GameObject.Find ("Timer").GetComponent<Image> ();
		clipDuration = videoScript.videoDuration;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer != null) {
			timeLeft = Time.timeSinceLevelLoad / clipDuration;
			timer.fillAmount = timeLeft;
		}
	}

}

