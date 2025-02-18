using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using UnityEngine.UI;


public class BlackjackGameManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] Button dealButton;
    [SerializeField] Button hitButton;
    [SerializeField] Button standButton;
    [SerializeField] Button betButton;
    [SerializeField] Button doubleButton;
    [SerializeField] Button splitButton;
    
    [Header("Textos")]
    [SerializeField] Text scoreText;
    [SerializeField] Text dealScoreText;
    [SerializeField] Text betsText;
    [SerializeField] Text cashText;
    [SerializeField] Text doubleText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
