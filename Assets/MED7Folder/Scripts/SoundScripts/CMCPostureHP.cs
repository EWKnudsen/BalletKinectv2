//Esben: current problem: it cant find the calibration script, but it can find the two others. ask malte

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;


public class CMCPostureHP : MonoBehaviour
{
    public CubemanController CMCScript;
    public Calibration CalibraScript;
	//public CalibrationTimer caliTimer;

    public AudioMixer theMixer;
    public PlayVideo video;
    StreamWriter writer;

    public float torso_score, pelvis_score, score;

    double T_distHipShoulder, T_distShoulderNeck, T_totalDist;
    double P_rotX, P_rotZ, P_totalRot;
    double C_totalComb, C_HPfilterVal;

    double minFreq = 20;
    double maxFreq = 22000;
    double HPfilterVal;

    static double T_minDist = 0.011; //0.0085f
    static double T_maxDist = 0.187; //0.125f

    static double P_minRot = 0.063;
    static double P_maxRot = 0.31;


    bool finishedCali;
    Vector3 calibraShoulderCenVec;
    Vector3 calibraHipVec;
    Vector3 calibraNeckVec;
    Quaternion calibraHipRot;

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();

        if (GameObject.Find("CalibrationData") != null)
        {
            CalibraScript = GameObject.Find("CalibrationData").GetComponent<Calibration>();

            finishedCali = CalibraScript.finishedCali;
            calibraShoulderCenVec = CalibraScript.Vec_calibraShoulderCen;
            calibraHipVec = CalibraScript.Vec_calibraHip;
            calibraNeckVec = CalibraScript.Vec_calibraNeck;
            calibraHipRot = CalibraScript.Qua_calibraHip;
        }
        else
        {
            Debug.Log("MED7 Warning: GameObject.Find(CalibrationData) is null");
            Debug.Log("MED7 Warning: meaining calibration values hasnt been obtained and are set to 0");
            finishedCali = true;
            calibraShoulderCenVec = Vector3.zero;
            calibraHipVec = Vector3.zero;
            calibraNeckVec = Vector3.zero;
            calibraHipRot = Quaternion.identity;
        }
        
		//caliTimer = GameObject.Find("UIManager").GetComponent<CalibrationTimer>();

        video = GameObject.Find("Video").GetComponent<PlayVideo>();
        writer = new StreamWriter("Assets/MED7Folder/Txtfiles/MPCTest.txt", true);
        writer.WriteLine("New score data:");

        theMixer.SetFloat("Torso_Attenuation", 0);
    }


    void Update()
    {
        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
				if (finishedCali) //CalibraScript.hasCalibrated
                {
                    T_distHipShoulder = CalXZdist(calibraShoulderCenVec, calibraHipVec, CMCScript.shoulderCenVec, CMCScript.hipCenVec);
                    T_distShoulderNeck = CalXZdist(calibraShoulderCenVec, calibraNeckVec, CMCScript.shoulderCenVec, CMCScript.neckVec);
                    T_totalDist = T_distHipShoulder + T_distShoulderNeck;

                    P_rotX = Math.Abs(CMCScript.hipCenRot.x - calibraHipRot.x);
                    P_rotZ = Math.Abs(CMCScript.hipCenRot.z - calibraHipRot.z);
                    P_totalRot = P_rotX + P_rotZ;

                    UpdateHPfilterValue(T_totalDist, P_totalRot);

                    UpdateScores(T_totalDist, T_minDist, T_maxDist, P_totalRot, P_minRot, P_maxRot);

                    StreamWriteValues(T_totalDist, P_totalRot); //was used for MPC

                    //Debug.Log("T: " + T_totalDist + "     R: " + P_totalRot);
                    
                }
                else
                {
                    Debug.Log("The calibration scene has not been run. Run the calibration scene before continuing");
                }
            }
        }
    }


    //Calculates the distance of two points A and B, and corrects the value to two reference points 
    private double CalXZdist(Vector3 refVecA, Vector3 refVecB, Vector3 vecA, Vector3 vecB)
    {
        double xAxisA, zAxisA, xAxisB, zAxisB;
        xAxisA = vecA.x - refVecA.x;
        zAxisA = vecA.z - refVecA.z;
        xAxisB = vecB.x - refVecB.x;
        zAxisB = vecB.z - refVecB.z;

        return Math.Sqrt(Math.Pow(xAxisB - xAxisA, 2) + Math.Pow(zAxisB - zAxisA, 2));
    }


    //Scales a limited linear value into a limited logarithmic value that fits a Highpass filter  
    private double HighPassScaling(double unscaledVal, double minOld, double maxOld, double minNew, double maxNew) 
    {
        if (unscaledVal <= minOld)
            unscaledVal = minOld;
        if (unscaledVal > maxOld)
            unscaledVal = maxOld;
        
        return minNew * Math.Pow((Math.Pow((maxNew / minNew), (1 / (maxOld - minOld)))), (maxOld - unscaledVal));
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


    //Updates the Highpass filter value. Picks either the pelvis or the torso depending on which is highest 
    void UpdateHPfilterValue(double Torso, double Pelvis)
    {
        double T_HPfilterVal = HighPassScaling(T_totalDist, T_minDist, T_maxDist, minFreq, maxFreq);
        double P_HPfilterVal = HighPassScaling(P_totalRot, P_minRot, P_maxRot, minFreq, maxFreq);
        

        double C_HPfilterVal = 0;

        if (T_HPfilterVal <= P_HPfilterVal)
            C_HPfilterVal = T_HPfilterVal;
        if (T_HPfilterVal > P_HPfilterVal)
            C_HPfilterVal = P_HPfilterVal;

        Debug.Log("T_HPfilterVal: " + T_HPfilterVal + "    P_HPfilterVal: " + P_HPfilterVal + "    C_HPfilterVal: " + C_HPfilterVal);

        theMixer.SetFloat("Torso_CutOffFreqHP", (float)C_HPfilterVal);
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


    //Stream writes the two values into a txt file 
    void StreamWriteValues(double firstVal, double secondVal)
    {
        if (writer.BaseStream != null)
            writer.WriteLine(firstVal + ", " + secondVal);

        if (Time.time > video.movie.duration - 0.5f)
            writer.Close();
    }

}
