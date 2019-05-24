using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

// Comparer class for Element Point Similarity - For Sorting Viability //
public class ElementCompareSimFloat : IComparer<ElementPoint>
{
    public int Compare(ElementPoint x, ElementPoint y)
    {
        // Check For Null Comparisons //
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
                return 1;
            else
            {
                // Compare Similarity if ID is not the same //
                if (x.PlotPointIndex != y.PlotPointIndex)
                {
                    return x.SimilarityValue.CompareTo(y.SimilarityValue);
                }
                else
                    return 0;
            }
        }
    }
}

// Comparer class for Element Point Similarity - For Sorting Viability //
public class ElementCompareViaIndex : IComparer<ElementPoint>
{
    public int Compare(ElementPoint x, ElementPoint y)
    {
        // Check For Null Comparisons //
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
                return 1;
            else
            {
                // Compare Similarity if ID is not the same //
                if (x.PlotPointIndex != y.PlotPointIndex)
                {
                    return x.ViabilityIndex.CompareTo(y.ViabilityIndex);
                }
                else
                    return 0;
            }
        }
    }
}

// Comparer Class for Element Space Competition //
public class ElementSpaceCompareCompetition : IComparer<ElementSpace>
{
    public int Compare(ElementSpace x, ElementSpace y)
    {
        // Check For Null Comparisons //
        if (x == null)
        {
            if (y == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (y == null)
                return 1;
            else
            {
                int xCompetition = x.CompetitionScore();
                int yCompetition = y.CompetitionScore();

                // Compare Competition //
                if (xCompetition != yCompetition)
                {
                    return xCompetition.CompareTo(yCompetition);
                }
                else
                {
                    // If Competition is the same - most relevant when only a single shared competitor //
                    // Sub-order by trace down //
                    int xLowestTrace = x.LowestChainIndex();
                    int yLowestTrace = y.LowestChainIndex();

                    if (xLowestTrace == yLowestTrace)
                        return 0;
                    else
                    {
                        return xLowestTrace.CompareTo(yLowestTrace);
                    }
                }
            }
        }
    }
}



public class ElementSpace : System.Object
{
    // Variables //
    // Ordering Index
    public int ReplacementOrderIndex;
    public int SpaceID;
    // Housed Element
    public ElementPoint HousedElement;
    // Competing Elements
    public List<ElementPoint> CompetingElements = new List<ElementPoint>();

    public int LowestTraceIndex = -1;

    // Get Difference Between Indices of Top Competitor //
    public int CompetitionScore()
    {
        if (CompetingElements.Count == 0)
            // Return 0 (Null) //
            return 0;
        else if (CompetingElements.Count == 1)
            // Return Value //
            return CompetingElements[0].ViabilityIndex;
        else
        {
            // Return Difference //
            return CompetingElements[CompetingElements.Count - 1].ViabilityIndex 
                 - CompetingElements[CompetingElements.Count - 2].ViabilityIndex;
        }
    }

    // Get Lowest Index of Chained W-set elements - i.e. what point will get popped off //
    public int LowestChainIndex()
    {
        // Initialise Lowest Index //
        int lowestIndex = HousedElement.ViabilityIndex;

        // Trace Down will update lowest Index //
        TraceDown(HousedElement, ref lowestIndex, 0);

        return lowestIndex;
    }

    public void TraceDown(ElementPoint queryPoint, ref int lowestIndex, int depth)
    {
        int exitDepth = 10; // exit the search after this value

        // Increment Depth //
        depth++;
        if (depth >= exitDepth)
            return;

        /* NOTE: There will still be duplicate searches but that may be acceptable (might be more expensive to check for unique cases */

        // Using this Element //
        // Get Potential Spaces (Copy List) //
        List<ElementSpace> replaceableSpaces = new List<ElementSpace>(queryPoint.PotentialSpaces);

        // Remove Spaces With An Occupying Element that Beats this Element //
        for (int i = replaceableSpaces.Count - 1; i >= 0; i--)
        {
            // Remove Replaceable Space if it cannot be replaced //
            if (queryPoint.ViabilityIndex <= replaceableSpaces[i].HousedElement.ViabilityIndex)
                replaceableSpaces.RemoveAt(i);
        }

        // If Replaceable World-set Elements List is Empty //
        if (replaceableSpaces.Count == 0)
        {
            // Compare This Point's Viability Index With The Lowest //
            if (queryPoint.ViabilityIndex < lowestIndex)
                // Lowest Index Becomes This Point's Index if it is lower //
                lowestIndex = queryPoint.ViabilityIndex;
        }
        else
        {
            // For Each Replaceable World-set Element //
            for (int i = 0; i < replaceableSpaces.Count; i++)
            {
                // Trace Down (ReplaceableElement, ref lowestIndex //
                TraceDown(replaceableSpaces[i].HousedElement, ref lowestIndex, depth);

                if (lowestIndex == 0)
                    return;
            }
        }
    }
}



public class ElementPoint : System.Object
{
    // Variables //
    // ViabilityIndex
    public int ViabilityIndex;
    public float SimilarityValue;
    // PlotPointIndex
    public int PlotPointIndex;
    // Empty Flag 
    public bool IsEmpty = false;
    // PotentialSpaces
    public List<ElementSpace> PotentialSpaces = new List<ElementSpace>();

