using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FocusManager : MonoBehaviour {

	bool sceneChanged = false;
	public int focusIndex;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
	
	}

	void Update () {
		if (SceneManager.GetActiveScene().name == "KinectAvatarsDemo1" && !sceneChanged) {
			
			//CMCPelvisHP pelvisHp = GameObject.Find ("FilterController").GetComponent<CMCPelvisHP>();
			//CMCPelvisNotchAtten pelvisNotch = GameObject.Find ("FilterController").GetComponent<CMCPelvisNotchAtten>();
			//CMCTorsoHP torsoHp = GameObject.Find ("FilterController").GetComponent<CMCTorsoHP>();
			//CMCTorsoNotchAtten torsoNotch = GameObject.Find ("FilterController").GetComponent<CMCTorsoNotchAtten>(); 

			//pelvisHp.enabled = false;
			//pelvisNotch.enabled = false;
			//torsoHp.enabled = false;
			//torsoNotch.enabled = false;

			/*Component[] scripts = GameObject.Find ("FilterController").GetComponents(typeof(Component));

			for (int i = 2; i < scripts.Length; i++) {

				scripts [i].enabled = false;

				if (i = focusIndex) {
					scripts [i].enabled = true;
				}
			}*/

		}
	}

	public void ChooseFocus (int i) {
		focusIndex = i;
	}
}
