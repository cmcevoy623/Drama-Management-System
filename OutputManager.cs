using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class OutputManager {

    // History Variables //
    public static List<int>         FiredPointHistory       = new List<int>();
    public static List<int>         FiredSpaceHistory       = new List<int>();
    public static List<float[]>     FiredWeightsHistory     = new List<float[]>();
    public static List<float[]>     OldPMWeightHistory      = new List<float[]>();
    public static List<float[]>     NewPMWeightHistory      = new List<float[]>();
    public static List<float[]>     OriginalFiredWeights    = new List<float[]>();
    public static List<float>       SimilarityToPMHistory   = new List<float>();
    public static List<float>       MaxSimToPMHistory       = new List<float>();

    public static List<float>       ProcessingTimeHistory   = new List<float>();

    public static List<List<int>>   NewActivePointHistory   = new List<List<int>>();
    public static List<List<int>>   NewClosedPointHistory   = new List<List<int>>();
    public static List<List<int>>   VoidToWorldSetHistory   = new List<List<int>>();
    public static List<List<int>>   WorldToVoidSetHistory   = new List<List<int>>();
    public static List<List<int>>   NewEndpointDependencies = new List<List<int>>();  // stored as Endpoint, Dependency, Endpoint, Dependency etc

    // A class for handling output to a screen overlay and a text file //
    static OutputToScreen screenOutputScript;


    static float beforeReplacementTime = 0.0f;
    static float afterReplacementTime = 0.0f;


    public static void StartTime(float time) { beforeReplacementTime = time; }
    public static void StopTime(float time)
    {
        afterReplacementTime = time;

        float pTime = afterReplacementTime - beforeReplacementTime;
        ProcessingTimeHistory.Insert(0, pTime);

        //if (ProcessingTimeHistory.Count == 50)
        //    PrintProcessingTimes();
    }

    static OutputManager()
    {
        screenOutputScript = Object.FindObjectOfType<OutputToScreen>();
    }

    public static void RefreshText()
    {
        screenOutputScript.RefreshPages();
    }

    public static void NewHistoryEntry()
    {
        // Create New Spaces to Insert Things //
        NewActivePointHistory.Insert(0, new List<int>());
        NewClosedPointHistory.Insert(0, new List<int>());
        VoidToWorldSetHistory.Insert(0, new List<int>());
        WorldToVoidSetHistory.Insert(0, new List<int>());
        NewEndpointDependencies.Insert(0, new List<int>());
    }

    // Print Processing Times //
    public static void PrintProcessingTimes()
    {
        StreamWriter textWriter = new StreamWriter("Assets/Scripts/Output/ProcessingTimes.txt");

        textWriter.WriteLine("Processing Times After Firing Points");

        float runningTotal = 0;
        for (int i = 0; i < ProcessingTimeHistory.Count; i++)
        {
            textWriter.WriteLine((i + 1).ToString() + ". " + ProcessingTimeHistory[i].ToString() + "s");
            runningTotal += ProcessingTimeHistory[i];
        }

        float averageTime = runningTotal / ProcessingTimeHistory.Count;
        textWriter.WriteLine("");
        textWriter.WriteLine("Average Time: " + averageTime.ToString() + "s");

        textWriter.Close();
    }

    // Print Database //
    public static void PrintEntireDataSet()
    {
        StreamWriter textWriter = new StreamWriter("Assets/Scripts/Output/Dataset.txt");

        textWriter.WriteLine("Entire Dataset at Initialisation");
        textWriter.WriteLine("");

        // Write Locations //
        textWriter.WriteLine("Room Indices");
        textWriter.WriteLine(" 0. Control Room          |  0- 1");
        textWriter.WriteLine(" 1. Sonar Room            |  2- 7");
        textWriter.WriteLine(" 2. Radio Room            |  8- 9");
        textWriter.WriteLine(" 3. CRT Room              | 10-11");
        textWriter.WriteLine(" 4. Recording Room        | 12-17");
        textWriter.WriteLine(" 5. Intelligence Room     | 18-21");
        textWriter.WriteLine(" 6. Equipment Room        | 22-23");
        textWriter.WriteLine(" 7. Maneuvering Room      | 24-25");
        textWriter.WriteLine(" 8. Recreation Room       | 26-27");
        textWriter.WriteLine(" 9. Engine Room           | 28-33");
        textWriter.WriteLine("10. Hovering Pumps Room   | 34-37");
        textWriter.WriteLine("11. Silo Control Room     | 38-39");
        textWriter.WriteLine("12. Battery Room          | 40-45");
        textWriter.WriteLine("13. Missile Launch Room   | 46-51");
        textWriter.WriteLine("14. Status Room           | 52-57");
        textWriter.WriteLine("15. Officer Berthing      | 58-61");
        textWriter.WriteLine("16. Captain Quarters      | 62-63");
        textWriter.WriteLine("");
        textWriter.WriteLine("");
        textWriter.WriteLine("");

        textWriter.WriteLine("Plot Point Information");
        textWriter.WriteLine("");

        // For Each Plot Point //
        for (int i = 0; i < DramaManager.DM_FullSet().Count; i++)
        {
            // Get Structure - Preconditions and Effects //
            List<List<int>> conds = DramaManager.DM_FullSet()[i].GetPreconditionLogic();
            List<int> effs = DramaManager.DM_FullSet()[i].GetEffects();

            // Preconditions //
            string conditionString = "";
            for (int j = 0; j < conds.Count; j++)
            {
                conditionString += "(";
                for (int k = 0; k < conds[j].Count; k++)
                {
                    conditionString += conds[j].ToString();
                    if (k < conds[j].Count - 1)
                        conditionString += " && ";
                }
                conditionString += ")";
                if (j < conds.Count - 1)
                    conditionString += " | ";
            }
            if (conditionString == "")
                conditionString = "No Conditions";

            // Effects //
            string effectsString = "";
            for (int j = 0; j < effs.Count; j++)
            {
                effectsString += effs[j].ToString();
                if (j < effs.Count - 1)
                    effectsString += ", ";
            }
            if (effectsString == "")
                effectsString = "No Effects";

            // Get Weights //
            string weightsStr = "";
            for (int j = 0; j < PageTextBank.GetPointInfoValElement(i).InfoWeights().Length; j++)
            {
                weightsStr += PageTextBank.GetPointInfoValElement(i).InfoWeights()[j].ToString("F2");
                if (j < PageTextBank.GetPointInfoValElement(i).InfoWeights().Length - 1)
                    weightsStr += ", ";
            }

            // Get Text //
            string pageText = PageTextBank.GetTextPage(i);
            pageText = pageText.Replace("\n", System.Environment.NewLine);

            // Get Possible Locations //
            string locationsText = "";
            List<ElementSpace> pointSpaces = PageTextBank.GetPointInfoValElement(i).GetPossibleSpaces();
            for (int j = 0; j < pointSpaces.Count; j++)
            {
                locationsText += pointSpaces[j].SpaceID.ToString();
                if (j < pointSpaces.Count - 1)
                    locationsText += ", ";
            }
            if (locationsText == "")
                locationsText = "No Locations";


            // Write Plot Info //
            textWriter.WriteLine("Plot Point ID: " + i.ToString());
            textWriter.WriteLine("Conditions: " + conditionString);
            textWriter.WriteLine("Effects: " + effectsString);
            textWriter.WriteLine("Weights: " + weightsStr);
            textWriter.WriteLine("Accessible From Locations: " + locationsText);
            textWriter.WriteLine("");
            textWriter.WriteLine("Narrative Text: ");
            textWriter.WriteLine(pageText);
            textWriter.WriteLine("");
            textWriter.WriteLine("");
            textWriter.WriteLine("");
        }

        textWriter.WriteLine("");
        textWriter.WriteLine("");
        textWriter.WriteLine("");

        // Get Endpoints //
        textWriter.WriteLine("EndPoint IDs:");
        textWriter.WriteLine("");
        List<int> endPoints = DramaManager.DM_EndPoints();
        for (int i = 0; i < endPoints.Count; i++)
            textWriter.WriteLine("EndID: " + endPoints[i].ToString());

        textWriter.Close();
    }

    public static void PrintToFile(int endID)
    {
        OutputToScreen screenLog = Object.FindObjectOfType<OutputToScreen>();

        // Print All Data To File - could get text of OutputToScreen //
        StreamWriter textWriter = new StreamWriter("Assets/Scripts/Output/Output_ID" + endID.ToString() + "_PathLength" + FiredPointHistory.Count.ToString() + ".txt");

        // Fired Point History //
        string firedPointHistory = screenLog.WeightChangeHistoryData[0].text;
        firedPointHistory = firedPointHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Fired Point History:");
        textWriter.WriteLine(firedPointHistory);
        textWriter.WriteLine("");

        // Fired Space History //
        string firedSpaceHistory = screenLog.WeightChangeHistoryData[5].text;
        firedSpaceHistory = firedSpaceHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Fired from Space History:");
        textWriter.WriteLine(firedSpaceHistory);
        textWriter.WriteLine("");

        // Original Weights //
        string originalWeightHistory = screenLog.WeightChangeHistoryData[4].text;
        originalWeightHistory = originalWeightHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Original Weights:");
        textWriter.WriteLine(originalWeightHistory);
        textWriter.WriteLine("");

        // Fired Weights //
        string firedWeightHistory = screenLog.WeightChangeHistoryData[1].text;
        firedWeightHistory = firedWeightHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Fired Weights:");
        textWriter.WriteLine(firedWeightHistory);
        textWriter.WriteLine("");

        // Old PM Weights //
        string oldPMWeightHistory = screenLog.WeightChangeHistoryData[2].text;
        oldPMWeightHistory = oldPMWeightHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Old PM Weights:");
        textWriter.WriteLine(oldPMWeightHistory);
        textWriter.WriteLine("");

        // New PM Weights //
        string newPMWeightHistory = screenLog.WeightChangeHistoryData[3].text;
        newPMWeightHistory = newPMWeightHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("New PM Weights:");
        textWriter.WriteLine(newPMWeightHistory);
        textWriter.WriteLine("");

        // Similarity to PM //
        string pmSimilarityHistory = screenLog.WeightChangeHistoryData[6].text;
        pmSimilarityHistory = pmSimilarityHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Similarity to PM:");
        textWriter.WriteLine(pmSimilarityHistory);
        textWriter.WriteLine("");

        // Maximum Similarity //
        string pmMaxSimilarityHistory = screenLog.WeightChangeHistoryData[7].text;
        pmMaxSimilarityHistory = pmMaxSimilarityHistory.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("Max Similarity to PM:");
        textWriter.WriteLine(pmMaxSimilarityHistory);
        textWriter.WriteLine("");

        // True Similarity Difference //
        string difference;
        textWriter.WriteLine("True PM Similarity Difference:");
        for (int i = 0; i < MaxSimToPMHistory.Count; i++)
        {
            difference = (MaxSimToPMHistory[i] - SimilarityToPMHistory[i]).ToString();
            textWriter.WriteLine(difference);
        }
        textWriter.WriteLine("");

        // New Active Set Points //
        string newActiveSet = screenLog.SetChangeHistoryData[1].text;
        newActiveSet = newActiveSet.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("New Active Set Points:");
        textWriter.WriteLine(newActiveSet);
        textWriter.WriteLine("");

        // New Closed Set Points //
        string newClosedSet = screenLog.SetChangeHistoryData[2].text;
        newClosedSet = newClosedSet.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("New Closed Set Points:");
        textWriter.WriteLine(newClosedSet);
        textWriter.WriteLine("");

        // !W-Set Points Moved to W-Set //
        string movedVoidSet = screenLog.WorldSetChangeHistoryData[1].text;
        movedVoidSet = movedVoidSet.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("!W-Set Points Moved to W-Set:");
        textWriter.WriteLine(movedVoidSet);
        textWriter.WriteLine("");

        // W-Set Points Moved to !W-Set //
        string movedWorldSet = screenLog.WorldSetChangeHistoryData[2].text;
        movedWorldSet = movedWorldSet.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("W-Set Points Moved to !W-Set:");
        textWriter.WriteLine(movedWorldSet);
        textWriter.WriteLine("");

        // New Endpoint Dependencies //
        string newEndpointDependencies = screenLog.NewEndpointDependencyData[1].text;
        newEndpointDependencies = newEndpointDependencies.Replace("\n", System.Environment.NewLine);

        textWriter.WriteLine("New Endpoint Dependencies (Endpoint, Dependency):");
        textWriter.WriteLine(newEndpointDependencies);
        textWriter.WriteLine("");

        float runningTotal = 0;
        for (int i = 0; i < ProcessingTimeHistory.Count; i++)
        {
            textWriter.WriteLine((i + 1).ToString() + ". " + ProcessingTimeHistory[i].ToString() + "s");
            runningTotal += ProcessingTimeHistory[i];
        }

        textWriter.WriteLine(""); textWriter.WriteLine("");

        // Endpoint Reached //
        // - Endpoint ID
        textWriter.WriteLine("Endpoint ID: " + endID.ToString());
        // - Path Length
        textWriter.WriteLine("Path Length: " + FiredPointHistory.Count.ToString());
        // - Final Similarity to PM (before updating PM)
        textWriter.WriteLine("End Similarity to PM: " + SimilarityToPMHistory[0].ToString());
        textWriter.WriteLine("End Max Similarity to PM: " + MaxSimToPMHistory[0].ToString());
        float averageTime = runningTotal / ProcessingTimeHistory.Count;
        textWriter.WriteLine("Average Processing Time: " + averageTime.ToString() + "s");
        textWriter.WriteLine("");

        // Undiscovered Set //
        string undiscoveredSet = screenLog.CurrentSetsData[0].text;
        textWriter.WriteLine("Undiscovered Set:");
        textWriter.WriteLine(undiscoveredSet);
        textWriter.WriteLine("Set Size: " + DramaManager.DM_UndiscoveredSet().Count.ToString());
        textWriter.WriteLine("");

        // Closed Set //
        string closedSet = screenLog.CurrentSetsData[1].text;
        textWriter.WriteLine("Closed Set:");
        textWriter.WriteLine(closedSet);
        textWriter.WriteLine("Set Size: " + DramaManager.DM_ClosedSet().Count.ToString());
        textWriter.WriteLine("");

        // W-Set //
        string worldSet = screenLog.CurrentSetsData[2].text;
        textWriter.WriteLine("W-Set:");
        textWriter.WriteLine(worldSet);
        textWriter.WriteLine("Set Size: " + InformationEnactor.WorldSet().Count.ToString());
        textWriter.WriteLine("");

        // !W-Set //
        string voidSet = screenLog.CurrentSetsData[3].text;
        textWriter.WriteLine("!W-Set:");
        textWriter.WriteLine(voidSet);
        textWriter.WriteLine("Set Size: " + InformationEnactor.VoidSet().Count.ToString());
        textWriter.WriteLine("");

        // Done //
        textWriter.Close();
    }
}
