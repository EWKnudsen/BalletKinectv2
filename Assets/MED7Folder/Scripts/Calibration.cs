using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

public class Calibration : MonoBehaviour
{
    [HideInInspector]
    public CalibrationTimer caliTimer;
    [HideInInspector]
    public CubemanController cubeman;
    [HideInInspector]
    public bool finishedCali, hasCalibrated;


    ArrayList Vec_ArrListShoulder, Vec_ArrListNeck, Vec_ArrListHip;
    Vector3[] Vec_ArrShoulder, Vec_ArrNeck, Vec_ArrHip;
    public Vector3 Vec_calibraShoulderCen, Vec_calibraNeck, Vec_calibraHip;

    ArrayList Qua_ArrListHip;
    Quaternion[] Qua_ArrHip;
    public Quaternion Qua_calibraHip;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        caliTimer = GameObject.Find("UIManager").GetComponent<CalibrationTimer>();
        cubeman = GameObject.Find("Cubeman").GetComponent<CubemanController>();

        Vec_ArrListShoulder = new ArrayList();
        Vec_ArrListNeck = new ArrayList();
        Vec_ArrListHip = new ArrayList();

        Qua_ArrListHip = new ArrayList();

        hasCalibrated = false;
    }


    void Update()
    {
        //Adds the user's relevant joint-positions and rotaions into an arraylist as long as 'isCalibratingJoints' is true (about 5 seconds)
        if (caliTimer != null)
        {
            if (caliTimer.isCalibratingJoints)
            {
                Vec_ArrListShoulder.Add(cubeman.shoulderCenVec);
                Vec_ArrListNeck.Add(cubeman.neckVec);
                Vec_ArrListHip.Add(cubeman.hipCenVec);

                //discards rotations that are all zero
                if (cubeman.hipCenRot.x != 0 && cubeman.hipCenRot.y != 0 && cubeman.hipCenRot.z != 0)
                    Qua_ArrListHip.Add(cubeman.hipCenRot);
            }
            finishedCali = caliTimer.finishedCalibrating;
        }

        if (finishedCali && !hasCalibrated)
        {
            Debug.Log("caliTimer.isCalibratingJoints, finishedCali: " + finishedCali);

            Vec_ArrShoulder = Vec_ConvertToArray(Vec_ArrListShoulder);
            Vec_ArrNeck = Vec_ConvertToArray(Vec_ArrListNeck);
            Vec_ArrHip = Vec_ConvertToArray(Vec_ArrListHip);

            Vec_ArrShoulder = Vec_SortByDist(Vec_ArrShoulder);
            Vec_ArrNeck = Vec_SortByDist(Vec_ArrNeck);
            Vec_ArrHip = Vec_SortByDist(Vec_ArrHip);

            Vec_calibraShoulderCen = Vec_CalCustomAverage(Vec_ArrShoulder);
            Vec_calibraNeck = Vec_CalCustomAverage(Vec_ArrNeck);
            Vec_calibraHip = Vec_CalCustomAverage(Vec_ArrHip);

            Qua_ArrHip = Qua_ConvertToArray(Qua_ArrListHip);

            Qua_calibraHip = Qua_CalCustomAverage(Qua_ArrHip);
            
            finishedCali = false;
            hasCalibrated = true;

            /* just for testing. Showing that we have to use Lerp and cant just take the mean of a each axis of a Quaternion.
            float meanZ = 0, meanY = 0, meanX = 0;

            for (int index = 0; index < Qua_ArrHip.Length; index++)
            {
                Debug.Log("Qua X: " + Qua_ArrHip[index].x + "     Y: " + Qua_ArrHip[index].y + "     Z: " + Qua_ArrHip[index].z + "    W: " + Qua_calibraHip.w);

                meanZ += Qua_ArrHip[index].z;
                meanY += Qua_ArrHip[index].y;
                meanX += Qua_ArrHip[index].x;
                
            }
            Debug.Log("Qua the other mean X: " + (meanX / Qua_ArrHip.Length) + "     Y: " + (meanY / Qua_ArrHip.Length) + "     Z: " + (meanZ / Qua_ArrHip.Length));
            Debug.Log("Qua my MEAN X: " + Qua_calibraHip.x + "     Y: " + Qua_calibraHip.y + "     Z: " + Qua_calibraHip.z + "    W: " + Qua_calibraHip.w);
            */

            /*
			//-----------------can be deleted after the values has been discussed------------
			int hep1 = ArrShoulder.Length;
			int hep2 = ArrNeck.Length;
			int hep3 = ArrHip.Length;
			Debug.Log ("LENGTH:    S: " + hep1 + "    N: " + hep2 + "    H: " + hep3);


			Vector3 hup1 = CalAverage (ArrShoulder);
			Vector3 hup2 = CalAverage (ArrNeck);
			Vector3 hup3 = CalAverage (ArrHip);
			Debug.Log ("S MEAN:    X: " + hup1.x + "    Y: " + hup1.y + "    Z: " + hup1.z);


			Vector3 hyp1 = CalMedian (ArrShoulder);
			Vector3 hyp2 = CalMedian (ArrNeck);
			Vector3 hyp3 = CalMedian (ArrHip);
			Debug.Log ("S MEDIAN:    X: " + hyp1.x + "    Y: " + hyp1.y + "    Z: " + hyp1.z);


			Vector3 hop1 = CalCustomAverage (ArrShoulder);
			//Vector3 hop2 = CalCustomAverage(ArrNeck);
			//Vector3 hop3 = CalCustomAverage(ArrHip);
			Debug.Log ("S customMEAN:    X: " + hop1.x + "    Y: " + hop1.y + "    Z: " + hop1.z);
			//-------------------------------------------------------------------------------
            */
        }

    }


    //Converts an ArrayList of vector3's to an Array 
    Vector3[] Vec_ConvertToArray(ArrayList theArrayList)
    {
        object[] arr = theArrayList.ToArray();
        Vector3[] arrVec = new Vector3[arr.Length];

        for (int index = 0; index < arr.Length; index++)
        {
            arrVec[index] = (Vector3)arr[index];
        }

        return arrVec;
    }


    //Sorts Array of vector3's by magnitude AND removes vector3's with magnitude of 0
    Vector3[] Vec_SortByDist(Vector3[] arr)
    {
        int zeroCounter = 0;

        Vector3 temp;

        for (int theSize = 0; theSize < arr.Length; theSize++)
        {
            if (arr[theSize].magnitude < 0.001)
                zeroCounter++;

            for (int index = 0; index < arr.Length - 1; index++)
            {
                if (arr[index].magnitude > arr[index + 1].magnitude)
                {
                    temp = arr[index + 1];
                    arr[index + 1] = arr[index];
                    arr[index] = temp;
                }
            }
        }

        Vector3[] arrWithoutZero = new Vector3[arr.Length - zeroCounter];

        for (int index = 0; index < arr.Length - zeroCounter; index++)
            arrWithoutZero[index] = arr[index + zeroCounter];

        return arrWithoutZero;
    }


    //Calculates the average of vector3's (Not Used, but an alternative way to decide the reference position)
    Vector3 Vec_CalAverage(Vector3[] arr)
    {
        Vector3 average = Vector3.zero;

        for (int i = 0; i < arr.Length; i++)
            average += arr[i];

        return average / arr.Length;
    }


    //Calculates the median of vector3's (Not Used, but an alternative way to decide the reference position)
    Vector3 Vec_CalMedian(Vector3[] arr)
    {
        return arr[arr.Length / 2];
    }


    //Calculates a customized Average of vector3's 
    Vector3 Vec_CalCustomAverage(Vector3[] arr)
    {
        Vector3 average = Vector3.zero;

        int inLiners = arr.Length / 24;

        //Debug.Log("inLiners: " + inLiners);

        for (int i = inLiners; i < arr.Length - inLiners; i++)
            average += arr[i];

        average /= (arr.Length - inLiners);

        return average;
    }
    

    //Converts an ArrayList of Quaternion's to an Array 
    Quaternion[] Qua_ConvertToArray(ArrayList theArrayList)
    {
        object[] arr = theArrayList.ToArray();
        Quaternion[] arrVec = new Quaternion[arr.Length];

        for (int index = 0; index < arr.Length; index++)
        {
            arrVec[index] = (Quaternion)arr[index];
        }

        return arrVec;
    }


    //Calculates the average of an array of Quaternions trough a linear interpolation loop
    Quaternion Qua_CalCustomAverage(Quaternion[] arr) {
        Quaternion average = Quaternion.identity;

        average = Quaternion.Lerp(arr[1], arr[2], 0.5f);

        for (int index = 3; index < arr.Length; index++)
            average = Quaternion.Lerp(average, arr[index], (1 / index));

        return average;
    }


    /*
    //removes zeros from array   NOT DONE. AND MAYBE NOT NEEDED
    Quaternion[] Qua_RemoveZeros(Quaternion[] arr)
    {
        //not done

        int noneZeroCounter = 0;

        for (int index = 0; index < arr.Length; index++)
        {
            if (arr[index].x != 0 && arr[index].y != 0 && arr[index].z != 0)
                noneZeroCounter++;
        }

        Quaternion[] arrWithoutZero = new Quaternion[noneZeroCounter];

        for (int index = 0; index < arr.Length; index++)
        {
            if (arr[index].x != 0 && arr[index].y != 0 && arr[index].z != 0)
            {
                arrWithoutZero[index] = 0;
            }

        }

        return arrWithoutZero;
    }
    */  // MAYBE NOT NEEDED
}





