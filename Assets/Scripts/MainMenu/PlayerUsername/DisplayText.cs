using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DisplayText : MonoBehaviour
{
    public TextMeshProUGUI obj_text;
    public TextMeshProUGUI display;

    // Start is called before the first frame update
    void Start()
    {
        obj_text.text = PlayerPrefs.GetString("user_name");
    }

    public void Create()
    {

            obj_text.text = display.text;
            PlayerPrefs.SetString("user_name", obj_text.text);
            PlayerPrefs.Save();
    }
}
