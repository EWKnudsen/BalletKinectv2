using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

	//public Text uiText;
	[HideInInspector]
	public PlayVideo videoScript;
	[HideInInspector]
	public Image timer;
	[HideInInspector]
	public GameObject background;
	[HideInInspector]
	public Text instructions;
	public bool calibrationTimer;
	public bool calibrating;
	[HideInInspector]
	public bool finished;
	float duration;
	float timeLeft;
	float elapsedTime;

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Timer").GetComponent<Image> ();
		background = GameObject.Find ("Background");
		if (!calibrationTimer) {
			videoScript = GameObject.Find ("Video").GetComponent<PlayVideo> ();
			duration = videoScript.videoDuration;
		} else {
			background.gameObject.SetActive(false);
			instructions = GameObject.Find ("Instructions").transform.GetChild(0).GetComponent<Text>();
			duration = 10f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// For calculating time that can be set
		elapsedTime += Time.deltaTime;

		// For MainScene to display timer
		if (timer != null && videoScript != null) {
			timeLeft = Time.timeSinceLevelLoad / duration;
			timer.fillAmount = timeLeft;
		}

		// For checking when a key is down - only used in calibration scene
		if (calibrationTimer && Input.anyKeyDown) {
			calibrating = true;
			elapsedTime = 0f;
			background.gameObject.SetActive(true);
			GameObject.Find ("Instructions").transform.GetChild(1).gameObject.SetActive(false);
			instructions.text = "CALIBRATING...\n Please hold your position";
		}

		// When calibrating fill up the timer image. When it is filled change text and change scene
		if (calibrating) {
			timeLeft = elapsedTime / duration;
			timer.fillAmount = timeLeft;

			if (timer.fillAmount == 1f) {
				StartCoroutine("Finished");
			}
		}
	}

	IEnumerator Finished() {
		instructions.text = "FINISHED CALIBRATING";
		yield return new WaitForSeconds(1.5f);
		calibrating = false;
		finished = true;
		SceneManager.LoadScene("PlieDetails");
	}

}

