using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAService : MonoBehaviour

{

	private int numSeg = 0; // NOTE: segment number.
	private int populationSize;
	private int[] segManifest; // NOTE: Achieved segment number of all individuals.
	private double[] fitnesses;
	private static int K = 3; // NOTE: K-size tournament.

	private void Awake()
	{
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
	}

	public void process()
	{
		calculateFitnesses();
		int[] parents = doKSizeTournament(K);
	}

	/* Calculate fitness value for each individual.
	*/
	private void calculateFitnesses()
	{
		for (int i = 0; i < populationSize; i++)
		{
			fitnesses[i] = (double)segManifest[i] / numSeg;
		}
	}

	/* Do K-size tournament.
	* @param     k     Size.
	* @return            Selected individuals as parents. 
	*/
	private int[] doKSizeTournament(int k)
	{
		double sumFitnesses = 0.0f;
		foreach (var i in fitnesses)
			sumFitnesses += i;

		double[] probabilities = calculateProbabilities(sumFitnesses);
		double[] comulativeProbabilities = calculateComulativeProbabilities(probabilities);

		int[] parents = new int[populationSize];
		for (int i = 0; i < populationSize; i++)
			parents[i] = doOneTournament(comulativeProbabilities, k);

		return parents;
	}

	/* Calculate fitness probability of each individual.
	* @param     sumFitnesses     Sum of all fitnesses.
	* @return     						   Probabilities.
	*/
	private double[] calculateProbabilities(double sumFitnesses)
	{
		double[] probabilities = new double[populationSize];
		for (int i = 0; i < populationSize; i++)
			probabilities[i] = fitnesses[i] / sumFitnesses;
		return probabilities;
	}

	/* Calculate comulative probability of each individual.
	* @param     probabilities    Fitness probability of the entire population.
	* @return                           Comulative probabilities.
	*/
	private double[] calculateComulativeProbabilities(double[] probabilities)
	{
		double[] comulativeProbabilities = new double[populationSize];
		comulativeProbabilities[0] = probabilities[0];
		for (int i = 1; i < populationSize; i++)
		{
			comulativeProbabilities[i] = comulativeProbabilities[i - 1] + probabilities[i];
		}
		return comulativeProbabilities;
	}

	/* Do one tournament in K-size tournament selection.
	* @param     comulativeProbabilities     Comulative probabilities of the population.
	* @param     k                                      k in K-size tournament.
	* @return                                             Index of the selected individual as a parent.
	*/
	private int doOneTournament(double[] comulativeProbabilities, int k)
	{
		int[] selectedIndexes = selectKIndividuals(comulativeProbabilities, k);
		return getHighestFitnessIndex(selectedIndexes);
	}

	/* Generate 'num' random numbers in [0; 1]
	* @param     num     Number of random numbers to return.
	* @return                Generated random numbers.     
	*/
	private double[] generateRandoms(int num)
	{
		double[] randoms = new double[num];
		for (int i = 0; i < num; i++)
		{
			randoms[i] = Random.Range(0.0f, 1.0f);
		}
		return randoms;
	}

	/* Select k individuals based on their comulative probabilities.
	* @param     comulativeProbabilities     Comulative probabilities of the entire population.
	* @param     k									   Select k individuals.
	* @return     									     K selected individuals.
	*/
	private int[] selectKIndividuals(double[] comulativeProbabilities, int k)
	{
		double[] randoms = generateRandoms(k);
		int[] selectedIndexes = new int[k];
		for (int i = 0; i < k; i++)
		{
			for (int j = 0; j < populationSize; j++)
			{
				if (randoms[i] <= comulativeProbabilities[j])
				{
					selectedIndexes[i] = j;
					break;
				}
			}
		}

		return selectedIndexes;
	}

	/* Get index of the individual having the highest fitness in k selected individuals from one loop of K-size tournament.
	* @param     indexes     K indexes.
	* @return                     Index of the individual with the highest fitness.
	*/
	private int getHighestFitnessIndex(int[] indexes)
	{
		int highestFitnessIndex = indexes[0];
		foreach (var i in indexes)
			if (fitnesses[i] > fitnesses[highestFitnessIndex])
				highestFitnessIndex = i;
		return highestFitnessIndex;
	}

	private void printInts(int[] x)
	{
		for(int i = 0; i < x.Length; i++)
			Debug.Log($"{i}: " + x[i]);
	}

	private void printDoubles(double[] x)
	{
		for(int i = 0; i < x.Length; i++)
			Debug.Log($"{i}: " + x[i]);
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

	public void setPopulationSize(int size)
	{
		this.populationSize = size;
		fitnesses = new double[populationSize]; // NOTE: inititate fitnesses.
	}

	public int getPolulationSize()
	{
		return this.populationSize;
	}
}
