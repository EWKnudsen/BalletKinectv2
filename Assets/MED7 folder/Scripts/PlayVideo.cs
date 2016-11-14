using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayVideo : MonoBehaviour {

	public MovieTexture movie;

	// Use this for initialization
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
