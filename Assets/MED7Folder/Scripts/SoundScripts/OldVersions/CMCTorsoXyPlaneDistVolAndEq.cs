﻿//-----------------------------------------------------
//Controls Volume and 4 equalizers, by calculating the
//distance of the center hip, shoulder and neck
//in zx-plane.
//
//Sound if your posture is NOT vertical.
//Different sound for each axis.
//-----------------------------------------------------

using UnityEngine;
//using Windows.Kinect;
using System;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class CMCTorsoXyPlaneDistVolAndEq : MonoBehaviour
{
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Whether the cubeman is allowed to move vertically or not.")]
    public bool verticalMovement = true;

    [Tooltip("Whether the cubeman is facing the player or not.")]
    public bool mirroredMovement = false;

    [Tooltip("Rate at which the cubeman will move through the scene.")]
    public float moveRate = 1f;

    //public GameObject debugText;

    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Neck;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;
    public GameObject Spine_Shoulder;
    public GameObject Hand_Tip_Left;
    public GameObject Thumb_Left;
    public GameObject Hand_Tip_Right;
    public GameObject Thumb_Right;

    public LineRenderer skeletonLine;
    public LineRenderer debugLine;

    private GameObject[] bones;
    private LineRenderer[] lines;

    private LineRenderer lineTLeft;
    private LineRenderer lineTRight;
    private LineRenderer lineFLeft;
    private LineRenderer lineFRight;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialPosOffset = Vector3.zero;
    private Int64 initialPosUserID = 0;


    //MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public AudioMixer theMixer;
    float distHipShoulder = 0;
    float distShoulderNeck = 0;
    float totalDist = 0;
    float totalAttenuation = -80;
    float minAtten = -80;
    float maxAtten = 0;
    public float score;
    bool isAttenReset = false;

    float NegaScaledAxisZ = 0, PosiScaledAxisZ = 0, NegaScaledAxisX = 0, PosiScaledAxisX = 0;

    //Change these values to calibrate the sound sensitivity. Be careful, REMEMBER THE OLD VALUES
    //float correctionVal = 40;
    static float minDist = 0.01f;
    static float maxDist = 0.25f;

    static float minAxis = 0.02f;
    static float maxAxis = 0.20f;
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    void Start()
    {
        //store bones in a list for easier access
        bones = new GameObject[] {
            Hip_Center,
            Spine,
            Neck,
            Head,
            Shoulder_Left,
            Elbow_Left,
            Wrist_Left,
            Hand_Left,
            Shoulder_Right,
            Elbow_Right,
            Wrist_Right,
            Hand_Right,
            Hip_Left,
            Knee_Left,
            Ankle_Left,
            Foot_Left,
            Hip_Right,
            Knee_Right,
            Ankle_Right,
            Foot_Right,
            Spine_Shoulder,
            Hand_Tip_Left,
            Thumb_Left,
            Hand_Tip_Right,
            Thumb_Right
        };

        // array holding the skeleton lines
        lines = new LineRenderer[bones.Length];

        //		if(skeletonLine)
        //		{
        //			for(int i = 0; i < lines.Length; i++)
        //			{
        //				Debug.Log ("Line: " + i + " instantiate started.");
        //
        //				if((i == 22 || i == 24) && debugLine)
        //					lines[i] = Instantiate(debugLine) as LineRenderer;
        //				else
        //					lines[i] = Instantiate(skeletonLine) as LineRenderer;
        //
        //				lines[i].transform.parent = transform;
        //			}
        //		}

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //transform.rotation = Quaternion.identity;
    }


    void Update()
    {
        //MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        if (!isAttenReset)
        {
            theMixer.SetFloat("Torso_Attenuation", -50);
            isAttenReset = true;
        }
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        KinectManager manager = KinectManager.Instance;

        // get 1st player
        Int64 userID = manager ? manager.GetUserIdByIndex(playerIndex) : 0;

        if (userID <= 0)
        {
            initialPosUserID = 0;
            initialPosOffset = Vector3.zero;

            // reset the pointman position and rotation
            if (transform.position != initialPosition)
            {
                transform.position = initialPosition;
            }

            if (transform.rotation != initialRotation)
            {
                transform.rotation = initialRotation;
            }

            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].gameObject.SetActive(true);

                bones[i].transform.localPosition = Vector3.zero;
                bones[i].transform.localRotation = Quaternion.identity;

                if (lines[i] != null)
                {
                    lines[i].gameObject.SetActive(false);
                }
            }

            return;
        }

        // set the position in space
        Vector3 posPointMan = manager.GetUserPosition(userID);
        Vector3 posPointManMP = new Vector3(posPointMan.x, posPointMan.y, !mirroredMovement ? -posPointMan.z : posPointMan.z);


        // store the initial position
        if (initialPosUserID != userID)
        {
            initialPosUserID = userID;
            //initialPosOffset = transform.position - (verticalMovement ? posPointMan * moveRate : new Vector3(posPointMan.x, 0, posPointMan.z) * moveRate);
            initialPosOffset = posPointMan;
        }

        Vector3 relPosUser = (posPointMan - initialPosOffset);
        relPosUser.z = !mirroredMovement ? -relPosUser.z : relPosUser.z;

        transform.position = initialPosOffset + (verticalMovement ? relPosUser * moveRate : new Vector3(relPosUser.x, 0, relPosUser.z) * moveRate);

        // update the local positions of the bones
        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                int joint = !mirroredMovement ? i : (int)KinectInterop.GetMirrorJoint((KinectInterop.JointType)i);
                if (joint < 0)
                    continue;

                if (manager.IsJointTracked(userID, joint))
                {
                    bones[i].gameObject.SetActive(true);

                    Vector3 posJoint = manager.GetJointPosition(userID, joint);
                    posJoint.z = !mirroredMovement ? -posJoint.z : posJoint.z;

                    Quaternion rotJoint = manager.GetJointOrientation(userID, joint, !mirroredMovement);
                    rotJoint = initialRotation * rotJoint;

                    posJoint -= posPointManMP;

                    if (mirroredMovement)
                    {
                        posJoint.x = -posJoint.x;
                        posJoint.z = -posJoint.z;
                    }

                    bones[i].transform.localPosition = posJoint;
                    bones[i].transform.rotation = rotJoint;

                    if (lines[i] == null && skeletonLine != null)
                    {
                        lines[i] = Instantiate((i == 22 || i == 24) && debugLine ? debugLine : skeletonLine) as LineRenderer;
                        lines[i].transform.parent = transform;
                    }

                    if (lines[i] != null)
                    {
                        lines[i].gameObject.SetActive(true);
                        Vector3 posJoint2 = bones[i].transform.position;

                        Vector3 dirFromParent = manager.GetJointDirection(userID, joint, false, false);
                        dirFromParent.z = !mirroredMovement ? -dirFromParent.z : dirFromParent.z;
                        Vector3 posParent = posJoint2 - dirFromParent;

                        //lines[i].SetVertexCount(2);
                        lines[i].SetPosition(0, posParent);
                        lines[i].SetPosition(1, posJoint2);
                    }

                    ///MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    if (i == 6)
                    {
                        Vector3 posHipCenter = bones[0].transform.localPosition;
                        Vector3 posShoulderCenter = bones[1].transform.localPosition;
                        Vector3 posNeck = bones[2].transform.localPosition;

                        distHipShoulder  = CalXZdist(posShoulderCenter, posHipCenter);
                        distShoulderNeck = CalXZdist(posShoulderCenter, posNeck);

                        totalDist = distHipShoulder + distShoulderNeck;
                        totalAttenuation = ScalingBetween(totalDist, minAtten, maxAtten, minDist, maxDist);



                        //testing this...
                        float linear = totalAttenuation;

                        double hepson = (70 * Math.Log10(((double)totalAttenuation / 2) + 42)) - 80;
                        totalAttenuation = (float)hepson + 20;
                        
                        Debug.Log("OFF linear: " + linear + "    ON log: " + hepson);



                        if (totalAttenuation > maxAtten) {
                            totalAttenuation = maxAtten;
                        } else if (totalAttenuation < minAtten) {
                            totalAttenuation = minAtten;
                        }

                        theMixer.SetFloat("Torso_Attenuation", totalAttenuation);        

                        //Debug.Log("totalDist: " + totalDist + "   totalAttenuation: " + totalAttenuation);


                        //---------------------------------------------------------------------------
                        //current problem: only 2 points... 
                        float AxisZ = posHipCenter.z - posShoulderCenter.z;
                        float AxisX = posHipCenter.x - posShoulderCenter.x;

                        if (AxisZ < 0)
                        {
                            NegaScaledAxisZ = ScalingBetween(-AxisZ, 0.3f, 1f, minAxis, maxAxis);
                            theMixer.SetFloat("Torso_EqFreqGain_00", NegaScaledAxisZ);
                        }
                        else if (AxisZ >= 0)
                        {
                            PosiScaledAxisZ = ScalingBetween(AxisZ, 0.3f, 1f, minAxis, maxAxis);
                            theMixer.SetFloat("Torso_EqFreqGain_01", PosiScaledAxisZ);
                        }

                        if (AxisX < 0)
                        {
                            NegaScaledAxisX = ScalingBetween(-AxisX, 0.3f, 1f, minAxis, maxAxis);
                            theMixer.SetFloat("Torso_EqFreqGain_02", NegaScaledAxisX);
                        }
                        else if (AxisX >= 0)
                        {
                            PosiScaledAxisX = ScalingBetween(AxisX, 0.3f, 1f, minAxis, maxAxis);
                            theMixer.SetFloat("Torso_EqFreqGain_03", PosiScaledAxisX);
                        }

                        //Debug.Log("negZ: " + NegaScaledAxisZ + "    posZ: " + PosiScaledAxisZ + "    negX: " + NegaScaledAxisX + "    posX: " + PosiScaledAxisX);


                        //-------------------------------------------------------------------------------
                        //Score
                        score = 100 - ScalingBetween(totalDist, 0, 100, minDist, maxDist);
                        if (score < 0)
                            score = 0;
                        else if (score > 100)
                            score = 100;
                        //Debug.Log("score: " + score);
                    }
                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                }
                else
                {
                    bones[i].gameObject.SetActive(false);

                    if (lines[i] != null)
                    {
                        lines[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    ///MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    protected float CalXZdist(Vector3 vecA, Vector3 vecB)
    {
        return (float) Math.Sqrt( Math.Pow(vecB.x - vecA.x, 2) + Math.Pow(vecB.z - vecA.z, 2) ) ;
    }


    protected float ScalingBetween(float unscaledVal, float minNew, float maxNew, float minOld, float maxOld)
    {
        return (maxNew - minNew) * (unscaledVal - minOld) / (maxOld - minOld) + minNew;
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}