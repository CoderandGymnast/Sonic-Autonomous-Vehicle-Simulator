using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class AIController: MonoBehaviour
{
	public GameObject car;
	private GameObject carClone;
private SegmentDetector segDetector;
	private float[,] coms = new float[,] { {0.1f,0.1f, 0.1f }, {0.2f,0.3f, 0.3f }, {-0.2f,0.5f, 0.5f }, {-1,0.3f, 0.3f } };
	private bool isCloned = true;
	// Start is called before the first frame update
	void Start()
    {
		segDetector = gameObject.GetComponent<SegmentDetector>(); // TODO: refactor code.
		segDetector.vehicle = car;
	//	generatePopulation();
		}

	private void Update()
	{
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (isCloned)
		{
			//Debug.Log("[AI CONTROLLER]: fixed update");
			Vector3 pos = new Vector3(Random.Range(0, 44), 0, 5);
			carClone = Instantiate(car, pos, Quaternion.Euler(0, 0, 0));
			isCloned = false;
		}

		CarController carController = carClone.GetComponent<CarController>();

	//Debug.Log(carController.CurrentSpeed);
		float steer = coms[segDetector.prevSeg, 0];
		float acc = coms[segDetector.prevSeg, 1];
		float footBrake = coms[segDetector.prevSeg, 2];
		carController.Move(steer,acc,footBrake,0.0f);
		//CollisionResponsor collisionResponsor = car.GetComponent<CollisionResponsor>(); // NOTE: placing in assets to use.
																						//Debug.Log("AI controller" + collisionResponsor.haveCollided);
	}

	// TODO: mô - đun hóa.
	private void generatePopulation()
	{
		}

}