    public ElementPoint(int ppIndex, List<ElementSpace> spaces)
    {
        PlotPointIndex = ppIndex;
        PotentialSpaces = new List<ElementSpace>(spaces);

        ViabilityIndex = 0; //initialise
    }

    public ElementPoint DeepCopy()
    {
        ElementPoint copy = new ElementPoint(PlotPointIndex, PotentialSpaces);

        copy.IsEmpty = this.IsEmpty;
        copy.SimilarityValue = this.SimilarityValue;
        copy.ViabilityIndex = this.ViabilityIndex;

        return copy;
    }
}



// Contains Heuristic Weights and Connected Plot Point ID //
public class PointInfoVals : System.Object
{
    // Point Variables - Allows Enactor to Connect to Info Banks and Allows Information Replacement //
    float[] infoWeights = { 0.5f, 0.5f, 0.5f, 0.5f };
    int plotPointID = -1;

    float[] originalWeights = { 0.5f, 0.5f, 0.5f, 0.5f };

    List<ElementSpace> possibleSpaces = new List<ElementSpace>();

    // Constructor //
    public PointInfoVals(float[] weights, int iID)
    {
        infoWeights = weights;
        plotPointID = iID;

        System.Array.Copy(weights, originalWeights, weights.Length);
    }

    public PointInfoVals(int iID)
    {
        plotPointID = iID;

        System.Array.Copy(infoWeights, originalWeights, infoWeights.Length);
    }

    // Getters/Setters //
    public float[] InfoWeights() { return infoWeights; }
    public void SetWeights(float[] weights) { infoWeights = weights; }
    public int PlotPointID() { return plotPointID; }
    public void SetPlotPointID(int id) { plotPointID = id; }

    public void SetPossibleSpaces(List<ElementSpace> posSpaces) { possibleSpaces = new List<ElementSpace>(posSpaces); }
    public List<ElementSpace> GetPossibleSpaces() { return possibleSpaces; }

    public float[] GetOriginalWeights() { return originalWeights; }
    public void InitOriginalWeights() { System.Array.Copy(infoWeights, originalWeights, infoWeights.Length); }

    public void RemoveSpaceWithID(int id)
    {
        for (int i = 0; i < possibleSpaces.Count; i++)
        {
            if (possibleSpaces[i].SpaceID == id)
            {
                possibleSpaces.RemoveAt(i);
                return;
            }
        }
    }
}



// The Connector Between The DM and the Info Banks - Handles Messy Stuff, Passes Info to and from DM //
public static class InformationEnactor {

    private static Thread replacementThread;

    // Element Sets //
    static List<ElementSpace> allSpaces = new List<ElementSpace>();    // Spaces that can be occupied by an element
    static List<ElementPoint> worldESet = new List<ElementPoint>();    // W-set, elements in the world
    static List<ElementPoint> voidESet = new List<ElementPoint>();     // !W-set, elements not in the world, both from active set

    static List<ElementSpace> unavailableSpaces = new List<ElementSpace>();

    public static List<ElementPoint> WorldSet() { return worldESet; }
    public static List<ElementPoint> VoidSet() { return voidESet; }

    // Remove from search space after accessing the space for its point //
    public static void RemoveElementSpace(ElementSpace accessedSpace)
    {
        for (int i = 0; i < worldESet.Count; i++)
        {
            if (accessedSpace.HousedElement.PlotPointIndex == worldESet[i].PlotPointIndex)
            {
                worldESet.RemoveAt(i);
                break;
            }
        }
        //worldESet.Remove(accessedSpace.HousedElement);
        allSpaces.Remove(accessedSpace);
        unavailableSpaces.Add(accessedSpace);

        // Remove from Competition Considerations //
        for (int i = 0; i < worldESet.Count; i++)
            worldESet[i].PotentialSpaces.Remove(accessedSpace);

        //for (int i = 0; i < voidESet.Count; i++)
        //    voidESet[i].PotentialSpaces.Remove(accessedSpace);
    }

    public static void InitWorldHousedElements(List<ElementSpace> allSpaces)
    {
        for (int i = 0; i < allSpaces.Count; i++)
        {
            allSpaces[i].SpaceID = i;
            CreateWorldElementFromPlotPointIndex(allSpaces[i], i);
        }
    }

    public static void CreateWorldElementFromPlotPointIndex(ElementSpace space, int plotPointID)
    {
        if (DramaManager.CheckIDValid(plotPointID) == false)
            return;

        // Get Info Type //
        PlotPoint.PLOTPOINTTYPE infoType = DramaManager.DM_FullSet()[plotPointID].GetPointType();

        // Declare List of Spaces this Element can Access //
        List<ElementSpace> potentialSpaces = new List<ElementSpace>();

        // Use Info Type and PlotPointID to get Info ID
        int infoID = DramaManager.DM_FullSet()[plotPointID].GetInfoID();

        // Use Info Type and ID to get Potential Spaces //
        switch (infoType)
        {
            // NOTE: ADD FUNCTIONS HERE - AND IN ASSOCIATED INFOBANK //
            case PlotPoint.PLOTPOINTTYPE.TEXTPAGE:
                {
                    if (PageTextBank.CheckIDValid(infoID))
                        potentialSpaces.AddRange(PageTextBank.GetPlotPointPotentialSpaces(infoID));
                    break;
                }
            case PlotPoint.PLOTPOINTTYPE.RADIO:
                { break; }
            case PlotPoint.PLOTPOINTTYPE.CASSETTE:
                { break; }
            default:
                { break; }
        }

        space.HousedElement = new ElementPoint(plotPointID, potentialSpaces);
    }

