using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Text uiText;
	public float score;

	// Use this for initialization
	void Start () {
		uiText = FindObjectOfType<Text>();
		score = 100f;
	}
	
	// Update is called once per frame
	void Update () {
		uiText.text = "SCORE: " + score;
	}



}
