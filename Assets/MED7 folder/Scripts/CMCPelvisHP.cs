//-----------------------------------------------------
//Controls a low pass filter, by calculating the
//distance/disalignment of the left and right hip
//in the y-axis
//(Now also take rotation/angle in consideration) *not yet
//Sound if your hip is NOT horizontal
//-----------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCPelvisHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    double dist;

    //range has been obtained by trail and error, you can change values to tune it. REMEMBER OLD VALUES
    static double minDist = 0.04; //0f
    static double maxDist = 0.18; //0.125f
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
                dist = Math.Abs(CMCScript.hipLeftPos.y - CMCScript.hipRightPos.y);

                if (dist > maxDist)
                    dist = maxDist;

                double HPfilterVal = highPassFilterVal(dist, maxDist, interval, minFreq, maxFreq);
                theMixer.SetFloat("Pelvis_CutOffFreqHP", (float)HPfilterVal);

                score = 100 - (float)ScalingBetween(dist, 0, 100, minDist, maxDist);
                if (score < 0)
                    score = 0;
                else if (score > 100)
                    score = 100;
            }
        }
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
