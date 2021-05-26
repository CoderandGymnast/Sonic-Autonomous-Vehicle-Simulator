using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SegmentDetector : MonoBehaviour
{
	private static int START_SEGMENT = 0;
	public GameObject vehicle;
	private int preSeg;
	public int currSeg { get; private set; }

	int[] stopZs = {100};
	int[] stopXs = {50, 150};


	// Start is called before the first frame update
	void Start()
	{
		currSeg = preSeg = START_SEGMENT;
	}

	// Update is called once per frame
	void Update()
	{

		if (vehicle) // NOTE: all individuals are destroyed, this logic is still executed.
		{
			// TODO: not hard-coded.
			double z = vehicle.transform.position.z;
			double x = vehicle.transform.position.x;
			if (z <= stopZs[0])
			{
				if (z < 0) // NOTE: Math.Floor(-0.1) = -1.
				{
					z = 0;
					currSeg = 0;
					return;
				}

				currSeg = (int)Math.Floor(z / 10);
			}
			else if (x <= stopXs[0])
			{
				currSeg = (int)Math.Floor((Math.Atan((z - stopZs[0]) / (stopXs[0] - x)) / (Math.PI / 4))) + 9 + 1; // NOTE: Alpha < 45 => Additional = 0.
			} else if(x <= stopXs[1])
			{
				int additional = (int)Math.Floor((x - stopXs[0]) / 10);
				currSeg = 12 + additional;
			} else {

			}

			if (currSeg < 0)
			{
				Debug.Log($"negative current segment: {currSeg}, z: {z}");
			}

			if (currSeg != preSeg)
			{
				preSeg = currSeg;
				notiSegment(preSeg);
			}


		}
	}

	public void resetCurrentSegment()
	{ // NOTE: to avoid error when there are some individuals falling off the terrain and cause infinite spawn & destroy loop.
		currSeg = 0;
	}

	private void notiSegment(int i)
	{
	}
}
