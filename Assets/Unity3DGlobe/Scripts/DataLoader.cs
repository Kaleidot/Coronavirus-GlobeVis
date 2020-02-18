using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DataLoader : MonoBehaviour {

    public DataVisualizer DayVisualizer;
    public DataVisualizer NewCaseVisualizer;
    public Dropdown dateDropDown;

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
        DayVisualizer.CreateMeshes(data.AllData, 0, 4);

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
        DayVisualizer.CreateMeshes(data.AllData, value, 4);
        NewCaseVisualizer.CreateMeshes(data.AllData, value, 5);
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