//-----------------------------------------------------
//Controls Volume and 4 equalizers, by calculating the
//distance of the center hip, shoulder and neck
//in zx-plane.
//
//Sound if your posture is NOT vertical.
//Different sound for each axis.
//-----------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCTorsoNotchAtten : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    double distHipShoulder;
    double distShoulderNeck;
    double totalDist;
    double totalAtten;
    double totalAttenLogScaled;
    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;

    double NegaScaledAxisZ = 0, PosiScaledAxisZ = 0, NegaScaledAxisX = 0, PosiScaledAxisX = 0;

    //Change these values to calibrate the sound sensitivity. Be careful, REMEMBER THE OLD VALUES
    static double minDist = 0.01; //0.01;
    static double maxDist = 0.30; //0.25;

    static double minAxis = 0.02;
    static double maxAxis = 0.20;

    void Start ()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void Update ()
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
                distHipShoulder = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.hipCenterPos);
                distShoulderNeck = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.neckPos);

                totalDist = distHipShoulder + distShoulderNeck;

                totalAtten = ScalingBetween(totalDist, minAtten, maxAtten, minDist, maxDist);

                totalAttenLogScaled = LogScaling(totalAtten);
                
                if (totalAttenLogScaled > maxAtten)
                    totalAttenLogScaled = maxAtten;
                else if (totalAttenLogScaled < minAtten)
                    totalAttenLogScaled = minAtten;
                
                theMixer.SetFloat("Torso_Attenuation", (float)totalAttenLogScaled);
                //theMixer.SetFloat("Torso_Attenuation", (float)totalAtten);
                Debug.Log("lin: " + totalAtten + "      log: " + totalAttenLogScaled + "     hep: " + (float)ScalingBetween(totalAttenLogScaled, 0, 100, minAtten, maxAtten));

                score = 100 - (float)ScalingBetween(totalAtten, 0, 100, minAtten, maxAtten);
                if (score < 0)
                    score = 0;
                else if (score > 100)
                    score = 100;


                double AxisZ = CMCScript.hipCenterPos.z - CMCScript.shoulderCenterPos.z;
                double AxisX = CMCScript.hipCenterPos.x - CMCScript.shoulderCenterPos.x;

                if (AxisZ < 0) {
                    NegaScaledAxisZ = ScalingBetween(-AxisZ, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_00", (float) NegaScaledAxisZ);
                }
                else if (AxisZ >= 0) {
                    PosiScaledAxisZ = ScalingBetween(AxisZ, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_01", (float)PosiScaledAxisZ);
                }
                
                if (AxisX < 0) {
                    NegaScaledAxisX = ScalingBetween(-AxisX, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_02", (float)NegaScaledAxisX);
                }
                else if (AxisX >= 0) {
                    PosiScaledAxisX = ScalingBetween(AxisX, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_03", (float)PosiScaledAxisX);
                }
            }
        }
    }


    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow((double)vecB.x - vecA.x, 2) + Math.Pow((double)vecB.z - vecA.z, 2));
    }
    
    private double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        return (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;
    }

    private double LogScaling(double unscaledVal)
    {
        double hepson = (70 * Math.Log10((unscaledVal / 2) + 42)) - 80;
        return hepson + 15;
    }


}
