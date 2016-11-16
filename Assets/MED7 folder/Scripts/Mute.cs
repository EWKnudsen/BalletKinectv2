using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Mute : MonoBehaviour {

	public Toggle tgl;
	private Scene scene;

	bool muted = false;
	bool sceneChanged = false;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
		scene = SceneManager.GetSceneByName("KinectAvatarsDemo1");
		tgl = GameObject.Find("MuteButton").GetComponent<Toggle>();
	}

	void Update () {

		if (tgl != null) {
			muted = tgl.isOn;
			Debug.Log(muted);
		}

		if (SceneManager.GetActiveScene() == scene) {
			Debug.Log("Changed scene");
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
