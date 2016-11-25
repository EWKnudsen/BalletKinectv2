using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayVideo : MonoBehaviour {

	public MovieTexture movie;
	public float videoDuration;
	private bool moviePlayed;
	public float warmUpTime = 3.2f;
	private int i;

	// Use this for initialization
	void Awake() {
		videoDuration = movie.duration;
	}

	void Start () {
		movie.Play();
		i = 0;
		StartCoroutine("MovieEnded");
		StartCoroutine("WarmUpSession");
	}
	
	// Update is called once per frame
	void Update () {
		// This is in order to freeze the first frame while warm-up session is ongoing
		i++;
		if (i==2) {
			movie.Pause();
		}
	}

	void PlayMovie() {
		movie.Play();
	}

	private IEnumerator WarmUpSession() {
		yield return new WaitForSeconds(warmUpTime);
		movie.Play();
	}

	private IEnumerator MovieEnded() {
		yield return new WaitForSeconds(videoDuration+warmUpTime);
		SceneManager.LoadScene ("GameOver");
	}
}
