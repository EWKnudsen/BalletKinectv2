using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CalibrationTimer : MonoBehaviour {

	//public Text uiText;
	[HideInInspector]
	public Image timer;
	[HideInInspector]
	public GameObject background;
	[HideInInspector]
	public Text instructions;
	//public bool calibrationTimer;
	public bool isCalibratingJoints;
	[HideInInspector]
	public bool finishedCalibrating;
	bool isGettingReady = false;
	int i = 5;
	float duration;
	float timeLeft;
	float elapsedTime;


	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () {
		timer = GameObject.Find ("Timer").GetComponent<Image> ();
		background = GameObject.Find ("Background");

		background.gameObject.SetActive(false);
		instructions = GameObject.Find ("Instructions").transform.GetChild(0).GetComponent<Text>();
		duration = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		// For calculating time that can be set
		elapsedTime += Time.deltaTime;

		// For checking when a key is down - only used in calibration scene
		if (Input.anyKeyDown && isGettingReady == false) {
			isGettingReady = true;
			GameObject.Find ("Instructions").transform.GetChild(1).gameObject.SetActive(false);
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

	IEnumerator Finished() {
		instructions.text = "FINISHED CALIBRATING";
		yield return new WaitForSeconds(1.5f);
		isCalibratingJoints = false;
		finishedCalibrating = true;
		isGettingReady = false;
		SceneManager.LoadScene("PlieDetails");
	}

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

