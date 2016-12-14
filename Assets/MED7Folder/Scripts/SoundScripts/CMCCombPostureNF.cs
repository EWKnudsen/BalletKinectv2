using System;
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
    double P_totalRot, P_totalAtten; //P_totalAttenLogScaled
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
                T_totalAtten = LinearScaling(T_totalDist, minAtten, maxAtten, T_minDist, T_maxDist);
                T_totalAttenLogScaled = LogScaling(T_totalAtten);
                

                P_totalRot = Math.Abs(CMCScript.hipCenRot.z) + Math.Abs(CMCScript.hipCenRot.x);
                P_totalAtten = LinearScaling(P_totalRot, minAtten, maxAtten, P_minRot, P_maxRot);
                //P_totalAttenLogScaled = LogScaling(P_totalAtten);

                C_totalAttenLogScaled = (T_totalAttenLogScaled + P_totalAtten) / 2; //!
                theMixer.SetFloat("Torso_Attenuation", (float)C_totalAttenLogScaled);

                
                T_axisZ = CMCScript.hipCenVec.z - CMCScript.shoulderCenVec.z;
                T_axisX = CMCScript.hipCenVec.x - CMCScript.shoulderCenVec.x;
                P_axisZ = CMCScript.hipCenRot.z;
                P_axisX = CMCScript.hipCenRot.x;

                C_axisZ = T_axisZ + P_axisZ;
                C_axisX = T_axisX + P_axisX;

                SettingEqFilterVals(C_axisZ, (P_minRot + T_minAxis), (P_maxRot + T_maxAxis), "Torso_EqFreqGain_00", "Torso_EqFreqGain_01");
                SettingEqFilterVals(C_axisX, (P_minRot + T_minAxis), (P_maxRot + T_maxAxis), "Torso_EqFreqGain_02", "Torso_EqFreqGain_03");

                UpdateScores(T_totalDist, T_minDist, T_maxDist, P_totalRot, P_minRot, P_maxRot);
            }
        }
    }


    //Calculates the distance for the Torso
    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow((double)vecB.x - vecA.x, 2) + Math.Pow((double)vecB.z - vecA.z, 2));
    }


    //Scales a limited value into another limited range
    protected double LinearScaling(double unscaledVal, double minOld, double maxOld, double minNew, double maxNew)
    {
        if (unscaledVal <= minOld)
            unscaledVal = minOld;
        if (unscaledVal > maxOld)
            unscaledVal = maxOld;

        return (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;
    }


    //Sets and scales the value for two notchfilters, for a single axis in positive and negative direction/rotation.
    void SettingEqFilterVals(double axisVal, double minAxisVal, double maxAxisVal, string filterName1, string filterName2)
    {
        if (axisVal < 0)
        {
            double negaScaledAxisVal = LinearScaling(-axisVal, 0.3, 1, minAxisVal, maxAxisVal);
            theMixer.SetFloat(filterName1, (float)negaScaledAxisVal);
        }
        else if (axisVal >= 0)
        {
            double posiScaledAxisVal = LinearScaling(axisVal, 0.3, 1, minAxisVal, maxAxisVal);
            theMixer.SetFloat(filterName2, (float)posiScaledAxisVal);
        }
    }


    //Scales a value into a specific logarithmic value.
    //The Log Equtation has been set to fit the sensitivity of percieveing a sounds volume gradually going up and down. 
    private double LogScaling(double unscaledVal)
    {
        double LogScale = (70 * Math.Log10((unscaledVal / 2) + 42)) - 80;
        return LogScale; //+ 15
    }


    //Updates each score
    void UpdateScores(double T_val, double T_min, double T_max, double P_val, double P_min, double P_max)
    {
        torso_score = 100 - (float)LinearScaling(T_val, T_min, T_max, 0, 100);
        pelvis_score = 100 - (float)LinearScaling(P_val, P_min, P_max, 0, 100);

        if (torso_score <= pelvis_score)
            score = torso_score;
        if (torso_score > pelvis_score)
            score = pelvis_score;
    }

}
