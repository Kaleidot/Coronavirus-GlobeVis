using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint : MonoBehaviour
{
    private int totalNum;
    private int newNum;
    private string cityName;
    private string countryName;

    public int GetTotalNum()
    {
        return totalNum;
    }

    public void SetTotalNum(int input)
    {
        totalNum = input;
    }

    public int GetNewNum()
    {
        return newNum;
    }

    public void SetNewNum(int input)
    {
        newNum = input;
    }

    public string GetCityName()
    {
        return cityName;
    }

    public void SetCityName(string input)
    {
        cityName = input;
    }

    public string GetCountryName()
    {
        return countryName;
    }

    public void SetCountryName(string input)
    {
        countryName = input;
    }
}
