using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    
    [SerializeField] private AudioSource audioSource;


    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] public TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI betText;
    [SerializeField] private TextMeshProUGUI betText2;

    [SerializeField] private GameObject panelbetwin;
    [SerializeField] private GameObject panelbettie;

    [Header("Scripts")]
    [SerializeField] private NewCardScript cardScript;
    [SerializeField] DeckScript deckScript;
    [SerializeField] private BetScript betScript;

    [Header("Dinero")] 
    [SerializeField] public int startMoney = 1000; //DINERO INICIAL
    [SerializeField] public int moneyLeft; //DINERO RESTANTE TOTAL
    [SerializeField] public int currentBet; //ACA SE ALMACENAN LAS APUESTAS
    
    [Header("Cartas")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int playerHandValue;
    [SerializeField] private int dealerHandValue;
    public float distanceBetweenCards;

    
    [Header("Spawns")]
    [SerializeField] private Transform playerCardSpawn; 
    [SerializeField] private Transform dealerCardSpawn;
    
    [SerializeField] private int timeBetweenMessages;

    [SerializeField] private int delay=2;
    
    [Header("Bets")]
    [SerializeField] private GameObject betCanvas;
    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;

    //private int splitClicks;
    //private int dealerCards;
    private int hitClicks;
    private int bet;
    //private int dealerCardNumber = 0;
    private Vector3 distanceFromCardToCardPlayer; 
    private Vector3 distanceFromCardToCardDealer;
    public float distance;

    private int counter;
    
    private List<int> playerHandCards = new List<int>();
    private List<int> dealerHandCards = new List<int>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 0;
        startButton.onClick.AddListener(StartGame);
        dealButton.onClick.AddListener(DealClicked);
        hitButton.onClick.AddListener(HitClicked);
        standButton.onClick.AddListener(StandClicked);
        doubleButton.onClick.AddListener(DoubleClicked);
        
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
        
        betCanvas.gameObject.SetActive(false);
        timerCanvas.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        
        
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
        //splitClicks = 0;
        //dealerCards = 0;
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
        
        dealerScoreText.text = dealerHandValue.ToString();
        
        if (moneyLeft == 0)
        {
            EndGame();
        }
        
    }
    void DestroyAllSpawnedObjects()
    {
        foreach (Transform child in playerCardSpawn) // O Spawner.transform si el script no está en el Spawner
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in dealerCardSpawn)
        {
            Destroy(child.gameObject);
        }
        StartRound();
    }

    public void BetClicked()
    {
        betCanvas.gameObject.SetActive(true);
        timerCanvas.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);

        StartCoroutine(betScript.Bet()); // Inicia la corrutina correctamente
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

        DrawCard(true, true);
        counter++;

        StartCoroutine(AddDistanceBetweenCards());

        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(false);
        doubleButton.gameObject.SetActive(true);
        dealButton.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);  
        dealerScoreText.gameObject.SetActive(true);
  
        if (playerHandValue == 21 )//BLACKJACK>>>>>>
        {
            BlackJack();
        }
       
    }

    private float restingDistance;
    IEnumerator AddDistanceBetweenCards()
    {
        distance = distanceBetweenCards + distance; //-restingDistance;
        
        if (playerHandValue == 100)//ARREGLAR ESTA LOGICA
        {
            restingDistance = distanceBetweenCards*hitClicks;
        }

        
        yield return distance;
    }

    private int hiddenCardValue;
    void DrawCard(bool isDealer, bool isHidden = false)
    {
        distanceFromCardToCardDealer = dealerCardSpawn.transform.position+ new Vector3(distance, 0, 0);
        distanceFromCardToCardPlayer = playerCardSpawn.transform.position+ new Vector3(distance, 0, 0);
        

        int hiddenCardRotation = 0;

        if (counter == 3)
        {
            hiddenCardRotation = 180;
        }
        
        GameObject newCard = Instantiate(cardPrefab, isDealer ? distanceFromCardToCardDealer : distanceFromCardToCardPlayer, Quaternion.Euler(hiddenCardRotation, 90, 0));
        newCard.transform.SetParent(isDealer ? dealerCardSpawn : playerCardSpawn); // Asigna el padre
        if (isHidden)
        {
            newCard.transform.rotation = Quaternion.Euler(180, 90, 0);
            newCard.tag = "HiddenCard"; // Puedes usar otro tag, pero que sea único
        }
        
        newCard.transform.SetParent(isDealer ? dealerCardSpawn : playerCardSpawn);
        
        NewCardScript cardScript = newCard.GetComponent<NewCardScript>();
        int cardValue = deckScript.DealCard(cardScript);

        List<int> handCards = isDealer ? dealerHandCards : playerHandCards;
        handCards.Add(cardValue);
        
        if (isDealer)
        {
            dealerHandCards.Add(cardValue);
            if (isHidden)
            {
                hiddenCardValue = cardValue;
                dealerHandValue -= cardValue;
            }
            dealerHandValue += cardValue;
            AdjustAceValue(ref dealerHandValue, dealerHandCards);
            dealerScoreText.text = "Dealer: " + dealerHandValue;
        }
        else
        {
            playerHandCards.Add(cardValue);
            playerHandValue += cardValue;
            AdjustAceValue(ref playerHandValue, playerHandCards);
            scoreText.text = "Jugador: " + playerHandValue;
        }
        
    }


    void HitClicked()
    {
        doubleButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        DrawCard(false);
        StartCoroutine(AddDistanceBetweenCards());

        AdjustAceValue(ref playerHandValue, playerHandCards);

        if (playerHandValue >21) //Se pasa
        {
            Lose();
        }

        if (playerHandValue == 21)//BLACKJACK>>>>>>
        {
            BlackJack();
        }

        hitClicks++;
    }
    
    IEnumerator DealerHitCard()
    {
        DrawCard(true);
        StartCoroutine(AddDistanceBetweenCards());
        yield return new WaitForSeconds(0.05f);
        if (dealerHandValue < 17)
        {
            StartCoroutine(DealerHitCard()); // El dealer sigue recibiendo cartas
        }
        else
        {
            EvaluateGameResult(); // Evaluar el resultado del juego
        }
        
    }
    int count;
    private bool isChupando=false;
    private bool isStanding = false;

    public void StandClicked()
    {
        if (isStanding) return; // Evitar múltiples llamadas
        isStanding = true;
        
        Debug.Log("Stand Clicked");
        
        dealButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        
        GameObject hiddenCard = GameObject.FindWithTag("HiddenCard");
        if (hiddenCard != null)
        {
            hiddenCard.transform.rotation = Quaternion.Euler(0, -90, 0);

            // Ahora sí, sumamos su valor a la mano del dealer
            dealerHandValue += hiddenCardValue;
            AdjustAceValue(ref dealerHandValue, dealerHandCards);
            dealerScoreText.text = "Dealer: " + dealerHandValue;
        } 
     
        if (dealerHandValue <17)
        {
            StartCoroutine(DealerHitCard());
        }
        else
        {
            EvaluateGameResult();
        }
        
    }

    private void EvaluateGameResult()
    {
        if (dealerHandValue == 21)
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("blackjack lose");
            BlackJack();
        }
        if (dealerHandValue > 21)
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("WIN DEALERHANDVALUE >21");
            Win().ConfigureAwait(false);
              
        }  
        
        if (playerHandValue <21 && playerHandValue > dealerHandValue) //Gana
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("WIN");
            Win().ConfigureAwait(false);
           
        }
        if (playerHandValue <21 && playerHandValue < dealerHandValue && dealerHandValue < 22) //pierde
        {                   
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("LOSE");
            Lose().ConfigureAwait(false); 
        }

        if (playerHandValue == dealerHandValue)
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("tie");

            Tie().ConfigureAwait(false);
        }
        isStanding = false;
    }

    void DoubleClicked()
    {
        hitClicks++;
        DrawCard(false);
        StartCoroutine(AddDistanceBetweenCards());

        doubleButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        
        moneyLeft = moneyLeft - bet;
        cashText.text = moneyLeft.ToString();
        
        StandClicked();
    }

    private bool isLosing = false;

    private async Task Lose()
    {
        if (isLosing) return;
        isLosing = true;
        isChupando = true;
        await Task.Delay(delay * 1000);
        if (mainText != null)
        {
          Debug.Log("LOSE");
        }
        await Task.Delay(delay * 500);

        EndRound();

        isLosing = false;
    }


    private async Task Win()
    {
        isChupando = true;
        await Task.Delay(delay*1000);

        moneyLeft += (currentBet*2);
       // mainText.gameObject.SetActive(true);
       // mainText.text = "Felicitaciones, ganaste:" + (currentBet*2).ToString();
        panelbetwin.gameObject.SetActive(true);
        betText.text = "$ " + (currentBet*2)+ " COP".ToString();
        audioSource.Play();
        
        await Task.Delay(delay*1000);
        panelbetwin.gameObject.SetActive(false);
        EndRound();
    }

    private async Task BlackJack()
    {
        if (dealerHandValue == 21)
        {
            await Task.Delay(delay*1000);
            Lose();
        }
        else if (playerHandValue == 21)
        {
            moneyLeft = moneyLeft + (currentBet*3);
            panelbetwin.gameObject.SetActive(true);

            betText.text = "$ " + (currentBet*3) + " COP".ToString();
            audioSource.Play();
            await Task.Delay(delay * 1000);
            panelbetwin.gameObject.SetActive(false);

        EndRound();
        }
    }

    private async Task Tie()
    {
        moneyLeft += currentBet;
        panelbettie.gameObject.SetActive(true);

        betText2.text = "$ " + (currentBet) + " COP".ToString();
        await Task.Delay(delay*1000); 
        panelbettie.gameObject.SetActive(false);

        EndRound();
        
    }

    private async Task EndRound()
    {
        isChupando = false;
        playerHandValue = 0;
        scoreText.text = playerHandValue.ToString();

        dealerHandValue = 0;
        dealerScoreText.text = dealerHandValue.ToString();

        hiddenCardValue = 0;

        counter = 0;
        hitClicks = 0;
        distance = 0;
        await Task.Delay(delay*1000);
        DestroyAllSpawnedObjects();
    }
    
    void AdjustAceValue(ref int handValue, List<int> handCards)
    {
        int aceCount = 0;

        foreach (int card in handCards)
        {
            if (card == 11) // Contar la cantidad de Ases en la mano
            {
                aceCount++;
            }
        }

        while (handValue > 21 && aceCount > 0)
        {
            handValue -= 10; // Cambiar un As de 11 a 1
            aceCount--;
        }
    }

    private async Task EndGame()
    {
        mainText.text = "No te queda dinero LUDOPATA, Chau";
        await Task.Delay(5000);
        Application.Quit();

    }

    private void Update()
    {
        print(isChupando);
    }
}
