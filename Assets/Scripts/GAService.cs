using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine;

public class GAService : MonoBehaviour

{

	private int numSeg = 0; // NOTE: segment number.
	private int populationSize;
	private int chromosomesSize;
	private int[] segManifest; // NOTE: Achieved segment number of all individuals.
	private double[] fitnesses;
	private static int K = 3; // NOTE: K-size tournament.
	private float[,] accelerationChromosomes;
	private float[,] steeringChromosomes;
	private float[,] childAccelerationChromosomes;
	private float[,] childSteeringChromosomes;



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
		initChildChromosomes();
		calculateFitnesses();
		int[] population = doKSizeTournament(K);
		doCrossover(population, 0.5f);
		doMutation(0.1f);
	}

	private void mutateOutlier(int oID, int cID)
	{
		try
		{
			int outlierSeg = segManifest[oID];
			for (int i = outlierSeg - 1; i < chromosomesSize; i++)
			{
				childSteeringChromosomes[cID, i] = (float)Math.Round(Random.Range(0.9f, 1.0f), 2); // TODO: not hard-coded.
				steeringChromosomes[cID, i] = (float)Math.Round(Random.Range(-1.0f, 1.0f), 2);
			}
		}
		catch (Exception e)
		{
			Debug.Log("[ERROR]: mutateOutlier" + e);
		}
	}

	/* Do mutation process. 
	* @param     rate     Mutation rate.
	*/
	private void doMutation(float rate)
	{
		int numGenes = populationSize * chromosomesSize;
		int numMutation = (int)Mathf.Ceil(rate * numGenes);
		int[] mutationGenes = new int[numMutation];
		for (int i = 0; i < numMutation; i++)
		{
			mutationGenes[i] = (int)Mathf.Ceil(100 * Random.Range((float)chromosomesSize / 100, (float)numGenes / 100));
		}
		mutate(mutationGenes);
	}

	/* Mutate specified genes.
	* @param     mutationGenes     All mutation gene indexes.
	*/
	private void mutate(int[] mutationGenes)
	{
		foreach (var i in mutationGenes)
		{
			int r = (int)Mathf.Floor(i / chromosomesSize);
			int c = (i % chromosomesSize) - 1; // NOTE: [FIX BUG]: index was outside the bounds of the array.
			c = c < 0 ? 0 : c;
			r = r == populationSize ? populationSize - 1 : r;
			childAccelerationChromosomes[r, c] += Random.Range(-0.2f, 0.2f);
			childSteeringChromosomes[r, c] += Random.Range(-0.2f, 0.2f);
		}
	}

	/* Get the index of the individual with the highest fitness value.
	* @return     Index.
	*/
	private int getHighestFitnessIndex()
	{
		int index = 0;
		for (int i = 1; i < populationSize; i++)
			if (fitnesses[i] > fitnesses[index])
				index = i;
		return index;
	}

	/* Inititate the next generation.
	*/
	private void initChildChromosomes()
	{
		childAccelerationChromosomes = new float[populationSize, chromosomesSize];
		childSteeringChromosomes = new float[populationSize, chromosomesSize];
	}

	/* Do crossover with the specified crossover rate.
	* @param     parents     Candidate	parents.
	* @param     rate          Crossover rate.
	* @return                     Population after crossover.
	*/
	private int[] doCrossover(int[] population, double rate)
	{

		ArrayList selected = new ArrayList();
		ArrayList nonSelected = new ArrayList();
		while (selected.Count == 0) // NOTE: [FIX BUG]: selected.Count = 0;
		{
			ArrayList splittedPopulation = selectParentsForCrossover(population, rate);
			selected = (ArrayList)splittedPopulation[0];
			nonSelected = (ArrayList)splittedPopulation[1];
		}

		int lowestFitnessIndexInNonSelected = getLowestFitnessIndexInNonSelected(nonSelected);
		int highestFitnessIndex = getHighestFitnessIndex();
		// NOTE: Elitism: 
		//  - Make sure the individual with the highest fitness has a slot in the next generation.
		//  - Replace the non-selected individual by the highest fitness individual.
		nonSelected[lowestFitnessIndexInNonSelected] = highestFitnessIndex;
		putNonSelectedToNextGeneration(nonSelected);

		mutateOutlier(highestFitnessIndex, lowestFitnessIndexInNonSelected);

		doUniformCrossover(selected, nonSelected.Count);
		return new int[populationSize];
	}

	private int getLowestFitnessIndexInNonSelected(ArrayList nonSelected)
	{
		int lowestFitnessOnNonSelected = 0;
		for (int i = 0; i < nonSelected.Count; i++)
			if (fitnesses[(int)nonSelected[i]] < fitnesses[lowestFitnessOnNonSelected])
				lowestFitnessOnNonSelected = i;
		return lowestFitnessOnNonSelected;
	}

	/* Put all individuals not selected as parents for crossover to the next generation.
	* @param     nonSelected     All non-selected individuals
	*/
	private void putNonSelectedToNextGeneration(ArrayList nonSelected)
	{
		for (int i = 0; i < nonSelected.Count; i++)
			for (int j = 0; j < chromosomesSize; j++)
			{
				childAccelerationChromosomes[i, j] = accelerationChromosomes[(int)nonSelected[i], j];
				childSteeringChromosomes[i, j] = steeringChromosomes[(int)nonSelected[i], j];
			}

	}

	/* Do uniform crossover over individials with specified indexes.
	* @param     selected     Indexes of selected individuals as parents.
	*/
	private void doUniformCrossover(ArrayList selected, int nonSelectedCount)
	{
		ArrayList clone = createShiftedClone(selected);

		for (int i = 0; i < selected.Count; i++)
			for (int j = 0; j < chromosomesSize; j++)
			{
				double random = generateRandoms(1)[0];
				childAccelerationChromosomes[nonSelectedCount + i, j] = random <= 0.5 ? accelerationChromosomes[(int)selected[i], j] : accelerationChromosomes[(int)clone[i], j];
				childSteeringChromosomes[nonSelectedCount + i, j] = random <= 0.5 ? steeringChromosomes[(int)selected[i], j] : steeringChromosomes[(int)clone[i], j];
			}
	}

	/* Create shifted clone of selected to crossover.
	* @param     selected     Indexes of selected individuals as parents.
	* @return                      Shifted clone.
	*/
	private ArrayList createShiftedClone(ArrayList selected)
	{
		ArrayList clone = new ArrayList();
		for (int i = 1; i < selected.Count; i++)
			clone.Add(selected[i]);
		clone.Add(selected[0]);
		return clone;
	}

	/* Select parents for crossover with the specified crossover rate.
	* @param     candidates     Candidates to select as parents.
	* @param     rate               Crossover rate.
	* @return                          Selected parents.	
	*/
	private ArrayList selectParentsForCrossover(int[] candidates, double rate)
	{
		double[] randoms = generateRandoms(populationSize);
		var selected = new ArrayList();
		var nonSelected = new ArrayList();
		for (int i = 0; i < populationSize; i++)
			if (randoms[i] <= rate)
				selected.Add(candidates[i]);
			else
				nonSelected.Add(candidates[i]);
		return new ArrayList() { selected, nonSelected };
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
		for (int i = 0; i < x.Length; i++)
			Debug.Log($"{i}: " + x[i]);
	}

	private void printDoubles(double[] x)
	{
		for (int i = 0; i < x.Length; i++)
			Debug.Log($"{i}: " + x[i]);
	}

	private void printArrList(ArrayList x)
	{
		for (int i = 0; i < x.Count; i++)
			Debug.Log($"{i}: " + x[i]);
	}

	private void printParentChromosomes()
	{
		Debug.Log("Parent: ");
		Debug.Log("Acceleration: ");
		for (int i = 0; i < populationSize; i++)
		{
			Debug.Log($"Individual {i}:");
			for (int j = 0; j < chromosomesSize; j++)
				Debug.Log($"Gene {j}: {this.accelerationChromosomes[i, j]}");
		}

		Debug.Log("Steering: ");
		for (int i = 0; i < populationSize; i++)
		{
			Debug.Log($"Individual {i}:");
			for (int j = 0; j < chromosomesSize; j++)
				Debug.Log($"Gene {j}: {this.steeringChromosomes[i, j]}");
		}
	}

	private void printChildChromosomes()
	{
		Debug.Log("Child: ");
		Debug.Log("Acceleration: ");
		for (int i = 0; i < populationSize; i++)
		{
			Debug.Log($"Individual {i}:");
			for (int j = 0; j < chromosomesSize; j++)
				Debug.Log($"Gene {j}: {this.childAccelerationChromosomes[i, j]}");
		}

		Debug.Log("Steering: ");
		for (int i = 0; i < populationSize; i++)
		{
			Debug.Log($"Individual {i}:");
			for (int j = 0; j < chromosomesSize; j++)
				Debug.Log($"Gene {j}: {this.childSteeringChromosomes[i, j]}");
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

	public void setPopulationSize(int size)
	{
		this.populationSize = size;
		fitnesses = new double[populationSize]; // NOTE: inititate fitnesses.
	}

	public int getPolulationSize()
	{
		return this.populationSize;
	}

	public void setChromosomesSize(int size)
	{
		this.chromosomesSize = size;
	}

	public int getChromosomesSize()
	{
		return this.chromosomesSize;
	}

	public void setAccelerationChromosomes(float[,] chromosomes)
	{
		this.accelerationChromosomes = chromosomes;
	}

	public float[,] getAccelerationChromosomes()
	{
		return this.accelerationChromosomes;
	}

	public void setSteeringChromosomes(float[,] chromosomes)
	{
		this.steeringChromosomes = chromosomes;
	}

	public float[,] getChildAccelerationChromosomes()
	{
		return childAccelerationChromosomes;
	}

	public float[,] getChildSteeringChromosomes()
	{
		return childSteeringChromosomes;
	}
}
