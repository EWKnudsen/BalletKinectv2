using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public CMCTorsoXyPlaneDistVolAndEq script;
	public float score;
	private Vector3 pos;
	private float ratio;
	private float amount;

	void Start () {
		script = GameObject.Find ("Cubeman").GetComponent<CMCTorsoXyPlaneDistVolAndEq> ();
		pos = transform.position;
	}

	void Update () {
		CalculatePosition();
	}

	void CalculatePosition() {
		float yMin = 0.55f;
		score = script.score;
		ratio = score/100;
		amount = 0.20f*ratio;
		pos.y = yMin + pos.y*amount;
		transform.position = pos;
	}
}
