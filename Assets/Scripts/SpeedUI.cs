using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpeedUI : MonoBehaviour
{

    /*public const string SPEED = "speed";
    public static event EventHandler SpeedChange;
    private static int speed; 
    public static SpeedUI Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI SpeedText;

    /*private void Start()
    {
        TextElement.Speed = Speed;
    }*/

    /*private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one Instance");
        }

        Instance = this;

        SpeedUI.SpeedChange += SpeedChange;

    }*/




    /*public void UpdateSpeedText(int speed)
    {
        SpeedText.text = speed.ToString();
    }*/

    [SerializeField] private TextMeshProUGUI output;
    /*public string output = "";*/
    

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        /*output = logString;*/
        
    }
}
