//-----------------------------------------------------
//Controls the pitch of a sound, by calculating the
//distance of the center hip and center shoulder
//in zx-plane  
//
//Low pitch if your spine is NOT vertical
//-----------------------------------------------------

using UnityEngine;
//using Windows.Kinect;
using System;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;

public class CMCxyPlaneHipShoulderDistPitch : MonoBehaviour
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
    //Version: PITCH  Velocity
    private Vector3 lastPos;
    int FilterCounter = 0;

    public AudioMixer theMixer;
    float pitch;
    float velSum;

    //Version: PITCH  Dist
    //range has been obtained by trail and error, you can change values to tune it
    /*
    static float oldMin = 0.3f;
    static float newMin = 0.5f;
    float oldRange = (2f - oldMin);
    float newRange = (1.5f - newMin);
    */
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

        //MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        lastPos = transform.position;
        theMixer.GetFloat("PitchShift", out pitch);
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        //transform.rotation = Quaternion.identity;
    }


    void Update()
    {
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
                    /// <summary>
                    /// Version: Controlling PITCH with velocity of left hand joint
                    /// </summary>
                    if (i == 6) //left hand on cube man. remember he is mirrored (your right hand)
                    {
                        float velocity = Vector3.Distance(lastPos, posJoint) / Time.deltaTime;

                        print("current: " + posJoint.magnitude + "     last: " + lastPos.magnitude);
                        //print("Distance to other: " + vectorDist + "   pos: " + posJoint.x + " , " + posJoint.y + " , " + posJoint.z);

                        lastPos = posJoint;

                        velSum += velocity;
                        FilterCounter++;

                        if (FilterCounter % 3 == 0)
                        {
                            if (velSum > 0.3f) //move, and pitch increases
                            {
                                if (pitch < 1.5f)
                                {
                                    pitch += 0.001f * velSum;
                                    theMixer.SetFloat("PitchShift", pitch);
                                }
                            }
                            else if (pitch > 0.4f) //if you dont move, pitch decreases
                            {
                                pitch -= 0.03f;
                                theMixer.SetFloat("PitchShift", pitch);
                            }
                            FilterCounter = 0;
                            velSum = 0;
                        }
                    }
                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


                    ///MED7 ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    /// <summary>
                    /// Version: Controlling PITCH relative to the distance of hip center and both hands
                    /// Remember to outcomment the other version if you use this
                    /// </summary>1
                    /*
                    if (i == 6)
                    {
                        Vector3 leftHandPos = bones[7].transform.localPosition;
                        Vector3 RightHandPos = bones[11].transform.localPosition;
                        Vector3 HipCenterPos = bones[0].transform.localPosition;

                        float dist = Vector3.Distance(leftHandPos, HipCenterPos) + Vector3.Distance(RightHandPos, HipCenterPos);
                  
                        float RangeConvertedPitchValue = (((dist - oldMin) * newRange) / oldRange) + newMin;
                        //print("dist: " + RangeConvertedPitchValue);

                        theMixer.SetFloat("PitchShift", RangeConvertedPitchValue);
                    }
                    */
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
}
