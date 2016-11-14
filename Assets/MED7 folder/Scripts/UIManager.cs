using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text uiText;
	public CMCTorsoXyPlaneDistVolAndEq script;

	// Use this for initialization
	void Start () {
		uiText = FindObjectOfType<Text>();
		script = GameObject.Find ("Cubeman").GetComponent<CMCTorsoXyPlaneDistVolAndEq> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (uiText != null) {
			uiText.text = "SCORE: " + Mathf.RoundToInt(script.score);
		}
	}



}
