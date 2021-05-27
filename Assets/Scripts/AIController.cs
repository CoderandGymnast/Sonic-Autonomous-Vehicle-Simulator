using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using System;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{

	private static int NUM_SEGMENT = 33;

	private static int VEHICLE_WIDTH = 3; // TODO: not hard-coded.
	private static int BARRIERS_WIDTH = 1; // TODO: nothard-coded.
	private static int ROAD_WIDTH = 50; // TODO: not hard-coded.
	private static float[] STARTING_POSITION = { ROAD_WIDTH / 2, 0.5f, 5 }; // NOTE: to avoid cars falling off the map.
	private static int POPULATION_SIZE = 5;
	private static int CHROMOSOMES_SIZE = NUM_SEGMENT; // NOTE: total segments.
	private float[,] accelerationChromosomes = new float[POPULATION_SIZE, CHROMOSOMES_SIZE];

	private float[,] steeringChromosomes = new float[POPULATION_SIZE, CHROMOSOMES_SIZE];
	private GameObject[] individuals = new GameObject[POPULATION_SIZE];

	public GameObject car; // NOTE: car is assigned at AIController's component using UI.
	private GameObject carClone;
	private SegmentDetector segmentDetector;
	private float[,] coms = new float[,] { { 0.1f, 0.1f, 0.1f }, { 0.2f, 0.3f, 0.3f }, { -0.2f, 0.5f, 0.5f }, { -1, 0.3f, 0.3f } };
	// Start is called before the first frame update

	private int destroyCounter = 0; // TODO: move.

	private int[] segManifest = new int[POPULATION_SIZE]; // NOTE: track latest segments of all entities (individuals) to calculate fitness.
	private GAService GAService;
	private void Awake()
	{
		//GameObject roadSegment = (GameObject)Resources.Load("RoadSegment", typeof(GameObject));
		//roadSegment = Instantiate(roadSegment, new Vector3(0, 0, 0), Quaternion.identity);
		//Debug.Log(roadSegment.GetComponentInChildren<Collider>().bounds.size.x);
	}

	void Start()
	{
		segmentDetector = gameObject.GetComponent<SegmentDetector>(); // TODO: refactor code.
		GAService = (GAService) gameObject.GetComponent<GAService>(); // NOTE: GAService component belongs to AIController.
		GAService.setNumSeg(NUM_SEGMENT);
		GAService.setPopulationSize(POPULATION_SIZE);

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

			GAService.setSegManifest(segManifest); // NOTE: update segment manifest to calculate fitness.
			GAService.process();
			
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
			int currSeg = segmentDetector.currSeg;

			segManifest[i] = currSeg; // NOTE: update latest segment of each entitiy (individual)

			AccidentDetector accidentDetector = individual.GetComponent<AccidentDetector>(); // NOTE: placing in assets to use.
			if (accidentDetector.isFalling() || accidentDetector.isCollided) // NOTE: check whether individual 'falled off the terrain' or 'is collided'
			{
				Destroy(individual);
				destroyCounter++;
				individuals[i] = null;
				continue;
			}
			CarController controller = individual.GetComponent<CarController>();
			float acc = accelerationChromosomes[i, currSeg];
			float steer = steeringChromosomes[i, currSeg];
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

			// TODO: spawn with positions relative to map, not terrain.
			Vector3 pos = new Vector3(Random.Range(BARRIERS_WIDTH + VEHICLE_WIDTH / 2, ROAD_WIDTH - BARRIERS_WIDTH - VEHICLE_WIDTH / 2), STARTING_POSITION[1], STARTING_POSITION[2]); // NOTE: Change this to a fixed position and to detect segments in debug mode.
			Quaternion rot = Quaternion.Euler(0, 0, 0); // NOTE: rotation.
			individuals[i] = Instantiate(car, pos, rot);

		}
	}

	// TODO: Modularization.
	private void generatePopulation()
	{
	}
	private void printComponents(GameObject o)
	{

		Component[] components = o.GetComponents(typeof(Component));
		foreach (Component component in components)
		{
			Debug.Log("Component: " + component.ToString());
		}
	}

	private void printSegmentManifest()
	{
		for (int i = 0; i < POPULATION_SIZE; i++)
		{
			Debug.Log(segManifest[i]);
		}
	}
}
