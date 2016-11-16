using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	public CMCTorsoNotchAtten cubemanScript;
	public float score;
	public float scoreTemp;
	private int counter;
	private Scene scene;
	private bool sceneChanged;


	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
		sceneChanged = false;
		scene = SceneManager.GetActiveScene();
		cubemanScript = GameObject.Find ("FilterController").GetComponent<CMCTorsoNotchAtten> ();
		if (cubemanScript != null) {
			StartCoroutine("CalculateAverage");
		}
	}

	void Update () {
		if (cubemanScript != null) {
			score = cubemanScript.score;
		}
		if (scene.name != "KinectAvatarsDemo1" && sceneChanged == false) {
			StopCoroutine("CalculateAverage");
			score = scoreTemp/counter;
			AssignScore ();
		}
	}

	void AssignScore () {
		Text title = GameObject.Find("Title").GetComponent<Text>();
		Slider slider = GameObject.Find("Bar").GetComponent<Slider>();
		Text sliderPercentage = GameObject.Find("Percentage").GetComponent<Text>();
		if (score <= 60f) {
			title.text = "Try harder!";
		} else if (score > 60f && score <= 80f) {
			title.text = "Well done!";
		} else if (score > 80f) {
			title.text = "Excellent!";
		}
		sceneChanged = true;
		slider.value = score;
		sliderPercentage.text = Mathf.RoundToInt(score) + "%";
	}

	IEnumerator CalculateAverage() {
		scoreTemp = scoreTemp + score;
		counter++;
		yield return new WaitForSeconds(2);
		StartCoroutine("CalculateAverage");
	}
}
