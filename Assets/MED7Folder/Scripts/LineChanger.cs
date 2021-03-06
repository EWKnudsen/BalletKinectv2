﻿using UnityEngine;
using System.Collections;

public class LineChanger : MonoBehaviour {

	public CMCPostureHP CMCPostureScriptHP;				// The combined HP script for posture and pelvis

    public LineRenderer[] lines = new LineRenderer[25];	// A LineRenderer array for the lines generated by CubeMan

    public CubemanController cubeman;					// The CubemanController

	private float colorChangeThreshold = 60;			// Threshold for changing colors	
    private bool foundPlayer = false;					// Is player found?
	private float scoreTorso;							// Torso score
	private float scorePelvis;							// Pelvis score

	private Color redStart; 							// Starting color for red gradient
	private Color redEnd; 								// End color for red gradient
	private Color greenStart; 							// Starting color for green gradient
	private Color greenEnd; 							// End color for green gradient

	// Use this for initialization
	void Start () {
        cubeman = FindObjectOfType<CubemanController>();	// Finds and initializes CubemanController
        CMCPostureScriptHP = GameObject.Find("FilterController").GetComponent<CMCPostureHP>();	// Finds and initializes posture script
        greenStart = new Color(0f,0.26f,0.01f,1f);			// Sets colors for red
		greenEnd = new Color(0f,1f,0.235f,1f);				// Sets colors for red
		redStart = new Color(0.66f,0f,0.01f,1f);			// Sets colors for green
		redEnd = new Color(1f,0f,0.0f,1f);					// Sets colors for green
	}
	
	// Update is called once per frame
	void Update () {
		
		if (CMCPostureScriptHP.enabled)					// Is posture script enabled? Then set the score
        {
            scoreTorso = CMCPostureScriptHP.torso_score;
            scorePelvis = CMCPostureScriptHP.pelvis_score;
        }

        if (cubeman.hasValues == true && foundPlayer == false) {	// When a player is found, fill line array with the lines from CubemanController
            for (int i = 0; i < 25; i++) {
                lines[i] = GameObject.Find("Line" + i).GetComponent<LineRenderer>();
            }
            foundPlayer = true;
        }

        if (foundPlayer == true)
        {
			// TORSO: Line1 and Line20
			// HIPS: Line16 and Line12
			if (scoreTorso < colorChangeThreshold)		// Set lines in torso red if below threshold
            {
				lines[1].SetColors(redStart, redEnd);
				lines[20].SetColors(redStart, redEnd);
            }
			if (scoreTorso >= colorChangeThreshold)		// Set lines in torso green if above threshold
            {
				lines[1].SetColors(greenStart, greenEnd);
				lines[20].SetColors(greenStart, greenEnd);
            }
			if (scorePelvis < colorChangeThreshold)		// Set lines in pelvis red if below threshold
			{
				lines[16].SetColors(redStart, redEnd);
				lines[12].SetColors(redStart, redEnd);
			}
			if (scorePelvis >= colorChangeThreshold)	// Set lines in pelvis green if above threshold
			{
				lines[16].SetColors(greenStart, greenEnd);
				lines[12].SetColors(greenStart, greenEnd);
			}
        }
	}
}
