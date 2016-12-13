using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayVideo : MonoBehaviour {

	public MovieTexture movie;		// The movie texture
	public float videoDuration;		// Float to store video duration
	public float videoStartTime;	// Used for delay
	public float delay = 0.8f;		// Delay used for syncing with music
	private int i;					// Integer used to freeze video at the second frame

	// Use this for initialization
	void Awake() {
		videoDuration = movie.duration+delay;
	}

	void Start () {
		movie.Play();					// Plays movie
		i = 0;			
		StartCoroutine("MovieEnded");	// Starts coroutine that changes scene once the movie has ended
		StartCoroutine("Delay");		// Starts delay used for syncing
	}
	
	// Update is called once per frame
	void Update () {
		// This is in order to freeze the first frame while delay is ongoing. If this would not run the upper left corner would be black during delay
		i++;
		if (i==2) {
			movie.Pause();
		}
	}

	// Used for syncing with audio
	private IEnumerator Delay() {
		yield return new WaitForSeconds(delay);
		movie.Play();
		videoStartTime = Time.timeSinceLevelLoad;
		Debug.Log (videoStartTime);
	}

	// Used to change scene once the movie has ended
	private IEnumerator MovieEnded() {
		yield return new WaitForSeconds(videoDuration);
		SceneManager.LoadScene ("GameOver");
	}
}
