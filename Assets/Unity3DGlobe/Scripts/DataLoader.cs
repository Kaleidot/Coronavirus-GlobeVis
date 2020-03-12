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
    public GameObject canvas;

    private string[] dates;
    private Dictionary<string, int> hashedDates = new Dictionary<string, int>();
    private SeriesArray data;
    private bool isDetailsOnDemandTriggered = false;
    private bool isAnimated = false;
    private bool isReset = false;

    private GameObject animateButton;
    private GameObject stopButton;
    private GameObject resetButton;
    private GameObject selectionPanel;


    // string array to store data     
    private Dictionary<string, string[]> hashedData = new Dictionary<string, string[]>();

    // Use this for initialization
    void Start()
    {
        // Load json data file
        //TextAsset jsonData = Resources.Load<TextAsset>("population");
        //TextAsset jsonData = Resources.Load<TextAsset>("coronavirusConfirmed0210_3cols_v2");
        TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0311_all");
        //TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0210_all");
        string json = jsonData.text;
        //SeriesArray data = JsonUtility.FromJson<SeriesArray>(json);


        // Process data to get dates
        dates = json.Split('\n');
        int hashIndex = -1;
        dateDropDown.options.Clear();

        data = JsonUtility.FromJson<SeriesArray>(json);
        dayVisualizer.CreateMeshes(data.AllData, 0, 4, dates[4].Split(':')[1].Replace("[", "").Replace("]", "").Trim().Split(','));
        newCaseVisualizer.CreateMeshes(data.AllData, 0, 5, dates[4].Split(':')[1].Replace("[", "").Replace("]", "").Trim().Split(','));

        for (int i = 3; i < dates.Length - 1; i += 4)
        {

            // Extract dates from the data source
            string[] dateStrings = dates[i].Split(':');
            string dateString = dateStrings[1].Replace("\"", "").Replace(",", "").Trim();
            //Debug.Log(dateString);

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

        // set day1 date in the header
        headDateText.text = FindDateStringByIndex(0);

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

        animateButton = canvas.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        animateButton.GetComponent<Button>().onClick.AddListener(OnStart);

        stopButton = canvas.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
        stopButton.GetComponent<Button>().onClick.AddListener(OnStop);

        resetButton = canvas.transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
        resetButton.GetComponent<Button>().onClick.AddListener(OnReset);

        selectionPanel = canvas.transform.GetChild(2).gameObject;

        //Debug.Log("buttons: " + animateButton.name + stopButton.name + resumeButton.name);

        //Start the coroutine we define below named ExampleCoroutine.
        //StartCoroutine(ExampleCoroutine());

    }

    private void RepeatFunction() {
        int total = hashedDates.Count;
        string date = headDateText.text;
        int index = FindDateIndex(date);

        if (index < total - 1)
        {
            index++;
            string[] dataStrings = FindDataString(date);
            headDateText.text = FindDateStringByIndex(index);
            LoadNewData(index, dataStrings);
        }
        else {
            index = 0;
            string[] dataStrings = FindDataString(FindDateStringByIndex(index));
            headDateText.text = FindDateStringByIndex(index);
            LoadNewData(index, dataStrings);
            
        }
        
        

        //date = FindDateStringByIndex(index);

        //while (isAnimated && index < total)
        //{
        //    //Print the time of when the function is first called.
        //    //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //    string[] dataStrings = FindDataString(date);
        //    headDateText.text = FindDateStringByIndex(index);

        //    LoadNewData(index, dataStrings);
        //    index++;
        //    date = FindDateStringByIndex(index);

        //    ////After we have waited 5 seconds print the time again.
        //    //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        //}
    }

    IEnumerator ExampleCoroutine()
    {
        int total = hashedDates.Count;
        string date = headDateText.text;
        int index = FindDateIndex(date);

        Debug.Log("total: " + total);
        Debug.Log("index: " + index);
        Debug.Log("isAnimated: " + isAnimated);

        while (isAnimated && index < total)
        {
            //Print the time of when the function is first called.
            Debug.Log("Started Coroutine at timestamp : " + Time.time);

            string[] dataStrings = FindDataString(date);
            headDateText.text = FindDateStringByIndex(index);

            LoadNewData(index, dataStrings);
            index++;
            date = FindDateStringByIndex(index);

            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(2);

            ////After we have waited 5 seconds print the time again.
            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        }
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

        if (isAnimated)
        {
            animateButton.transform.GetChild(0).GetComponent<Text>().text = "Resume";
            selectionPanel.SetActive(false);
        }
        else
        {
            selectionPanel.SetActive(true);
        }

        if (isReset)
        {
            animateButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
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
            //Debug.Log("dateText: " + dateText);
            //foreach (string i in dataString)
            //{
            //    Debug.Log("dataString info: " + i);
            //}
            LoadNewData(value, dataString);
        }
    }

    void OnStart()
    {
        isAnimated = true;
        InvokeRepeating("RepeatFunction", 2f, 2f);
    }

    void OnStop()
    {
        isAnimated = false;
        CancelInvoke();
    }

    void OnReset()
    {
        //int value = FindDateIndex("1/22/2020");
        //Debug.Log("value: " + value);

        //bool isHave = hashedData.ContainsKey(" 1/22/2020");
        //Debug.Log("Bool: " + isHave);

        string dateKey = FindDateStringByIndex(0);
        string[] dataStrings = FindDataString(dateKey);
        headDateText.text = dateKey;
        //foreach (string str in dataStrings)
        //{
        //    Debug.Log(str);
        //}

        LoadNewData(0, dataStrings);
        isReset = true;
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

    public string FindDateStringByIndex(int index)
    {
        string dateKey = "";
        if (hashedDates.ContainsValue(index))
        {
            foreach (KeyValuePair<string, int> pair in hashedDates)
            {
                if (index.Equals(pair.Value))
                {
                    dateKey = pair.Key;
                    break;
                }
            }     
        }
        return dateKey;
    }

    public string[] FindDataString(string dateString)
    {
        string[] dataStrings = new string[100];
        if (hashedData.ContainsKey(dateString))
        {
            dataStrings = hashedData[dateString];
            //foreach (string str in dataStrings)
            //{
            //    Debug.Log(str);
            //}
        }
            
        return dataStrings;
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