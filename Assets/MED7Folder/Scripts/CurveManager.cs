using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

    public CMCPostureHP postureHPScript;		// The posture script
	public float score;							// The score from the posture script
	private ParticleSystem ps;					// The particle system used to simulate the curve
	private Vector3 pos;						// Position of the partickle system
	private float ratio;						// Used to calculate the position
	private float amount;						// Used to calculate the position 

	void Start () {
        postureHPScript = GameObject.Find("FilterController").GetComponent<CMCPostureHP>();	// Find and initialises the posture script
		ps = transform.gameObject.GetComponent<ParticleSystem> ();		// Finds and initializes the particle system component
		pos = transform.position;										// Sets the starting position
	}

	void Update () {
		CalculatePosition();
		CalculateColor ();
	}

	// This function calculates the position of the gameobject. It does so by multiplying the y position of the transform with an amount determined by the score
	void CalculatePosition() {
		float yMin = 0.6f;
		if (postureHPScript.enabled) {
            score = postureHPScript.score;
        }
		ratio = score/100;
		amount = 0.20f*ratio;
		pos.y = yMin + pos.y*amount;
		transform.position = pos;
	}

	// This function uses a lerp function to determine the color of the particle that is 'fired'.
	void CalculateColor() {
		ratio = score/100;
		ps.startColor = Color.Lerp (Color.red, Color.green, ratio);
	}
}
