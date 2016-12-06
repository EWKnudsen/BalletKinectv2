using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;


public class CMCPostureHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public Calibration CalibraScript;
    public PlayVideo video;
    StreamWriter writer;
    public float torso_score, pelvis_score, score;

    double T_distHipShoulder, T_distShoulderNeck, T_totalDist;
    double P_rotX, P_rotZ, P_totalRot;
    double C_totalComb, C_HPfilterVal;

    double T_axisZ, T_axisX, P_axisZ, P_axisX, C_axisZ, C_axisX;

    double minFreq = 20;
    double maxFreq = 22000;
    double HPfilterVal;

    static double T_minDist = 0.011; //0.0085f
    static double T_maxDist = 0.187; //0.125f

    static double P_minRot = 0.063;
    static double P_maxRot = 0.31;


    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
        CalibraScript = GameObject.Find("CalibrationData").GetComponent<Calibration>();

        video = GameObject.Find("Video").GetComponent<PlayVideo>();
        writer = new StreamWriter("Assets/MED7Folder/Txtfiles/MPCTest.txt", true);
        writer.WriteLine("New score data:");

        theMixer.SetFloat("Torso_Attentuation", 0);
    }


    void Update()
    {
        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                
                T_distHipShoulder  = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.hipCenterPos);
                T_distShoulderNeck = CalXZdist(CMCScript.shoulderCenterPos, CMCScript.neckPos);
                T_totalDist = T_distHipShoulder + T_distShoulderNeck;

                P_rotX = Math.Abs(CMCScript.hipCenterRot.x);
                P_rotZ = Math.Abs(CMCScript.hipCenterRot.z);
                P_totalRot = P_rotX + P_rotZ; //y is side to side

                SettingHPfilterValue(T_totalDist, P_totalRot);

                UpdateScores(T_totalDist, T_minDist, T_maxDist, P_totalRot, P_minRot, P_maxRot);
                
                StreamWriteValues(T_totalDist, P_totalRot); //was used for MPC
                    
                Debug.Log("T: " + T_totalDist + "     R: " + P_totalRot);
            }
        }
    }

    private double CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return Math.Sqrt(Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2));
    }

    private double HighPassScaling(double unscaledVal, double minOld, double maxOld, double minNew, double maxNew) 
    {
        if (unscaledVal < minOld)
            unscaledVal = minOld;
        if (unscaledVal > maxOld)
            unscaledVal = maxOld;
        
        return minNew * Math.Pow((Math.Pow((maxNew / minNew), (1 / (maxOld - minOld)))), (maxOld - unscaledVal));
    }

    protected double LinearScaling(double unscaledVal, double minOld, double maxOld, double minNew, double maxNew)
    {
        if (unscaledVal < minOld)
            unscaledVal = minOld;
        if (unscaledVal > maxOld)
            unscaledVal = maxOld;
        
        return (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;
    }

    void SettingHPfilterValue(double Torso, double Pelvis)
    {
        double T_HPfilterVal = HighPassScaling(T_totalDist, T_minDist, T_maxDist, minFreq, maxFreq);
        double P_HPfilterVal = HighPassScaling(P_totalRot, P_minRot, P_maxRot, minFreq, maxFreq);

        double C_HPfilterVal = 0;

        if (T_HPfilterVal < P_HPfilterVal)
            C_HPfilterVal = P_HPfilterVal;
        if (T_HPfilterVal > P_HPfilterVal)
            C_HPfilterVal = T_HPfilterVal;
        
        theMixer.SetFloat("Torso_CutOffFreqHP", (float)C_HPfilterVal);
    }

    void UpdateScores(double T_val, double T_min, double T_max, double P_val, double P_min, double P_max)
    {
        torso_score = 100 - (float)LinearScaling(T_val, 0, 100, T_min, T_max);
        pelvis_score = 100 - (float)LinearScaling(P_val, 0, 100, P_min, P_max);

        if (torso_score < pelvis_score)
            score = pelvis_score;
        if (torso_score > pelvis_score)
            score = torso_score;
    }

    void StreamWriteValues(double firstVal, double secondVal)
    {
        if (writer.BaseStream != null)
            writer.WriteLine(firstVal + ", " + secondVal);

        if (Time.time > video.movie.duration - 0.5f)
            writer.Close();
    }

}
