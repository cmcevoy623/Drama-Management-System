using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputToScreen : MonoBehaviour {

    // This is likely to be a messy class with many public variables - Will be stored as a prefab //

    // the currently shown tab, -1 denotes showing nothing
    int overlayNum = -1;

    // The Tab objects themselves - to swap between //
    public GameObject[] DebuggingOverlays;

    ///// Each Tab's Text Objects for Displaying Data /////

    public Text[] WeightChangeHistoryData;          // Fired ID  Fired Weights   Old PM Weights  New PM Weights  Original Fired Weights  Space ID  PM Similarity //
    public Text[] SetChangeHistoryData;             // Fired ID  Newly Active Set Points    Newly Closed Set Points //
    public Text[] WorldSetChangeHistoryData;        // Fired ID  !W-points moved to W-set   W-points moved to !W-set //
    public Text[] CurrentSetsData;                  // Current Undiscovered Set     Current Closed Set  Current W-Set   Current !W-set //
    public Text[] NewEndpointDependencyData;        // Fired ID     New Endpoint Dependency (pair Endpoint ID and Dependency ID within brackets) //
    public Text[] CurrentEndpointDependencyData;    // Unclosed Endpoint ID     Endpoint Dependencies for that point //

    public void RefreshPages()
    {
        // Get Variables from OutputManager and Assign them to the appropriate text boxes, converting to a readable format //
        /*
            FiredPointHistory                           -   WeightChangeHistoryData[0], SetChangeHistoryData[0], 
                                                            WorldSetChangeHistoryData[0], NewEndpointDependencyData[0]
            FiredWeightsHistory                         -   WeightChangeHistoryData[1]
            OldPMWeightHistory                          -   WeightChangeHistoryData[2]
            NewPMWeightHistory                          -   WeightChangeHistoryData[3]
            OriginalFiredWeights                        -   WeightChangeHistoryData[4]
            
            NewActivePointHistory                       -   SetChangeHistoryData[1]
            NewClosedPointHistory                       -   SetChangeHistoryData[2]

            VoidToWorldSetHistory                       -   WorldSetChangeHistoryData[1]
            WorldToVoidSetHistory                       -   WorldSetChangeHistoryData[2]

            DramaManager.UndiscoveredSet                -   CurrentSetsData[0]    
            DramaManager.ClosedSet                      -   CurrentSetsData[1]
            InformationEnactor.WorldSet                 -   CurrentSetsData[2]
            InformationEnactor.VoidSet                  -   CurrentSetsData[3]
               
            NewEndpointDependencies                     -   NewEndpointDependencyData[1]

            DramaManager.Endpoints (check not closed)   -   CurrentEndpointDependencyData[0]
            UnclosedEndpoints (get all dependencies)       -   CurrentEndpointDependencyData[1]
         */

        ClearOverlayText();

        // FiredPointHistory // 
        string firedIDHistoryStr = "";
        for (int i = 0; i < OutputManager.FiredPointHistory.Count; i++)
            { firedIDHistoryStr += OutputManager.FiredPointHistory[i].ToString() + "\n"; }
        WeightChangeHistoryData[0].text     = firedIDHistoryStr;
        SetChangeHistoryData[0].text        = firedIDHistoryStr;
        WorldSetChangeHistoryData[0].text   = firedIDHistoryStr;
        NewEndpointDependencyData[0].text   = firedIDHistoryStr;



        // FiredWeightsHistory //
        string firedWeightHistoryStr = "";
        for (int i = 0; i < OutputManager.FiredWeightsHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.FiredWeightsHistory[i].Length; j++)
            {
                firedWeightHistoryStr += OutputManager.FiredWeightsHistory[i][j].ToString("F2") + ", ";
            }
            firedWeightHistoryStr += "\n";
        }
        WeightChangeHistoryData[1].text = firedWeightHistoryStr;



        // OldPMWeightHistory //
        string oldPMWeightHistoryStr = "";
        for (int i = 0; i < OutputManager.OldPMWeightHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.OldPMWeightHistory[i].Length; j++)
            {
                oldPMWeightHistoryStr += OutputManager.OldPMWeightHistory[i][j].ToString("F2") + ", ";
            }
            oldPMWeightHistoryStr += "\n";
        }
        WeightChangeHistoryData[2].text = oldPMWeightHistoryStr;



        // NewPMWeightHistory //
        string newMWeightHistoryStr = "";
        for (int i = 0; i < OutputManager.NewPMWeightHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.NewPMWeightHistory[i].Length; j++)
            {
                newMWeightHistoryStr += OutputManager.NewPMWeightHistory[i][j].ToString("F2") + ", ";
            }
            newMWeightHistoryStr += "\n";
        }
        WeightChangeHistoryData[3].text = newMWeightHistoryStr;



        // OriginalFiredWeights //
        string originalFiredWeightHistoryStr = "";
        for (int i = 0; i < OutputManager.OriginalFiredWeights.Count; i++)
        {
            for (int j = 0; j < OutputManager.OriginalFiredWeights[i].Length; j++)
            {
                originalFiredWeightHistoryStr += OutputManager.OriginalFiredWeights[i][j].ToString("F2") + ", ";
            }
            originalFiredWeightHistoryStr += "\n";
        }
        WeightChangeHistoryData[4].text = originalFiredWeightHistoryStr;


        // Space ID History //
        string spaceIDHistoryStr = "";
        for (int i = 0; i < OutputManager.FiredSpaceHistory.Count; i++)
            { spaceIDHistoryStr += OutputManager.FiredSpaceHistory[i].ToString() + "\n"; }
        WeightChangeHistoryData[5].text     = spaceIDHistoryStr;


        // PM Similarity History //
        string pmSimilarityHistoryStr = "";
        for (int i = 0; i < OutputManager.SimilarityToPMHistory.Count; i++)
            { pmSimilarityHistoryStr += OutputManager.SimilarityToPMHistory[i].ToString() + "\n"; }
        WeightChangeHistoryData[6].text = pmSimilarityHistoryStr;


        // Max PM Similarity History //
        string pmMaxSimilarityHistoryStr = "";
        for (int i = 0; i < OutputManager.MaxSimToPMHistory.Count; i++)
        { pmMaxSimilarityHistoryStr += OutputManager.MaxSimToPMHistory[i].ToString() + "\n"; }
        WeightChangeHistoryData[7].text = pmMaxSimilarityHistoryStr;


        // NewActivePointHistory //
        string newActivePointHistoryStr = "";
        for (int i = 0; i < OutputManager.NewActivePointHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.NewActivePointHistory[i].Count; j++)
            {
                newActivePointHistoryStr += OutputManager.NewActivePointHistory[i][j].ToString() + ", ";
            }
            newActivePointHistoryStr += "\n";
        }
        SetChangeHistoryData[1].text = newActivePointHistoryStr;



        // NewClosedPointHistory //
        string newClosedPointHistoryStr = "";
        for (int i = 0; i < OutputManager.NewClosedPointHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.NewClosedPointHistory[i].Count; j++)
            {
                newClosedPointHistoryStr += OutputManager.NewClosedPointHistory[i][j].ToString() + ", ";
            }
            newClosedPointHistoryStr += "\n";
        }
        SetChangeHistoryData[2].text = newClosedPointHistoryStr;



        // VoidToWorldSetHistory //
        string voidToWorldHistoryStr = "";
        for (int i = 0; i < OutputManager.VoidToWorldSetHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.VoidToWorldSetHistory[i].Count; j++)
            {
                voidToWorldHistoryStr += OutputManager.VoidToWorldSetHistory[i][j].ToString() + ", ";
            }
            voidToWorldHistoryStr += "\n";
        }
        WorldSetChangeHistoryData[1].text = voidToWorldHistoryStr;



        // WorldToVoidSetHistory //
        string worldToVoidHistoryStr = "";
        for (int i = 0; i < OutputManager.WorldToVoidSetHistory.Count; i++)
        {
            for (int j = 0; j < OutputManager.WorldToVoidSetHistory[i].Count; j++)
            {
                worldToVoidHistoryStr += OutputManager.WorldToVoidSetHistory[i][j].ToString() + ", ";
            }
            worldToVoidHistoryStr += "\n";
        }
        WorldSetChangeHistoryData[2].text = worldToVoidHistoryStr;



        // Current Sets //
        string currentUndiscoveredStr = "";
        for (int i = 0; i < DramaManager.DM_UndiscoveredSet().Count; i++) { currentUndiscoveredStr += DramaManager.DM_UndiscoveredSet()[i].ToString() + ", "; }
        CurrentSetsData[0].text = currentUndiscoveredStr;

        string currentClosedSetStr = "";
        for (int i = 0; i < DramaManager.DM_ClosedSet().Count; i++) { currentClosedSetStr += DramaManager.DM_ClosedSet()[i].ToString() + ", "; }
        CurrentSetsData[1].text = currentClosedSetStr;

        string currentWorldSetStr = "";
        for (int i = 0; i < InformationEnactor.WorldSet().Count; i++) { currentWorldSetStr += InformationEnactor.WorldSet()[i].PlotPointIndex.ToString() + ", "; }
        CurrentSetsData[2].text = currentWorldSetStr;

        string currentVoidSetStr = "";
        for (int i = 0; i < InformationEnactor.VoidSet().Count; i++) { currentVoidSetStr += InformationEnactor.VoidSet()[i].PlotPointIndex.ToString() + ", "; }
        CurrentSetsData[3].text = currentVoidSetStr; 



        // NewEndpointDependencies //
        string newEndpointDependenciesStr = "";
        for (int i = 0; i < OutputManager.NewEndpointDependencies.Count; i++)
        {
            for (int j = 0; j < OutputManager.NewEndpointDependencies[i].Count; j+=2)
            {
                newEndpointDependenciesStr += "(" + OutputManager.NewEndpointDependencies[i][j].ToString() + ", " + OutputManager.NewEndpointDependencies[i][j + 1].ToString() + "), ";
            }
            newEndpointDependenciesStr += "\n";
        }
        NewEndpointDependencyData[1].text = newEndpointDependenciesStr;



        // Current Unclosed Endpoint Dependencies //
        string currentUnclosedEndpointsStr = ""; 
        string currentDependenciesStr = "";
        for (int i = 0; i < DramaManager.DM_EndPoints().Count; i++)
        {
            if (DramaManager.GetIsClosed(DramaManager.DM_EndPoints()[i]) == false)
            {
                currentUnclosedEndpointsStr += DramaManager.DM_EndPoints()[i].ToString();
                for (int j = 0; j < DramaManager.DM_FullSet()[DramaManager.DM_EndPoints()[i]].GetTurnOffEndPoints().Count; j++)
                {
                    if (DramaManager.GetIsClosed(DramaManager.DM_FullSet()[DramaManager.DM_EndPoints()[i]].GetTurnOffEndPoints()[j]) == false)
                        currentDependenciesStr += DramaManager.DM_FullSet()[DramaManager.DM_EndPoints()[i]].GetTurnOffEndPoints()[j].ToString() + ", ";
                }
                currentDependenciesStr += "\n";
                currentUnclosedEndpointsStr += "\n";
            }
        }
        CurrentEndpointDependencyData[0].text = currentUnclosedEndpointsStr;
        CurrentEndpointDependencyData[1].text = currentDependenciesStr;
    }

    void ClearOverlayText()
    {
        for (int i = 0; i < WeightChangeHistoryData.Length; i++) { WeightChangeHistoryData[i].text = ""; }
        for (int i = 0; i < SetChangeHistoryData.Length; i++) { SetChangeHistoryData[i].text = ""; }
        for (int i = 0; i < WorldSetChangeHistoryData.Length; i++) { WorldSetChangeHistoryData[i].text = ""; }
        for (int i = 0; i < CurrentSetsData.Length; i++) { CurrentSetsData[i].text = ""; }
        for (int i = 0; i < NewEndpointDependencyData.Length; i++) { NewEndpointDependencyData[i].text = ""; }
        for (int i = 0; i < CurrentEndpointDependencyData.Length; i++) { CurrentEndpointDependencyData[i].text = ""; }
    }

    // Use this for initialization
    void Start ()
    {
        // Clear Test Text at Beginning //
        ClearOverlayText();
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Display Debug Information //
        if (Input.GetButtonDown("Tab"))
        {
            // Hide previous tab //
            if (overlayNum >= 0)
                DebuggingOverlays[overlayNum].SetActive(false);

            overlayNum++;

            // Show new tab //
            if (overlayNum >= DebuggingOverlays.Length)
                overlayNum = -1;
            else
                DebuggingOverlays[overlayNum].SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.P))
            OutputManager.PrintToFile(-1);
	}
}
