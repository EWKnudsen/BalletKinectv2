using UnityEngine;
using System.Collections;

public class RotatingItem : MonoBehaviour {

	void Update () {
		transform.Rotate (new Vector3 (0, Time.deltaTime*50, 0));
	}

}
