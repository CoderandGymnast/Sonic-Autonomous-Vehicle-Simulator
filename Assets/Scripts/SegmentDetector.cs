using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SegmentDetector : MonoBehaviour
{
	private static int START_SEGMENT = 0;
	public GameObject vehicle;
	private int previousSegment;
	public int currentSegment { get; private set; }


	// Start is called before the first frame update
	void Start()
	{
		currentSegment = previousSegment = START_SEGMENT;
	}

	// Update is called once per frame
	void Update()
	{

		if (vehicle) // NOTE: all individuals are destroyed, this logic is still executed.
		{
			// TODO: not hard-coded.
			double z = vehicle.transform.position.z;
			double x = vehicle.transform.position.x;
			if (z <= 80)
			{
				if (z < 0) // NOTE: Math.Floor(-0.1) = -1.
				{
					z = 0;
					currentSegment = 0;
					return;
				}

				currentSegment = (int)Math.Floor(z / 10);

			}
			else if (x <= 44)
			{

				currentSegment = (int)Math.Floor((Math.Atan((z - 80) / (44 - x)) / (Math.PI / 4))) + 7 + 1;
			}
			else
			{

			}

			if (currentSegment < 0)
			{
				Debug.Log($"negative current segment: {currentSegment}, z: {z}");
			}

			if (currentSegment != previousSegment)
			{
				previousSegment = currentSegment;
				notiSegment(previousSegment);
			}
		}
	}

	public void resetCurrentSegment()
	{ // NOTE: to avoid error when there are some individuals falling off the terrain and cause infinite spawn & destroy loop.
		currentSegment = 0;
	}

	private void notiSegment(int i)
	{
	}
}
