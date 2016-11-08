using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class CMCfilterRESETTER : MonoBehaviour
{
    public AudioMixer theMixer;

    void Start ()
    {
        theMixer.SetFloat("PitchShift", 1);
        theMixer.SetFloat("CutOffFreqHP", 10);
        theMixer.SetFloat("CutOffFreqLP", 22000);
    }
}
