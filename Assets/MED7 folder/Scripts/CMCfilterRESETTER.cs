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
    private float minFrequence = 20;
    private float maxFrequence = 12000;

    void Start ()
    {
        ResetAllFilterValues();
    }


    public void ResetAllFilterValues()
    {
        theMixer.SetFloat("Torso_PitchShift", 1);
        theMixer.SetFloat("Torso_CutOffFreqHP", 10);
        theMixer.SetFloat("Torso_CutOffFreqLP", 22000);
        theMixer.SetFloat("Torso_Attenuation", 00);

        theMixer.SetFloat("Pelvis_PitchShift", 1);
        theMixer.SetFloat("Pelvis_CutOffFreqHP", 10);
        theMixer.SetFloat("Pelvis_CutOffFreqLP", 22000);
        theMixer.SetFloat("Pelvis_Attenuation", 00);

        int[] CenterFreqs = CalCenterFreqs(numberOfEqs, minFrequence, maxFrequence);
        for (int i = 0; i < CenterFreqs.Length; i++)
        {
            string filterNameCenterFreq1 = "Torso_EqCenterFreq_0" + i;
            theMixer.SetFloat(filterNameCenterFreq1, CenterFreqs[i]);
            string filterNameFreqGain1 = "Torso_EqFreqGain_0" + i;
            theMixer.SetFloat(filterNameFreqGain1, 1);

            string filterNameCenterFreq2 = "Pelvis_EqCenterFreq_0" + i;
            theMixer.SetFloat(filterNameCenterFreq2, CenterFreqs[i]);
            string filterNameFreqGain2 = "Pelvis_EqFreqGain_0" + i;
            theMixer.SetFloat(filterNameFreqGain2, 1);
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
