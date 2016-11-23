using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCCombPostureNF : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score; 
    float minScore = 0, maxScore = 100;

    double T_distHipShoulder, T_distShoulderNeck;
    double T_totalDist, T_totalAtten, T_totalAttenLogScaled;

    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;

    double T_axisZ, T_axisX;
    double T_NegaScaledAxisZ, T_PosiScaledAxisZ, T_NegaScaledAxisX, T_PosiScaledAxisX;

    //Change these values to calibrate the sound sensitivity. Be careful, REMEMBER THE OLD VALUES
    static double T_minDist = 0.006;
    static double T_maxDist = 0.11; 

    static double T_minAxis = 0.02;
    static double T_maxAxis = 0.20;
    //------------------------------------------------------------------------
    static double P_minDist = 0.006;
    static double P_maxDist = 0.03;

    double P_axisZ;

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
                T_distHipShoulder = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.hipCenterPos);
                T_distShoulderNeck = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.neckPos);

                T_totalDist = T_distHipShoulder + T_distShoulderNeck;
                T_totalAtten = ScalingBetween(T_totalDist, minAtten, maxAtten, T_minDist, T_maxDist);
                T_totalAttenLogScaled = LogScaling(T_totalAtten);


                 T_axisZ = CMCScript.hipCenterPos.z - CMCScript.shoulderCenterPos.z;
                 T_axisX = CMCScript.hipCenterPos.x - CMCScript.shoulderCenterPos.x;

                if (T_axisZ < 0)
                {
                    T_NegaScaledAxisZ = ScalingBetween(-T_axisZ, 0.3, 1, T_minAxis, T_maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_00", (float)T_NegaScaledAxisZ);
                }
                else if (T_axisZ >= 0)
                {
                    T_PosiScaledAxisZ = ScalingBetween(T_axisZ, 0.3, 1, T_minAxis, T_maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_01", (float)T_PosiScaledAxisZ);
                }

                if (T_axisX < 0)
                {
                    T_NegaScaledAxisX = ScalingBetween(-T_axisX, 0.3, 1, T_minAxis, T_maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_02", (float)T_NegaScaledAxisX);
                }
                else if (T_axisX >= 0)
                {
                    T_PosiScaledAxisX = ScalingBetween(T_axisX, 0.3, 1, T_minAxis, T_maxAxis);
                    theMixer.SetFloat("Torso_EqFreqGain_03", (float)T_PosiScaledAxisX);
                }

                //--------------------------------------------------------------


                //P_axisY




                //---------------------------------------------------------------

                //same log?
                //P_totalAttenLogScaled = Math.Abs(CMCScript.hipLeftPos.y - CMCScript.hipRightPos.y);
                //C_totalAtten = T_totalAttenLogScaled + P_rotation + P_PosiScaledAxisY + P_NegaScaledAxisY

                //theMixer.SetFloat("Torso_Attenuation", (float)CombinedtotalAtten);

                score = maxScore - (float)ScalingBetween(T_totalAtten, minScore, maxScore, minAtten, maxAtten);
            }
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
        return hepson + 15;
    }


}
