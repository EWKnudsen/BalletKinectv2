using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text uiText;
	public float score;
	public CMCTorsoXyPlaneDistVolAndEq script;

	// Use this for initialization
	void Start () {
		uiText = FindObjectOfType<Text>();
		script = GameObject.Find ("Cubeman").GetComponent<CMCTorsoXyPlaneDistVolAndEq> ();
	}
	
	// Update is called once per frame
	void Update () {
		uiText.text = "SCORE: " + script.score;
	}



}
