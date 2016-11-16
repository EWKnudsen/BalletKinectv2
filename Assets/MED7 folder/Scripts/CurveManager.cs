using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public CMCPelvisHP pelvisHp;
	public CMCPelvisNotchAtten pelvisNotch;
	public CMCTorsoHP torsoHp;
	public CMCTorsoNotchAtten torsoNotch; 
	public float score;
	private ParticleSystem ps;
	private Vector3 pos;
	private float ratio;
	private float amount;

	void Start () {
		pelvisHp = GameObject.Find ("FilterController").GetComponent<CMCPelvisHP>();
		pelvisNotch = GameObject.Find ("FilterController").GetComponent<CMCPelvisNotchAtten>();
		torsoHp = GameObject.Find ("FilterController").GetComponent<CMCTorsoHP>();
		torsoNotch = GameObject.Find ("FilterController").GetComponent<CMCTorsoNotchAtten>(); 
		ps = transform.gameObject.GetComponent<ParticleSystem> ();
		pos = transform.position;
	}

	void Update () {
		CalculatePosition();
		CalculateColor ();
	}

	void CalculatePosition() {
		float yMin = 0.55f;
		if (pelvisHp.enabled) {
			score = pelvisHp.score;
		} else if (pelvisNotch.enabled) {
			score = pelvisNotch.score;
		} else if (torsoHp.enabled) {
			score = torsoHp.score;
		} else if (torsoNotch.enabled) {
			score = torsoNotch.score;
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
