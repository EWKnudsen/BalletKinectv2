using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public CMCCombinedTorAndPelHP postureScriptHP;
	public CMCCombPostureNF postureScriptNF;
	public CMCHandSymHP symmetryScript;
	public float score;
	private ParticleSystem ps;
	private Vector3 pos;
	private float ratio;
	private float amount;

	void Start () {
		postureScriptHP = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
		postureScriptNF = GameObject.Find ("FilterController").GetComponent<CMCCombPostureNF>();
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
		} else if (postureScriptNF.enabled) {
			score = postureScriptNF.score;
		} else if (symmetryScript.enabled) {
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
