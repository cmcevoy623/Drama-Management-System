using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This Entire Class is The Setup of an Acrylic Directed Graph Structure - Uses RBS Design Considerations //
public static class PlotPointSpace {

    // Acrylic Directed Graph of Points //
    static List<PlotPoint> plotPoints = new List<PlotPoint>();

    // Unique Point ID //
    public static int pID = -1;
    public static int spaceSize = 150;  // most need conditions and effects

    // Return Full Set //
    public static List<PlotPoint> PPS_FullSet() { return plotPoints; }

    static void InitTestData0(ref List<List<int>> conds, ref List<int> effs)
    {
        //List<int> example = new List<int>() { 5, 4, 3 }; // NOTE: may replace current params with this method

        // Conditions for Each Plot Point - Essentially a Spreadsheet of Data in Variable Format //    
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE)); // INDEX 0
        //conds.Add(InitCondDataRow(new int[] { 0 })); effs.AddRange(new int[] { 2, 7 }); // INDEX 1
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds, effs)); conds.Clear(); effs.Clear();
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE)); // INDEX 2
        //conds.Add(InitCondDataRow(new int[] { 2 })); conds.Add(InitCondDataRow(new int[] { 0 })); effs.Add(5); // INDEX 3
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds, effs)); conds.Clear(); effs.Clear();
        //conds.Add(InitCondDataRow(new int[] { 1 })); conds.Add(InitCondDataRow(new int[] { 2 })); effs.Add(8); effs.Add(9); // INDEX 4
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds, effs)); conds.Clear(); effs.Clear();
        //conds.Add(InitCondDataRow(new int[] { 0 })); // INDEX 5
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds)); conds.Clear();
        //conds.Add(InitCondDataRow(new int[] { 4, 5 })); // INDEX 6
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds)); conds.Clear();
        //conds.Add(InitCondDataRow(new int[] { 4 })); // INDEX 7
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds)); conds.Clear();
        //conds.Add(InitCondDataRow(new int[] { 3 })); conds.Add(InitCondDataRow(new int[] { 7 })); // INDEX 8
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds, true)); conds.Clear();
        //conds.Add(InitCondDataRow(new int[] { 7 })); // INDEX 9
        //plotPoints.Add(new PlotPoint(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, conds, true)); conds.Clear();
    }

    static void InitTestData1(ref List<List<int>> conds, ref List<int> effs)
    {
        plotPoints.Add(new PlotPoint());                                // INDEX 0 //
        conds.Add(new List<int>() { 0 });
        plotPoints.Add(new PlotPoint(conds)); conds.Clear();            // INDEX 1 //
        plotPoints.Add(new PlotPoint());                                // INDEX 2 //
        conds.Add(new List<int>() { 1, 2 });
        plotPoints.Add(new PlotPoint(conds)); conds.Clear();            // INDEX 3 //
        conds.Add(new List<int>() { 3 });
        plotPoints.Add(new PlotPoint(conds)); conds.Clear();            // INDEX 4 //
        conds.Add(new List<int>() { 4 });
        plotPoints.Add(new PlotPoint(conds, true)); conds.Clear();      // INDEX 5 (END) //
        effs.Add(1);
        plotPoints.Add(new PlotPoint(effs)); effs.Clear();              // INDEX 6 //

        plotPoints.Add(new PlotPoint());                                // INDEX 7 //
        plotPoints.Add(new PlotPoint());                                // INDEX 8 //
        conds.Add(new List<int>() { 7 });                                                // 2 AND 1 OR STRUCTURE //
        conds.Add(new List<int>() { 8, 11 });
        //conds.Add(new List<int>() { 8 });
        plotPoints.Add(new PlotPoint(conds)); conds.Clear();            // INDEX 9 //
        conds.Add(new List<int>() { 9 });
        plotPoints.Add(new PlotPoint(conds, true)); conds.Clear();      // INDEX 10 (END) //
        plotPoints.Add(new PlotPoint());                                // INDEX 11 //

        plotPoints[0].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 0);
        plotPoints[1].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 1);
        plotPoints[2].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 2);
        plotPoints[3].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 3);
        plotPoints[4].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 4);
        plotPoints[5].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 5);
        plotPoints[6].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, 6);
    }

    //static void InitTestData2(ref List<List<int>> conds, ref List<int> effs)
    //{
        /*
        effs.Add(35); effs.Add(120);
        plotPoints[34].SetEffects(effs); effs.Clear();
        effs.Add(90);
        plotPoints[35].SetEffects(effs); effs.Clear();
        conds.Add(new List<int>() { 1 });
        plotPoints[64].SetPreconditionLogic(conds); conds.Clear();
        conds.Add(new List<int>() { 64 }); conds.Add(new List<int>() { 15 });
        effs.Add(122); effs.Add(117);
        plotPoints[65].SetPreconditionLogic(conds); plotPoints[65].SetEffects(effs); conds.Clear(); effs.Clear();
        conds.Add(new List<int>() { 65 });
        plotPoints[66].SetPreconditionLogic(conds); conds.Clear();
        conds.Add(new List<int>() { 66 }); conds.Add(new List<int>() { 52, 135 });
        plotPoints[67].SetPreconditionLogic(conds); conds.Clear();
        conds.Add(new List<int>() { 67 });
        effs.Add(85);
        plotPoints[68].SetPreconditionLogic(conds); plotPoints[68].SetEffects(effs); conds.Clear(); effs.Clear();
        conds.Add(new List<int>(68));
        plotPoints[69].SetIsEndpoint(true);
        plotPoints[69].SetPreconditionLogic(conds); conds.Clear();

        new List<List<int>>() { new List<int>() { 1 }, new List<int>() { 2 } };
        */
    //}

    // Constructor for class //
    static PlotPointSpace()
    {
        // List of Lists //
        List<List<int>> conds = new List<List<int>>();
        List<int> effs = new List<int>();

        // Setup Directed Graph Structure //
        for (int i = 0; i < spaceSize; i++)
        {
            plotPoints.Add(new PlotPoint());                                        // INDEX i //
            plotPoints[i].SetPointInfo(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, i);
        }


        // Assign Conditions and Effects //
        plotPoints[0].SetEffects(new List<int>() { 1 });
        plotPoints[34].SetEffects(new List<int>() { 35, 120 });
        plotPoints[35].SetEffects(new List<int>() { 90 });
        plotPoints[64].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 1 } });
        plotPoints[65].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 64 }, new List<int>() { 15 }, new List<int>() { 10 }, new List<int>() { 145 }, new List<int>() { 146 } });
        plotPoints[65].SetEffects(new List<int>() { 122, 117 });
        plotPoints[66].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 65 } });
        plotPoints[67].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 66 }, new List<int>() { 52, 135 } });
        plotPoints[68].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 67 } });
        plotPoints[68].SetEffects(new List<int>() { 85 });
        plotPoints[69].SetIsEndpoint(true);
        plotPoints[69].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 68 } });
        plotPoints[70].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 68 } });
        plotPoints[71].SetIsEndpoint(true);
        plotPoints[71].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 70 } });
        plotPoints[72].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 0, 2 }, new List<int>() { 0, 5 } });
        plotPoints[73].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 5, 8 } });
        plotPoints[74].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 8 }, new List<int>() { 10, 21 } });
        plotPoints[75].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 73 } });
        plotPoints[76].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 72 }, new List<int>() { 75 } });
        plotPoints[76].SetEffects(new List<int>() { 81 });
        plotPoints[77].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 75 }, new List<int>() { 74 } });
        plotPoints[77].SetEffects(new List<int>() { 78 });
        plotPoints[78].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 76 } });
        plotPoints[79].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 77 } });
        plotPoints[80].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 77 } });
        plotPoints[80].SetEffects(new List<int>() { 79 });
        plotPoints[81].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 80 } });
        plotPoints[82].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 78 } });
        plotPoints[82].SetEffects(new List<int>() { 74 });
        plotPoints[83].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 78} });
        plotPoints[83].SetEffects(new List<int>() { 101 });
        plotPoints[84].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 79 } });
        plotPoints[84].SetEffects(new List<int>() { 101 });
        plotPoints[85].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 80 } });
        plotPoints[86].SetIsEndpoint(true);
        plotPoints[86].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 82 }, new List<int>() { 83 }, new List<int>() { 84 }, new List<int>() { 85 } });
        plotPoints[87].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 76 } });
        plotPoints[88].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 87 } });
        plotPoints[89].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 53 } });
        plotPoints[90].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 89 } });
        plotPoints[91].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 89 } });
        plotPoints[92].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 89 } });
        plotPoints[93].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 90, 91 }, new List<int>() { 92 } });
        plotPoints[93].SetEffects(new List<int>() { 100 });
        plotPoints[94].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 93 } });
        plotPoints[95].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 94 } });
        plotPoints[96].SetIsEndpoint(true);
        plotPoints[96].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 95 } });
        plotPoints[97].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 60, 53 } });
        plotPoints[98].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 60, 53 } });
        plotPoints[99].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 97, 98 } });
        plotPoints[100].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 99 } });
        plotPoints[101].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 100 } });
        plotPoints[101].SetEffects(new List<int>() { 93 });
        plotPoints[102].SetIsEndpoint(true);
        plotPoints[102].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 101 } });
        plotPoints[103].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 93 } });
        plotPoints[104].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 103 } });
        plotPoints[105].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 104 }, new List<int>() { 100 } });
        plotPoints[106].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 105 } });
        plotPoints[107].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 106, 93 } });
        plotPoints[108].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 106, 100 } });
        plotPoints[109].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 98 } });
        plotPoints[110].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 99 } });
        plotPoints[110].SetEffects(new List<int>() { 92 });
        plotPoints[111].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 3 } });
        plotPoints[112].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 3 } });
        plotPoints[112].SetEffects(new List<int>() { 92 });
        plotPoints[113].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 40 } });
        plotPoints[114].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 113 } });
        plotPoints[115].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 113 } });
        plotPoints[116].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 35 } });
        plotPoints[117].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 116 } });
        plotPoints[118].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 116, 119 } });
        plotPoints[119].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 113 } });
        plotPoints[120].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 12 }, new List<int>() { 14 } });
        plotPoints[120].SetEffects(new List<int>() { 91 });
        plotPoints[121].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 120 } });
        plotPoints[122].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 121 } });
        plotPoints[123].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 121 } });
        plotPoints[124].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 121 } });
        plotPoints[125].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 55 } });
        plotPoints[125].SetEffects(new List<int>() { 91 });
        plotPoints[126].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 125 } });
        plotPoints[127].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 126 } });
        plotPoints[128].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 126 } });
        //plotPoints[129].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 64 } });
        plotPoints[130].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 129 } });
        plotPoints[130].SetEffects(new List<int>() { 103, 3, 6 });
        //plotPoints[131].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 64 } });
        plotPoints[131].SetEffects(new List<int>() { 98, 112 });
        //plotPoints[132].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 64 } });
        plotPoints[133].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 67 } });
        plotPoints[134].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 67 } });
        plotPoints[134].SetEffects(new List<int>() { 16, 17, 106 });
        plotPoints[135].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 33 }, new List<int>() { 34 } });
        plotPoints[136].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 19, 20 }, new List<int>() { 21 } });
        plotPoints[137].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 21 } });
        plotPoints[138].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 136 } });
        plotPoints[139].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 137 } });
        plotPoints[140].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 139 } });
        plotPoints[141].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 140 } });
        plotPoints[142].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 141 } });
        plotPoints[143].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 138 } });
        plotPoints[144].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 143 } });
        plotPoints[148].SetIsEndpoint(true);
        plotPoints[148].SetPreconditionLogic(new List<List<int>>() { new List<int>() { 142 } });

        // Setup Connection to Information //
        // INSERT INFO TYPE AND ID FOR EACH PLOT POINT HERE //
        // For each point, call InformationEnactor.InitPlotPointInfo to connect the plot point to the information via a shared index //
        for (int i = 0; i < plotPoints.Count; i++)
            InformationEnactor.InitPlotPointInfo(plotPoints[i].GetPointType(), plotPoints[i].GetInfoID(), plotPoints[i].GetID());


        // Initialise Next Points //
        InitNextPoints();

        // Reverse Effects Lists //
        SetupEffectsTrace();

        // Initialise End Point Connections and Traceback //
        SetupEndPointConnections();
    }

    // Initialise Next Points - Setting a Reference of a PlotPoint to its Preconditions //
    static void InitNextPoints()
    {
        // Initialise Next Points //
        for (int i = 0; i < plotPoints.Count; i++)
        {
            // Get Precondition Logic - Flatten with Function //
            List<int> previousPoints = new List<int>(FlattenList(plotPoints[i].GetPreconditionLogic()));

            // For Each (Existing) PlotPoint In This List //
            for (int l = 0; l < previousPoints.Count; l++)
            {
                // Add Current Plot Point to its 'Next' point list - so that it knows this point should be opened next //
                if (previousPoints[l] < plotPoints.Count)
                    plotPoints[previousPoints[l]].AddNextPoint(plotPoints[i].GetID());
            }
        }
    }

    // Convert 2D list into single list with no repeated elements //
    public static List<int> FlattenList(List<List<int>> multiList)
    {
        List<int> flattenedList = new List<int>();
        List<List<int>> fatList = new List<List<int>>(multiList);

        // Convert Preconditions Into a Flat List //
        for (int j = 0; j < fatList.Count; j++)
        {
            for (int k = 0; k < fatList[j].Count; k++)
            {
                // Add to Flat List - if not already there //
                if (flattenedList.Contains(fatList[j][k]) == false)
                    flattenedList.Add(fatList[j][k]);
            }
        }

        return flattenedList;
    }

    // Reverse Effects Trace //
    static void SetupEffectsTrace()
    {
        // Add Off Switches (i.e. sources) to Each Plot Point that is Listed as an Effect //
        for (int i = 0; i < plotPoints.Count; i++)
        {
            // Add Plot Point to Effect's List of Off Switches //
            List<int> effects = new List<int>(plotPoints[i].GetEffects());
            for (int j = 0; j < effects.Count; j++)
            {
                plotPoints[effects[j]].AddOffSwitch(i);

                // NOTE: GET BACK TO THIS AFTER ENDPOINT DEPENDENCY UPDATE CHECK //
                if (plotPoints[effects[j]].GetEffects().Contains(i) == false)
                {
                    // Check if Effects Have This Point Stored as an Effect- Update Effects of Effects and Add OffSwitch to This Point //
                    // NOTE: may not need offswitch anymore, and only use effects //
                    plotPoints[effects[j]].GetEffects().Add(i);
                    plotPoints[i].AddOffSwitch(effects[j]);
                }
            }
        }
    }

    // Backprop endPoint to all potentially connected nodes //
    static void SetupEndPointConnections()
    {
        // Get All EndPoints to Cycle Through //
        List<int> endPoints = DramaManager.DM_EndPoints();

        // For Each Point in the Current Progress of the Recursive Function //
        for (int i = 0; i < endPoints.Count; i++)
        {
            // Get Previous Points and Off Switches //
            List<int> previousPoints = new List<int>(FlattenList(plotPoints[endPoints[i]].GetPreconditionLogic()));

            // Add End Point Connections to All Points that can Potentially Prevent Reaching an Endpoint - Off Switches and Previous Steps 
            // - The logic of deciding what to show will be checked at each step, no need to set it up here -

            // Initialise EndPoint Connection to Have Reference to Self //
            plotPoints[endPoints[i]].AddEndPointConnection(endPoints[i]);

            // Propate Through Previous and Off Switches //
            for (int j = 0; j < previousPoints.Count; j++)
                BackPropogateConnections(previousPoints[j], endPoints[i]);
        }
    }

    // Propagate EndPoint Backwards //
    static void BackPropogateConnections(int point, int endPointSource)
    {
        // If Plot Point does not already have a connection to the source end point - add it to the list and propagate backwards //
        if (plotPoints[point].GetEndPointConnections().Contains(endPointSource) == false)
        {
            // Add EndPoint Connection //
            plotPoints[point].AddEndPointConnection(endPointSource);

            // Get Previous Points and Off Switches //
            List<int> previousPoints = new List<int>(FlattenList(plotPoints[point].GetPreconditionLogic()));

            // Propate Through Previous and Off Switches //
            for (int j = 0; j < previousPoints.Count; j++)
                BackPropogateConnections(previousPoints[j], endPointSource);
        }
    }

    // DECREPATED //
    // Return a row of Plot Point IDs to be added to a plot point's precondition matrix //
    static List<int> InitCondDataRow(int[] condInts)        // may replace with alternative parameter method that may be more efficient
    {
        // Put Together Row of Plot Point Indices to be ANDed Together in the Condition Logic Tests //
        List<int> conds = new List<int>();

        // Convert Array of Ints into List of Ints //
        for (int i = 0; i < condInts.Length; i++)
            conds.Add(condInts[i]);

        return conds;
    }
}
