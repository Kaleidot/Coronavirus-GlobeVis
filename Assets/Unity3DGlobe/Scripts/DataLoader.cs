using UnityEngine;
using System.Collections;

public class DataLoader : MonoBehaviour {
    public DataVisualizer Visualizer;
	// Use this for initialization
	void Start () {
        //TextAsset jsonData = Resources.Load<TextAsset>("population");
        //TextAsset jsonData = Resources.Load<TextAsset>("coronavirusConfirmed0210_3cols_v2");
        TextAsset jsonData = Resources.Load<TextAsset>("CoronavirusData0210_all");
        string json = jsonData.text;
        SeriesArray data = JsonUtility.FromJson<SeriesArray>(json);
        Visualizer.CreateMeshes(data.AllData);

    }
	
	void Update () {
	
	}
}
[System.Serializable]
public class SeriesArray
{
    public SeriesData[] AllData;
}