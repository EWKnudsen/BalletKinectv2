using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCHandSymHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score, wristScore, elbowScore;
    float minScore = 0, maxScore = 100;
    double HPfilterVal;

    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;

    double minFreq = 20;
    double maxFreq = 22000;

    static double minDist = 0.02; 
    static double maxDist = 0.10; 
    double interval = (maxDist - minDist);

    double WL_Dist, WR_Dist, EL_Dist, ER_Dist;
    double W_DistVar, E_DistVar;
    double WL_axisZ, WR_axisZ, EL_axisZ, ER_axisZ;
    double W_axisZVar, E_axisZVar;
    double WL_axisY, WR_axisY, EL_axisY, ER_axisY;
    double W_axisYVar, E_axisYVar;

    double W_DistVar_HPfilterVal, E_DistVar_HPfilterVal, W_axisZVar_HPfilterVal;
    double E_axisZVar_HPfilterVal, W_axisYVar_HPfilterVal, E_axisYVar_HPfilterVal, Combined_HPfilterVal;

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void Update()
    {
        if (!isAttenReset)
        {
            theMixer.SetFloat("Torso_Attenuation", 0);
            isAttenReset = true;
        }

        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                WL_Dist = Vector3.Distance(CMCScript.shoulderCenterPos, CMCScript.wristLeftPos);
                WR_Dist = Vector3.Distance(CMCScript.shoulderCenterPos, CMCScript.wristRightPos);

                EL_Dist = Vector3.Distance(CMCScript.shoulderCenterPos, CMCScript.elbowLeftPos);
                ER_Dist = Vector3.Distance(CMCScript.shoulderCenterPos, CMCScript.elbowRightPos);

                W_DistVar = Math.Abs(WL_Dist - WR_Dist);
                E_DistVar = Math.Abs(EL_Dist - ER_Dist);


                WL_axisZ = CMCScript.wristLeftPos.x;
                WR_axisZ = CMCScript.wristRightPos.x;

                EL_axisZ = CMCScript.elbowLeftPos.x;
                ER_axisZ = CMCScript.elbowRightPos.x;

                W_axisZVar = Math.Abs(WL_axisZ - WR_axisZ);
                E_axisZVar = Math.Abs(EL_axisZ - ER_axisZ);


                WL_axisY = CMCScript.wristLeftPos.y;
                WR_axisY = CMCScript.wristRightPos.y;

                EL_axisY = CMCScript.elbowLeftPos.y;
                ER_axisY = CMCScript.elbowRightPos.y;

                W_axisYVar = Math.Abs(WL_axisY - WR_axisY);
                E_axisYVar = Math.Abs(EL_axisY - ER_axisY);

                //Debug.Log("W_DistVar: " + W_DistVar + "      E_DistVar: " + E_DistVar);
                //MIN: 0.02 MAX 0.08
                //Debug.Log("W_axisZVar: " + W_axisZVar + "      E_axisZVar: " + E_axisZVar);
                //Debug.Log("W_axisYVar: " + W_axisYVar + "      E_axisYVar: " + E_axisYVar);

                W_DistVar_HPfilterVal = highPassFilterVal(W_DistVar, maxDist, interval, minFreq, maxFreq);
                E_DistVar_HPfilterVal = highPassFilterVal(E_DistVar, maxDist, interval, minFreq, maxFreq);

                W_axisZVar_HPfilterVal = highPassFilterVal(W_axisZVar, maxDist, interval, minFreq, maxFreq);
                E_axisZVar_HPfilterVal = highPassFilterVal(E_axisZVar, maxDist, interval, minFreq, maxFreq);

                W_axisYVar_HPfilterVal = highPassFilterVal(W_axisYVar, maxDist, interval, minFreq, maxFreq);
                E_axisYVar_HPfilterVal = highPassFilterVal(E_axisYVar, maxDist, interval, minFreq, maxFreq);

                Combined_HPfilterVal = (W_DistVar_HPfilterVal + E_DistVar_HPfilterVal + W_axisZVar_HPfilterVal + W_axisYVar_HPfilterVal + E_axisYVar_HPfilterVal) / 6;
                
                Debug.Log("Combined_HPfilterVal: " + Combined_HPfilterVal);

                theMixer.SetFloat("Torso_CutOffFreqHP", (float)Combined_HPfilterVal);

                //wristScore = 100 - ScalingBetween(!!!!hep, 0, 100, !!!!!minOld, !!!!!maxOld);

                //elbowScore = 100 - ScalingBetween(!!!!hep, 0, 100, !!!!!minOld, !!!!!maxOld);

                //score = (wristScore + elbowScore) / 2;
            }
        }
    }



    private double highPassFilterVal(double val, double maxVal, double _interval, double _minFreq, double _maxFreq)
    {
        return _minFreq * Math.Pow((Math.Pow((_maxFreq / _minFreq), (1 / _interval))), (maxVal - val));
    }

    protected double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        double val = (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;

        if (val < minNew)
            val = minNew;
        else if (val > maxNew)
            val = maxNew;

        return val;
    }


}