    public static void CreateElementsFromNewActivePoints(List<int> newActiveIndices)
    {
        // Add to !W-set //
        for (int i = 0; i < newActiveIndices.Count; i++)
        {
            bool alreadyInSet = false;
            for (int j = 0; j < voidESet.Count; j++)
            {
                if (newActiveIndices[i] == voidESet[j].PlotPointIndex)
                {
                    alreadyInSet = true;
                    break;
                }
            }
            if (alreadyInSet)
                continue;

            // Get Info Type //
            PlotPoint.PLOTPOINTTYPE infoType = DramaManager.DM_FullSet()[newActiveIndices[i]].GetPointType();

            // Declare List of Spaces this Element can Access //
            List<ElementSpace> potentialSpaces = new List<ElementSpace>();

            // Use Info Type and PlotPointID to get Info ID
            int infoID = DramaManager.DM_FullSet()[newActiveIndices[i]].GetInfoID();

            //if (infoID == 64)
            //    Debug.Log("Oh No");

            // Use Info Type and ID to get Potential Spaces //
            switch (infoType)
            {
                // NOTE: ADD FUNCTIONS HERE - AND IN ASSOCIATED INFOBANK //
                case PlotPoint.PLOTPOINTTYPE.TEXTPAGE:
                    {
                        if (PageTextBank.CheckWeightsExist(infoID))
                            potentialSpaces.AddRange(PageTextBank.GetPlotPointPotentialSpaces(infoID));
                        break;
                    }
                case PlotPoint.PLOTPOINTTYPE.RADIO:
                    { break; }
                case PlotPoint.PLOTPOINTTYPE.CASSETTE:
                    { break; }
                default:
                    { break; }
            }

            // Ignore all spaces that have already been made unavailable - NOTE: still expensive //
            for (int j = 0; j < unavailableSpaces.Count; j++)
                potentialSpaces.Remove(unavailableSpaces[j]);

            if (potentialSpaces.Count == 0)
                DramaManager.ClosePoint(newActiveIndices[i], false);
            else
            {
                // Add to !W-set //
                voidESet.Add(new ElementPoint(newActiveIndices[i], potentialSpaces));
            }
        }
    }

    // Constructor //
    public static void InitialiseEnactor()
    {
        // Get Spaces from Monitor Text Medium //
        allSpaces = PageTextBank.GetWorldTextSpaces(); // NOTE: can append other spaces later //

        // Get Spaces from Radio/Cassette Medium //

        InitWorldHousedElements(allSpaces);

        // Create Elements from Spaces Stored Info Type and ID //
        for (int i = 0; i < allSpaces.Count; i++)
        {
            // Get Housed Element from Space and Check Drama Manager to Get Potential Spaces //
            int ppID = allSpaces[i].HousedElement.PlotPointIndex;
            if (DramaManager.CheckIDValid(ppID))
            {
                worldESet.Add(allSpaces[i].HousedElement);
                DramaManager.DM_FullSet()[ppID].SetIsInWorld(true);
            }
        }

        // Initialise !W-set //
        for (int j = 0; j < DramaManager.DM_ActiveSet().Count; j++)
        {
            if (DramaManager.DM_FullSet()[DramaManager.DM_ActiveSet()[j]].GetIsInWorld() == false)
            {
                // Get Info Type //
                PlotPoint.PLOTPOINTTYPE infoType = DramaManager.DM_FullSet()[DramaManager.DM_ActiveSet()[j]].GetPointType();

                // Declare List of Spaces this Element can Access //
                List<ElementSpace> potentialSpaces = new List<ElementSpace>();

                // Use Info Type and PlotPointID to get Info ID
                int infoID = DramaManager.DM_FullSet()[DramaManager.DM_ActiveSet()[j]].GetInfoID();

                // Use Info Type and ID to get Potential Spaces //
                switch (infoType)
                {
                    // NOTE: ADD FUNCTIONS HERE - AND IN ASSOCIATED INFOBANK //
                    case PlotPoint.PLOTPOINTTYPE.TEXTPAGE:
                        {
                            if (PageTextBank.CheckWeightsExist(infoID))
                                potentialSpaces.AddRange(PageTextBank.GetPlotPointPotentialSpaces(infoID));
                            break;
                        }
                    case PlotPoint.PLOTPOINTTYPE.RADIO:
                        { break; }
                    case PlotPoint.PLOTPOINTTYPE.CASSETTE:
                        { break; }
                    default:
                        { break; }
                }

                // Add to !W-set //
                voidESet.Add(new ElementPoint(DramaManager.DM_ActiveSet()[j], potentialSpaces));
            }
        }
    }

    // Connect to Other Banks of Information using Passed Parameters //
    public static void InitPlotPointInfo(PlotPoint.PLOTPOINTTYPE bankType, int infoID, int plotPointID)
    {
        // Set PointInfo plot point ID to calling plot point - allow reference back up //
        PointInfoVals pointInfo = BankPoint(bankType, infoID);
        if (pointInfo != null)
            pointInfo.SetPlotPointID(plotPointID);
    }

