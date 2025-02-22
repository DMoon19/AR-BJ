using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] Button startButton;
    [SerializeField] Button dealButton;
    [SerializeField] Button hitButton;
    [SerializeField] Button standButton;
    [SerializeField] Button betButton;
    [SerializeField] Button doubleButton;
    [SerializeField] Button splitButton;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI mainText;
    
    [Header("Scripts")]
    [SerializeField] private NewCardScript cardScript;
    [SerializeField] DeckScript deckScript;

    [Header("Dinero")] 
    [SerializeField] private int startMoney = 1000;
    [SerializeField] private int moneyLeft;
    [SerializeField] private int currentBet = 200; //MEJORAR LA LOGICA DE APOSTAR
    
    [Header("Cartas")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int playerHandValue;
    [SerializeField] private int dealerHandValue;
    public float distanceBetweenCards;

    
    [Header("Spawns")]
    [SerializeField] private Transform playerCardSpawn; 
    [SerializeField] private Transform dealerCardSpawn;

    
    [SerializeField] private int timeBetweenMessages;

    private int splitClicks;
    private int dealerExtraCards;
    private int hitClicks;
    private int bet;
    private int dealerCardNumber = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        dealButton.onClick.AddListener(DealClicked);
        hitButton.onClick.AddListener(HitClicked);
        standButton.onClick.AddListener(StandClicked);
        betButton.onClick.AddListener(BetClicked);
        doubleButton.onClick.AddListener(DoubleClicked);
        //splitButton.onClick.AddListener(SplitClicked);
        
        startButton.gameObject.SetActive(true);
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        betButton.gameObject.SetActive(false);
        doubleButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        
        scoreText.gameObject.SetActive(false);  
        dealerScoreText.gameObject.SetActive(false);  
        cashText.gameObject.SetActive(false);  
        mainText.gameObject.SetActive(true);
        StartCoroutine(ShowMessage("Bienvenido al Blackjack AR", timeBetweenMessages, "Haga Click en Start"));
    }
    
    
    IEnumerator ShowMessage(string message, float delay, string newMessage)
    {
        mainText.text = message;  // Mostrar el mensaje inicial
        yield return new WaitForSeconds(delay);  // Esperar X segundos
        mainText.text = newMessage;  // Cambiar al nuevo mensaje
    }

    public void StartGame()
    {
        bet = 0;
        splitClicks = 0;
        dealerExtraCards = 0;
        hitClicks = 0;
        moneyLeft = startMoney;
        cashText.gameObject.SetActive(true);    
        
        StartRound();
        
    }

    void StartRound()
    {
        startButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        betButton.gameObject.SetActive(true);
        doubleButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        
        StartCoroutine(ShowMessage("Apueste por favor", timeBetweenMessages, "APUESTE"));
        
        cashText.text = moneyLeft.ToString();

        splitClicks = 0;
        dealerExtraCards = 0;
        hitClicks = 0;
    }

    void BetClicked()
    {
        dealButton.gameObject.SetActive(false);
        bet = currentBet;   
        moneyLeft = moneyLeft - bet;
        cashText.text = moneyLeft.ToString();
        dealButton.gameObject.SetActive(true);
        StartCoroutine(ShowMessage("Su apuesta fue de" + bet, timeBetweenMessages, "Haga click en Repatir para comenzar"));
        
        
    }
    void DealClicked()
    {
        DrawCard(false);
        DrawCard(true);
        DrawCard(false);
        DrawCard(true);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(false);
        doubleButton.gameObject.SetActive(true);
        mainText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);  
        dealerScoreText.gameObject.SetActive(true);
        
        /*if () //LOGICA SPLIT
        {
            splitButton.gameObject.SetActive(true);

        }*/
       
    }
    
    void DrawCard(bool isDealer)
    {
        GameObject newCard = Instantiate(cardPrefab, isDealer ? dealerCardSpawn.position : playerCardSpawn.position, Quaternion.Euler(0, 90, 0));
        newCard.transform.SetParent(isDealer ? dealerCardSpawn : playerCardSpawn); // Asigna el padre
        Debug.unityLogger.Log("Carta instanciada");

        NewCardScript cardScript = newCard.GetComponent<NewCardScript>();
        int cardValue = deckScript.DealCard(cardScript);

        if (isDealer)
        {
            dealerHandValue += cardValue;
            dealerScoreText.text = "Dealer: " + dealerHandValue;
        }
        else
        {
            playerHandValue += cardValue;
            scoreText.text = "Jugador: " + playerHandValue;
        }

        Vector3 newPosition = new Vector3(
            (isDealer ? dealerCardSpawn.position.x : playerCardSpawn.position.x) + distanceBetweenCards * (isDealer ? dealerCardNumber : hitClicks),
            isDealer ? dealerCardSpawn.position.y : playerCardSpawn.position.y,
            isDealer ? dealerCardSpawn.position.z : playerCardSpawn.position.z
        );
        newCard.transform.position = newPosition;

        if (isDealer && dealerCardNumber == 2)
        {
            cardScript.SetCardBack();
        }

        Debug.Log("Carta repartida con valor: " + cardValue + (isDealer ? " (Dealer)" : " (Jugador)"));
    }


    void HitClicked()
    {
        DrawCard(false);
        if (playerHandValue >21)
        {
                Lose();
        }
        
        //playerScript.GetCard();
        //scoreText.text = "Jugador: " + playerScript.handValue;
        //if (playerScript.handValue > 21) RoundOver();
    }

    void StandClicked()
    {
        
    }

    void DoubleClicked()
    {
        
    }

    void Lose()
    {
        
    }

    void Win()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
