using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text uiText;
	public MovieTexture movie;
	public CMCTorsoXyPlaneDistVolAndEq script;

	// Use this for initialization
	void Start () {
		uiText = FindObjectOfType<Text>();
		script = GameObject.Find ("Cubeman").GetComponent<CMCTorsoXyPlaneDistVolAndEq> ();
		movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
		uiText.text = "SCORE: " + script.score;
		if (!movie.isPlaying) {
			SceneManager.LoadScene ("GameOver");
		}
	}



}
