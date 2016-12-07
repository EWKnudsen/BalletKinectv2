﻿using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCCombPostureNF : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score, torso_score, pelvis_score; 
    float minScore = 0, maxScore = 100;

    double T_distHipShoulder, T_distShoulderNeck;
    double T_totalDist, T_totalAtten, T_totalAttenLogScaled;
    double P_totalRot, P_totalAtten, P_totalAttenLogScaled;
    double C_totalAttenLogScaled;

    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;

    double T_axisZ, T_axisX, P_axisZ, P_axisX, C_axisZ, C_axisX;

    //Change these values to calibrate the sound sensitivity. Be careful, REMEMBER THE OLD VALUES
    static double T_minDist = 0.006;
    static double T_maxDist = 0.11; 

    static double T_minAxis = 0.02;
    static double T_maxAxis = 0.20;


    static double P_minRot = 0.004;
    static double P_maxRot = 0.10;




    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void Update()
    {
        if (!isAttenReset)
        {
            theMixer.SetFloat("Torso_Attenuation", -50);
            isAttenReset = true;
        }

        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                T_distHipShoulder = CalXZdist(CMCScript.shoulderCenVec, CMCScript.hipCenVec);
                T_distShoulderNeck = CalXZdist(CMCScript.shoulderCenVec, CMCScript.neckVec);
                T_totalDist = T_distHipShoulder + T_distShoulderNeck;
                T_totalAtten = ScalingBetween(T_totalDist, minAtten, maxAtten, T_minDist, T_maxDist);
                T_totalAttenLogScaled = LogScaling(T_totalAtten);
                

                P_totalRot = Math.Abs(CMCScript.hipCenterRot.z) + Math.Abs(CMCScript.hipCenterRot.x) + Math.Abs(CMCScript.hipCenterRot.x);
                P_totalAtten = ScalingBetween(P_totalRot, minAtten, maxAtten, P_minRot, P_maxRot);
                P_totalAttenLogScaled = LogScaling(P_totalAtten);
                //P_totalAttenLogScaled = LogScaling(P_totalAtten);

                C_totalAttenLogScaled = (T_totalAttenLogScaled + P_totalAtten) / 2; //!
                theMixer.SetFloat("Torso_Attenuation", (float)C_totalAttenLogScaled);

                
                T_axisZ = CMCScript.hipCenVec.z - CMCScript.shoulderCenVec.z;
                T_axisX = CMCScript.hipCenVec.x - CMCScript.shoulderCenVec.x;
                P_axisZ = CMCScript.hipCenterRot.z;
                P_axisX = CMCScript.hipCenterRot.x;

                C_axisZ = T_axisZ + P_axisZ;
                C_axisX = T_axisX + P_axisX;

                SettingEqFilterVals(C_axisZ, (P_minRot + T_minAxis), (P_maxRot + T_maxAxis), "Torso_EqFreqGain_00", "Torso_EqFreqGain_01");
                SettingEqFilterVals(C_axisX, (P_minRot + T_minAxis), (P_maxRot + T_maxAxis), "Torso_EqFreqGain_02", "Torso_EqFreqGain_03");

                
                //Debug.Log("T: " + T_totalAttenLogScaled + "      P: " + P_totalAtten + "        CLog: " + C_totalAttenLogScaled);

                //Debug.Log("T: " + T_totalAttenLogScaled + "      P: " + P_totalAttenLogScaled + "        C: " + C_totalAttenLogScaled);

                //Debug.Log("w: " + CMCScript.hipCenterRot.w + "     x: " + CMCScript.hipCenterRot.x + "     y: " + CMCScript.hipCenterRot.y + "     z: " + CMCScript.hipCenterRot.z);

                torso_score = 100 - (float)ScalingBetween(T_totalDist, 0, 100, T_minDist, T_maxDist);
                pelvis_score = 100 - (float)ScalingBetween(P_totalRot, 0, 100, P_minRot, P_maxRot);

                score = maxScore - (float)ScalingBetween((T_totalAtten + P_totalAtten), minScore, maxScore, minAtten, maxAtten);
            }
        }
    }

    void SettingEqFilterVals(double axisVal, double minAxisVal, double maxAxisVal, string filterName1, string filterName2)
    {
        if (axisVal < 0)
        {
            double negaScaledAxisVal = ScalingBetween(-axisVal, 0.3, 1, minAxisVal, maxAxisVal);
            theMixer.SetFloat(filterName1, (float)negaScaledAxisVal);
        }
        else if (axisVal >= 0)
        {
            double posiScaledAxisVal = ScalingBetween(axisVal, 0.3, 1, minAxisVal, maxAxisVal);
            theMixer.SetFloat(filterName2, (float)posiScaledAxisVal);
        }
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow((double)vecB.x - vecA.x, 2) + Math.Pow((double)vecB.z - vecA.z, 2));
    }

    private double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        double val = (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;

        if (val < minNew)
            val = minNew;
        else if (val > maxNew)
            val = maxNew;

        return val;
    }

    private double LogScaling(double unscaledVal)
    {
        double hepson = (70 * Math.Log10((unscaledVal / 2) + 42)) - 80;
        return hepson; //+ 15
    }


}
