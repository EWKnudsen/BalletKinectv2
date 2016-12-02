using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Calibration : MonoBehaviour {

	[HideInInspector]
	public UIManager uiManager;
	[HideInInspector]
	public CubemanController cubeman;

	public Vector3 calibratedHip, calibratedShoulder, calibratedNeck;
	int counter;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () {
		uiManager = GameObject.Find ("UIManager").GetComponent<UIManager> ();
		cubeman = GameObject.Find ("Cubeman").GetComponent<CubemanController> ();
	}
	

	void Update () {
		if (uiManager.calibrating) {
			StartCoroutine("CalculateAverage");
		} 
		if (uiManager.finished) {
			calibratedHip = calibratedHip/counter;
			calibratedShoulder = calibratedShoulder/counter;
			calibratedNeck = calibratedNeck/counter;
			StopCoroutine("CalculateAverage");
			uiManager.finished = false;
		}
	}

	IEnumerator CalculateAverage() {
		calibratedHip += cubeman.Hip_Center.transform.position;
		calibratedShoulder += cubeman.shoulderCenterPos;
		calibratedNeck += cubeman.Neck.transform.position;

		counter++;
		yield return new WaitForSeconds(0.2f);
		StartCoroutine("CalculateAverage");
	}
}
