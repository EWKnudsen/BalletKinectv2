using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class ScoreManager : MonoBehaviour {
	
    public CMCPostureHP postureHPScript;		// The posture script
    public float score;							// The score
	private float scoreTemp;					// A temporary value used for incrementing the score and calculating average
	private int counter;						// Integer used for calculating 
	private Scene scene;						// Used for determining which scene is active
	private bool sceneChanged;					// Has the scene changed?


	void Awake() {
		DontDestroyOnLoad(transform.gameObject);	// Makes sure that this object is transferred between scenes
	}

	void Start () {
		sceneChanged = false;			
		scene = SceneManager.GetActiveScene();		// Sets scene to the active scene (MainScene)
        postureHPScript = GameObject.Find("FilterController").GetComponent<CMCPostureHP>(); // Finds posture script

        if (GameObject.Find("FilterController") != null) {
			StartCoroutine("CalculateAverage");		// Only in a scene with the gameobject "FilterController" calculate the score average
		}
	}

	void Update () {
        if (GameObject.Find("FilterController") != null) {
			if (postureHPScript.enabled) {
                score = postureHPScript.score;		// Sets the score to the score obtained from posture script
            }
		}
		// When scene has changed from either MainScene or MainSceneLong: start calculating the score average and assign it to the elements in the GameOver scene
		if (scene.name != "MainScene" && scene.name != "MainSceneLong" && sceneChanged == false) {
			StopCoroutine("CalculateAverage");
			score = scoreTemp/counter;
			AssignScore ();
		}
	}

	// Finds objects in the GameOver scene and sets a string value depending on the sore average, as well as a slider value
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
        Destroy(this.gameObject);
	}

	// Increments score and counter. Is looped until "StopCoroutine" is called.
	IEnumerator CalculateAverage() {
		scoreTemp = scoreTemp + score;
		counter++;
		yield return new WaitForSeconds(1);
		StartCoroutine("CalculateAverage");
	}
}
