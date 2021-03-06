﻿using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;


public class CMCCombinedTorAndPelHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float torso_score, pelvis_score, score;

    double T_distHipShoulder, T_distShoulderNeck, T_totalDist, T_totalDistScaled;
    double P_totalRot, P_totalRotScaled;
    double C_totalComb, C_HPfilterVal;

    double minFreq = 20;
    double maxFreq = 22000;

    static double T_minDist = 0.011; //0.0085f
    static double T_maxDist = 0.187; //0.125f
    double interval = (T_maxDist - T_minDist);

    static double P_minRot = 0.063;
    static double P_maxRot = 0.31;


    StreamWriter writer;
	public PlayVideo video;


    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
		video = GameObject.Find("Video").GetComponent<PlayVideo>();
		writer = new StreamWriter("Assets/MED7Folder/Txtfiles/MPCTest.txt", true);
		writer.WriteLine("New score data:");

        theMixer.SetFloat("Torso_Attenuation", 0);
    }


    void Update()
    {
		if (CMCScript != null) {
			if (CMCScript.hasValues) {

				T_distHipShoulder = CalXZdist (CMCScript.shoulderCenVec, CMCScript.hipCenVec);
				T_distShoulderNeck = CalXZdist (CMCScript.shoulderCenVec, CMCScript.neckVec);
				T_totalDist = T_distHipShoulder + T_distShoulderNeck;
				T_totalDistScaled = ScalingBetween (T_totalDist, 0, 100, T_minDist, T_maxDist);

				P_totalRot = Math.Abs (CMCScript.hipCenRot.x) + Math.Abs (CMCScript.hipCenRot.y) + Math.Abs (CMCScript.hipCenRot.z);
				P_totalRotScaled = ScalingBetween (P_totalRot, 0, 100, P_minRot, P_maxRot);

                //Debug.Log("T: " + T_totalDist + "TS: " + T_totalDistScaled + "R: " + P_totalRot + "RS: " + P_totalRotScaled);

				C_totalComb = (T_totalDistScaled + P_totalRotScaled) / 2;

                double highPassFilterVal = minFreq * Math.Pow((Math.Pow((22000 / 20), (1 / interval))), (T_maxDist - T_totalDist));

                theMixer.SetFloat ("Torso_CutOffFreqHP", (float)highPassFilterVal);


				torso_score = 100 - (float)T_totalDistScaled;
				pelvis_score = 100 - (float)P_totalRotScaled;
				score = 100 - (float)C_totalComb;


				if (writer.BaseStream != null)
					writer.WriteLine (T_totalDist + ", " + P_totalRot);
                
				if (Time.time > video.movie.duration - 0.5f)
					writer.Close();
			}
		} 
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2));
    }

    private double highPassFilterVal(double val, double _maxVal, double _interval, double _minFreq, double _maxFreq)
    {
        return _minFreq * Math.Pow((Math.Pow((_maxFreq / _minFreq), (1 / _interval))), (val)); //_maxVal - 
    }

    protected double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        double val = (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;

        if (val < minNew)
            val = minNew;
        if (val > maxNew)
            val = maxNew;

        return val;
    }
    
}
