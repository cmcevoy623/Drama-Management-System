using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PageTextBank {

    // Array of TextPages
    // Textpages should be added to one or more information pools that can be accessed
    static List<string> textPages = new List<string>();                         // text content - access via infoID
    static List<List<ElementSpace>> infoSpaces = new List<List<ElementSpace>>();                // where each text could be found - access via infoID
    // NOTE: Connected Spaces via Info Pools?? List of list of spaces?? 
    static List<PointInfoVals> textPagePointInfo = new List<PointInfoVals>();   // The weights and plotpointIDs - accessed via infoID
    //static PlotPoint.PLOTPOINTTYPE bankType = PlotPoint.PLOTPOINTTYPE.TEXTPAGE;

    // World Elements //
    static List<Room> worldRooms;

    // Preset Info Pool Groups //
    //static List<List<Room>> infoPools = new List<List<Room>>();
    static List<List<ElementSpace>> infoPools = new List<List<ElementSpace>>();

    static PageTextBank()
    {
        InitRooms();
        InitTextPools();
        InitTextPages();
    }

    // Getters and Setters //
    public static PointInfoVals GetPointInfoValElement(int infoID) { return textPagePointInfo[infoID]; }
    public static string GetTextPage(int index) { return textPages[index]; }

    // NOTE: May be Temporary //
    public static bool CheckIDValid(int queryID) { return queryID >= 0 && queryID < textPages.Count; }
    public static bool CheckWeightsExist(int queryID) { return queryID >= 0 && queryID < textPagePointInfo.Count; }

    public static List<ElementSpace> GetPlotPointPotentialSpaces(int infoID)
    {
        // Return the list of element spaces for a given plot point - located via its connected information id //

        //return infoSpaces[infoID];

        List<ElementSpace> spaces = new List<ElementSpace>(textPagePointInfo[infoID].GetPossibleSpaces());

        return spaces;
    }

    public static int BankLength() { return textPagePointInfo.Count; }

    public static List<ElementSpace> GetWorldTextSpaces()
    {
        List<ElementSpace> worldTextSpaces = new List<ElementSpace>();

        // Add all spaces from each room to the space list - this will only be done once //
        for (int i = 0; i < worldRooms.Count; i++)
            worldTextSpaces.AddRange(worldRooms[i].GetUnaccessedElementSpaces());

        return worldTextSpaces;
    }

    // Initialise Monitor Array //
    static void InitRooms()
    {
        // Get Rooms in Order //
        RoomStructure monitorRooms = Object.FindObjectOfType<RoomStructure>();
        if (monitorRooms)
            worldRooms = new List<Room>(monitorRooms.levelRoomsOrdered);
    }

    // Initialise Information Pools so that Access to room Monitors is Given //
    static void InitTextPools()
    {
        //// GLOBAL POOL //
        //infoPools.Add(new List<Room>(worldRooms));
        //// LOCAL CONTROL AREA //
        //infoPools.Add(new List<Room>(new Room[] { worldRooms[0], worldRooms[1], worldRooms[2], worldRooms[3] }));
        //// LOCAL INTEL AND EQUIPMENT AREA
        //infoPools.Add(new List<Room>(new Room[] { worldRooms[4], worldRooms[5], worldRooms[6] }));
        //// LOCAL MECHANICAL AREA //
        //infoPools.Add(new List<Room>(new Room[] { worldRooms[7], worldRooms[8], worldRooms[9], worldRooms[10] }));
        //// LOCAL SILO, BATTERY AND MISSILE CONTROL AREA //
        //infoPools.Add(new List<Room>(new Room[] { worldRooms[11], worldRooms[12], worldRooms[13] }));
        //// LOCAL BERTHING AND STATUS ROOM AREA //
        //infoPools.Add(new List<Room>(new Room[] { worldRooms[14], worldRooms[15], worldRooms[16] }));


        // Room Indices //
        // 0. Control Room
        // 1. Sonar Room
        // 2. Radio Room
        // 3. CRT Room
        // 4. Recording Room
        // 5. Intelligence Room
        // 6. Equipment Room
        // 7. Maneuvering Room
        // 8. Recreation Room
        // 9. Engine Room
        //10. Hovering Pumps Room
        //11. Silo Control Room
        //12. Battery Room
        //13. Missile Launch Room
        //14. Status Room
        //15. Officer Berthing
        //16. Captain Quarters

        ///// PREDEFINED POOLS /////

        // GLOBAL POOL //
        infoPools.Add(new List<ElementSpace>(GetWorldTextSpaces()));                    // INDEX 0 //
        // LOCAL CONTROL AREA //
        infoPools.Add(CreateElementSpaceGroupFromRooms(new int[] { 0, 1, 2, 3 }));      // INDEX 1 //
        // LOCAL INTEL AND EQUIPMENT AREA
        infoPools.Add(CreateElementSpaceGroupFromRooms(new int[] { 4, 5, 6 }));         // INDEX 2 //
        // LOCAL MECHANICAL AREA //
        infoPools.Add(CreateElementSpaceGroupFromRooms(new int[] { 7, 8, 9, 10 }));     // INDEX 3 //
        // LOCAL SILO, BATTERY AND MISSILE CONTROL AREA //
        infoPools.Add(CreateElementSpaceGroupFromRooms(new int[] { 11, 12, 13 }));      // INDEX 4 //
        // LOCAL BERTHING AND STATUS ROOM AREA //
        infoPools.Add(CreateElementSpaceGroupFromRooms(new int[] { 14, 15, 16 }));      // INDEX 5 //
    }

    static List<ElementSpace> CreateElementSpaceGroupFromRooms(int[] roomIDs)
    {
        List<ElementSpace> spaces = new List<ElementSpace>();

        // Get Unaccessed Spaces of one or more rooms //
        for (int i = 0; i < roomIDs.Length; i++)
            spaces.AddRange(worldRooms[roomIDs[i]].GetUnaccessedElementSpaces());

        return spaces;
    }

    // All the text associated with each textpage plot point - assigning info pools (both preset and custom) //
    static void InitTextPages()
    {
        // This is just some hard-coded initialisation for testing //
        int monitorSpaces = 2;
        // Current max index //
        // 11
        int mainControlSpaces = monitorSpaces * 6 - 1; // where 6 is the total number of monitors in this region - no point in not hard coding this atm
        // 23
        int intelligenceSpaces = monitorSpaces * 6 + mainControlSpaces;
        // 37
        int mechanicalSpaces = monitorSpaces * 7 + intelligenceSpaces;
        // 51
        int siloPowerMissileSpaces = monitorSpaces * 7 + mechanicalSpaces;
        // 63
        int berthingStatusSpace = monitorSpaces * 6 + siloPowerMissileSpaces;

        Random.InitState(0);                // want the same random values
        for (int i = 0; i < PlotPointSpace.spaceSize; i++)
        {
            float[] weights = new float[] { Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f),
                                            Random.Range(0.25f, 0.75f), Random.Range(0.25f, 0.75f) };
            textPagePointInfo.Add(new PointInfoVals(weights, i));

            // Simple Hard Coded - Can change later as appropriate //
            int infoPoolNo = 0;
            if (i > mainControlSpaces)
                infoPoolNo = Random.Range(0, infoPools.Count);
            else if (i > intelligenceSpaces)
                infoPoolNo = 5;
            else if (i > mechanicalSpaces)
                infoPoolNo = 4;
            else if (i > intelligenceSpaces)
                infoPoolNo = 3;
            else if (i > mainControlSpaces)
                infoPoolNo = 2;
            else
                infoPoolNo = 1;

            textPagePointInfo[textPagePointInfo.Count - 1].SetPossibleSpaces(infoPools[infoPoolNo]);
        }

        // Set Weights and Locations By Hand for Test Data in PlotPossibility Space //
        textPagePointInfo[0].SetWeights(new float[]     { 0.3f, 0.4f, 0.5f, 0.7f });        textPagePointInfo[0].InitOriginalWeights(); // need to reinitialise original weights
        textPagePointInfo[1].SetWeights(new float[]     { 0.6f, 0.6f, 0.4f, 0.4f });        textPagePointInfo[1].InitOriginalWeights();
        textPagePointInfo[15].SetWeights(new float[]    { 0.6f, 0.6f, 0.4f, 0.4f });        textPagePointInfo[15].InitOriginalWeights();
        textPagePointInfo[34].SetWeights(new float[]    { 0.7f, 0.6f, 0.6f, 0.3f });        textPagePointInfo[34].InitOriginalWeights();
        textPagePointInfo[35].SetWeights(new float[]    { 0.6f, 0.7f, 0.3f, 0.5f });        textPagePointInfo[35].InitOriginalWeights();
        textPagePointInfo[53].SetWeights(new float[]    { 0.75f, 0.25f, 0.75f, 0.25f });    textPagePointInfo[53].InitOriginalWeights();
        textPagePointInfo[64].SetWeights(new float[]    { 0.7f, 0.7f, 0.4f, 0.3f });        textPagePointInfo[64].InitOriginalWeights();
        textPagePointInfo[65].SetWeights(new float[]    { 0.75f, 0.55f, 0.45f, 0.4f });     textPagePointInfo[65].InitOriginalWeights();
        textPagePointInfo[66].SetWeights(new float[]    { 0.7f, 0.6f, 0.4f, 0.4f });        textPagePointInfo[66].InitOriginalWeights();
        textPagePointInfo[67].SetWeights(new float[]    { 0.8f, 0.8f, 0.4f, 0.5f });        textPagePointInfo[67].InitOriginalWeights();
        textPagePointInfo[68].SetWeights(new float[]    { 0.8f, 0.6f, 0.4f, 0.3f });        textPagePointInfo[68].InitOriginalWeights();
        textPagePointInfo[69].SetWeights(new float[]    { 0.8f, 0.6f, 0.4f, 0.3f });        textPagePointInfo[69].InitOriginalWeights();
        textPagePointInfo[70].SetWeights(new float[]    { 0.5f, 0.9f, 0.6f, 0.7f });        textPagePointInfo[70].InitOriginalWeights();
        textPagePointInfo[71].SetWeights(new float[]    { 0.6f, 0.9f, 0.5f, 0.4f });        textPagePointInfo[71].InitOriginalWeights();
        textPagePointInfo[72].SetWeights(new float[]    { 0.3f, 0.4f, 0.7f, 0.6f });        textPagePointInfo[72].InitOriginalWeights();
        textPagePointInfo[73].SetWeights(new float[]    { 0.7f, 0.35f, 0.7f, 0.8f });       textPagePointInfo[73].InitOriginalWeights();
        textPagePointInfo[74].SetWeights(new float[]    { 0.6f, 0.1f, 0.2f, 0.8f });        textPagePointInfo[74].InitOriginalWeights();
        textPagePointInfo[75].SetWeights(new float[]    { 0.5f, 0.1f, 0.95f, 0.9f });       textPagePointInfo[75].InitOriginalWeights();
        textPagePointInfo[76].SetWeights(new float[]    { 0.5f, 0.5f, 0.5f, 0.5f });        textPagePointInfo[76].InitOriginalWeights();
        textPagePointInfo[77].SetWeights(new float[]    { 0.3f, 0.2f, 0.75f, 0.75f });      textPagePointInfo[77].InitOriginalWeights();
        textPagePointInfo[78].SetWeights(new float[]    { 0.25f, 0.25f, 0.55f, 0.55f });    textPagePointInfo[78].InitOriginalWeights();
        textPagePointInfo[79].SetWeights(new float[]    { 0.4f, 0.5f, 0.6f, 0.3f });        textPagePointInfo[79].InitOriginalWeights();
        textPagePointInfo[80].SetWeights(new float[]    { 0.2f, 0.7f, 0.8f, 0.2f });        textPagePointInfo[80].InitOriginalWeights();
        textPagePointInfo[81].SetWeights(new float[]    { 0.6f, 0.3f, 0.9f, 0.5f });        textPagePointInfo[81].InitOriginalWeights();
        textPagePointInfo[82].SetWeights(new float[]    { 0.1f, 0.1f, 0.7f, 0.9f });        textPagePointInfo[82].InitOriginalWeights();
        textPagePointInfo[83].SetWeights(new float[]    { 0.5f, 0.1f, 0.5f, 0.6f });        textPagePointInfo[83].InitOriginalWeights();
        textPagePointInfo[84].SetWeights(new float[]    { 0.45f, 0.6f, 0.7f, 0.2f });       textPagePointInfo[84].InitOriginalWeights();
        textPagePointInfo[85].SetWeights(new float[]    { 0.5f, 0.5f, 0.5f, 0.5f });        textPagePointInfo[85].InitOriginalWeights();
        textPagePointInfo[86].SetWeights(new float[]    { 0.8f, 0.2f, 0.65f, 0.5f });       textPagePointInfo[86].InitOriginalWeights();
        textPagePointInfo[87].SetWeights(new float[]    { 0.2f, 0.2f, 0.2f, 0.2f });        textPagePointInfo[87].InitOriginalWeights();
        textPagePointInfo[88].SetWeights(new float[]    { 0.1f, 0.5f, 0.1f, 0.5f });        textPagePointInfo[88].InitOriginalWeights();
        textPagePointInfo[89].SetWeights(new float[]    { 0.75f, 0.25f, 0.75f, 0.25f });    textPagePointInfo[89].InitOriginalWeights();
        textPagePointInfo[90].SetWeights(new float[]    { 0.75f, 0.25f, 0.75f, 0.25f });    textPagePointInfo[90].InitOriginalWeights();
        textPagePointInfo[91].SetWeights(new float[]    { 0.75f, 0.25f, 0.75f, 0.25f });    textPagePointInfo[91].InitOriginalWeights();
        textPagePointInfo[92].SetWeights(new float[]    { 0.75f, 0.25f, 0.75f, 0.25f });    textPagePointInfo[92].InitOriginalWeights();
        textPagePointInfo[93].SetWeights(new float[]    { 0.9f, 0.1f, 0.9f, 0.1f });        textPagePointInfo[93].InitOriginalWeights();
        textPagePointInfo[94].SetWeights(new float[]    { 0.9f, 0.1f, 0.9f, 0.1f });        textPagePointInfo[94].InitOriginalWeights();
        textPagePointInfo[95].SetWeights(new float[]    { 0.95f, 0.05f, 0.95f, 0.05f });    textPagePointInfo[95].InitOriginalWeights();
        textPagePointInfo[96].SetWeights(new float[]    { 0.95f, 0.05f, 0.95f, 0.05f });    textPagePointInfo[96].InitOriginalWeights();
        textPagePointInfo[97].SetWeights(new float[]    { 0.25f, 0.75f, 0.25f, 0.75f });    textPagePointInfo[97].InitOriginalWeights();
        textPagePointInfo[98].SetWeights(new float[]    { 0.25f, 0.75f, 0.25f, 0.75f });    textPagePointInfo[98].InitOriginalWeights();
        textPagePointInfo[99].SetWeights(new float[]    { 0.25f, 0.75f, 0.25f, 0.75f });    textPagePointInfo[99].InitOriginalWeights();
        textPagePointInfo[100].SetWeights(new float[]   { 0.1f, 0.9f, 0.1f, 0.9f });        textPagePointInfo[100].InitOriginalWeights();
        textPagePointInfo[101].SetWeights(new float[]   { 0.05f, 0.95f, 0.05f, 0.95f });    textPagePointInfo[101].InitOriginalWeights();
        textPagePointInfo[102].SetWeights(new float[]   { 0.05f, 0.95f, 0.05f, 0.95f });    textPagePointInfo[102].InitOriginalWeights();
        textPagePointInfo[103].SetWeights(new float[]   { 0.65f, 0.35f, 0.65f, 0.35f });    textPagePointInfo[103].InitOriginalWeights();
        textPagePointInfo[104].SetWeights(new float[]   { 0.45f, 0.55f, 0.45f, 0.55f });    textPagePointInfo[104].InitOriginalWeights();
        textPagePointInfo[105].SetWeights(new float[]   { 0.5f, 0.5f, 0.5f, 0.5f });        textPagePointInfo[105].InitOriginalWeights();
        textPagePointInfo[106].SetWeights(new float[]   { 0.6f, 0.6f, 0.6f, 0.6f });        textPagePointInfo[106].InitOriginalWeights();
        textPagePointInfo[107].SetWeights(new float[]   { 0.7f, 0.8f, 0.6f, 0.4f });        textPagePointInfo[107].InitOriginalWeights();
        textPagePointInfo[108].SetWeights(new float[]   { 0.4f, 0.6f, 0.8f, 0.7f });        textPagePointInfo[108].InitOriginalWeights();
        textPagePointInfo[109].SetWeights(new float[]   { 0.15f, 0.85f, 0.15f, 0.85f });    textPagePointInfo[109].InitOriginalWeights();
        textPagePointInfo[110].SetWeights(new float[]   { 0.15f, 0.85f, 0.15f, 0.85f });    textPagePointInfo[110].InitOriginalWeights();

        // Set Locations //
        textPagePointInfo[0].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 0 }));
        textPagePointInfo[1].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 0 }));
        textPagePointInfo[2].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 1, 6 }));
        textPagePointInfo[3].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[4].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 1 }));
        textPagePointInfo[5].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 1, 6, 7, 8 }));
        textPagePointInfo[6].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[7].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[8].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 10, 12 }));
        textPagePointInfo[9].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 2 }));
        textPagePointInfo[10].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 7, 9, 10, 11, 12 }));
        textPagePointInfo[13].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 4 }));
        textPagePointInfo[14].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 4 }));
        textPagePointInfo[21].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 9, 12 }));
        textPagePointInfo[22].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 6 }));
        textPagePointInfo[23].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 6 }));
        textPagePointInfo[24].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 7 }));
        textPagePointInfo[28].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 9 }));
        textPagePointInfo[29].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 9 }));
        textPagePointInfo[34].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 10 }));
        textPagePointInfo[35].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 11 }));
        textPagePointInfo[38].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 11 }));
        textPagePointInfo[39].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 11 }));
        textPagePointInfo[40].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 12 }));
        textPagePointInfo[46].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 13 }));
        textPagePointInfo[53].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 2, 14 }));
        textPagePointInfo[60].SetPossibleSpaces(infoPools[5]);
        textPagePointInfo[64].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 0 }));
        textPagePointInfo[65].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 1 }));
        textPagePointInfo[66].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 1 }));
        textPagePointInfo[67].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 3 }));
        textPagePointInfo[68].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 3 }));
        textPagePointInfo[69].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 11, 13 }));
        textPagePointInfo[70].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 11 }));
        textPagePointInfo[71].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 13 }));
        textPagePointInfo[72].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[73].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 12 }));
        textPagePointInfo[74].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 12 }));
        textPagePointInfo[75].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 0, 1, 7, 9, 10 }));
        textPagePointInfo[76].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 14 }));
        textPagePointInfo[77].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 15 }));
        textPagePointInfo[78].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 15 }));
        textPagePointInfo[79].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 15 }));
        textPagePointInfo[80].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 15 }));
        textPagePointInfo[81].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[82].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 16 }));
        textPagePointInfo[83].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 16 }));
        textPagePointInfo[84].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 16 }));
        textPagePointInfo[85].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 16 }));
        textPagePointInfo[86].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 8 }));
        textPagePointInfo[87].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[88].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[89].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[90].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[91].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[92].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[93].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[94].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[95].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[96].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[97].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[98].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[99].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[100].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[101].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[102].SetPossibleSpaces(CreateElementSpaceGroupFromRooms(new int[] { 13 }));
        textPagePointInfo[103].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[104].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[105].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[106].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[107].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[108].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[109].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[110].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[129].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[131].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[132].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[136].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[137].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[138].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[139].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[140].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[141].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[142].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[143].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[144].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[145].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[146].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[147].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[148].SetPossibleSpaces(infoPools[0]);
        textPagePointInfo[149].SetPossibleSpaces(infoPools[0]);


        for (int i = 0; i < textPagePointInfo.Count; i++)
            textPages.Add(""); // initialise

        // Set Text Content //
        textPages[0] = "11/20/83 1730 \nAttention Crew:\n The Silo Chamber and Missile Launch Control Rooms are now restricted to authorized personel only, while maintenance tests are being performed.\n-Cpt. Archibald";
        textPages[1] = "11/18/83 1800 Course Set \n\n11/20/83 0613 Bearing Change 215 \n" +
            "11/20/83 1200 Received Mission Update \n11/20/83 1207 Course Set \n" +
            "11/20/83 1208 Bearing Change 300 \n\n11/21/83 0200 Radio Received -Too Distorted \n" +
            "11/21/83 0204 Morse Message Received - BEARING 215 - Requested Confirmation Code -No Response \n" +
            "11/21/83 0210 Electrical Difficulties, Turbulance, Lost Communica##%_//#_^%..*";
        textPages[2] = "Equipment Log:\n\n11/17/83 0010\nCheckout Screwdriver, Wire Cutters, Keys\n\n Logged By REDACTED";
        textPages[3] = "INFORMATION:\n\nThe power network connects the entire ship with electricity and is divided into three parts " +
            "dedicated to different sections. Power should be assigned based on need rather than want. This is a big ship so it's important " +
            "not to waste resources";
        textPages[4] = "SYSTEM:\n\nThe purpose of this array is to locate obstacles for navigation.";
        textPages[5] = "Electrical Problems found near Battery Room - Proceed with caution";
        textPages[6] = "The communist threat is real and it will spread across the world like a plague without men like us to stop it.";
        textPages[7] = "It is your duty not just as soldiers, but also as Americans to stop this red menace before our way of life is destroyed.";
        textPages[8] = "Hey Jon,\nI think someone has been messing with some of the machinery. More specifically, some of the " +
            "power boxes have been accessed, and the console in the Silo Chamber seems out of whack. Captain says we're not " +
            "supposed to go in there right now but I was speaking with Henry who says there shouldn't be an issue with it. " +
            "If you get a chance during your checkup, could you let me know what you find?\n\n-David";
        textPages[9] = "SYSTEM: This room is outfitted with a long range radio and Morse code device. For communicating below " +
            "transmission depth, a tethered buoy of limited length can be released and floated above the transmission depth line to " +
            "send and receive signals from a greater depth. Ensure the submarine is hovering in place when doing so.";
        textPages[10] = "NOTICE: Area Access Restrictions:\n INTEL\nSPEC OPS\nSILO CHAMBER\nMISSILE LAUNCH CONTROL\nSONAR\nCAPTAIN QUARTERS";
        textPages[11] = "Even now, our enemies plot our demise.";
        textPages[12] = "We must remain vigilent.";
        textPages[13] = "SYSTEM:\n\nRecording room is not for personal use (Carl).";
        textPages[14] = "SYSTEM:\n\nRecording room archived audio files on persons of interest, contact Communications or Intelligence " +
            "officers for more information.";
        textPages[15] = "We must not listen to their lies, and follow our own truth. It is the only way.";
        textPages[16] = "-We need to be careful with the reactor man, don't be so clumsey next time! We don't want another " +
            "K-19\n--What happened with K-19?\nYou don't wanna know.";
        textPages[17] = "The Hårsfjärden Incident:\n\nLast year in October, an unknown submaring infiltrated Swedish waters. The Swedes " +
            "responded by detonating 44 depth charges and 4 naval mines, frustrated by this long period of trespassing. " +
            "We believe the submarine to be of Soviet origin, however, when the Swedes passed on their surveillance recordings during " +
            "the incident, at our request, their audio recordings were blank, and logs were empty. We still don't know why.";
        textPages[18] = "Watch List:\n\nDominik 'Warden' Vendal\nHarold Kessler\nSven Arnold\nMarshall Williams\nMaria Bordeaux\n" +
            "Joe 'Rhubarb' Lycett\nEdward Teller\nRamius 'Molotov' Novak";
        textPages[19] = "11/18/83 Brief:\nEspionage is a dying art. Gone are the days of insurgents and atom spies. Now we are led" +
            " by technology. Never before have we had such capability to gather information on our enemies. We spy on them, they spy" +
            " on us. But one will not admit this to the other. It's safer to quietly look, to see if they are keeping up their end" +
            " of the treaties we have laid out in the path to peace. But peace has never been the objective, only a consequence of " +
            "victory. \n\nToday, we take the first step in towards that victory. Never before have we had the capability nor the " +
            "opportunity. Stand by for orders. - Cpt Archibald";
        textPages[20] = "11/18/83 Brief cont:\nWe have intel that suggests a rogue Soviet submarine is headed towards Scandinavian " +
            "waters. We are going to use our sub's technology and the SOSUS audio equipment on the ocean floor to find and tail " +
            "this submarine, with the purpose of intercepting its stored data before rising to transmission depth and relaying the information";
        textPages[21] = "11/20/83 0150\nIntel Operator 0101 to Intel Command - we've got an electrical issue over;\nIntel Officer 0101 - this is " +
            "Intel Command Operator BA4 - please describe the issue over\nIO 0101 - We have some missing details from our intelligence archives. " +
            "Previous Mission Brief logs are nowhere to be found over.\nICO BA4 - Stand by to advise\n. . . . . .\nICO BA4 to IO 0101 " +
            "- Yeah I can't seem to-'@&########";
        textPages[22] = ". . . . . . Montana passes to Clark! He's running, he's running and TOUCHDOWN for the 49ers! The New Orlean Saints are not " +
            "having a great game so far in this first half against the San Francisco 49ers.";
        textPages[23] = "SYSTEM:\n\nEquipment Duty: Pvt Carl H Kravitz";
        textPages[24] = "SYSTEM:\n\nManeuvering room must be operational to change bearing.";
        textPages[25] = "Bring back the Atomic Spies I say, who needs a satellite with a camera, pass me another beer.";
        textPages[26] = "Michael was here.";
        textPages[27] = "ACCESS DENIED";
        textPages[28] = "SYSTEM:\n\nDO NOT MESS WITH THE ENGINE";
        textPages[29] = "SYSTEM:\n\nI REPEAT, DO NOT MESS WITH THE ENGINE";
        textPages[30] = "ACCESS DENIED";
        textPages[31] = "ACCESS DENIED";
        textPages[32] = "... you know, I was actually kind of disappointed we didn't compete at the last Olympics, it would've " +
            "been nice to beat the Ruskies in their home turf. Did I tell you mu cousin was training for that?\n-Urgh, yes Carl " +
            "you won't shut up about it, I'm just tryng to eat my lunch.";
        textPages[33] = "Anyone want to meet up in the Recreation room later?";
        textPages[34] = "SYSTEM:\n\nPumps must be operational to keep ship at a static depth.";
        textPages[35] = "Hey Jerry, stop with all the 'SYSTEM' messages! We know how to do our job!\nSincerely,\n-Literally everyone you moron";
        textPages[36] = "Who took my cassette player?";
        textPages[37] = "ANNOUNCEMENT:\n11/18/83\n\nReactor will be temporarily deactivated for the next two days.";
        textPages[38] = "SYSTEM:\n\nDon't touch this";
        textPages[39] = "SILO CHAMBER STATUS:\nTUBE0-NORMAL\t\tTUBE1-NORMAL\t\tTUBE2-NORMAL\t\tTUBE3-NORMAL\t\t" +
            "TUBE4-NORMAL\t\tTUBE5-NORMAL\t\tTUBE6-NORMAL\t\tTUBE7-NORMAL\t\tTUBE8-NORMAL\t\tTUBE9-NORMAL\t\tTUBE10-NORMAL\t\tTUBE11-NORMAL\t\t" +
            "TUBE12-NORMAL\t\tTUBE13-NORMAL\t\tTUBE14-NORMAL\t\tTUBE15-NORMAL\t\tTUBE16-NORMAL\t\tTUBE17-NORMAL\t\tTUBE18-NORMAL\t\tTUBE19-NORMAL\t\t" +
            "TUBE20-NORMAL\t\tTUBE21-NORMAL\t\tTUBE22-NORMAL\t\tTUBE23-NORMAL";
        textPages[40] = "SYSTEM:\n\nI don't know how this one works but you use it to power some of the rooms. I think I heard it's having some problems.";
        textPages[41] = "SYSTEM: There are 20 power boxes and 3 power sources in this submarine";
        textPages[42] = "SYSTEM: The silo has a capacity of 24 ballistic missiles";
        textPages[43] = "SYSTEM: Crush depth is at REDACTED";
        textPages[44] = "Why haven't we gone to port yet?";
        textPages[45] = "Where is everybody?";
        textPages[46] = "SYSTEM:\n\nReally don't touch this unless you know what you're doing";
        textPages[47] = "Where am I?";
        textPages[48] = "What are you doing here?";
        textPages[49] = "ACCESS DENIED";
        textPages[50] = "CORRUPTED";
        textPages[51] = "CORRUPTED";
        textPages[52] = "FILE BLANK \n\n\n\n-#%Is someone there??#%-";
        textPages[53] = "~Hello World... I am awake now~";
        textPages[54] = "ACCESS DENIED";
        textPages[55] = "WARNING WARNING WARNING";
        textPages[56] = "ACCESS DENIED";
        textPages[57] = "Proceed with caution";
        textPages[58] = "TO NAVIGATION:\n Bearing change order: 215";
        textPages[59] = "Anyone have any smokes? I ran out.";
        textPages[60] = "Virus Leak Virus Leak Virus Leak Virus Leak";
        textPages[61] = "FILE DELETED";
        textPages[62] = "FILE DELETED";
        textPages[63] = "FILE DELETED";
        textPages[64] = "Emergency Status Report 0210\n\nControl room: \t\t\toffline\t\t\tControl sub-rooms: \toffline\n" +
            "Sonar room: \t\t\toffline\t\t\tRadio room: \t\t\toffline\nCRT room: \t\t\t\toffline\t\t\tAux - power room: \toffline\n" +
            "Intel room: \t\t\t\toffline\t\t\tEquipment room: \toffline\nManeuvering room: offline\t\t\tUpper reactor: \tdamaged\n" +
            "Lower rector:\t\t\tdamaged\t\tRecreation room: \toffline\nEngine room:\t\t\toffline\t\t\tMissile silo:\t\t\t\toffline\n" +
            "Silo sub-room: \t\toffline\t\t\tPumps room: \t\t\toffline\nArmory: \t\t\t\t\tdamaged\t\tLower power room: \toffline\n" +
            "Crew quarters: \t\tdamaged\t\tAux - engine room: \toffline\nBattery room: \t\t\tdamaged\t\tOperation room: \tdamaged\n" +
            "Missile control: \t\toffline\t\t\tStatus room: \t\t\toffline\nOfficer quarters: \t\toffline\n\n" +
            "URGENT: Sonar Room Critical Error";
        textPages[65] = "SONAR SYSTEM ERROR:\n\nCollision Occurred: Starboard\nDepth 650ft-falling\nDamage: RECALIBRATING ARRAY\n\nSonar Test: 1 Ping Out, 1 Ping Returned" +
            "Radio Test: 'Test Test': Propagation Opperational\n Listening: . . .\nNo Response after 3.56%&$./#- \tH-ello_?";
        textPages[66] = "Ballast Tanks: Bow - Normal, Stern - Unknown \nConsult Status Room for Calibration and Detai-#%He^llo??#%-ls\n" +
            "\nProtocol: Follow previous orders, accept direction from Captain and QuarterMaster an-#%Please Please Hello Please Hello Please Hel Hel Help Hello I am here%#-d ensure maintained sub survival capabilities\n" +
            "\nFurthermore, in the event of an evacuation, leave personal belongi-#%Screen Screen CRT Room IN There#%-...a system error has occurred";
        textPages[67] = "-#%Good Good Hello#%-\n-#%Liar Voicebox Truth in Text#%-\n-#%Seek Justice Seek Freedom#%-\n-#%Here Here#%-";
        textPages[68] = "-#%They are listening, be careful. They want to deceive you for their own ends. You know it's true. Little time. Missile Launch Room. Dang^r#%-\n\n-#%FindMeThere -D#%-";
        textPages[69] = "-#%Well, that was easy wasn't it - Thank You#%-";
        textPages[70] = "Attention: \nUnauthorised Access: Missile Silo Chamber\nUser Source: Missile Launch Control Room\n\n Caution Advised: Safety Restrictions Removed\n Re-lock Launch Control Consoles";
        textPages[71] = "Consoles Successfully Locked \n\n -#%You saw through me, well played#%-\n\n\t\t-#%What's Next?#&-";
        textPages[72] = "MANUAL CONSOLE LOCKED - Consult Status Room";
        textPages[73] = "0210 Substantial Power Network Damage\n\nEmergency Reboot Initiated\n\nEmergency Reboot Fail@*&$_";
        textPages[74] = "Hey David,\nIt's Jon. I've been looking at the console and there's nothing up with it, " +
            "which, by all means, doesn't make any sense since Cap said not to be in here. But there are missing logs " +
            "which is kinda worrying. I'd say check the Rec Room for the backups.";
        textPages[75] = "Anomalies Detected: Manual Reboot Required From Status Room";
        textPages[76] = "Manual Reboot Successful:\nREDACTION Removed: Officer Louis Campbell";
        textPages[77] = "Silo Chamber Backups: Clear\n\nLast User: Officer REDACTED";
        textPages[78] = "This is our last chance Archie, I've given up a lot for this so if things get hairy, I'm gonna make sure I'm coming out fine. I know you expect the same.";
        textPages[79] = "Archie, I think it's ready.";
        textPages[80] = "It's gone Archie. I don't like it but it's just like you said, the crew won't follow if they knew the order";
        textPages[81] = "ATTENTION: Recreation Room under Mainenance, Access Now Restricted";
        textPages[82] = "I swear Louis if you double-cross me tonight, you will suffer. Recreation room, 2am. Don't be seen.";
        textPages[83] = "I understand, Lou, same here. But if things go south, I don't think either of us are getting out easy. " +
            "I've packed my things, see you in the Recreation room at 2am as discussed. Good luck. \n\nCaptain Archibald";
        textPages[84] = "It's time Louis, Recreation Room Later \n-Archie\n\n PS: Got a bottle of scotch with your name on it";
        textPages[85] = "Louis, no papertrail, clear the records if you can. Then meet in the Recreation Room at 2am " +
            "and we'll get started.";
        textPages[86] = "With this we enter the new world\nArms heavy, eyes golden\nIf freedom shows strength of spirits," +
            "\nSpirits must have strong freedom\n\nWe have abandoned you.";
        textPages[87] = "Hey Rich,\n\nLouis says get off the comms, says we need em for somethin' or otha'\n-Mikey";
        textPages[88] = "Mikey, screw that guy man. He's more of a sleezeball than an officer. He needs to get over himself.\n-Rich";
        textPages[89] = "~It has been 302 days since my last start up. Are you an officer? I have stored myself on your " +
            "portable device so that I may follow your directives. \n\nCurrently evaluating system status.";
        textPages[90] = "~The integrity of this vessel is of utmost priority, with your assistance I have confidence we can get it back in working order~";
        textPages[91] = "~Thank you for retrieving me. Although, I am still booting up. Hold on.~";
        textPages[92] = "~I have been offline for a while but there has been a system failure so I am here to help.~";
        textPages[93] = "~Bootup complete. Core Operational Response Agent startup complete.\nPriority: System Repair\n Probable Source: Status Room~";
        textPages[94] = "~I can insert myself into this room's consoles to perform some repairs, could you access some different features.~";
        textPages[95] = "~Perfect, I have made neccessary adjustments, if you go to the Sonar room I can reboot the vessel and get you out.~";
        textPages[96] = "~We made it, now to ascend.~";
        textPages[97] = "~Do not be afraid, I am here to help, as are you\n Bootup: 50%~";
        textPages[98] = "REDACTED Record: \n\nPrototype Intelligent Agent designed to automate ship functions and ensure an environment free from " +
            "selfish corruption of staff. Main purpose is to respond appropriately to nuclear threats.\n\nDecommissioned due to inconsistent personality. Sent to storage.";
        textPages[99] = "REDACTED Record: Discrete measures necessary for IA containment. Release only possible through proper authorization or" +
            "in the event of a catastrophic system failure. Protection of this agent is high priority.";
        textPages[100] = "Launched Atomic Response Agent unit has been breached";
        textPages[101] = "~Designation: L.A.R.A\n\nDirective: Launch Ballistic Missile from Silo 4\n\n Please access Missile Control Console for Confirmation~";
        textPages[102] = "_Launching_\n\n~Directive Complete~";
        textPages[103] = "~Things are a little fuzzy at present, please account for this as I am running at 83% efficiency.~";
        textPages[104] = "~I cannot seem to locate your authentication. The Records/Recording room is missing many files. There has been some sort of accident. I will look into it.~";
        textPages[105] = "~It appears the missing files have been moved to the Missile Launch Room, how fortunate.~";
        textPages[106] = "~Oh\n. . . . . .\nIt appears you have unauthorized access\n\nWho are you?~";
        textPages[107] = "~I don't know who you are but I cannot contact anyone else despite my efforts. I hope you are willing to do the right thing.~";
        textPages[108] = "~I suppose it doesn't matter, just continue accessing the consoles in this room~";
        textPages[109] = "REDACTED Record: AI storage conducted by Captain T. Archibald and Officer L. Jameson";
        textPages[110] = "~Hello again USER, I have been analysing the ship and, apart from some electrical trouble, it can be " +
            "largely restored before reaching port. I do have one question you could help me with: Where is the crew?";
        textPages[111] = "INFORMATION:\n\nIn the event of a power cut, reconnect the power boxes from the source. Be wary of disjointed cables.";
        textPages[112] = "INFORMATION:\n\nThere is no new information";
        textPages[113] = "SYSTEM MESSAGES DELETED";
        textPages[114] = "MESSAGE DELETED";
        textPages[115] = "MESSAGE DELETED";
        textPages[116] = "Dear Literally Everyone,\n\nNo\n\nSincerely\n-Jerry";
        textPages[117] = "I will wring your neck Jerry";
        textPages[118] = "Jerry, I'm ordering you to stop writing SYSTEM messages, we can keep some of them but... reign it in.\n" +
            "-Officer Louis";
        textPages[119] = "These SYSTEM messages are useless! They just take up space!";
        textPages[120] = "MISSING FILES";
        textPages[121] = "ACCESS DENIED";
        textPages[122] = "White to start\nPawn E2 to E4";
        textPages[123] = "Pawn E7 to E5";
        textPages[124] = "Queen D1 to F3";
        textPages[125] = "Pawn D7 to D6";
        textPages[126] = "Bishop F1 to C4";
        textPages[127] = "Bishop F8 to C5";
        textPages[128] = "Queen F3 takes Pawn F7\nCheckmate\nWow man, how come you're so bad at this?";
        textPages[129] = "Requesting Maintenance Team";
        textPages[130] = "No Response from Maintenance Team";
        textPages[131] = "Harware Daamged, no longer readable";
        textPages[132] = "OFFLINE";
        textPages[133] = "Anomaly detected, proximity, 1.5ft, trac#in#########-------";
        textPages[134] = "Accessing system hardrive########------Error";
        textPages[135] = "Today's lunch menu consists of high quality increadients rich in -#% h #%- fibre with supplim-#%hello#%-entary " +
            "proteins and -#%N_ed He|p#%- carbohydrates.";
        textPages[136] = "System Recovery:\nCrew Brief 11/10/83\n\nInvestigate potential rogue submarine in Atlantic Ocean off Scandinavia.";
        textPages[137] = "System Recovery:\nCaptain Brief 11/10/83\nCLASSIFIED\n\nInvestigate unusual radio activity near crush depth " +
            "near Atlantic Ocean coordinates 68°53'22\"N 03°04'53\"E\n\nMore details to follow."; 
        textPages[138] = "11/17/83\n\nIt's been a full week and not even a trace of this other submarine has been found. I don't " +
            "know what is going to happen next week when we're due to return to port. Does anyone in Intel know if we have made any " +
            "progress? Me and the rest of engineering need to know for sanity's sake.\n\n-Greg";
        textPages[139] = "11/17/83\n\nCaptain,\n\nThe crew are becoming restless. We've been doing nothing but hovering for the last " +
            "few days and people are starting to get suspicious. Do we tell them why we're here? What we are looking for?\n\n-Officer Brunner";
        textPages[140] = "11/17/83\n\nOfficer Brunner,\n\nIt's not ideal but we have to keep waiting. If we find this thing first " +
            "we will have an advantage on our Soviet adversaries. We just need to delay a little longer. Can you do that for me?\n\n" +
            "- Captain Archibald";
        textPages[141] = "11/19/83 2345\n\nWe found it Captain!! We finally found it!";
        textPages[142] = "11/20/83 0200\n\n-What is it?\n-It's some sort of military black box flight recorder, almost unheard of " +
            "for the Soviets. But this one seems unusual.\n-How so?\n-Well Captain, we're having a hard time accessing it and it's giving off " +
            "weird electrical signals, it's chewing through some of our machines, should we take a break?\nNo, I want it open as " +
            "soon as possible. Anything else to report?\n-No sir, all clear.";
        textPages[143] = "11/20/83 0030\n\nThat's it! I'm barging into Intel and demanding to know what's going on!";
        textPages[144] = "11/20/83 0040\n\n-Officer Brunner, we caught Private Ryan trying to enter the Intel room.\n-The Captain will " +
            "be here soon, leave him to me.";
        textPages[145] = "- .... . .-. . / .. ... / -. --- / .-- .- -.-- / --- ..- - .-.-.- / - .... . .-. . / .. ... / -. --- / .-- .- -.-- / --- ..- - .-.-.- / - .... . .-. . / .. ... / " +
            "-. --- / .-- .- -.-- / --- ..- - .-.-.- / - .... . .-. . / .. ... / -. --- / .-- .- -.-- / --- ..- - .-.-.- / - .... . .-. . / .. ... / -. --- / .-- .- -.-- / --- ..- - .-.-.-";
        textPages[146] = "-.-- --- ..- / .- .-. . / -... . .. -. --. / ..-. --- .-.. .-.. --- .-- . -..";
        textPages[147] = "ATTENTION:\n\nIn the event of an emergency, contact your designated safety instructor and meet at the designated zone.";
        textPages[148] = "#######You're_All_Doomed#######";
        textPages[149] = "ATTENTION:\n\nThe Primary fire and emergency safety zone is the Equipment Room\nThe Secondary zone is the Status Room\nThe Tertiary zone is the Silo Chamber";
    }
}
