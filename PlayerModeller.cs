using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerModeller {

    public enum PLAYSTYLE { LISTENER, EXPLORER, RESEARCHER, SOLVER };
    static float[] playerWeights = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };

	// Getters and Setters //
    public static float[] GetPlaystyleWeights() { return playerWeights; }
    public static void SetPlaystyleWeights(float[] weights) { playerWeights = weights; }

    // Update Player Values //
    public static void AdjustPlayerWeights(float[] adjWs)
    {
        playerWeights = AdjustWeights(playerWeights, adjWs);
    }

    // Updating Set Info //
    public static float[] AdjustWeights(float[] curWs, float[] adjWs)
    {
        // New Weight Values //
        float[] adjustedWeights = curWs;

        // Calculation Variables //
        float mid = 0.5f;       // halfway point for comparison
        float a_cVal = 0.25f;   // how much influence the difference in adjusting and current weights has
        float a_mVal = 0.125f;  // how much influence the difference in adjusting weight and mid value has

        // For Each Weight to Adjust //
        for (int i = 0; i < curWs.Length; i++)
        {
            // (tw - aw) * x + (tw - m) * y // Clamp between 0 and 1 //
            adjustedWeights[i] += (adjWs[i] - curWs[i]) * a_cVal + (adjWs[i] - mid) * a_mVal;
            adjustedWeights[i] = Mathf.Clamp(adjustedWeights[i], 0.0f, 1.0f);
        }

        /* The Calculation Provides No Increase for an Adjusting Weight value of 0.5,
           There is a small increase or decrease if the adjusting weight is equal to the current weight,
           There is a larger shift in weight when the adjusting and current weights are farther apart,
           especially if on opposite sides of the midpoint,
           The scale values can be adjusted as needed. */

        return adjustedWeights;
    }

    // Make Gap Between A Set of Weights Closer //
    public static float[] TightenWeightDistances(float[] currentWeights, float[] targetWeights, float tightenAmount)
    {
        float tightenFactor = 2.5f; // how much influence does similarity have on the lerp factor - bigger the less the jump, 1 is normal
        tightenAmount /= tightenFactor;

        float[] adjustedWeights = currentWeights;

        // Adjust Weights to Move Next Point's Weights Towards Newly-Adjusted Player Weights //
        for (int i = 0; i < targetWeights.Length; i++)
            adjustedWeights[i] = Mathf.Lerp(currentWeights[i], targetWeights[i], tightenAmount);

        return adjustedWeights;
    }

    // Returns Higher With Values Closer to Each Other - Weighted so That Dominent Classes Have More Pull //
    public static float CalculateSimilarity(float[] inputWeights, float[] comparisonWeights)
    {
        float similarity = 0.0f;

        // Add to Similatity Measure with Each Class Comparison - weighted by higher scoring classes - NOTE: May Change //
        for (int i = 0; i < inputWeights.Length; i++)
            similarity += inputWeights[i] * (1 - Mathf.Abs(inputWeights[i] - comparisonWeights[i]));

        similarity /= inputWeights.Length;  //ensure between 0 and 1

        return similarity;
    }
}
