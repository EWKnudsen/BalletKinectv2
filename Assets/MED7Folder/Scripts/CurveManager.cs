using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public CMCCombinedTorAndPelHP postureScriptHP;
	public CMCHandSymHP symmetryScript;
	public float score;
	private ParticleSystem ps;
	private Vector3 pos;
	private float ratio;
	private float amount;

	void Start () {
		postureScriptHP = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
		symmetryScript = GameObject.Find ("FilterController").GetComponent<CMCHandSymHP>();
		ps = transform.gameObject.GetComponent<ParticleSystem> ();
		pos = transform.position;
	}

	void Update () {
		CalculatePosition();
		CalculateColor ();
	}

	void CalculatePosition() {
		float yMin = 0.6f;
		if (postureScriptHP.enabled) {
			score = postureScriptHP.score;
		}  else if (symmetryScript.enabled) {
			score = symmetryScript.score;
		}
		ratio = score/100;
		amount = 0.20f*ratio;
		pos.y = yMin + pos.y*amount;
		transform.position = pos;
	}

	void CalculateColor() {
		ratio = score/100;
		ps.startColor = Color.Lerp (Color.red, Color.green, ratio);
	}
}