//MALTES VERSION
/*
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Calibration : MonoBehaviour
{
[HideInInspector]
public UIManager uiManager;
[HideInInspector]
public CubemanController cubeman;

public Vector3 calibratedHip, calibratedShoulder, calibratedNeck;
int counter;

void Awake()
{
    DontDestroyOnLoad(transform.gameObject);
}

void Start ()
{
    uiManager = GameObject.Find ("UIManager").GetComponent<UIManager> ();
    cubeman = GameObject.Find ("Cubeman").GetComponent<CubemanController> ();
}


void Update ()
{
    if (uiManager.isCalibratingJoints)
        StartCoroutine("CalculateAverage");

    if (uiManager.finishedCalibrating) {
        calibratedHip = calibratedHip/counter;
        calibratedShoulder = calibratedShoulder/counter;
        calibratedNeck = calibratedNeck/counter;

        Debug.Log("av " + calibratedShoulder);

        StopCoroutine("CalculateAverage");
        uiManager.finishedCalibrating = false;
    }
}

IEnumerator CalculateAverage()
{
    calibratedHip += cubeman.Hip_Center.transform.position;
    calibratedShoulder += cubeman.shoulderCenterPos;
    calibratedNeck += cubeman.Neck.transform.position;

    Debug.Log("S " + cubeman.shoulderCenterPos + "    added: " + calibratedShoulder);

    counter++;
    yield return new WaitForSeconds(0.2f);
    StartCoroutine("CalculateAverage");
}
}
*/
