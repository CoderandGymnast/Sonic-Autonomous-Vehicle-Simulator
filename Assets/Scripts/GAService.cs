using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAService : MonoBehaviour

{

	private int numSeg = 0; // NOTE: segment number.
	private int populationSize;
	private int[] segManifest; // NOTE: Achieved segment number of all individuals.
	private double[] fitnesses;

	private void Awake() {
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}

	public void process() {

		calculateFitnesses();

	}
	private void calculateFitnesses() {
		for(int i = 0; i < populationSize; i++) {
			fitnesses[i]= (double) segManifest[i]/numSeg;
		}
	}

	public void setNumSeg(int numSeg)
	{
		this.numSeg = numSeg;
	}

	public int getNumSeg()
	{
		return this.numSeg;
	}

	public void setSegManifest(int[] segManifest)
	{
		this.segManifest = segManifest;
	}

	public int[] getSegManifest()
	{
		return this.segManifest;
	}

	public void setPopulationSize(int size) {
		this.populationSize = size;
		fitnesses = new double[populationSize]; // NOTE: inititate fitnesses.
	}

	public int getPolulationSize() {
		return this.populationSize;
	}
}
