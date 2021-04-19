using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SegmentDetector : MonoBehaviour
{
	private static int START_SEGMENT = 0;
	public GameObject vehicle;
	public int prevSeg = START_SEGMENT;
	private int currSeg = START_SEGMENT;


    // Start is called before the first frame update
    void Start()
    {
		notiSeg(prevSeg);
    }

	// Update is called once per frame
	void Update()
	{
		// TODO: not hard-coded.
		double z = vehicle.transform.position.z;
		double x = vehicle.transform.position.x;
		if (z <= 80)
		{
			currSeg = (int)Math.Floor(z / 10);
	
		} else if(x<=44)
		{

			currSeg = (int) Math.Floor((Math.Atan((z - 80) / (44 - x)) / (Math.PI / 4))) + 7 + 1;
		} else
		{

		}

		if (currSeg != prevSeg)
		{
			prevSeg = currSeg;
			notiSeg(prevSeg);
		}
	}

	private void notiSeg(int i)
	{
		Debug.Log($"[SEGMENT DETECTOR]: segment no.{i}");
	}
}
