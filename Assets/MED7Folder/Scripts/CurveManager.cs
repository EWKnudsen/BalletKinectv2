using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public CMCCombinedTorAndPelHP CMCCombinedTorAndPelHPScript;
    public CMCPostureHP postureHPScript;
    public CMCHandSymHP symmetryScript;
	public float score;
	private ParticleSystem ps;
	private Vector3 pos;
	private float ratio;
	private float amount;

	void Start () {
		CMCCombinedTorAndPelHPScript = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
        postureHPScript = GameObject.Find("FilterController").GetComponent<CMCPostureHP>();
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
		if (CMCCombinedTorAndPelHPScript.enabled) {
			score = CMCCombinedTorAndPelHPScript.score;
		}  else if (symmetryScript.enabled) {
			score = symmetryScript.score;
		}  else if (postureHPScript.enabled) {
            score = postureHPScript.score;
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
