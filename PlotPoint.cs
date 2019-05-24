using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotPoint : System.Object {

    public enum PLOTPOINTTYPE { TEXTPAGE, RADIO, CASSETTE, PLAYERACTION, EVENT };
    PLOTPOINTTYPE pointType = PLOTPOINTTYPE.TEXTPAGE;

    // Condition Check List //
    List<List<int>> preconditionLogic;
    List<int> effectsPlotPoints;                // plot points to turn off after firing the effect - add to closed set
    List<int> nextPoints = new List<int>();     // plot points to add to openSet when this point is added to the active set

    // Next Point Traceback - Off Switches for this PlotPoint //
    List<int> offSwitches = new List<int>();    // only matters for points that traceback from endpoint

    // Connected EndPoints - Which EndPoints this PlotPoint Eventually Connects to //
    List<int> connectedEndPoints = new List<int>();
    List<int> endPointDependencies = new List<int>();

    // ID and Fired State //
    int pID = -1;                           // Plot Point ID for accessing structure
    int iID = -1;                           // Bank ID (use pointType with this) to connect to information bank with weights and info
    bool isFired = false;                   // has been accessed in the world

    // Logic //
    bool isSatisfied = false;
    bool isEnd = false;                     // Has the player reached a last plot point? - NOTE: can set this depending on variation etc - Later
    bool isInWorld = false;

    // Constructors //
    public PlotPoint()
    {
        pointType = PLOTPOINTTYPE.TEXTPAGE;
        preconditionLogic = new List<List<int>>();
        effectsPlotPoints = new List<int>();
        DramaManager.pID++;
        pID = DramaManager.pID;
    }

    public PlotPoint(List<List<int>> preCond)
    {
        preconditionLogic = new List<List<int>>(preCond);
        effectsPlotPoints = new List<int>();
        DramaManager.pID++;
        pID = DramaManager.pID;
    }

    public PlotPoint(List<int> effs)
    {
        preconditionLogic = new List<List<int>>();
        effectsPlotPoints = new List<int>(effs);
        DramaManager.pID++;
        pID = DramaManager.pID;
    }

    public PlotPoint(List<List<int>> preCond, List<int> effs)
    {
        preconditionLogic = new List<List<int>>(preCond);
        effectsPlotPoints = new List<int>(effs);
        DramaManager.pID++;
        pID = DramaManager.pID;
    }

    public PlotPoint(List<List<int>> preCond, bool end)
    {
        preconditionLogic = new List<List<int>>(preCond);
        effectsPlotPoints = new List<int>();
        isEnd = end;
        DramaManager.pID++;
        pID = DramaManager.pID;

        if (isEnd)
            DramaManager.AddEndPoint(this);
    }

    public PlotPoint(List<List<int>> preCond, List<int> effs, bool end)
    {
        preconditionLogic = new List<List<int>>(preCond);
        effectsPlotPoints = new List<int>(effs);
        isEnd = end;
        DramaManager.pID++;
        pID = DramaManager.pID;

        if (isEnd)
            DramaManager.AddEndPoint(this);
    }

    // Getters and Setters //
    public void SetPointInfo(PLOTPOINTTYPE infoType, int infoID)
    {
        pointType = infoType;
        iID = infoID;
    }

    public PLOTPOINTTYPE GetPointType() { return pointType; }
    public int GetID() { return pID; }
    public void SetInfoID(int id) { iID = id; }
    public int GetInfoID() { return iID; }

    public bool GetIsFired() { return isFired; }
    public void SetIsFired(bool b) { isFired = b; }
    public bool GetIsSatisfied() { return isSatisfied; }
    public void SetIsSatisfied(bool b) { isSatisfied = b; }

    public bool GetIsInWorld() { return isInWorld; }
    public void SetIsInWorld(bool b) { isInWorld = b; }

    public bool GetIsEndpoint() { return isEnd; }
    public void SetIsEndpoint(bool b) { isEnd = b; if (isEnd == true) DramaManager.AddEndPoint(this); }

    public List<List<int>> GetPreconditionLogic() { return preconditionLogic; }
    public void SetPreconditionLogic(List<List<int>> preLogic) { preconditionLogic = new List<List<int>>(preLogic); }
    public List<int> GetEffects() { return effectsPlotPoints; }
    public void SetEffects(List<int> effs) { effectsPlotPoints = new List<int>(effs); }

    public void AddNextPoint(int next) { nextPoints.Add(next); }
    public List<int> GetNextPoints() { return nextPoints; }

    public void AddOffSwitch(int off) { if (offSwitches.Contains(off) == false) { offSwitches.Add(off); } }
    public List<int> GetOffSwitches() { return offSwitches; }

    public void AddEndPointConnection(int end) { if (connectedEndPoints.Contains(end) == false) { connectedEndPoints.Add(end); } }
    public List<int> GetEndPointConnections() { return connectedEndPoints; }
    public void AddEndPointDependency(int end) { endPointDependencies.Add(end); }
    public List<int> GetEndPointDependencies() { return endPointDependencies; }

    // List of EndPoint Dependencies for the Endpoints - reverse of endpoint dependencies above //
    List<int> pointsThatTurnOffThisEndPoint = new List<int>();
    public void AddTurnOffEndPoint(int off) { if (pointsThatTurnOffThisEndPoint.Contains(off) == false) { pointsThatTurnOffThisEndPoint.Add(off); } }
    public List<int> GetTurnOffEndPoints() { return pointsThatTurnOffThisEndPoint; }



    // Test if Preconditions are Satisfied //
    bool ConditionSatisfied()
    {
        // Error Handle List Size //
        if (preconditionLogic.Count <= 0)
        {
            Debug.Log("ID: " + pID + " contains no conditions");
            return true;
        }
        else if (preconditionLogic[0].Count <= 0)
        {
            Debug.Log("Precondition Plot Point List is Empty");
            return false;
        }

        // Cycle through ORs //
        bool logicORSatisfied = false;
        for (int i = 0; i < preconditionLogic.Count; i++)
        {
            // Cycle through ANDs //
            bool logicANDSatisfied = true;
            for (int j = 0; j < preconditionLogic[i].Count; j++)
            {
                // AND together current logic and the fired bool of the preconditions //
                logicANDSatisfied = logicANDSatisfied && DramaManager.DM_FullSet()[preconditionLogic[i][j]].isFired;
            }

            // Update Running Logic //
            logicORSatisfied = logicORSatisfied | logicANDSatisfied;
        }

        // Return Final Logic Output //
        return logicORSatisfied;
    }

    // Can A Plot Point Ever Be Satisfied Given Precondition Logic and Current Bool States of Preconditions //
    public bool CanBeSatisfied()
    {
        /* This is similar to the above function (may need to improve conventions and condense, 
           This function tests if a plot point can ever be satisfied by assuming all future conditions are met.
           Thus, this function returns false if closed set preconditions were never fired and if that results in
           the requirements being impossible to fulfill */

        // Cycle through ORs //
        bool logicORSatisfied = false;
        for (int i = 0; i < preconditionLogic.Count; i++)
        {
            // Cycle through ANDs //
            bool logicANDSatisfied = true;
            for (int j = 0; j < preconditionLogic[i].Count; j++)
            {
                //logicANDSatisfied = logicANDSatisfied && preconditionLogic[i][j].GetIsSatisfied();
                if (DramaManager.IsInClosedSet(preconditionLogic[i][j]))
                    logicANDSatisfied = logicANDSatisfied && DramaManager.DM_FullSet()[preconditionLogic[i][j]].isFired; // is true if closed set point was fired
                else
                    logicANDSatisfied = logicANDSatisfied && true;  // Assume an unseen point has been fired
            }

            // Update Running Logic //
            logicORSatisfied = logicORSatisfied | logicANDSatisfied;
        }

        // Return Final Logic Output //
        return logicORSatisfied;
    }

    // Can A Plot Point Ever Be Satisfied Given Precondition Logic and Current Bool States of Preconditions //
    public bool CanBeSatisfied(int queryCondID)
    {
        /* This is a check if turning off a precondition will prevent a plotpoint from being possible */

        // Cycle through ORs //
        bool logicORSatisfied = false;
        for (int i = 0; i < preconditionLogic.Count; i++)
        {
            // Cycle through ANDs //
            bool logicANDSatisfied = true;
            for (int j = 0; j < preconditionLogic[i].Count; j++)
            {
                //logicANDSatisfied = logicANDSatisfied && preconditionLogic[i][j].GetIsSatisfied();
                if (DramaManager.IsInClosedSet(preconditionLogic[i][j]))
                    logicANDSatisfied = logicANDSatisfied && DramaManager.DM_FullSet()[preconditionLogic[i][j]].isFired; // is true if closed set point was fired
                else
                {
                    // Check if precondition is the one passed in //
                    if (preconditionLogic[i][j] == queryCondID)
                        logicANDSatisfied = false;
                    else
                        logicANDSatisfied = logicANDSatisfied && true;  // Assume an unseen point has been fired
                }
            }

            // Update Running Logic //
            logicORSatisfied = logicORSatisfied | logicANDSatisfied;
        }

        // Return Final Logic Output //
        return logicORSatisfied;
    }

    public bool InitSatisfied()
    {
        // Satisfied if no conditions //
        if (preconditionLogic.Count <= 0)
            isSatisfied = true;

        return isSatisfied;
    }

    public bool UpdateSatisfied()
    {
        // Update satisfied bool //
        isSatisfied = ConditionSatisfied();

        return isSatisfied;
    }

    public void FirePlotPoint()
    {
        // Turn off all effect plot points and move to DM closed set //
        for (int i = 0; i < effectsPlotPoints.Count; i++)
        {
            // Close Point With No Satisfaction //
            DramaManager.ClosePoint(effectsPlotPoints[i], false);
            DramaManager.UpdateEndpointDependencies(pID);
        }

        // Fire this point //
        isFired = true;

        // Close Point With it Satisfied //
        DramaManager.ClosePoint(pID, true);

        // Update Active Set Via DM - and close point while still satisfied //
        DramaManager.UpdateSets(pID);
        //DramaManager.UpdateSets();
    }
}
