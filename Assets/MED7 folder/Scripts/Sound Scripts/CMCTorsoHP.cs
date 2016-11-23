//-----------------------------------------------------
//Controls a high pass filter, by calculating the
//distance of the center hip and center shoulder
//in zx-plane  
//
//Sound if your spine is NOT vertical
//-----------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCTorsoHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    double dist;
    
    //range has been obtained by trail and error, you can change values to tune it. REMEMBER OLD VALUES
    static double minDist = 0.006; //0f
    static double maxDist = 0.11; //0.125f
    double interval = (maxDist - minDist);
    double minFreq = 20;
    double maxFreq = 22000;

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void Update()
    {
        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                dist = CalXZdist(CMCScript.hipCenterPos, CMCScript.shoulderCenterPos);
                
                if (dist > maxDist)
                    dist = maxDist;

                double HPfilterVal = highPassFilterVal(dist, maxDist, interval, minFreq, maxFreq);
                theMixer.SetFloat("Torso_CutOffFreqHP", (float) HPfilterVal);
                
                score = 100 - (float) ScalingBetween(dist, 0, 100, minDist, maxDist);
                if (score < 0)
                    score = 0;
                else if (score > 100)
                    score = 100;
            }
        }
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2));
    }

    private double highPassFilterVal(double val, double maxVal, double _interval, double _minFreq, double _maxFreq)
    {
        return minFreq * Math.Pow((Math.Pow((_maxFreq / _minFreq), (1 / _interval))), (maxVal - val));
    }

    protected double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        return (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;
    }

}
