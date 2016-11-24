using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayVideo : MonoBehaviour {

	public MovieTexture movie;
	public float videoDuration;

	// Use this for initialization
	void Awake() {
		videoDuration = movie.duration;
	}

	void Start () {
		movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (!movie.isPlaying) {
			SceneManager.LoadScene ("GameOver");
		}
	}
}
