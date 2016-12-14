using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CalibrationTimer : MonoBehaviour {

	[HideInInspector]
	public Image timer;				// The sprite image for the timer
	[HideInInspector]
	public GameObject background;	// The background of the timer (the dark area)
	[HideInInspector]
	public Text instructions;		// The text instructions
	public bool isCalibratingJoints;// Is the program calibrating?
	[HideInInspector]
	public bool finishedCalibrating;// Is the program finished calibrating?

	private bool isGettingReady = false;	// Calibration preparation
	private int i = 5;						// Used for calibration preperation
	private float duration = 10f;					// How long the calibration should take
	private float timeLeft;					// Used for calculating time left
	private float elapsedTime;				// Used to see how long time have elapsed from beginning of scene

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Timer").GetComponent<Image> ();	// Finds and initializes the sprite image for timer
		background = GameObject.Find ("Background");				// Finds and initialises background
		background.gameObject.SetActive(false);						// Sets background to be inactive
		instructions = GameObject.Find("Instructions").transform.GetChild(0).GetComponent<Text>();	// Initializes the instructions text
	}
	
	// Update is called once per frame
	void Update () {
		// For calculating time that can be set
		elapsedTime += Time.deltaTime;

		// For checking when a key is down - only used in calibration scene
		if (Input.anyKeyDown && isGettingReady == false) {
			isGettingReady = true;
			GameObject.Find ("Instructions").transform.GetChild(1).gameObject.SetActive(false); // Deactivates the red text
			StartCoroutine ("GetReady");
		}

		// When calibrating fill up the timer image. When it is filled change text and change scene
		if (isCalibratingJoints) {
			timeLeft = elapsedTime / duration;
			timer.fillAmount = timeLeft;

			if (timer.fillAmount == 1f) {
				StartCoroutine("Finished");
			}
		}
	}

	// This is called once the calibration is finished. Then it changes text, waits 1.5 sec and changes scene
	IEnumerator Finished() {		
        isCalibratingJoints = false;
        isGettingReady = false;
        finishedCalibrating = true;

        instructions.text = "FINISHED CALIBRATING";
		yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene("PlieDetails");
	}

	// This is called once a key is pressed. Counts down from five and calibrates once i = 0
	IEnumerator GetReady() {
		instructions.text = "GET READY \n" + i;
		yield return new WaitForSeconds(1f);
		i--;
		if (i == 0) {
			isCalibratingJoints = true;
			elapsedTime = 0f;
			background.gameObject.SetActive (true);
			instructions.text = "CALIBRATING...\n Please hold your position";
			StopCoroutine ("GetReady");
		} else {
			StartCoroutine ("GetReady");
		}
	}

}

