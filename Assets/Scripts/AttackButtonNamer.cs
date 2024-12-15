using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButtonNamer : MonoBehaviour
{
    Text buttonText;

    void Start()
    {
        buttonText = GetComponent<Text>();
    }



    public void SetButtonName(string newName)
    {
        buttonText.text = newName;
    }

}
