using System;
using UnityEngine;
using UnityEngine.Audio;


public class CMCCombinedTorAndPelHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float torso_score, pelvis_score, score;
    double torso_dist, pelvis_dist, Combined_dist;
    double HPfilterVal;

    double minFreq = 20;
    double maxFreq = 22000;

    static double torso_minDist = 0.006; //0f
    static double torso_maxDist = 0.11; //0.125f
    double torso_interval = (torso_maxDist - torso_minDist);


    static double pelvis_minDist = 0.006; //0f
    static double pelvis_maxDist = 0.03;  //0.125f

    
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
                torso_dist = CalXZdist(CMCScript.hipCenterPos, CMCScript.shoulderCenterPos);
                
                pelvis_dist = Math.Abs(CMCScript.hipLeftPos.y - CMCScript.hipRightPos.y);

                double P_total = Math.Abs(CMCScript.hipCenterRot.z) + Math.Abs(CMCScript.hipCenterRot.x) + pelvis_dist;

                Combined_dist = (torso_dist + P_total) / 3;
                
                HPfilterVal = highPassFilterVal(Combined_dist, torso_maxDist, torso_interval, minFreq, maxFreq);
                
                theMixer.SetFloat("Torso_CutOffFreqHP", (float)HPfilterVal);
                

                torso_score = 100 - (float)ScalingBetween(torso_dist, 0, 100, torso_minDist, torso_maxDist);
                pelvis_score = 100 - (float)ScalingBetween(P_total, 0, 100, pelvis_minDist, pelvis_maxDist);
                score = (torso_score + pelvis_score) / 2;
               
            }
        }
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2));
    }

    private double highPassFilterVal(double val, double maxVal, double _interval, double _minFreq, double _maxFreq)
    {
        return _minFreq * Math.Pow((Math.Pow((_maxFreq / _minFreq), (1 / _interval))), (maxVal - val));
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
