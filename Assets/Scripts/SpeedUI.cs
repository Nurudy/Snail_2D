using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SpeedUI : MonoBehaviour
{

   

    [SerializeField] private TextMeshProUGUI output;
   
    
    //creamos estas funciones que acudirán a nuestro evento para obedecerlo
    void OnEnable()
    {
        Snake.OnSpeedChange += ChangeSpeedText;
    }

    void OnDisable()
    {
        Snake.OnSpeedChange -= ChangeSpeedText;
    }

    void ChangeSpeedText(float speed) //con esta variable mostraremos la velocidad en pantalla
    {
        output.text = speed.ToString();
    }
}
