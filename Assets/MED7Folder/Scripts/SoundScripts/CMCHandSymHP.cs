using System;
using UnityEngine;
using UnityEngine.Audio;

public class CMCHandSymHP : MonoBehaviour
{
    public AudioMixer theMixer;
    public CubemanController CMCScript;
    public float score;
    float minScore = 0, maxScore = 100;

    double minAtten = -80;
    double maxAtten = 0;
    bool isAttenReset = false;


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
                score = 90;
            }
        }
    }
}
