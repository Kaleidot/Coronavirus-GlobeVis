using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class DataLoader : MonoBehaviour {

    public DataVisualizer dayVisualizer;
    public DataVisualizer newCaseVisualizer;
    public Dropdown dateDropDown;
    //public ButtonListContentManager buttonContentManager;
    public GameObject buttonPrefab;
    public GameObject parentToAttachButtonsTo;
    public Text headDateText;

    private string[] dates;
    private Dictionary<string, int> hashedDates = new Dictionary<string, int>();
    private SeriesArray data;

    // Use this for initialization
    void Start () {
        // Load json data file
        //TextAsset jsonData = Resources.Load<TextAsset>("population");
        //TextAsset jsonData = Resources.Load<TextAsset>("coronavirusConfirmed0210_3cols_v2");
        TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0210_all");
        string json = jsonData.text;
        //SeriesArray data = JsonUtility.FromJson<SeriesArray>(json);
        data = JsonUtility.FromJson<SeriesArray>(json);
        dayVisualizer.CreateMeshes(data.AllData, 0, 4);

        // Process data to get dates
        dates = json.Split('\n');
        int hashIndex = -1;
        dateDropDown.options.Clear();

        for (int i = 3; i < dates.Length - 1; i += 4) {

            // Extract dates from the data source
            string[] dateStrings = dates[i].Split(':');
            string dateString = dateStrings[1].Replace("\"", "").Replace(",", "");
            //Debug.Log(dateString);

            // Index the dates and store them in a dictionary
            hashIndex++;
            hashedDates.Add(dateString, hashIndex);

            // Feed the dates into a drop down list
            dateDropDown.options.Add(new Dropdown.OptionData(dateString));

            // Create buttons for the dates
            CreateButton(dateString);
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
            LoadNewData(value);
        }
    }

    public void CreateButton(string buttonText)
    {
        // Instantiate button
        GameObject button = (GameObject)Instantiate(buttonPrefab);

        // Set button parent
        button.transform.SetParent(parentToAttachButtonsTo.transform);

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
            Debug.Log("dateIndex display: " + dateString + " : " + dateIndex);
        }
        return dateIndex;
    }

    public void LoadNewData(int value)
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
        dayVisualizer.CreateMeshes(data.AllData, value, 4);
        newCaseVisualizer.CreateMeshes(data.AllData, value, 5);
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