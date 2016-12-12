using UnityEngine;
using System.Collections;

public class LineChanger : MonoBehaviour {

	public CMCCombinedTorAndPelHP CMCCombinedTorAndPelScriptHP;
    public CMCPostureHP CMCPostureScriptHP;

    public LineRenderer[] lines = new LineRenderer[25];

    public CubemanController cubeman;

	float colorChangeThreshold = 60;

    private bool foundPlayer = false;

	private float scoreTorso;
	private float scorePelvis;

	private Color redStart; 
	private Color redEnd; 
	private Color greenStart; 
	private Color greenEnd; 

	// Use this for initialization
	void Start () {
        cubeman = FindObjectOfType<CubemanController>();
		CMCCombinedTorAndPelScriptHP = GameObject.Find ("FilterController").GetComponent<CMCCombinedTorAndPelHP>();
        CMCPostureScriptHP = GameObject.Find("FilterController").GetComponent<CMCPostureHP>();
        greenStart = new Color(0f,0.26f,0.01f,1f);
		greenEnd = new Color(0f,1f,0.235f,1f);
		redStart = new Color(0.66f,0f,0.01f,1f);
		redEnd = new Color(1f,0f,0.0f,1f);
	}
	
	// Update is called once per frame
	void Update () {

		if (CMCCombinedTorAndPelScriptHP.enabled) {
			scoreTorso = CMCCombinedTorAndPelScriptHP.torso_score;
			scorePelvis = CMCCombinedTorAndPelScriptHP.pelvis_score;
		} else if (CMCPostureScriptHP.enabled)
        {
            scoreTorso = CMCPostureScriptHP.torso_score;
            scorePelvis = CMCPostureScriptHP.pelvis_score;
        }

        if (cubeman.hasValues == true && foundPlayer == false) {
            for (int i = 0; i < 25; i++) {
                lines[i] = GameObject.Find("Line" + i).GetComponent<LineRenderer>();
            }
            foundPlayer = true;
        }

        if (foundPlayer == true)
        {
			// TORSO: Line1 and Line20
			// HIPS: Line16 and Line12
			if (scoreTorso < colorChangeThreshold)
            {
				lines[1].SetColors(redStart, redEnd);
				lines[20].SetColors(redStart, redEnd);
            }
			if (scoreTorso >= colorChangeThreshold)
            {
				lines[1].SetColors(greenStart, greenEnd);
				lines[20].SetColors(greenStart, greenEnd);
            }
			if (scorePelvis < colorChangeThreshold)
			{
				lines[16].SetColors(redStart, redEnd);
				lines[12].SetColors(redStart, redEnd);
			}
			if (scorePelvis >= colorChangeThreshold)
			{
				lines[16].SetColors(greenStart, greenEnd);
				lines[12].SetColors(greenStart, greenEnd);
			}
        }
	}


    private double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        double val = (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;

        if (val < minNew)
            val = minNew;
        if (val > maxNew)
            val = maxNew;

        return val;
    }

}
