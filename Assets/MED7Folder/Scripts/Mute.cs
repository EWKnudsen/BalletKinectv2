using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Mute : MonoBehaviour {

	public Toggle tgl;

	bool muted = false;
	bool sceneChanged = false;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
		tgl = GameObject.Find("MuteButton").GetComponent<Toggle>();
	}

	void Update () {

		if (tgl != null) {
			sceneChanged = false;
			muted = tgl.isOn;
		}


		if (SceneManager.GetActiveScene().name == "MainScene" || SceneManager.GetActiveScene().name == "MainSceneLong" && !sceneChanged) {
			MuteMusic();
		}
	}

	void MuteMusic() {
		AudioSource music = GameObject.Find("Music").GetComponent<AudioSource>();
		if (muted) {
			music.mute = true;
		} else {
			music.mute = false;
		}
		sceneChanged = true;
	}
}
