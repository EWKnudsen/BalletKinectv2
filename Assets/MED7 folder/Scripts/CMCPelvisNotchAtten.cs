using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCPelvisNotchAtten : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    double dist;
    double atten;
    double attenLogScaled;
    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;

    double AxisY;
    double NegaScaledAxisY = 0, PosiScaledAxisY = 0; // NegaScaledAxisX = 0, PosiScaledAxisX = 0;

    //Change these values to calibrate the sound sensitivity. Be careful, REMEMBER THE OLD VALUES
    static double minDist = 0.01;
    static double maxDist = 0.25;

    static double minAxis = 0.02;
    static double maxAxis = 0.20;

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void Update()
    {
        if (!isAttenReset)
        {
            theMixer.SetFloat("Pelvis_Attenuation", -50);
            isAttenReset = true;
        }

        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                AxisY = CMCScript.hipLeftPos.y - CMCScript.hipRightPos.y;
                dist = Math.Abs(AxisY);

                atten = ScalingBetween(dist, minAtten, maxAtten, minDist, maxDist);

                attenLogScaled = LogScaling(atten);

                if (attenLogScaled > maxAtten)
                    attenLogScaled = maxAtten;
                else if (attenLogScaled < minAtten)
                    attenLogScaled = minAtten;

                theMixer.SetFloat("Pelvis_Attenuation", (float)attenLogScaled);


                score = 100.0f - (float)ScalingBetween(attenLogScaled, 0, 100, minAtten, maxAtten);
                if (score < 0)
                    score = 0;
                else if (score > 100)
                    score = 100;


                if (AxisY < 0)
                {
                    NegaScaledAxisY = ScalingBetween(-AxisY, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Pelvis_EqFreqGain_00", (float)NegaScaledAxisY);
                }
                else if (AxisY >= 0)
                {
                    PosiScaledAxisY = ScalingBetween(AxisY, 0.3, 1, minAxis, maxAxis);
                    theMixer.SetFloat("Pelvis_EqFreqGain_01", (float)PosiScaledAxisY);
                }


                /* ROTATION of center hip pos maybe. will be added later
                if (AxisX < 0) {
                    NegaScaledAxisX = ScalingBetween(-AxisX, 0.3, 1, minAxis, maxAxis); //some rotation val
                    theMixer.SetFloat("Pelvis_EqFreqGain_02", (float)NegaScaledAxisX);
                }
                else if (AxisX >= 0) {
                    PosiScaledAxisX = ScalingBetween(AxisX, 0.3, 1, minAxis, maxAxis); //some rotation val oposite direction
                    theMixer.SetFloat("Pelvis_EqFreqGain_03", (float)PosiScaledAxisX);
                }
                */
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
        return hepson + 20;
    }


}