    // Upwards Call to DM to Fire Plot Point and Conduct Weight Value Updates //
    public static void FirePointFromAction(PointInfoVals infoPoint)
    {
        // Call DM Fire Function, passing in values //
        if (infoPoint != null)
            DramaManager.FirePlotPointFromInfo(infoPoint);
    }

    // Upwards Call to DM to Fire Plot Point and Conduct Weight Value Updates //
    public static void FirePointFromAction(PlotPoint.PLOTPOINTTYPE bankType, int infoID)
    {
        // Get information point //
        PointInfoVals infoPoint = BankPoint(bankType, infoID);
        
        // Call DM Fire Function, passing in values //
        if (infoPoint != null)
            DramaManager.FirePlotPointFromInfo(infoPoint);
    }

    public static void FirePointFromAction(int plotpointID)
    {
        // Check point is valid //
        if (DramaManager.CheckIDValid(plotpointID))
        {
            PlotPoint.PLOTPOINTTYPE bankType = DramaManager.DM_FullSet()[plotpointID].GetPointType();
            int infoID = DramaManager.DM_FullSet()[plotpointID].GetInfoID();

            // Get information point //
            PointInfoVals infoPoint = BankPoint(bankType, infoID);

            // Call DM Fire Function, passing in values //
            if (infoPoint != null)
                DramaManager.FirePlotPointFromInfo(infoPoint);
        }
    }

    public static bool ValidateInfoType(PlotPoint.PLOTPOINTTYPE queryType, int plotpointID)
    {
        // Check if an input information type matches - so that the correct narrative delivery method is used //
        if (DramaManager.CheckIDValid(plotpointID))
        {
            if (DramaManager.DM_FullSet()[plotpointID].GetPointType() == queryType)
                return true;
        }
        return false;
    }

    // Should be called after being validated //
    public static int GetInfoIdFromPointID(int plotpointID)    { return DramaManager.DM_FullSet()[plotpointID].GetInfoID(); }
    public static bool GetIsPointEnding(int plotpointID) { return DramaManager.DM_FullSet()[plotpointID].GetIsEndpoint(); }

    // Return Point Information Values using Passed Info //
    public static PointInfoVals BankPoint(PlotPoint.PLOTPOINTTYPE bankType, int infoID)
    {
        // Case of Info Type //
        switch (bankType)
        {
            case PlotPoint.PLOTPOINTTYPE.TEXTPAGE:
                { if (PageTextBank.CheckWeightsExist(infoID)) return PageTextBank.GetPointInfoValElement(infoID); break; }
            case PlotPoint.PLOTPOINTTYPE.RADIO:
                { if (infoID < AudioBank.BankLength()) return AudioBank.GetPointInfoValElement(infoID); break; }
            case PlotPoint.PLOTPOINTTYPE.CASSETTE:
                { if (infoID < CassetteBank.BankLength()) return CassetteBank.GetPointInfoValElement(infoID); break; }
            case PlotPoint.PLOTPOINTTYPE.PLAYERACTION:
                return null;
            case PlotPoint.PLOTPOINTTYPE.EVENT:
                return null;
            default:
                return null;
        }

        return null;
    }

    // Pre-sorting cleanup //
    public static void CleanupSets()
    {
        // Check and Update Sets //
        // W-Set aka World Set //
        for (int i = 0; i < worldESet.Count; i++)
        {
            // Mark as Empty //
            if (DramaManager.IsInClosedSet(worldESet[i].PlotPointIndex))
                worldESet[i].IsEmpty = true;
        }

        // !W-set aka Void Set //
        for (int j = voidESet.Count - 1; j >= 0; j--)
        {
            // Remove From Set //
            voidESet[j].IsEmpty = false;    //reset empty flag
            if (DramaManager.IsInClosedSet(voidESet[j].PlotPointIndex))
                voidESet.RemoveAt(j);
            else
            {
                // Check if ran out of possible spaces to inhabit
                if (HasAvailableSpaces(voidESet[j]) == false)
                {
                    DramaManager.ClosePoint(voidESet[j].PlotPointIndex, false);
                    voidESet.RemoveAt(j);
                }
            }
        }
    }

    static bool HasAvailableSpaces(ElementPoint ePoint)
    {
        // See if a Point has any spaces that have not been accessed yet //
        // NOTE: needs future modification to account for non-text based spaces such as the radio mechanic
        for (int i = 0; i < ePoint.PotentialSpaces.Count; i++)
        {
            if (unavailableSpaces.Count < allSpaces.Count)  // just to speed up searching
            {
                if (unavailableSpaces.Contains(ePoint.PotentialSpaces[i]) == false)
                    return true;
            }
            else
            {
                if (allSpaces.Contains(ePoint.PotentialSpaces[i]) == true)
                    return true;
            }
        }

        return false;
    }

