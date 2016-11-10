using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveManager : MonoBehaviour {

	public Vector2 firstPoint;
	public Vector2 secondPoint;

	public Vector2 handlerFirstPoint;
	public Vector2 handlerSecondPoint;

	public int pointsQuantity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Calculate2DPoints ();
	}

	Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float u = 1f - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term

		return p;
	}

	public Vector2[] Calculate2DPoints()
	{
		List<Vector2> points = new List<Vector2>();

		points.Add(firstPoint);
		for(int i=1;i<pointsQuantity;i++)
		{
			points.Add(CalculateBezierPoint((1f/pointsQuantity)*i,firstPoint,handlerFirstPoint,handlerSecondPoint,secondPoint));
		}
		points.Add(secondPoint);

		return points.ToArray();
	}
}
