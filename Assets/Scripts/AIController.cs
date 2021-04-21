using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{

	private static int VEHICLE_WIDTH = 2; // TODO: not hard-coded.
	private static int BARRIERS_WIDTH = 2;
	private static int ROAD_WIDTH = 44;
	private static int[] STARTING_POSITION = { ROAD_WIDTH / 2, 0, 5 }; // NOTE: to avoid cars falling off the map.
	private static int POPULATION_SIZE = 100;
	private static int CHROMOSOMES_SIZE = 10;
	private float[,] accelerationChromosomes = new float[POPULATION_SIZE, CHROMOSOMES_SIZE];

	private float[,] steeringChromosomes = new float[POPULATION_SIZE, CHROMOSOMES_SIZE];
	private GameObject[] individuals = new GameObject[POPULATION_SIZE];

	public GameObject car; // NOTE: car is assigned at AIController's component using UI.
	private GameObject carClone;
	private SegmentDetector segmentDetector;
	private float[,] coms = new float[,] { { 0.1f, 0.1f, 0.1f }, { 0.2f, 0.3f, 0.3f }, { -0.2f, 0.5f, 0.5f }, { -1, 0.3f, 0.3f } };
	// Start is called before the first frame update

	private int destroyCounter = 0; // TODO: move.
	void Start()
	{
		segmentDetector = gameObject.GetComponent<SegmentDetector>(); // TODO: refactor code.
		initPopulation();
		spawn();
		car.SetActive(false); // NOTE: hide (deactivate) based individual.
	}

	private void Update()
	{
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (destroyCounter < POPULATION_SIZE)
		{
			movePopulation();
		}
		else
		{
			car.SetActive(true);  // NOTE: activate based individual to spawn.
			destroyCounter = 0;
			segmentDetector.vehicle = null;
			initPopulation();
			spawn();
			car.SetActive(false); // NOTE: hide (deactivate) based individual.
		}
	}

	private void movePopulation()
	{
		for (int i = 0; i < POPULATION_SIZE; i++)
		{
			if (!individuals[i])
			{
				continue; // NOTE: check whether individual is destroyed.
			}
			GameObject individual = individuals[i];
			segmentDetector.vehicle = individual;
			int currentSegment = segmentDetector.currentSegment;
			AccidentDetector accidentDetector = individual.GetComponent<AccidentDetector>(); // NOTE: placing in assets to use.
			if (accidentDetector.isFalling() || accidentDetector.isCollided) // NOTE: check whether individual 'falled off the terrain' or 'is collided'
			{
				Destroy(individual);
				destroyCounter++;
				individuals[i] = null;
				continue;
			}
			CarController controller = individual.GetComponent<CarController>();
			float acc = accelerationChromosomes[i, currentSegment];
			float steer = steeringChromosomes[i, currentSegment];
			float footBrake = acc; // TODO: check value of footBrake.

			controller.Move(steer, acc, footBrake, 0.0f); // TODO: not hard-coded.
		}
	}

	// TODO: not random init.
	private void initPopulation()
	{
		for (int i = 0; i < POPULATION_SIZE; i++)
		{
			for (int j = 0; j < CHROMOSOMES_SIZE; j++)
			{
				accelerationChromosomes[i, j] = (float)Math.Round(Random.Range(0.0f, 1.0f), 2); // TODO: not hard-coded.
				steeringChromosomes[i, j] = (float)Math.Round(Random.Range(-1.0f, 1.0f), 2);
			}

		}
	}

	private void spawn()
	{
		for (int i = 0; i < POPULATION_SIZE; i++)
		{


			Vector3 pos = new Vector3(Random.Range(BARRIERS_WIDTH + VEHICLE_WIDTH, ROAD_WIDTH - BARRIERS_WIDTH - VEHICLE_WIDTH), STARTING_POSITION[1], STARTING_POSITION[2]); // NOTE: position. 
			Quaternion rot = Quaternion.Euler(0, 0, 0); // NOTE: rotation.
			individuals[i] = Instantiate(car, pos, rot);
		}
	}

	// TODO: mô - đun hóa.
	private void generatePopulation()
	{
	}

}
