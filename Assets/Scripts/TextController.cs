using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour
{
    //public InputField input;
    public Text dateText;

    public void SetDateText(Button button)
    {
        dateText.text = button.transform.GetChild(0).GetComponent<Text>().text;
    }
}
