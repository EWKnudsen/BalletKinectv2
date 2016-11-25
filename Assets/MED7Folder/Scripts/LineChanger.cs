using UnityEngine;
using System.Collections;

public class LineChanger : MonoBehaviour {

    public float score;
    public LineRenderer[] lines = new LineRenderer[25];

    public CubemanController cubeman;
    private bool foundPlayer = false;

	// Use this for initialization
	void Start () {
        cubeman = FindObjectOfType<CubemanController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (cubeman.hasValues == true && foundPlayer == false) {
            for (int i = 0; i < 25; i++) {
                lines[i] = GameObject.Find("Line" + i).GetComponent<LineRenderer>();
            }
            foundPlayer = true;
        }
        //Debug.Log(foundPlayer);
        //Debug.Log(cubeman.playerIndex);
        if (foundPlayer == true)
        {
            if (score < 80)
            {
                lines[1].SetColors(Color.red, Color.red);
                lines[20].SetColors(Color.red, Color.red);
                // TORSO Line1 and Line20
                // Hips Line16 and Line12
            }
            if (score >= 80)
            {
                lines[1].SetColors(Color.green, Color.green);
                lines[20].SetColors(Color.green, Color.green);
            }
        }
	}
}
