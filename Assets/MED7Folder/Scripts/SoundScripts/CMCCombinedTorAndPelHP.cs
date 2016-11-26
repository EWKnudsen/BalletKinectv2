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

    //range has been obtained by trail and error, you can change values to tune it. REMEMBER OLD VALUES
    static double torso_minDist = 0.006; //0f
    static double torso_maxDist = 0.11; //0.125f
    double torso_interval = (torso_maxDist - torso_minDist);

    //range has been obtained by trail and error, you can change values to tune it. REMEMBER OLD VALUES
    static double pelvis_minDist = 0.006; //0f
    static double pelvis_maxDist = 0.03;  //0.125f
    //double pelvis_interval = (pelvis_maxDist - pelvis_minDist);
    
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
                
                Combined_dist = (torso_dist + pelvis_dist) / 2;

                //trying to scale 20 : 9750 Hz,  but hard cus its log jo. 



                HPfilterVal = highPassFilterVal(Combined_dist, torso_maxDist, torso_interval, minFreq, maxFreq);
                theMixer.SetFloat("Torso_CutOffFreqHP", (float)HPfilterVal);
                

                torso_score = 100 - (float)ScalingBetween(torso_dist, 0, 100, torso_minDist, torso_maxDist);
                pelvis_score = 100 - (float)ScalingBetween(pelvis_dist, 0, 100, pelvis_minDist, pelvis_maxDist);
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
