using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataVisualizer : MonoBehaviour {
    public Material PointMaterial;
    public Gradient Colors;
    public GameObject Earth;
    public GameObject PointPrefab;
    public float ValueScaleMultiplier = 1;
    GameObject[] seriesObjects;
    public GameObject currentGO;
    //int currentSeries = 0;
    int currentSeries;

    public void RefreshSeriesObjects() {

    }

    public void CreateMeshes(SeriesData[] allSeries, int dateIndex, int colGap)
    {
        seriesObjects = new GameObject[allSeries.Length];
        GameObject p = Instantiate<GameObject>(PointPrefab);
        Vector3[] verts = p.GetComponent<MeshFilter>().mesh.vertices;
        int[] indices = p.GetComponent<MeshFilter>().mesh.triangles;

        List<Vector3> meshVertices = new List<Vector3>(65000);
        List<int> meshIndices = new List<int>(117000);
        List<Color> meshColors = new List<Color>(65000);


        GameObject seriesObj = new GameObject(allSeries[dateIndex].Name);
        seriesObj.transform.parent = Earth.transform;
        seriesObjects[dateIndex] = seriesObj;
        SeriesData seriesData = allSeries[dateIndex];
        //Debug.Log(seriesData.Data.Length + "seriesData.Data.Length");
        for (int j = 2; j < seriesData.Data.Length; j += 8)
        {
            float lat = seriesData.Data[j];
            float lng = seriesData.Data[j + 1];
            float value = seriesData.Data[j + colGap];
            //Debug.Log("value: " + value);
            AppendPointVertices(p, verts, indices, lng, lat, value, meshVertices, meshIndices, meshColors);
            if (meshVertices.Count + verts.Length > 65000)
            {
                CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
                meshVertices.Clear();
                meshIndices.Clear();
                meshColors.Clear();
            }
        }
        //for (int j = 0; j < seriesData.Data.Length; j += 3)
        //{
        //    float lat = seriesData.Data[j];
        //    float lng = seriesData.Data[j + 1];
        //    float value = seriesData.Data[j + 2];
        //    AppendPointVertices(p, verts, indices, lng, lat, value, meshVertices, meshIndices, meshColors);
        //    if (meshVertices.Count + verts.Length > 65000)
        //    {
        //        CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
        //        meshVertices.Clear();
        //        meshIndices.Clear();
        //        meshColors.Clear();
        //    }
        //}

        CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
        meshVertices.Clear();
        meshIndices.Clear();
        meshColors.Clear();
        seriesObjects[dateIndex].SetActive(false);

        currentSeries = dateIndex;
        seriesObjects[currentSeries].SetActive(true);
        Destroy(p);
    }

    //public void CreateMeshes(SeriesData[] allSeries)
    //{
    //    seriesObjects = new GameObject[allSeries.Length];
    //    GameObject p = Instantiate<GameObject>(PointPrefab);
    //    Vector3[] verts = p.GetComponent<MeshFilter>().mesh.vertices;
    //    int[] indices = p.GetComponent<MeshFilter>().mesh.triangles;

    //    List<Vector3> meshVertices = new List<Vector3>(65000);
    //    List<int> meshIndices = new List<int>(117000);
    //    List<Color> meshColors = new List<Color>(65000);

    //    for (int i = 0; i < allSeries.Length; i++)
    //    {
    //        GameObject seriesObj = new GameObject(allSeries[i].Name);
    //        seriesObj.transform.parent = Earth.transform;
    //        seriesObjects[i] = seriesObj;
    //        SeriesData seriesData = allSeries[i];
    //        //Debug.Log(seriesData.Data.Length + "seriesData.Data.Length");
    //        for (int j = 2; j < seriesData.Data.Length; j+=8)
    //        {
    //            float lat = seriesData.Data[j];
    //            float lng = seriesData.Data[j + 1];
    //            float value = seriesData.Data[j + 4];
    //            AppendPointVertices(p, verts, indices, lng, lat, value, meshVertices, meshIndices, meshColors);
    //            if (meshVertices.Count + verts.Length > 65000)
    //            {
    //                CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
    //                meshVertices.Clear();
    //                meshIndices.Clear();
    //                meshColors.Clear();
    //            }
    //        }
    //        //for (int j = 0; j < seriesData.Data.Length; j += 3)
    //        //{
    //        //    float lat = seriesData.Data[j];
    //        //    float lng = seriesData.Data[j + 1];
    //        //    float value = seriesData.Data[j + 2];
    //        //    AppendPointVertices(p, verts, indices, lng, lat, value, meshVertices, meshIndices, meshColors);
    //        //    if (meshVertices.Count + verts.Length > 65000)
    //        //    {
    //        //        CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
    //        //        meshVertices.Clear();
    //        //        meshIndices.Clear();
    //        //        meshColors.Clear();
    //        //    }
    //        //}
    //        CreateObject(meshVertices, meshIndices, meshColors, seriesObj);
    //        meshVertices.Clear();
    //        meshIndices.Clear();
    //        meshColors.Clear();
    //        seriesObjects[i].SetActive(false);
    //    }


    //    seriesObjects[currentSeries].SetActive(true);
    //    Destroy(p);
    //}
    private void AppendPointVertices(GameObject p, Vector3[] verts, int[] indices, float lng,float lat,float value, List<Vector3> meshVertices,
    List<int> meshIndices,
    List<Color> meshColors)
    {
        Color valueColor = Colors.Evaluate(value);
        
        //valueColor.a = 0f;
        //Debug.Log("valueColor: " + valueColor.ToString());
        //Debug.Log("valueColor.a" + valueColor.a);

        Vector3 pos;
        pos.x = 0.5f * Mathf.Cos((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        pos.y = 0.5f * Mathf.Sin(lat * Mathf.Deg2Rad);
        pos.z = 0.5f * Mathf.Sin((lng) * Mathf.Deg2Rad) * Mathf.Cos(lat * Mathf.Deg2Rad);
        p.transform.parent = Earth.transform;
        p.transform.position = pos;
        p.transform.localScale = new Vector3(1, 1, Mathf.Max(0.001f, value * ValueScaleMultiplier));
        p.transform.LookAt(pos * 2);

        int prevVertCount = meshVertices.Count;

        for (int k = 0; k < verts.Length; k++)
        {
            meshVertices.Add(p.transform.TransformPoint(verts[k]));
            meshColors.Add(valueColor);
        }
        for (int k = 0; k < indices.Length; k++)
        {
            meshIndices.Add(prevVertCount + indices[k]);
        }
    }
    private void CreateObject(List<Vector3> meshertices, List<int> meshindecies, List<Color> meshColors, GameObject seriesObj)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshertices.ToArray();
        mesh.triangles = meshindecies.ToArray();
        mesh.colors = meshColors.ToArray();


        GameObject obj = new GameObject();
        obj.transform.parent = Earth.transform;
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>().material = PointMaterial;
        obj.transform.parent = seriesObj.transform;
    }
    public void ActivateSeries(int seriesIndex)
    {
        if (seriesIndex >= 0 && seriesIndex < seriesObjects.Length)
        {
            seriesObjects[currentSeries].SetActive(false);
            currentSeries = seriesIndex;
            seriesObjects[currentSeries].SetActive(true);

        }
    }
}
[System.Serializable]
public class SeriesData
{
    public string Name;
    public float[] Data;
}
