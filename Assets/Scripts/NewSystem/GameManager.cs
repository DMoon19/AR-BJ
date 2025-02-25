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
    private int dealerCards;
    private int hitClicks;
    private int bet;
    private int dealerCardNumber = 0;
    private Vector3 distanceFromCardToCardPlayer; 
    private Vector3 distanceFromCardToCardDealer;
    public float distance;

    private int counter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 0;
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
        dealerCards = 0;
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
        
        counter = 0;
        splitClicks = 0;
        dealerCards = 0;
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
        counter++;
        DrawCard(true);
        counter++;
        StartCoroutine(AddDistanceBetweenCards());

        DrawCard(false);
        counter++;
        DrawCard(true);
        counter++;
        StartCoroutine(AddDistanceBetweenCards());

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
        if (playerHandValue == 21)//BLACKJACK>>>>>>
        {
            //BlackJack();
        }
       
    }
    IEnumerator AddDistanceBetweenCards()
    {
        distance = distanceBetweenCards + distance;
        yield return distance;
    }

    private int hiddenCardValue;
    void DrawCard(bool isDealer)
    {
        distanceFromCardToCardDealer = dealerCardSpawn.transform.position+ new Vector3(distance, 0, 0);
        distanceFromCardToCardPlayer = playerCardSpawn.transform.position+ new Vector3(distance, 0, 0);

        int hiddenCardRotation = 0;

        if (counter == 3)
        {
            
            hiddenCardRotation = 180;
            Debug.Log(counter);
        }
        
        GameObject newCard = Instantiate(cardPrefab, isDealer ? distanceFromCardToCardDealer : distanceFromCardToCardPlayer, Quaternion.Euler(hiddenCardRotation, 90, 0));
        newCard.transform.SetParent(isDealer ? dealerCardSpawn : playerCardSpawn); // Asigna el padre
        if (counter == 3)
        {
            newCard.tag = "Mierda";
        }

        Debug.unityLogger.Log("Carta instanciada");
        
        NewCardScript cardScript = newCard.GetComponent<NewCardScript>();
        int cardValue = deckScript.DealCard(cardScript);
        
        if (hiddenCardRotation == 180)
        {
            hiddenCardValue = cardValue;
            dealerHandValue -= cardValue;
            Debug.Log("Valor Carta oculta "+ hiddenCardValue);
        }
        
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
        
        if (isDealer && dealerCardNumber == 2)
        {
            cardScript.SetCardBack();
        }

        Debug.Log("Carta repartida con valor: " + cardValue + (isDealer ? " (Dealer)" : " (Jugador)"));
    }


    void HitClicked()
    {
        DrawCard(false);
        StartCoroutine(AddDistanceBetweenCards());
        if (playerHandValue >21) //Se pasa
        {
            Lose();
        }

        if (playerHandValue == 21)//BLACKJACK>>>>>>
        {
            //BlackJack();
        }
    }
    
    IEnumerator DealerHitCard()
    {
        DrawCard(true);
        StartCoroutine(AddDistanceBetweenCards());
        yield return new WaitForSeconds(0.05f);
        if (dealerHandValue < 17)
        {
            StandClicked();
            //StartCoroutine(DealerHitCard());
        }
        
    }
    int count;
    public void StandClicked()
    {
        GameObject carta = GameObject.FindWithTag("Mierda");
        carta.transform.rotation = Quaternion.Euler(0, -90, 0);
        
        if (count == 0)
        {
            int realDealerValue = dealerHandValue+hiddenCardValue;
            dealerHandValue = realDealerValue;
            dealerScoreText.text = "Dealer: " + dealerHandValue;
            count++;
        } 
        if (dealerHandValue <17)
        {
            StartCoroutine(DealerHitCard());
        }
        else
        {
            if (dealerHandValue > 21)
            {
                Debug.Log("WIN");
                //Win;
            }  
        
            if (playerHandValue <21 && playerHandValue > dealerHandValue) //Gana
            {
                Debug.Log("WIN");
                //Win();
            }
            if (playerHandValue <21 && playerHandValue < dealerHandValue) //pierde
            {
                Debug.Log("LOSE");
                Lose();
            }

            if (playerHandValue == dealerHandValue)
            {
                Debug.Log("TIE");
                //Tie();
            }
        }
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
