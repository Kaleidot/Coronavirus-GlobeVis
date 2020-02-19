using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonListContentManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject parentToAttachButtonsTo;
    public Text headDateText;

    // Start is called before the first frame update
    void Start()
    {
        //// Instantiate button
        //GameObject button = (GameObject)Instantiate(buttonPrefab);

        //// Set button parent
        //button.transform.SetParent(parentToAttachButtonsTo.transform);

        //// Set what button does when clicked
        //button.GetComponent<Button>().onClick.AddListener(OnClick);

        //// Change button text
        //button.transform.GetChild(0).GetComponent<Text>().text = "This is button text";
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

    void OnClick()
    {
        //Debug.Log("Clicked!");
        GameObject currentSelectedGB = EventSystem.current.currentSelectedGameObject;
        string tag = currentSelectedGB.tag;
        if (tag == "Button")
        {
            headDateText.text = currentSelectedGB.transform.GetChild(0).GetComponent<Text>().text;
        }
    }

    public int FindDateIndex(Dictionary<string, int> dict, string dateString)
    {
        int dateIndex = 0;
        if (dict.ContainsKey(dateString))
        {
            dateIndex = dict[dateString];
            Debug.Log("dateIndex display: " + dateString + " : " + dateIndex);
        }
        return dateIndex;
    }

}
