using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class DataLoader : MonoBehaviour
{

    public DataVisualizer dayVisualizer;
    public DataVisualizer newCaseVisualizer;
    public Dropdown dateDropDown;
    //public ButtonListContentManager buttonContentManager;
    public GameObject buttonPrefab;
    public GameObject parentToAttachButtonsTo;
    public Text headDateText;
    public GameObject detailsOnDemandPanel;

    private string[] dates;
    private Dictionary<string, int> hashedDates = new Dictionary<string, int>();
    private SeriesArray data;
    private bool isDetailsOnDemandTriggered = false;

    // string array to store data     
    private Dictionary<string, string[]> hashedData = new Dictionary<string, string[]>();

    // Use this for initialization
    void Start()
    {
        // Load json data file
        //TextAsset jsonData = Resources.Load<TextAsset>("population");
        //TextAsset jsonData = Resources.Load<TextAsset>("coronavirusConfirmed0210_3cols_v2");
        TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0222_all");
        //TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0210_all");
        string json = jsonData.text;
        //SeriesArray data = JsonUtility.FromJson<SeriesArray>(json);


        // Process data to get dates
        dates = json.Split('\n');
        int hashIndex = -1;
        dateDropDown.options.Clear();

        data = JsonUtility.FromJson<SeriesArray>(json);
        dayVisualizer.CreateMeshes(data.AllData, 0, 4, dates[4].Split(':')[1].Replace("[", "").Replace("]", "").Trim().Split(','));

        for (int i = 3; i < dates.Length - 1; i += 4)
        {

            // Extract dates from the data source
            string[] dateStrings = dates[i].Split(':');
            string dateString = dateStrings[1].Replace("\"", "").Replace(",", "");
            Debug.Log(dateString);

            // Index the dates and store them in a dictionary
            hashIndex++;
            hashedDates.Add(dateString, hashIndex);

            // Feed the dates into a drop down list
            //dateDropDown.options.Add(new Dropdown.OptionData(dateString));

            // Create buttons for the dates
            CreateButton(dateString);

            // Extract data from the data source
            hashedData.Add(dateString, dates[i + 1].Split(':')[1].Replace("[", "").Replace("]", "").Trim().Split(','));
        }

        //// Debug dictionary
        //foreach (KeyValuePair<string, int> pair in hashedDates)
        //{
        //    Debug.Log("FOREACH KEY:" + pair.Key + "FOREACH Value: " + pair.Value);

        //}

        //dateDropDown.itemText.text = hashedDates.Keys.First().ToString();

        //dateDropDown.options.Clear();
        //foreach(string str in dates)
        //{
        //    Debug.Log("str: " + str);
        //    dateDropDown.options.Add(new Dropdown.OptionData(str));
        //    //Debug.Log("Added.");
        //}

    }

    /// ////////////////////////////////
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Point(Clone)")
                {
                    //Debug.Log("Country: " + hit.transform.GetComponent<DataPoint>().GetCountryName().Replace("\"", "").Replace("\"", "") + "; City: " + hit.transform.GetComponent<DataPoint>().GetCityName() + "; Total Number: " +
                    //    hit.transform.GetComponent<DataPoint>().GetTotalNum() + "; New Number: " + hit.transform.GetComponent<DataPoint>().GetNewNum());

                    // Details on Demand
                    string details = "Country: " + hit.transform.GetComponent<DataPoint>().GetCountryName().Replace("\"", "").Replace("\"", "") + "\n" +
                                    "Province: " + hit.transform.GetComponent<DataPoint>().GetCityName().Replace("\"", "").Replace("\"", "") + "\n" +
                                    "Total number of confirmed cases: " + hit.transform.GetComponent<DataPoint>().GetTotalNum() + "\n" +
                                    "Number of new cases: " + hit.transform.GetComponent<DataPoint>().GetNewNum() + "\n";

                    //hit.transform.GetChild(0).position = Input.mousePosition;
                    //hit.transform.GetChild(0).rotation = Quaternion.Euler(Vector3.zero);
                    //hit.transform.GetChild(0).GetChild(0).gameObject.AddComponent<Canvas>();
                    //Canvas myCanvas = hit.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Canvas>();
                    //myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    //myCanvas.tag = "DetailsOnDemand";

                    GameObject myCanvas = GameObject.Find("Canvas");
                    GameObject panel= (GameObject)Instantiate(detailsOnDemandPanel);

                    panel.transform.SetParent(myCanvas.transform);
                    //panel.transform.localPosition = Input.mousePosition;
                    panel.transform.position = Input.mousePosition;
                    panel.GetComponentInChildren<Text>().text = details;
                    isDetailsOnDemandTriggered = true;
                }
                else
                    isDetailsOnDemandTriggered = false;
            }
        }

        if (!isDetailsOnDemandTriggered)
        {
            //GameObject.FindWithTag("DetailsOnDemand").SetActive(false);
            Destroy(GameObject.FindWithTag("DetailsOnDemand"));
        }
    }
    /// /////////////////////////////

    void OnClick()
    {
        //Debug.Log("Clicked!");
        GameObject currentSelectedGB = EventSystem.current.currentSelectedGameObject;
        string tag = currentSelectedGB.tag;
        if (tag == "Button")
        {
            string dateText = currentSelectedGB.transform.GetChild(0).GetComponent<Text>().text;

            headDateText.text = dateText;

            int value = FindDateIndex(dateText);
            string[] dataString = FindDataString(dateText);
            LoadNewData(value, dataString);
        }
    }

    public void CreateButton(string buttonText)
    {
        // Instantiate button
        GameObject button = (GameObject)Instantiate(buttonPrefab);

        // Set button parent
        button.transform.SetParent(parentToAttachButtonsTo.transform);

        button.transform.localScale = Vector3.one;

        // Set what button does when clicked
        button.GetComponent<Button>().onClick.AddListener(OnClick);

        // Change button text
        button.transform.GetChild(0).GetComponent<Text>().text = buttonText;
    }

    public int FindDateIndex(string dateString)
    {
        int dateIndex = 0;
        if (hashedDates.ContainsKey(dateString))
        {
            dateIndex = hashedDates[dateString];
            //Debug.Log("dateIndex display: " + dateString + " : " + dateIndex);
        }
        return dateIndex;
    }

    public string[] FindDataString(string dateString)
    {
        string[] dataString = new string[100];
        if (hashedDates.ContainsKey(dateString))
            dataString = hashedData[dateString];
        return dataString;
    }

    public void LoadNewData(int value, string[] dataString)
    {
        Transform Earth = GameObject.Find("Earth").transform;
        //remove existing vis
        if (Earth.transform.childCount > 0)
        {
            foreach (Transform child in Earth.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        dayVisualizer.CreateMeshes(data.AllData, value, 4, dataString);
        newCaseVisualizer.CreateMeshes(data.AllData, value, 5, dataString);
    }
}

[System.Serializable]
public class SeriesArray
{
    public SeriesData[] AllData;
}

//[System.Serializable]
//public class DateArray
//{
//    public string[] dateInfo;
//}