    // Mark Empty Points Where Necessary //
    public static void ResolveEndpointConflicts()
    {
        List<int> uniqueEndpoints = new List<int>();                // a reference to all unique endpoints that may be conflicting
        List<int> undesirableEndpoints = new List<int>();
        List<ElementPoint> offSwitches = new List<ElementPoint>();                    // points with effects (may turn off a dependency)
        List<List<ElementPoint>> dependencies = new List<List<ElementPoint>>();

        // For Each Active Set Element Obtain Unique Endpoints from active dependencies and keep track of points that have effects //
        // For Each World Element //
        for (int i = 0; i < worldESet.Count; i++)
        {
            // Ignore if Empty //
            if (worldESet[i].IsEmpty == false)
            {
                // Get dependent endpoints if any exist //
                List<int> dependentEndPoints = DramaManager.DM_FullSet()[worldESet[i].PlotPointIndex].GetEndPointDependencies();
                for (int j = 0; j < dependentEndPoints.Count; j++)
                {
                    // If Contains an Effect with an Endpoint Dependency //
                        // Add Unique Endpoint to List of Conflicts //
                    if (uniqueEndpoints.Contains(dependentEndPoints[j]) == false)
                    { 
                        // Add to list of unique endpoints //
                        uniqueEndpoints.Add(dependentEndPoints[j]);

                        // Add to list of dependencies //
                        dependencies.Add(new List<ElementPoint>());
                        dependencies[uniqueEndpoints.Count - 1].Add(worldESet[i]);
                    }
                    else
                    {
                        // Add to list of elements that are dependent
                        int epIndex = uniqueEndpoints.IndexOf(dependentEndPoints[j]);

                        // Add to list of dependencies //
                        dependencies[epIndex].Add(worldESet[i]);
                    }
                }

                // Add to list of offswitches to reduce search space //
                if (DramaManager.DM_FullSet()[worldESet[i].PlotPointIndex].GetEffects().Count > 0)
                    offSwitches.Add(worldESet[i]);
            }
        }

        /* NOTE: The last part, using IndexOf may be more expensive than to simply recalculate similarity for each unique 
                 element-endpoint dependency relationship rather than searching for endpoints seen before - may change later 
           NOTE: Could replace these two loops with a single loop through allPoints?? */

        // For Each !World Element //
        for (int i = 0; i < voidESet.Count; i++)
        {
            // Get dependent endpoints if any exist //
            List<int> dependentEndPoints = DramaManager.DM_FullSet()[voidESet[i].PlotPointIndex].GetEndPointDependencies();
            for (int j = 0; j < dependentEndPoints.Count; j++)
            {
                // If Contains an Effect with an Endpoint Dependency //
                    // Add Unique Endpoint to List of Conflicts //
                if (uniqueEndpoints.Contains(dependentEndPoints[j]) == false)
                {
                    // Add to list of unique endpoints //
                    uniqueEndpoints.Add(dependentEndPoints[j]);

                    // Add to list of dependencies //
                    dependencies.Add(new List<ElementPoint>());
                    dependencies[uniqueEndpoints.Count - 1].Add(voidESet[i]);
                }
                else
                {
                    // Add to list of elements that are dependent
                    int epIndex = uniqueEndpoints.IndexOf(dependentEndPoints[j]);

                    // Add to list of dependencies //
                    dependencies[epIndex].Add(voidESet[i]);
                }
            }

            // Add to list of offswitches to reduce search space //
            if (DramaManager.DM_FullSet()[voidESet[i].PlotPointIndex].GetEffects().Count > 0)
                offSwitches.Add(voidESet[i]);
        }

        // Find Current Best Endpoint //
        float bestSimilarity = 999.99f;
        int bestElementPPID = -1;
        List<float> uniqueEndpointSimilarities = new List<float>();
        for (int i = 0; i < uniqueEndpoints.Count; i++)
        {
            // Calculate Similarity to PM //
            PointInfoVals endpointInfo = BankPoint(DramaManager.DM_FullSet()[uniqueEndpoints[i]].GetPointType(), DramaManager.DM_FullSet()[uniqueEndpoints[i]].GetInfoID());
            float endpointSimilarityToPM = PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), endpointInfo.InfoWeights());
            if (endpointSimilarityToPM < bestSimilarity)
            {
                bestSimilarity = endpointSimilarityToPM;
                bestElementPPID = uniqueEndpoints[i];
            }

            // Add to list so as to not calculate twice
            uniqueEndpointSimilarities.Add(endpointSimilarityToPM);
        }

