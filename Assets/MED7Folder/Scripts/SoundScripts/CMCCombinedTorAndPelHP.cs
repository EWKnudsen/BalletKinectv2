using System;
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

    static double T_minDist = 0.006; //0f
    static double T_maxDist = 0.11; //0.125f
    double interval = (T_maxDist - T_minDist);

    //missing: 
    static double P_minRot = 0;
    static double P_maxRot = 0;
    //static double pelvis_minDist = 0.006; //0f
    //static double pelvis_maxDist = 0.03;  //0.125f


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
                /*
                torso_dist = CalXZdist(CMCScript.hipCenterPos, CMCScript.shoulderCenterPos);
                pelvis_dist = Math.Abs(CMCScript.hipLeftPos.y - CMCScript.hipRightPos.y);
                double P_total = Math.Abs(CMCScript.hipCenterRot.z) + Math.Abs(CMCScript.hipCenterRot.x) + pelvis_dist;
                Combined_dist = (torso_dist + P_total) / 3;
                */

                T_distHipShoulder = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.hipCenterPos);
                T_distShoulderNeck = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.neckPos);
                T_totalDist = T_distHipShoulder + T_distShoulderNeck;
                T_totalDistScaled = ScalingBetween(T_totalDist, 0, 100, T_minDist, T_maxDist);

                P_totalRot = Math.Abs(CMCScript.hipCenterRot.x) + Math.Abs(CMCScript.hipCenterRot.y) + Math.Abs(CMCScript.hipCenterRot.z);
                P_totalRotScaled = ScalingBetween(P_totalRot, 0, 100, P_minRot, P_maxRot);

                C_totalComb = (T_totalDist + P_totalRot) / 2;
                C_HPfilterVal = highPassFilterVal(C_totalComb, (100*2), interval, minFreq, maxFreq);
                theMixer.SetFloat("Torso_CutOffFreqHP", (float)C_HPfilterVal);


                torso_score  = 100 - (float)T_totalDistScaled;
                pelvis_score = 100 - (float)P_totalRotScaled;
                score = 100 - (float)C_totalComb;
            }
        }
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2));
    }

    private double highPassFilterVal(double val, double _maxVal, double _interval, double _minFreq, double _maxFreq)
    {
        return _minFreq * Math.Pow((Math.Pow((_maxFreq / _minFreq), (1 / _interval))), (_maxVal - val));
        //max min remember
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
