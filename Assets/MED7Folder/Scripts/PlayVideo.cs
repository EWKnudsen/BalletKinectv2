using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayVideo : MonoBehaviour {

	public MovieTexture movie;
	public float videoDuration;
	public float videoStartTime;
	private bool moviePlayed;
	public float delay = 0.8f;
	private int i;

	// Use this for initialization
	void Awake() {
		videoDuration = movie.duration+delay;
	}

	void Start () {
		movie.Play();
		i = 0;
		StartCoroutine("MovieEnded");
		StartCoroutine("WarmUpSession");
	}
	
	// Update is called once per frame
	void Update () {
		// This is in order to freeze the first frame while delay is ongoing
		i++;
		if (i==2) {
			movie.Pause();
		}
	}

	void PlayMovie() {
		movie.Play();
	}

	private IEnumerator WarmUpSession() {
		yield return new WaitForSeconds(delay);
		movie.Play();
		videoStartTime = Time.timeSinceLevelLoad;
		Debug.Log (videoStartTime);
	}

	private IEnumerator MovieEnded() {
		yield return new WaitForSeconds(videoDuration);
		SceneManager.LoadScene ("GameOver");
	}
}
