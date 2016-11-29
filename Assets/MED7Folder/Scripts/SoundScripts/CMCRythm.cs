using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCRythm : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    float minScore = 0, maxScore = 100;

    Vector3 last_HipPos, currentHipPos;
    double hipVelAxisY, hipVelAxisY_Scaled;


    double lasthep;
    double hepfirst, hepafstand;
    

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
    }


    void FixedUpdate()
    {
        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {

                currentHipPos = CMCScript.hipCenterPos;
                hipVelAxisY = (currentHipPos.y - last_HipPos.y);
                

                if (hipVelAxisY > 0)
                {
                    hipVelAxisY_Scaled = (float) ScalingBetween(hipVelAxisY, 0, 100, 0.0007, 0.04);

                    Debug.Log("score: " + score + "      vel: " + hipVelAxisY_Scaled + "     curr: " + currentHipPos + "       last: " + last_HipPos);

                    //To Malte: make code in here
                    //...
                    //...

                }
                //Debug.Log("vel: " + hip_Velocity + "        axisY: " + axisY + "     curr: " + currentHipPos + "       last: " + last_HipPos + "       lastX: " + CMCScript.wristLeftPos.x);



                last_HipPos = currentHipPos;

                /* test
                hepfirst = CMCScript.wristLeftPos.y;
                hepafstand = Math.Abs(hepfirst - lasthep);
                Debug.Log("vel: " + hepafstand);
                lasthep = hepfirst;
                */
            }
        }
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
}




