using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Q3Movement;
using System;

public class SpeedOMeter : MonoBehaviour
{
    TextMeshProUGUI speedometer;
    Q3PlayerController player;
    
    void Start()
    {
        speedometer = GameObject.Find("Canvas/Speedometer").GetComponent<TextMeshProUGUI>();
        player = GameObject.Find("FirstPersonController").GetComponent<Q3PlayerController>(); 
    }

    void Update()
    {
        speedometer.text = Math.Round(player.Speed, 0).ToString();
    }
}
