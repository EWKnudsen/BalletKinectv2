//TO MALTE:
//Missing:
//it could be nice to have a timer on eg. 5 seconds for the user to
//find/get in the right posture right after they 'press any key'


//ESBENS VERSION
///*
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Linq;

public class Calibration : MonoBehaviour
{
    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public CubemanController cubeman;

    ArrayList ArrListShoulder, ArrListNeck, ArrListHip;
    Vector3[] ArrShoulder, ArrNeck, ArrHip;
    public Vector3 calibraShoulderCenVec, calibraNeckVec, calibraHipVec;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        cubeman = GameObject.Find("Cubeman").GetComponent<CubemanController>();

        ArrListShoulder = new ArrayList();
        ArrListNeck = new ArrayList();
        ArrListHip = new ArrayList();
    }


    void Update()
    {
        if (uiManager.isCalibratingJoints)
        {
            ArrListShoulder.Add(cubeman.shoulderCenVec);
            ArrListNeck.Add(cubeman.neckVec);
            ArrListHip.Add(cubeman.hipCenVec);

            //Debug.Log("in CalculateAverage, time: " + Time.time);
        }
        

        if (uiManager.finishedCalibrating)
        {
            ArrShoulder = ConvertToArray(ArrListShoulder);
            ArrNeck = ConvertToArray(ArrListNeck);
            ArrHip = ConvertToArray(ArrListHip);

            ArrShoulder = SortByDist(ArrShoulder);
            ArrNeck = SortByDist(ArrNeck);
            ArrHip = SortByDist(ArrHip);

            //-----------------can be deleted after the values has been discussed------------
            int hep1 = ArrShoulder.Length;
            int hep2 = ArrNeck.Length;
            int hep3 = ArrHip.Length;
            Debug.Log("LENGTH:    S: " + hep1 + "    N: " + hep2 + "    H: " + hep3);


            Vector3 hup1 = CalAverage(ArrShoulder);
            Vector3 hup2 = CalAverage(ArrNeck);
            Vector3 hup3 = CalAverage(ArrHip);
            Debug.Log("S MEAN:    X: " + hup1.x + "    Y: " + hup1.y + "    Z: " + hup1.z);


            Vector3 hyp1 = CalMedian(ArrShoulder);
            Vector3 hyp2 = CalMedian(ArrNeck);
            Vector3 hyp3 = CalMedian(ArrHip);
            Debug.Log("S MEDIAN:    X: " + hyp1.x + "    Y: " + hyp1.y + "    Z: " + hyp1.z);


            Vector3 hop1 = CalCustomAverage(ArrShoulder);
            //Vector3 hop2 = CalCustomAverage(ArrNeck);
            //Vector3 hop3 = CalCustomAverage(ArrHip);
            Debug.Log("S customMEAN:    X: " + hop1.x + "    Y: " + hop1.y + "    Z: " + hop1.z);
            //-------------------------------------------------------------------------------



            calibraShoulderCenVec = CalCustomAverage(ArrShoulder);
            calibraNeckVec = CalCustomAverage(ArrNeck);
            calibraHipVec = CalCustomAverage(ArrHip);
            uiManager.finishedCalibrating = false;
        }
    }


    //Converts an ArrayList of vector3's to an Array 
    Vector3[] ConvertToArray(ArrayList theArrayList)
    {
        object[] arr = theArrayList.ToArray();
        Vector3[] arrVec = new Vector3[arr.Length];

        for (int index = 0; index < arr.Length; index++)
        {
            arrVec[index] = (Vector3) arr[index];
        }

        return arrVec;
    }


    //Sorts Array of vector3's by magnitude AND removes vector3's with magnitude of 0
    Vector3[] SortByDist(Vector3[] arr) 
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


    //Calculates the average of vector3's
    Vector3 CalAverage(Vector3[] arr)
    {
        Vector3 average = Vector3.zero;

        for (int i = 0; i < arr.Length; i++)
            average += arr[i];

        return average / arr.Length;
    }


    //Calculates the median of vector3's
    Vector3 CalMedian(Vector3[] arr)
    {
        return arr[arr.Length / 2];
    }


    //Calculates a customized Average of vector3's
    Vector3 CalCustomAverage(Vector3[] arr)
    {
        Vector3 average = Vector3.zero;

        int inLiners = arr.Length / 24;

        //Debug.Log("inLiners: " + inLiners);

        for (int i = inLiners; i < arr.Length - inLiners; i++)
            average += arr[i];

        average /= (arr.Length - inLiners);
        
        return average;
    }
}
//*/




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
