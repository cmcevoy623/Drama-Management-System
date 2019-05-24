using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : Part {

    // View Point //
    protected Transform viewPoint;

    // Components and States //
    UITab[] PageTabs;
    int highlighedTabIndex = 0;
    int currentPage = 0;
    float timeBetweenSelections;

    // Unaccessed Pages and Variables //
    List<int> unaccessedPageIDs = new List<int>();

    // Getters and Setters //
    public Transform GetViewPoint() { return viewPoint; }
    public int UnseenPages() { return unaccessedPageIDs.Count; }
    
    public List<ElementSpace> GetElementSpaces()
    {
        List<ElementSpace> unaccessedSpaces = new List<ElementSpace>();

        if (PageTabs == null)
            InitUnaccessedTabs();

        // Get Spaces of all Unaccessed Info //
        for (int i = 0; i < unaccessedPageIDs.Count; i++)
            unaccessedSpaces.Add(PageTabs[unaccessedPageIDs[i]].GetElementSpace());

        return unaccessedSpaces;
    }

    void InitUnaccessedTabs()
    {
        PageTabs = transform.GetComponentsInChildren<UITab>();

        // Initialise Pages //
        if (PageTabs.Length > 0)
        {
            for (int i = 0; i < PageTabs.Length; i++)
            {
                if (PageTabs[i].gameObject.activeSelf)
                {
                    PageTabs[i].Init();
                    PageTabs[i].SetPageID(i);
                    PageTabs[i].GetTextObj().SetActive(false);

                    unaccessedPageIDs.Add(i);
                }
            }

            // Show First Page //
            PageTabs[currentPage].GetTextObj().SetActive(true);
            //unaccessedPageIDs.Remove(currentPage);  // Already Access First Page (Should Just Be Instructions)
        }
    }

    // Use this for initialization
    protected override void Awake ()
    {
        base.Awake();
        viewPoint = transform.Find("MonitorViewPoint");

        if (PageTabs == null)
            InitUnaccessedTabs();

        // Setup Nav //
        SetupSelectionNav();
    }

    void SetupSelectionNav()
    {
        // Test If Multiple Parts //
        if (PageTabs.Length > 1)
        {
            // For each Part //
            for (int i = 0; i < PageTabs.Length; i++)
            {
                // For Every Other Part //
                for (int j = 0; j < PageTabs.Length; j++)
                {
                    if (i != j)
                    {
                        // Get Local Direction Vector and Add to Selections //
                        Vector3 interTabVec = PageTabs[i].transform.localPosition - PageTabs[j].transform.localPosition;
                        interTabVec.z = 0.0f;
                        PageTabs[i].AddSelection(interTabVec, j);
                    }
                }
            }
        }
    }

    public override void Engage()
    {
        base.Engage();

        if (PageTabs.Length <= 0)
            return;
        
        GlowObject glowObj = PageTabs[currentPage].GetComponent<GlowObject>();

        if (glowObj)
            glowObj.GlowIn();
    }

    public void Use()
    {
        // Hide Current Page //
        PageTabs[currentPage].GetTextObj().SetActive(false);

        // Update Current Page //
        currentPage = highlighedTabIndex;

        // Display Tab's Page - Use Tab Number //
        PageTabs[currentPage].GetTextObj().SetActive(true);

        // Get Housed Element and Initialise //
        ElementPoint housedEl = PageTabs[currentPage].GetElementSpace().HousedElement;
        int pID = -1;

        // If Housed Element Exists //
        if (housedEl != null)
        {
            pID = housedEl.PlotPointIndex;

            // Get Space with Greatest Viability - Swap Current Page with that - So that the Greatest Viability Point is Fired //
            int bestPointTabID = currentPage;
            int bestPointViability = PageTabs[currentPage].GetElementSpace().HousedElement.ViabilityIndex;

            //for (int i = 0; i < unaccessedPageIDs.Count; i++)
            //{
            //    if (PageTabs[unaccessedPageIDs[i]].GetElementSpace().HousedElement.ViabilityIndex > bestPointViability)
            //    {
            //        bestPointViability = PageTabs[unaccessedPageIDs[i]].GetElementSpace().HousedElement.ViabilityIndex;
            //        bestPointTabID = unaccessedPageIDs[i];
            //    }
            //}
            //ElementPoint temporaryHousedPoint = housedEl.DeepCopy();
            //ElementPoint bestPointInMonitor = PageTabs[bestPointTabID].GetElementSpace().HousedElement;
            //housedEl = bestPointInMonitor.DeepCopy();
            //PageTabs[bestPointTabID].GetElementSpace().HousedElement = temporaryHousedPoint.DeepCopy();

            // Get Best Point In Room //
            if (unaccessedPageIDs.Contains(currentPage))
            {
                List<ElementSpace> allRoomUnaccessedPageIDs = GetComponentInParent<Room>().GetUnaccessedElementSpaces();
                if (allRoomUnaccessedPageIDs.Count > 0)
                {
                    for (int i = 0; i < allRoomUnaccessedPageIDs.Count; i++)
                    {
                        if (allRoomUnaccessedPageIDs[i].HousedElement.ViabilityIndex >= bestPointViability)
                        {
                            bestPointViability = allRoomUnaccessedPageIDs[i].HousedElement.ViabilityIndex;
                            bestPointTabID = i;
                        }
                    }

                    ElementPoint temporaryHousedPoint = housedEl.DeepCopy();
                    ElementPoint bestPointInMonitor = allRoomUnaccessedPageIDs[bestPointTabID].HousedElement;
                    housedEl = bestPointInMonitor.DeepCopy();
                    PageTabs[currentPage].GetElementSpace().HousedElement = housedEl;
                    allRoomUnaccessedPageIDs[bestPointTabID].HousedElement = temporaryHousedPoint.DeepCopy();

                    pID = housedEl.PlotPointIndex;
                }
            }
        }

        // Set text page contents to those found in the text bank with the point's info id //
        //if (PageTextBank.CheckIDValid())
        if (InformationEnactor.ValidateInfoType(PlotPoint.PLOTPOINTTYPE.TEXTPAGE, pID))
        {
            int infoID = InformationEnactor.GetInfoIdFromPointID(pID);
            if (PageTextBank.CheckIDValid(infoID))
                PageTabs[currentPage].GetTextPage().text = PageTextBank.GetTextPage(infoID);
            else
                PageTabs[currentPage].GetTextPage().text = "RECORD NOT FOUND";

            // Output pID
            PageTabs[currentPage].GetTextPage().text += "\n\n PlotPoint ID: " + pID;

            // Check if has weights to display //
            if (PageTextBank.CheckWeightsExist(infoID))
            {
                PageTabs[currentPage].GetTextPage().text += "\n Weights: ";
                for (int i = 0; i < PageTextBank.GetPointInfoValElement(infoID).InfoWeights().Length; i++)
                    PageTabs[currentPage].GetTextPage().text += PageTextBank.GetPointInfoValElement(infoID).InfoWeights()[i].ToString("F2") + ", ";
            }
            else
                PageTabs[currentPage].GetTextPage().text += "\n No Weights Exist: ";

            if (InformationEnactor.GetIsPointEnding(pID))
                PageTabs[currentPage].GetTextPage().text += "\n EndPoint Reached";
        }
        else
        {
            PageTabs[currentPage].GetTextPage().text = "No Connected Plot Point: ";
        }

        if (unaccessedPageIDs.Contains(currentPage) == true)    // fire plot point if not previously accessed
        {
            OutputManager.StartTime(Time.realtimeSinceStartup);

            // REMOVE FROM UNACCESSED PAGES //
            unaccessedPageIDs.Remove(currentPage);
            InformationEnactor.RemoveElementSpace(PageTabs[currentPage].GetElementSpace());

            int spaceID = PageTabs[currentPage].GetElementSpace().SpaceID;
            DramaManager.RemoveSpaceFromUndiscoveredSet(spaceID, pID);

            // Fire After Removing Space //
            OutputManager.FiredSpaceHistory.Insert(0, spaceID);
            InformationEnactor.FirePointFromAction(pID);
        }
    }

    public override void Disengage()
    {
        base.Disengage();

        //highlighedTabIndex = 0;
        timeBetweenSelections = 0.0f;

        if (PageTabs.Length <= 0)
            return;

        GlowObject glowObj = PageTabs[currentPage].GetComponent<GlowObject>();

        if (glowObj)
            glowObj.GlowOut();
    }

    void ChangeSelection()
    {
        // Get Input Vector
        float xDir = -Input.GetAxis("HorizontalUI");
        float yDir = Input.GetAxis("VerticalUI");

        Vector3 inputVec = new Vector3(xDir, yDir, 0.0f);

        if (PageTabs.Length <= 0)
            return;

        // Compare to List of Vectors stored in highlighted part, normalising for comparison
        List<Vector3> currentSelections = PageTabs[highlighedTabIndex].GetSelectionVectors();
        List<int> currentIndices = PageTabs[highlighedTabIndex].GetSelectionIndices();

        if (currentSelections.Count <= 0 || currentIndices.Count <= 0)
            return;

        // Get the index of the most similar vector
        int closestPartIndex = -1; // init to non-zero value
        float angleTolerance = 75.0f;
        float nearestAngle = 999.9f; // init to high value

        // Get Closest Nav //
        for (int i = 0; i < currentSelections.Count; i++)
        {
            if (MathUtilities.AreVectorsFacingSameDirection(currentSelections[i], inputVec))
            {
                float angle = Vector3.Angle(currentSelections[i].normalized, inputVec.normalized);

                if (angle < angleTolerance && angle < nearestAngle)
                {
                    nearestAngle = angle;
                    closestPartIndex = i;
                }
            }
        }

        // If no similar index return
        if (closestPartIndex < 0)
            return;

        if (timeBetweenSelections > Player.selectionRate)
        {
            if (highlighedTabIndex != currentIndices[closestPartIndex])
            {
                //// Change highlighted part index to new selection
                GlowObject glowObj = PageTabs[highlighedTabIndex].GetComponent<GlowObject>();

                if (glowObj)
                    glowObj.GlowOut();

                // Change highlighted part index to new selection
                highlighedTabIndex = currentIndices[closestPartIndex];

                glowObj = PageTabs[highlighedTabIndex].GetComponent<GlowObject>();

                if (glowObj)
                    glowObj.GlowIn();

                timeBetweenSelections = 0.0f;
            }
        }
    }

    private void Update()
    {
        if (isEngaged)
        {
            timeBetweenSelections += Time.deltaTime;
            ChangeSelection();
        }
    }
}
