using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour {

	public float delay = 1.1f; // 1.1 to sync with music

	void Start () {
		StartCoroutine ("Delay");
	}

	private IEnumerator Delay() {
		yield return new WaitForSeconds(delay);
		this.GetComponent<AudioSource> ().Play ();
	}

}
