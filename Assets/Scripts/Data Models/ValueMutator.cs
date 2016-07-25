using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;

public static class ValueMutator 
{
	public static float MutatePercentage(float inputValue, float mutateChance, float minDeviationPercent, float maxDeviationPercent)
	{
		var outputValue = inputValue;
		
		if (MutationSuccessful(mutateChance)) {
			var deviationAmount = Random.Range(minDeviationPercent, maxDeviationPercent);

			outputValue *= (1 + deviationAmount);
		}

		return outputValue;
	}
	public static float MutateFlat(float inputValue, float mutateChance, float minDeviationAmount, float maxDeviationAmount)
	{
		var outputValue = inputValue;

		if (MutationSuccessful(mutateChance)) {
			var deviationAmount = Random.Range(minDeviationAmount, maxDeviationAmount);

			outputValue += deviationAmount;
		}

		return outputValue;
	}

	public static int MutatePercentage(int inputValue, float mutateChance, float minDeviationPercent, float maxDeviationPercent)
	{
		var outputValue = inputValue;

		if (MutationSuccessful(mutateChance)) {
			var deviationAmount = Random.Range(minDeviationPercent, maxDeviationPercent);

			outputValue = (int)(outputValue * (1 + deviationAmount));
		}

		return outputValue;
	}
	public static int MutateFlat(int inputValue, float mutateChance, int minDeviationAmount, int maxDeviationAmount)
	{
		var outputValue = inputValue;

		if (MutationSuccessful(mutateChance)) {
			var deviationAmount = Random.Range(minDeviationAmount, maxDeviationAmount + 1);

			outputValue += deviationAmount;
		}

		return outputValue;
	}

	public static Color MutatePercentage(Color original, float mutateChance, float minDeviationPercent, float maxDeviationPercent)
	{
		var red = MutatePercentage(original.r, mutateChance, minDeviationPercent, maxDeviationPercent);
		var blue = MutatePercentage(original.b, mutateChance, minDeviationPercent, maxDeviationPercent);
		var green = MutatePercentage(original.g, mutateChance, minDeviationPercent, maxDeviationPercent);

		return new Color(red, green, blue);
	}
	public static Color MutateFlat(Color original, float mutateChance, float minDeviation, float maxDeviation)
	{
		var red = MutateFlat(original.r, mutateChance, minDeviation, maxDeviation);
		var blue = MutateFlat(original.b, mutateChance, minDeviation, maxDeviation);
		var green = MutateFlat(original.g, mutateChance, minDeviation, maxDeviation);

		return new Color(red, green, blue);
	}
    
	private static bool MutationSuccessful(float mutateChance)
	{
		return Random.Range(0, 1.0f) > (1 - mutateChance);
	}
}