        // Calculate Max Similarity - As Calculation is Weighted //
        float maxSim = PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), PlayerModeller.GetPlaystyleWeights());

        // Dynamic Range Allowance //
        // Range Becomes Distance from Best Endpoint to the PM Weights * a scalar factor //
        // Other Endpoints are Viable if their comparison falls within this range to their PM weights //
        float simRangeScale = 2.0f; // to scale up the range of possible endpoints
        float simToBeat = maxSim - ((maxSim - bestSimilarity) * simRangeScale); // to calculate the similarity to beat 

        // List of Endpoints that are not viable //
        List<int> redundantEndPointIDs = new List<int>();

        // For Each Conflict - Mark Redundant Dependencies Empty and Log Redundant Endpoints //
        for (int i = 0; i < uniqueEndpointSimilarities.Count; i++)
        { 
            // Compare Endpoint Predicted Weights to PM and Best Endpoint's Current Predicted Weights //
            if (uniqueEndpointSimilarities[i] < simToBeat)
            {
                // If Outside of Range of Acceptable Endpoint Heuristic Values //
                    // Mark Endpoint Dependency Elements as Empty //
                List<ElementPoint> redundantElements = dependencies[i];
                for (int j = 0; j < redundantElements.Count; j++)
                {
                    // Mark Element as Empty //
                    redundantElements[j].IsEmpty = true;
                }

                // Mark Endpoint as Redundant //
                redundantEndPointIDs.Add(uniqueEndpoints[i]);
            }
        }

        // For Each OffSwitch - Mark Empty if theu both turn off a viable endpoint and have no viable endpoint of their own //
        for (int i = 0; i < offSwitches.Count; i++)
        {
            /* NOTE: Does this effect turn off a viable endpoint dependency?? 
                     Does this effect have its own viable endpoint dependency?? 
                     If yes and yes - nothing   
                     If yes and no - turn this off
                     If no and yes - nothing
                     If no and no - nothing */

            bool turnsOffViableEndpoint = false;
            bool hasViableEndpoint = false;

            // Check if switch turns off a viable endpoint //
            List<int> offSwitchEffects = DramaManager.DM_FullSet()[offSwitches[i].PlotPointIndex].GetEffects();
            for (int j = 0; j < offSwitchEffects.Count; j++)
            {
                // Get Effect Endpoint Dependencies //
                List<int> effectDependentEndpoints = DramaManager.DM_FullSet()[offSwitchEffects[j]].GetEndPointDependencies();
                for (int k = 0; k < effectDependentEndpoints.Count; k++)
                {
                    // If Turns off an endpoint not contained in the redundant list //
                    if (redundantEndPointIDs.Contains(effectDependentEndpoints[k]) == false)
                        turnsOffViableEndpoint = true;

                    // exit inner loop //
                    if (turnsOffViableEndpoint)
                        break;
                }

                // exit outer loop
                if (turnsOffViableEndpoint)
                    break;
            }

            // NOTE: could put this next part inside the inner loop and return early instead of breaking?? //

            // If the first condition is met //
            if (turnsOffViableEndpoint)
            {
                // Check if switch has a viable endpoint //
                // Get endpoint dependencies of offswitch if they exist //
                List<int> offSwitchDependentEndpoints = DramaManager.DM_FullSet()[offSwitches[i].PlotPointIndex].GetEndPointDependencies();
                for (int j = 0; j < offSwitchDependentEndpoints.Count; j++)
                {
                    // Check if endpoint is not redundent
                    if (redundantEndPointIDs.Contains(offSwitchDependentEndpoints[j]) == false)
                        hasViableEndpoint = true;

                    // exit loop early //
                    if (hasViableEndpoint)
                        break;
                }

                // if this effect turns off a viable endpoint dependency but has no viable endpoint dependency itself
                if (hasViableEndpoint == false)
                {
                    // Mark Off Switch as Empty //
                    offSwitches[i].IsEmpty = true;
                }
            }
        }
    }

    // Sorting to Get a Viability Index - May or may not be needed anymore //
    public static void SortByViability()
    {
        List<ElementPoint> viabilityList = new List<ElementPoint>();    // sorted viability list
        List<ElementPoint> ecViabilityList = new List<ElementPoint>();  // sorted viability list of points with an endpoint connection - Append to end of viability list
        List<ElementPoint> edViabilityList = new List<ElementPoint>();
        List<ElementPoint> endViabilityList = new List<ElementPoint>();

        // Comparer //
        ElementCompareSimFloat eComp = new ElementCompareSimFloat();

        // For Each World Element //
        for (int i = 0; i < worldESet.Count; i++)
        {
            // If Marked Empty //
            if (worldESet[i].IsEmpty)
            {
                // Push front of List if in World Set //
                viabilityList.Insert(0, worldESet[i]);
            }
            else
            {
                // Calculate Similarity //
                PointInfoVals pointInfo = BankPoint(DramaManager.DM_FullSet()[worldESet[i].PlotPointIndex].GetPointType(), 
                                                    DramaManager.DM_FullSet()[worldESet[i].PlotPointIndex].GetInfoID());

                // Exit if point does not exist //
                if (pointInfo == null)
                    continue;

                float similarityToPM = PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), pointInfo.InfoWeights());
                worldESet[i].SimilarityValue = similarityToPM;

                // If has Endpoint Connection //
                if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetIsEndpoint() == true)
                {
                    // Insert into ED_Viability_List using Similarty to PM Calc //
                    int index = endViabilityList.BinarySearch(worldESet[i], eComp);

                    // If not in list //
                    if (index < 0)
                        endViabilityList.Insert(~index, worldESet[i]);
                }
                else if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetEndPointConnections().Count > 0)
                {
                    if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetEndPointDependencies().Count > 0)
                    {
                        // Insert into ED_Viability_List using Similarty to PM Calc //
                        int index = edViabilityList.BinarySearch(worldESet[i], eComp);

                        // If not in list //
                        if (index < 0)
                            edViabilityList.Insert(~index, worldESet[i]);
                    }
                    else
                    {
                        // Insert into EC_Viability_List using Similarty to PM Calc //
                        int index = ecViabilityList.BinarySearch(worldESet[i], eComp);

                        // If not in list //
                        if (index < 0)
                            ecViabilityList.Insert(~index, worldESet[i]);
                    }
                }
                else
                {
                    // Insert into Viability_List using Similarty to PM Calc //
                    int index = viabilityList.BinarySearch(worldESet[i], eComp);

                    // If not in list //
                    if (index < 0)
                        viabilityList.Insert(~index, worldESet[i]);
                }
            }
        }

        // For Each !World Element //
        for (int i = 0; i < voidESet.Count; i++)
        {
            // If !Marked Empty
            if (voidESet[i].IsEmpty == false)
            {
                // Calculate Similarity //
                PointInfoVals pointInfo = BankPoint(DramaManager.DM_FullSet()[voidESet[i].PlotPointIndex].GetPointType(),
                                                    DramaManager.DM_FullSet()[voidESet[i].PlotPointIndex].GetInfoID());

                // Exit if point does not exist //
                if (pointInfo == null)
                    continue;

                float similarityToPM = PlayerModeller.CalculateSimilarity(PlayerModeller.GetPlaystyleWeights(), pointInfo.InfoWeights());
                voidESet[i].SimilarityValue = similarityToPM;

                // If has Endpoint Connection, Dependency or is an Endpoint //
                if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetIsEndpoint() == true)
                {
                    // Insert into ED_Viability_List using Similarty to PM Calc //
                    int index = endViabilityList.BinarySearch(voidESet[i], eComp);

                    // If not in list //
                    if (index < 0)
                        endViabilityList.Insert(~index, voidESet[i]);
                }
                else if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetEndPointConnections().Count > 0)
                {
                    if (DramaManager.DM_FullSet()[pointInfo.PlotPointID()].GetEndPointDependencies().Count > 0)
                    {
                        // Insert into ED_Viability_List using Similarty to PM Calc //
                        int index = edViabilityList.BinarySearch(voidESet[i], eComp);

                        // If not in list //
                        if (index < 0)
                            edViabilityList.Insert(~index, voidESet[i]);
                    }
                    else
                    {
                        // Insert into EC_Viability_List using Similarty to PM Calc //
                        int index = ecViabilityList.BinarySearch(voidESet[i], eComp);

                        // If not in list //
                        if (index < 0)
                            ecViabilityList.Insert(~index, voidESet[i]);
                    }
                }
                else
                {
                    // Insert into Viability_List using Similarty to PM Calc //
                    int index = viabilityList.BinarySearch(voidESet[i], eComp);

                    // If not in list //
                    if (index < 0)
                        viabilityList.Insert(~index, voidESet[i]);
                }
            }
        }

        // Append EC_Viability_List and ED_Viability_List to end of Viability_List //
        viabilityList.AddRange(ecViabilityList);
        viabilityList.AddRange(edViabilityList);    // dependent points for viable endings more important than those that just have a connection
        viabilityList.AddRange(endViabilityList);   // viable endpoints added to the end

        // NOTE: Call this Function After Firing a Point and Updating Weights //
        for (int i = 0; i < viabilityList.Count; i++)
        {
            viabilityList[i].ViabilityIndex = i;    // give reference for comparison later
        }
    }

    // Initialise Competition //
    public static void IntialiseReplacementCompetition()
    {
        // Initialise Lowest Trace of Spaces //
        for (int i = 0; i < allSpaces.Count; i++)
        {
            allSpaces[i].LowestTraceIndex = allSpaces[i].LowestChainIndex();    // initialise - update after replacement
        }

        // Comparer //
        ElementCompareViaIndex eCompViaIndex = new ElementCompareViaIndex();

        // Initialise Replacement Competition for Each Space //
        // For each !World Element //
        for (int i = 0; i < voidESet.Count; i++)
        {
            //if (voidESet[i].PotentialSpaces.Count == 0)
            //    DramaManager.ClosePoint(voidESet[i].PlotPointIndex, false);

            // For Each Space Potential Space //
            for (int j = 0; j < voidESet[i].PotentialSpaces.Count; j++)
            {
                if (unavailableSpaces.Contains(voidESet[i].PotentialSpaces[j]))
                    continue;

                // Automatically add to competition if space has no housed element //
                if (voidESet[i].PotentialSpaces[j].HousedElement == null)
                {
                    // Insert into competing elements using Viability Index //
                    int index = voidESet[i].PotentialSpaces[j].CompetingElements.BinarySearch(voidESet[i], eCompViaIndex);

                    if (index < 0)
                        voidESet[i].PotentialSpaces[j].CompetingElements.Insert(~index, voidESet[i]);
                }
                else
                {
                    // NOTE: Modification to allow sub-optimal replacement for cases where the replacer is worse than the 
                    // housed element but better than the lowest trace element //
                    if (voidESet[i].ViabilityIndex > voidESet[i].PotentialSpaces[j].LowestTraceIndex)
                    {
                        // Insert into competing elements using Viability Index //
                        int index = voidESet[i].PotentialSpaces[j].CompetingElements.BinarySearch(voidESet[i], eCompViaIndex);

                        // If not in list //
                        if (index < 0)
                            voidESet[i].PotentialSpaces[j].CompetingElements.Insert(~index, voidESet[i]);

                        //voidESet[i].PotentialSpaces[j].CompetingElements.Add(voidESet[i]);
                    }
                }
            }
        }
    }

    public static void BeginReplacement()
    {
        //replacementThread = new Thread(ReplaceInfo);
        //replacementThread.Start();
        ReplaceInfo();
    }

    // ReplacementAlgorithm //
    public static void ReplaceInfo()
    {
        // Mark Empty Closed Points that are still in the world - still in the W-set (world set) //
        CleanupSets();

        // Perform Endpoint Conflict Resolution //
        ResolveEndpointConflicts();

        // Sort By Viability //
            // Calculate Similarity to PM and use Connection To Endpoint - call sort algorithm //
        SortByViability();

        IntialiseReplacementCompetition();

        ////////// NOTE: CREATE COMPARER INSTANCE AND CREATE SORTED LIST OF SPACES - then resort using below steps //////
        ElementSpaceCompareCompetition spaceComp = new ElementSpaceCompareCompetition();

        // List ordering replacement //
        List<ElementSpace> replacementOrderList = new List<ElementSpace>();

        // For Each Space - Insert into Replacement Order List //
        // sort by 'competition' and then by trace to least-viable point //
        for (int i = 0; i < allSpaces.Count; i++)
        {
            // If can be replaced i.e. if has competing elements //
            if (allSpaces[i].CompetingElements.Count > 0)
            {
                // Insert into Viability_List using Similarty to PM Calc //
                int index = replacementOrderList.BinarySearch(allSpaces[i], spaceComp);

                // If not in list //
                if (index < 0)
                    replacementOrderList.Insert(~index, allSpaces[i]);
            }
        }

        // repeat while can still replace //
        //while (replacementOrderList.Count > 0)
        if (replacementOrderList.Count > 0)
        {
            while (replacementOrderList[replacementOrderList.Count - 1].CompetingElements.Count > 0)
            {
                // remove replacement element from list of contestants of w-set elements //
                // add replaced element to contestant list of trace down
                // resort order of operations by competition and least-viable trace etc //
                StepReplacementCompetition(ref replacementOrderList);


                // NOTE: There is an infinite loop here - a competing element is not being removed //
            }
        }

        // Update Text //
        OutputManager.RefreshText();
    }

    // In a turn-based fashion - update replacement //
    public static void StepReplacementCompetition(ref List<ElementSpace> replacementOrderList)
    {
        // Choose Replacing Element - if sorted competition list, this should be the last element of the competition list //
        int orderIndex = replacementOrderList.Count - 1;
        ElementPoint replacingPoint = replacementOrderList[orderIndex].CompetingElements[replacementOrderList[orderIndex].CompetingElements.Count - 1];

        // remove replaced element from previous housing space //
        ElementPoint replacedPoint = replacementOrderList[orderIndex].HousedElement;

        // place new element in space //
        replacementOrderList[orderIndex].HousedElement = replacingPoint;
        replacementOrderList[orderIndex].CompetingElements.Remove(replacingPoint); // trying to make sure - a test (less efficient)

        // Update Space's Lowest Trace //
        replacementOrderList[orderIndex].LowestTraceIndex = replacementOrderList[orderIndex].LowestChainIndex();

        voidESet.Remove(replacingPoint);
        voidESet.Add(replacedPoint);
        worldESet.Remove(replacedPoint);
        worldESet.Add(replacingPoint);

        // Update History for Output //
        OutputManager.WorldToVoidSetHistory[0].Remove(replacingPoint.PlotPointIndex);   // removes if already there
        OutputManager.VoidToWorldSetHistory[0].Add(replacingPoint.PlotPointIndex);
        OutputManager.VoidToWorldSetHistory[0].Remove(replacedPoint.PlotPointIndex);    // removes if already there
        OutputManager.WorldToVoidSetHistory[0].Add(replacedPoint.PlotPointIndex);       

        // Comparer //
        ElementCompareViaIndex eCompViaIndex = new ElementCompareViaIndex();

        // Get potential spaces of replaced element //
        // For Each Space Potential Space //
        for (int j = 0; j < replacedPoint.PotentialSpaces.Count; j++)
        {
            if (unavailableSpaces.Contains(replacedPoint.PotentialSpaces[j]))
                continue;

            // add replaced element to competition list of all potential spaces - if viable enough to replace //
            if (replacedPoint.ViabilityIndex > replacedPoint.PotentialSpaces[j].HousedElement.ViabilityIndex)
            {
                // Insert into competing elements using Viability Index //
                int index = replacedPoint.PotentialSpaces[j].CompetingElements.BinarySearch(replacedPoint, eCompViaIndex);

                // If not in list //
                if (index < 0)
                    replacedPoint.PotentialSpaces[j].CompetingElements.Insert(~index, replacedPoint);
            }
        }

        // Get potential spaces of replacing element //
        for (int j = 0; j < replacingPoint.PotentialSpaces.Count; j++)
        {
            if (unavailableSpaces.Contains(replacingPoint.PotentialSpaces[j]))
                continue;

            // remove replacing element from competition list of each space - if there //
            int index = replacingPoint.PotentialSpaces[j].CompetingElements.BinarySearch(replacingPoint, eCompViaIndex);

            // If in list //
            if (index >= 0)
                replacingPoint.PotentialSpaces[j].CompetingElements.RemoveAt(index);
        }

        // Re-sort list if not empty //
        if (replacementOrderList.Count > 0)
        {
            // Space Comparer //
            ElementSpaceCompareCompetition eSpaceComp = new ElementSpaceCompareCompetition();

            // recalculate competition difference for each updated space
            replacementOrderList.Sort(eSpaceComp);
        }
    }
}
