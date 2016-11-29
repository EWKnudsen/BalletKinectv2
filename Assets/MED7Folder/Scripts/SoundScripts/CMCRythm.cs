using System;
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


    double lasthep;
    double hepfirst, hepafstand;

    int counter = 0;
    double hep;

    float TimeT;

    void Start()
    {
        CMCScript = GameObject.Find("Cubeman").GetComponent<CubemanController>();
		metronomeScript =  GameObject.Find("Metronome").GetComponent<Metronome>();
    }


    void FixedUpdate()
    {
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


                    hep += hipVelAxisY_Scaled;

                    //if delta.time % 6 ==  0  

                    if (counter % 4 == 0)
                    {
                        hep /= 4;

                        if (hep < 1 && hep > -1 && metronomeScript.beat == 6)
                        {
                            //Debug.Log("PLAY NICE SOUND");
                        }

                        //Debug.Log("Counter: " + counter + "        hep: " + hep);

                        hep = 0;
                    }
                    counter++;


                    ///*
                    TimeT += Time.deltaTime * 100;

                    
                    if (Math.Abs(TimeT) % 100 == 0)
                    {
                        Debug.Log("tick yo!");
                    }
                    //*/




                    //Debug.Log("vel: " + hipVelAxisY_Scaled);



                    //Debug.Log("score: " + score + "      vel: " + hipVelAxisY_Scaled + "     curr: " + currentHipPos + "       last: " + last_HipPos);

                    //To Malte: make code in here
                    //...
                    //...

                }
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
        if (val > maxNew)
            val = maxNew;

        return val;
    }
}




