using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DramaManager {

    // Different Sets //
    static List<PlotPoint> fullSet;                                     // all plotpoints
    static List<int> undiscoveredSet;                             // all points not discovered yet (for efficiency)
    //static List<int> openSet = new List<int>();             // all possibly-accessable plotpoints
    static List<int> closedSet = new List<int>();           // all resolved/inaccessable plotpoints
    static List<int> activeSet = new List<int>();           // all active/immediately-accessable plotpoints i.e. next turn

    // All EndPoints //
    static List<int> endPoints = new List<int>();

    // Newly Activated Points //
    static List<int> newlyActivatedIndices = new List<int>();           // To Tell the Enactor to Create New !W-set Elements for Replacement Algorithms

    // Global ID to Increment Each Plot Point with a Unique Number //
    public static int pID = -1;

    // Getters and Setters //
    public static List<PlotPoint> DM_FullSet() { return fullSet; }
    public static List<int> DM_UndiscoveredSet() { return undiscoveredSet; }
    public static List<int> DM_ClosedSet() { return closedSet; }
    public static List<int> DM_ActiveSet() { return activeSet; }
    public static List<int> DM_EndPoints() { return endPoints; }
    public static void AddEndPoint(PlotPoint ePoint) { endPoints.Add(ePoint.GetID()); }

    // Validity and Set Checks //
    public static bool CheckIDValid(int queryID) { return queryID >= 0 && queryID < fullSet.Count; }
    public static bool GetIsClosed(int queryID)
    {
        // Is this point closed? //
        if (CheckIDValid(queryID))
            return closedSet.Contains(queryID);

        return false;
    }

    // Check if in Set //
    public static bool IsInClosedSet(int queryId)
    {
        if (queryId < fullSet.Count) // NOTE: duplicate test done already in earlier function - remove later
            return closedSet.Contains(queryId);
        else
        {
            Debug.Log("ID: " + queryId + " not in fullSet, cannot find in closed set");
            return false;
        }
    }

    public static void TestData()
    {
        // test stuff
        //int[] testData = new int[] { 0, 1, 4, 5, 6 };

        //for (int i = 0; i < testData.Length; i++)
        //{
        //    if (activeSet.Contains(fullSet[testData[i]]))
        //    {
        //        fullSet[testData[i]].FirePlotPoint();
        //    }
        //    else
        //    {
        //        Debug.Log("ID: " + testData[i] + " is not in active set, cannot be fired");
        //    }
        //    Debug.Log(activeSet);
        //    //Debug.Log(openSet);
        //    //Debug.Log(closedSet);
        //    //Debug.Log(undiscoveredSet);
        //}




        // endpoint testing //
        //List<int> offEnds = fullSet[5].GetTurnOffEndPoints();
        ////Debug.Log(offEnds);

        //int[] testData = new int[] { 2, 6 };

        //for (int i = 0; i < testData.Length; i++)
        //{
        //    if (activeSet.Contains(fullSet[testData[i]]))
        //        fullSet[testData[i]].FirePlotPoint();
        //}



        // Testing Updated Endpoint Dependency Logic //
        //offEnds = fullSet[10].GetTurnOffEndPoints();
        ////Debug.Log(offEnds);
        //ClosePoint(7, false);
        //UpdateEndpointDependencies(7);
        //offEnds = fullSet[10].GetTurnOffEndPoints();
        //Debug.Log(offEnds);



        // Testing PM Calculations - Just comparing to paper - Step Through //
        // Adjusting Weights e.g. PM weights //
        //float[] testWeightAdj = PlayerModeller.AdjustWeights(new float[] { 0.6f, 0.9f, 0.4f, 0.5f, 0.7f, 0.725f, 0.8f, 0.1f, 0.3f, 0.5f }, 
        //                                                     new float[] { 0.9f, 0.6f, 0.1f, 0.5f, 0.7f, 0.7f, 0.2f, 0.5f, 0.7f, 0.9f });

        // Biased Similarity Calculation - output maximum sim, actual similarity and difference to maximum
        //float[] pmWeights = new float[] { 0.6f, 0.9f, 0.3f, 0.5f };
        //float maxSim = PlayerModeller.CalculateSimilarity(pmWeights, pmWeights);

        //float[] comparedWeights = new float[] { 0.6f, 0.8f, 0.5f, 0.3f };
        //float[] comparedWeights = new float[] { 0.5f, 0.6f, 0.2f, 0.5f };
        //float[] comparedWeights = new float[] { 0.6f, 0.1f, 0.3f, 0.5f };
        //float[] comparedWeights = new float[] { 0.1f, 0.9f, 0.8f, 0.0f };
        //float[] comparedWeights = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
        //float[] comparedWeights = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };

        //float biasedSim = PlayerModeller.CalculateSimilarity(pmWeights, comparedWeights);
        //float diff = maxSim - biasedSim;

        // Adjusting Future Weights //
        //float[] pmWeights = new float[] { 0.6f, 0.8f, 0.3f, 0.5f };
        //float[] nextWeights = new float[] { 0.7f, 0.7f, 0.4f, 0.2f };
        //float[] tightenedWeights = PlayerModeller.TightenWeightDistances(nextWeights, pmWeights, 
        //                                                                 PlayerModeller.CalculateSimilarity(pmWeights, nextWeights));
    }

    // Use this for initialization
    static DramaManager()
    {
        // Initialise Data Set //
        fullSet = new List<PlotPoint>(PlotPointSpace.PPS_FullSet());
        undiscoveredSet = new List<int>();
        for (int i = 0; i < fullSet.Count; i++)
            undiscoveredSet.Add(fullSet[i].GetID());

        // Perform Initial Activation //
        InitPointSatisfaction();

        OutputManager.NewEndpointDependencies.Insert(0, new List<int>());   // to prevent errors first time establishing dependencies

        // Initialise Dependencies //
        for (int i = 0; i < endPoints.Count; i++)
        {
            UpdateEndpointDependencies(endPoints[i]);
        }

        OutputManager.NewEndpointDependencies.Clear(); // only want updated history after points are fired, so ignore first time

        // Initialise Enactor //
        InformationEnactor.InitialiseEnactor();

        // Initialise OutputManager //
        OutputManager.RefreshText();


        // Print Dataset To File //
        //OutputManager.PrintEntireDataSet();   // Now Already Printed
    }

    // Check Preconditions in the beginning, and setup active and open sets //
    static void InitPointSatisfaction()
    {
        // For each plot point //
        for (int i = 0; i < fullSet.Count; i++)
        {
            // Initialise Satisfaction //
            if (fullSet[i].InitSatisfied())
            {
                // if satisfied, add to active set //
                //openSet.Add(i);            // initialise open set
                undiscoveredSet.Remove(i); // point now discovered //
                ActivatePoint(i);                   // then activate point (will remove from open set - but this way avoids additional variables)
            }
        }
    }

    public static void ClosePoint(int id, bool sat)
    {
        if (id < fullSet.Count) // ensure the id exists
        {
            // Check if already in closed set
            if (closedSet.Contains(id))
                return;

            // Remove ID from other sets - NOTE: need to initialise sets first
            activeSet.Remove(id);
            //openSet.Remove(id);
            undiscoveredSet.Remove(id);

            // Set Plot Point Satisfied to False
            fullSet[id].SetIsSatisfied(sat);

            // Add Plot Point to Closed Set
            closedSet.Add(id);
            if (OutputManager.NewClosedPointHistory.Count > 0)
                OutputManager.NewClosedPointHistory[0].Add(id);

            // Remove Enactor Element - NOTE: could check when about to replace instead //
            //InformationEnactor.RemoveElementPoint(id);

            // Exit Function Here If Closing A Fired Point - Not When An Effect is Closed //
            if (sat == false)
            {
                // test connected open set points; if never going to be triggered, move to closed set
                List<int> nextPoints = fullSet[id].GetNextPoints();

                // Check if next points can ever be true - if not, move to closed set and repeat
                for (int i = 0; i < nextPoints.Count; i++)
                {
                    if (nextPoints[i] < fullSet.Count)  // error handle
                    {
                        // Can the next point be satisfied?
                        if (fullSet[nextPoints[i]].CanBeSatisfied() == false)
                        {
                            // Close Point - Set To False - Recursion - Cutting off branches and subsequent points //
                            ClosePoint(nextPoints[i], false);
                        }
                    }
                }

                // Backprop Closing Redundant Preconditions //
                CheckToCloseRedundantPreconditions(id);
            }
        }
    }

    // Remove from Open, Add to Active, Expand Open //
    static void ActivatePoint(int id)
    {
        if (id < fullSet.Count) // ensure the id exists
        {
            // remove from open set - already done undiscovered
            //openSet.Remove(id);

            // add to active set
            if (activeSet.Contains(id) == false)
            {
                activeSet.Add(id);
                if (OutputManager.NewActivePointHistory.Count > 0)
                    OutputManager.NewActivePointHistory[0].Add(id); // add step to history

                // Check if preconditions should be closed //
                CheckToCloseRedundantPreconditions(id);

                // update open set (grow set) //
                //GrowOpenSet(id);
            }
        }
    }

    static void CheckToCloseRedundantPreconditions(int id)
    {
        List<int> previousPoints = new List<int>(PlotPointSpace.FlattenList(fullSet[id].GetPreconditionLogic()));
        for (int i = 0; i < previousPoints.Count; i++)
        {
            // Ignore Closed Preconditions //
            if (closedSet.Contains(previousPoints[i]) == false)    
            {
                List<int> nextPoints = new List<int>(fullSet[previousPoints[i]].GetNextPoints());
                nextPoints.Remove(id);
                for (int j = nextPoints.Count - 1; j >= 0; j--)
                {
                    // If not yet active and not closed - found a point that could come up in future - keep this condition //
                    if (closedSet.Contains(nextPoints[j]) == true)
                        nextPoints.RemoveAt(j);
                    else if (fullSet[nextPoints[j]].CanBeSatisfied() == true)
                        return;
                }

                // No next point that is 
                // This is to backprop closing redundant preconditions - first conditions satisfied are what persists
                ClosePoint(previousPoints[i], false);
                //CheckToCloseRedundantPreconditions(previousPoints[i]);
            }
        }
    }

    // Expand Open Set Using 'Next' Points from Fired Active Point //
    //static void GrowOpenSet(int activatedSource)
    //{
    //     List of Points to Add to Open Set //
    //    List<int> pointsToOpen = fullSet[activatedSource].GetNextPoints();
    //    for (int i = 0; i < pointsToOpen.Count; i++)
    //    {
    //        if (pointsToOpen[i] < fullSet.Count)
    //        {
    //             If Not Already in Open  or Closed Sets - i.e. in undiscovered set //
    //            if (undiscoveredSet.Contains(pointsToOpen[i]) == true)
    //            {
    //                 New Point Discovered //
    //                undiscoveredSet.Remove(pointsToOpen[i]);

    //                 Add Open Point //
    //                if (openSet.Contains(pointsToOpen[i]) == false)
    //                    openSet.Add(pointsToOpen[i]);
    //            }
    //        }
    //    }
    //}

    // After Each Player Action - using machine, listen to radio, read page //
    public static void UpdateSets(int firedPointId)     
    {
        // NOTE: Can Change Later to Pass in Next Points to Test Instead of Searching the Entire Open Set //
        //List<int> pointsToActivate = new List<int>();   // Points to Activate Next //

        // Check Open Set Points to Move to Active Set - NOTE: Change to Next Points of Fired Point //
        //for (int i = 0; i < openSet.Count; i++)
        //{
        //    // Update satisfied of open set
        //    if (fullSet[openSet[i]].UpdateSatisfied())
        //    {
        //        pointsToActivate.Add(openSet[i]);   // Add to list of IDs to call ActivatePoint //
        //    }
        //}

        // Perform Set Updates Here //
        //for (int j = 0; j < pointsToActivate.Count; j++)
        //    ActivatePoint(pointsToActivate[j]);

        List<int> pointsToActivate = new List<int>();
        List<int> nextPoints = fullSet[firedPointId].GetNextPoints();

        for (int i = 0; i < nextPoints.Count; i++)
        {
            if (fullSet[nextPoints[i]].UpdateSatisfied())
                pointsToActivate.Add(nextPoints[i]);
        }

        // Perform Set Updates Here //
        for (int j = 0; j < pointsToActivate.Count; j++)
            ActivatePoint(pointsToActivate[j]);


        InformationEnactor.CreateElementsFromNewActivePoints(pointsToActivate);
    }

        // NOTE: Add following lines (to the function above) after planning heuristic calculation //
        // Perform Selection of Active Points to Present Based On Heuristic //
            // For Each Type of Plot Point //
                // Enact Specific in World Stuff //
            // For Each Event Not Ruled Out //
                // Fire Event //

    // Check EndPoint Dependencies - For a Given EndPoint, what Connected Nodes are currently singularly able to turn off the endpoint //
    public static void UpdateEndpointDependencies(int effectPointID)
    {
        if (effectPointID >= fullSet.Count)
        {
            Debug.Log("Effect not in Plot Point set");
            return;
        }

        List<int> seenIDs = new List<int>();

        // When effect is fired that has at least one endpoint connection //
            // Back-trace from each endpoint connection //
                // For each backtrace point - starting with the endpoint itself //
        List<int> endCons = fullSet[effectPointID].GetEndPointConnections();
        for (int i = 0; i < endCons.Count; i++)
        {
            BackPropagateEndPointDependencies(endCons[i], endCons[i], ref seenIDs);
        }
    }

    // Recursive Function to Update Endpoint Dependencies //
    static void BackPropagateEndPointDependencies(int pointID, int endID, ref List<int> seenIDs)
    {
        // If point has all conditions closed with one exception //
        // If it is still possible for the conditions to be met, if the final point is fired true //
        // Add endpoint dependency to single exception //
        // Pass back endpoint source and check for singular dependency from point's conditions - recursion //

        // NOTE: THINGS TO CHANGE: 
            // MAKE IT EVALUATE 'AND' STATEMENTS TO CHECK FOR DEPENDENCIES WHERE TURNING OFF A CONDITION WILL TURN A POINT OFF //
            // MODIFY CANBESATISFIED FUNCTION - COULD OVERLOAD WITH INPUT CONDITION TO TEST FOR WHEN FIRED FALSE //

        // If point is not closed
        if (closedSet.Contains(pointID) == false)
        {
            // If Endpoint Dependency is not there //
            //if (fullSet[pointID].GetEndPointConnections().Contains(endID) == false)
            if (fullSet[endID].GetTurnOffEndPoints().Contains(pointID) == false)
            {
                // Add EndPoint Connection if not at endpoint //
                if (pointID != endID)
                {
                    //fullSet[pointID].AddEndPointConnection(endID);
                    fullSet[pointID].AddEndPointDependency(endID);
                    fullSet[endID].AddTurnOffEndPoint(pointID);

                    if (OutputManager.NewEndpointDependencies.Count > 0)
                    {
                        OutputManager.NewEndpointDependencies[0].Add(endID);    // Endpoint, Dependency pair
                        OutputManager.NewEndpointDependencies[0].Add(pointID);
                    }
                }
            }

            // Get Previous Points and Off Switches //
            List<int> previousPoints = new List<int>(PlotPointSpace.FlattenList(fullSet[pointID].GetPreconditionLogic()));
            List<int> unclosedPrevPoints = new List<int>();
            for (int i = 0; i < previousPoints.Count; i++)
            {
                // Get All Unclosed Previous Points //
                if (closedSet.Contains(previousPoints[i]) == false)
                    unclosedPrevPoints.Add(previousPoints[i]);
            }

            // NOTE: COULD MODIFY LATER INTO A SINGLE LOOP INSTEAD OF USING THIS IF STATEMENT FIRST //
            if (unclosedPrevPoints.Count == 1) // if only contains a single item
            {
                // If can still be satisfied
                if (fullSet[pointID].CanBeSatisfied())
                {
                    if (seenIDs.Contains(pointID) == false)
                        seenIDs.Add(pointID);

                    // Backprop endpoint
                    if (seenIDs.Contains(unclosedPrevPoints[0]) == false)
                        BackPropagateEndPointDependencies(unclosedPrevPoints[0], endID, ref seenIDs);
                }
            }
            else
            {
                // If can still be satisfied
                if (fullSet[pointID].CanBeSatisfied())
                {
                    if (seenIDs.Contains(pointID) == false)
                        seenIDs.Add(pointID);

                    // Check each unclosed precondition to see if this point would still be accessible if the condition was fired false //
                    for (int j = 0; j < unclosedPrevPoints.Count; j++)
                    {
                        // Check if this point relies on previous //
                        if (fullSet[pointID].CanBeSatisfied(unclosedPrevPoints[j]) == false)
                        {
                            // Backprop endpoint
                            if (seenIDs.Contains(unclosedPrevPoints[j]) == false)
                                BackPropagateEndPointDependencies(unclosedPrevPoints[j], endID, ref seenIDs);
                        }
                    }
                }
            }
        }
    }

    // Fire a plotpoint connected to information accessed in the world //
    public static void FirePlotPointFromInfo(PointInfoVals pointInfo)
    {
        // Add History Step for Metrics //
        OutputManager.NewHistoryEntry();

        // Fire Plot Point and Update Logic //
        fullSet[pointInfo.PlotPointID()].FirePlotPoint();
        OutputManager.FiredPointHistory.Insert(0, pointInfo.PlotPointID());
        OutputManager.FiredWeightsHistory.Insert(0, pointInfo.InfoWeights());
        float[] originalWeights = new float[pointInfo.GetOriginalWeights().Length];
        System.Array.Copy(pointInfo.GetOriginalWeights(), originalWeights, pointInfo.GetOriginalWeights().Length);
        OutputManager.OriginalFiredWeights.Insert(0, originalWeights);

        // Update Weights of Player and Future Plot Points //
        float[] oldWeights = new float[PlayerModeller.GetPlaystyleWeights().Length];
        System.Array.Copy(PlayerModeller.GetPlaystyleWeights(), oldWeights, PlayerModeller.GetPlaystyleWeights().Length);
        OutputManager.OldPMWeightHistory.Insert(0, oldWeights);
        UpdateModelWeights(pointInfo);
        float[] newWeights = new float[PlayerModeller.GetPlaystyleWeights().Length];
        System.Array.Copy(PlayerModeller.GetPlaystyleWeights(), newWeights, PlayerModeller.GetPlaystyleWeights().Length);
        OutputManager.NewPMWeightHistory.Insert(0, newWeights);

        // Perform Information Replacement //
        InformationEnactor.BeginReplacement();
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Debug.Log(fullSet);
        //Debug.Log(undiscoveredSet);
        //Debug.Log(activeSet);
        //Debug.Log(openSet);
        //Debug.Log(closedSet);
        //Debug.Log("Replaced");

        if (fullSet[pointInfo.PlotPointID()].GetIsEndpoint())
            OutputManager.PrintToFile(pointInfo.PlotPointID());

        OutputManager.StopTime(Time.realtimeSinceStartup);  // Record Processing Time //
    }

    // Update the Weights of the PM and the Future PlotPoints in the same Branch //
    public static void UpdateModelWeights(PointInfoVals pointInfo)
    {
        // Update Weights of Player Model //
        OutputManager.SimilarityToPMHistory.Insert(0, PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), pointInfo.InfoWeights())); // push back history
        OutputManager.MaxSimToPMHistory.Insert(0, PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), PlayerModeller.GetPlaystyleWeights())); // push back history

        PlayerModeller.AdjustPlayerWeights(pointInfo.InfoWeights());

        // Update weights of connected nodes
        List<int> branch = new List<int>(); // entire connected branch
        List<int> nextPoints = new List<int>(fullSet[pointInfo.PlotPointID()].GetNextPoints());
        branch.AddRange(nextPoints);

        // Initialise Previous Depth of Search //
        List<int> prevPoints = new List<int>(nextPoints);

        // Obtain Following Branch to Update //
        while (nextPoints.Count != 0)
        {
            int branchCount = branch.Count;

            // Clear Next Layer of Branch //
            nextPoints.Clear();

            // Get Next Layer of Branch //
            for (int i = 0; i < prevPoints.Count; i++)
                nextPoints.AddRange(fullSet[prevPoints[i]].GetNextPoints());

            // If Point Not Already Encountered in Branch - Add to Branch //
            for (int j = 0; j < nextPoints.Count; j++)
            {
                if (branch.Contains(nextPoints[j]) == false)
                    branch.Add(nextPoints[j]);
            }

            // Update Current Depth of Breadth-First Search //
            prevPoints = nextPoints;

            // Break out of While Loop if Branch Remains Unchanged (Error Handling)
            if (branch.Count == branchCount)
                nextPoints.Clear();
        }

        // Tighten Gap Between Future Plot Points //
        float tightenVal = PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), pointInfo.InfoWeights());

        // Go Through Entire Branch //
        for (int k = 0; k < branch.Count; k++)
        {
            // Get PointInfo Through Enactor //
            PointInfoVals bpInfo = InformationEnactor.BankPoint(fullSet[branch[k]].GetPointType(), 
                                         fullSet[branch[k]].GetInfoID());

            // Closening Gap i.e. making future points in this branch more viable //
            bpInfo.SetWeights(PlayerModeller.TightenWeightDistances(bpInfo.InfoWeights(), PlayerModeller.GetPlaystyleWeights(), tightenVal));
        }
    }

    public static void RemoveSpaceFromUndiscoveredSet(int spaceID, int firedPointID)
    {
        List<int> pointsToClose = new List<int>();

        // Search Undiscovered Set //
        for (int i = 0; i < undiscoveredSet.Count; i++)
        {
            PointInfoVals bpInfo = InformationEnactor.BankPoint(fullSet[undiscoveredSet[i]].GetPointType(),
                             fullSet[undiscoveredSet[i]].GetInfoID());

            // Remove Space from Potential Spaces if Exists //
            bpInfo.RemoveSpaceWithID(spaceID);

            // If No Spaces Left, Add to Points to Close //
            if (bpInfo.GetPossibleSpaces().Count <= 0)
                pointsToClose.Add(undiscoveredSet[i]);
        }

        pointsToClose.Remove(firedPointID);

        // Close Points //
        for (int j = 0; j < pointsToClose.Count; j++)
            ClosePoint(pointsToClose[j], false);
    }
}