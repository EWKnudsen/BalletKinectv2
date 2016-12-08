using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class ScoreManager : MonoBehaviour {

	public CMCCombinedTorAndPelHP CMCCombinedTorAndPelHPScript;
	public CMCHandSymHP symmetryScript;
    public CMCPostureHP postureHPScript;
    public float score;
	public float scoreTemp;
	private int counter;
	private Scene scene;
	private bool sceneChanged;


	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
		//writer = new StreamWriter("Assets/MED7Folder/Txtfiles/StreamWriterTest.txt", true);
		//writer.WriteLine("New score data:");
		sceneChanged = false;
		scene = SceneManager.GetActiveScene();
		CMCCombinedTorAndPelHPScript = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
		symmetryScript = GameObject.Find ("FilterController").GetComponent<CMCHandSymHP>();
        postureHPScript = GameObject.Find("FilterController").GetComponent<CMCPostureHP>();

        if (GameObject.Find("FilterController") != null) {
			StartCoroutine("CalculateAverage");
		}
	}

	void Update () {
        if (GameObject.Find("FilterController") != null) {
			if (CMCCombinedTorAndPelHPScript.enabled) {
				score = CMCCombinedTorAndPelHPScript.score;
			}  else if (symmetryScript.enabled) {
				score = symmetryScript.score;
			}  else if (postureHPScript.enabled) {
                score = postureHPScript.score;
            }
		}
		if (scene.name != "MainScene" && scene.name != "MainSceneLong" && sceneChanged == false) {
			StopCoroutine("CalculateAverage");
			//writer.Close();
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
		//writer.WriteLine(score);
		counter++;
		yield return new WaitForSeconds(1);
		StartCoroutine("CalculateAverage");
	}
}
