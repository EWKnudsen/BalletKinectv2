using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FocusManager : MonoBehaviour {

	bool sceneChanged = false;
	public GameObject prefab;
	public int focusIndex;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
	
	}

	void Update () {
		if (SceneManager.GetActiveScene().name == "MainScene" && !sceneChanged) {

			GameObject controller = GameObject.Find("KinectControllerCursor");

			if (controller != null) {
				Destroy(controller);
			}
				
			CMCCombinedTorAndPelHP postureScript = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
			CMCHandSymHP symmetryScript = GameObject.Find ("FilterController").GetComponent<CMCHandSymHP>();

			postureScript.enabled = false;
			symmetryScript.enabled = false;

			if (focusIndex == 0) {
				postureScript.enabled = true;	
			}
			if (focusIndex == 1) {
				symmetryScript.enabled = true;	
			}

			sceneChanged = true;
		}

		if (SceneManager.GetActiveScene().name == "GameOver" && sceneChanged) {

			prefab = (GameObject) Instantiate(Resources.Load ("KinectControllerCursor"));

			sceneChanged = false;
		}
	}

	public void ChooseFocus (int i) {
		focusIndex = i;
	}
}
