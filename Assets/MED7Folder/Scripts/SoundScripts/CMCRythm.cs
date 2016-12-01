using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class CMCRythm : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
	public Metronome metronomeScript;
    public float score;
    float minScore = 0, maxScore = 100;

    Vector3 last_HipPos, currentHipPos;
    double hipVelAxisY, hipVelAxisY_Scaled;

    int counter = 0;
    float timescaled;
    //float[] tickTimes = new float[6] { 1380, 1770, 2310, 2960, 3316, 3850 }; //maltes values
    float[] tickTimes = new float[6] { 1400, 1818, 2370, 3030, 3380, 3890 };
    float timeInterval = 30;
    double bendInterval = 0.0007 + 0.0013;
    float tickDuration = 4f;

    public float LerpingGainMetroVal;

    bool outOfsync = false;
    float timeStramp;


    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
		metronomeScript =  GameObject.Find("Metronome").GetComponent<Metronome>();
    }


    void FixedUpdate()
    {
        Debug.Log("Lerp: " + LerpingGainMetroVal + "    timeStramp: " + timeStramp + "    Time.time: " + Time.time);
        hipVelAxisY_Scaled = bendInterval * 2; //for testing
        
        if (outOfsync)
        {
            LerpingGainMetroVal = Mathf.Lerp(0.6f, 0f, (Time.time - timeStramp)/4);
            metronomeScript.gain = LerpingGainMetroVal;

            if (tickDuration < (Time.time - timeStramp)) //tickDuration must be smaller than our lerp
                outOfsync = false;
        }
        

        if (CMCScript != null)
        {
            if (CMCScript.hasValues)
            {
                currentHipPos = CMCScript.hipCenterPos;
                hipVelAxisY = (currentHipPos.y - last_HipPos.y);
                
                if (hipVelAxisY > 0 || hipVelAxisY < 0)
                {
                    if (hipVelAxisY > 0)
                        hipVelAxisY_Scaled = (float)ScalingBetween(hipVelAxisY, 0, 100, 0.0007, 0.04);
                    else if (hipVelAxisY < 0)
                        hipVelAxisY_Scaled = -(float)ScalingBetween(-hipVelAxisY, 0, 100, 0.0007, 0.04);

                    if (counter < tickTimes.Length && Math.Abs(Time.time * 100) > (tickTimes[counter] - timeInterval) && Math.Abs(Time.time * 100) < (tickTimes[counter] + timeInterval))
                    {
                        if (hipVelAxisY_Scaled > bendInterval || hipVelAxisY_Scaled < -bendInterval)
                        {
                            outOfsync = true;
                            timeStramp = Time.time;
                        }
                        counter++;
                    }
                }
                last_HipPos = currentHipPos;
            }
        }
    }


    private double ScalingBetween(double unscaledVal, double minNew, double maxNew, double minOld, double maxOld)
    {
        double val = (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;

        if (val < minNew)
            val = minNew;
        if (val > maxNew)
            val = maxNew;

        return val;
    }
}




