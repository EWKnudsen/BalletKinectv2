//-----------------------------------------------------
//Resets all filtervalues
//distance/disalignment of the left and right hip
//in the y-axis
//
//Sound if your hip is NOT horizontal
//-----------------------------------------------------

using UnityEngine;
//using Windows.Kinect;
using System;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class CMCfilterRESETTER : MonoBehaviour
{
    public AudioMixer theMixer;

    private int numberOfEqs = 4;
    public float minFrequence = 20;
    public float maxFrequence = 12000;

    void Start ()
    {
        ResetAllFilterValues();
    }


    public void ResetAllFilterValues()
    {
        theMixer.SetFloat("PitchShift", 1);
        theMixer.SetFloat("CutOffFreqHP", 10);
        theMixer.SetFloat("CutOffFreqLP", 22000);
        theMixer.SetFloat("ISAttenuation", -80);

        int[] CenterFreqs = CalCenterFreqs(numberOfEqs, minFrequence, maxFrequence);
        for (int i = 0; i < CenterFreqs.Length; i++)
        {
            string filterNameCenterFreq = "EqCenterFreq_" + i;
            theMixer.SetFloat(filterNameCenterFreq, CenterFreqs[i]);

            string filterNameFreqGain = "EqFreqGain_" + i;
            theMixer.SetFloat(filterNameFreqGain, 1);
        }
    }

    private int[] CalCenterFreqs(int numOfEqs, float minFreq, float maxFreq)
    {
        float intervals = numOfEqs * 2;
        int[] centerFreqs = new int[numOfEqs];
        int index = 0;
        double hep;

        for (int i = 1; i < intervals; i+=2)
        {
            hep = minFreq * Math.Pow( Math.Pow( (maxFreq / minFreq), (1 / intervals) ), i);
            centerFreqs[index++] = (int)hep;
        }
        return centerFreqs;
    }

}
