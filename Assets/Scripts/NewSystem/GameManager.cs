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
    [SerializeField] private TextMesh scorePlayer;
    [SerializeField] private TextMesh scoreDealer;

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
    private int hitCount = 0;
    
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
        
        //dealerScoreText.text = dealerHandValue.ToString();
        scoreDealer.text = dealerHandValue.ToString();
        
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
        

        if (playerHandValue == 21 )//BLACKJACK>>>>>>
        {
            BlackJack();
            return;
        }
        
        
        
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(false);
        doubleButton.gameObject.SetActive(true);
        dealButton.gameObject.SetActive(false);
        mainText.gameObject.SetActive(false);
        //scoreText.gameObject.SetActive(true);  
        //dealerScoreText.gameObject.SetActive(true);
  
       
    }

    private float restingDistance;
    IEnumerator AddDistanceBetweenCards()
    {
        distance = distanceBetweenCards + distance; //-restingDistance;
        if (playerHandValue==100)
        {
            distanceBetweenCards -= distanceBetweenCards * hitCount;
            distance += distanceBetweenCards;
        }

        yield return distance; // Agrega un pequeño delay si lo necesitas
    }

    private int hiddenCardValue;
    void DrawCard(bool isDealer, bool isHidden = false)
    {

        distanceFromCardToCardDealer = dealerCardSpawn.transform.position + new Vector3(distance, 0, 0);
        distanceFromCardToCardPlayer = playerCardSpawn.transform.position + new Vector3(distance, 0, 0);

        int hiddenCardRotation = 0;
        GameObject newCard = Instantiate(cardPrefab, isDealer ? distanceFromCardToCardDealer : distanceFromCardToCardPlayer, Quaternion.Euler(hiddenCardRotation, 90, 0));
        newCard.transform.SetParent(isDealer ? dealerCardSpawn : playerCardSpawn);

        if (isHidden)
        {
            newCard.transform.rotation = Quaternion.Euler(180, 90, 0);
            newCard.tag = "HiddenCard";
        }

        NewCardScript cardScript = newCard.GetComponent<NewCardScript>();
        int cardValue = deckScript.DealCard(cardScript);

        if (isDealer)
        {
            dealerHandCards.Add(cardValue); // Ahora solo se añade una vez

            if (isHidden)
            {
                hiddenCardValue = cardValue;
                dealerHandValue -= cardValue;
            }
            dealerHandValue += cardValue;
            scoreDealer.text = "" + dealerHandValue;
            AdjustAceValue(ref dealerHandValue, dealerHandCards);
        }
        else
        {
            playerHandCards.Add(cardValue); // Ahora solo se añade una vez
            playerHandValue += cardValue;
            AdjustAceValue(ref playerHandValue, playerHandCards);
            scorePlayer.text = "" + playerHandValue;
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
            //dealerScoreText.text = "Dealer: " + dealerHandValue;
            scoreDealer.text =""+ dealerHandValue;

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
            Debug.Log("BJ LOSE (LOSE 1)");
            BlackJack().ConfigureAwait(false);
        }
        if (dealerHandValue > 21 && playerHandValue < 22)
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("WIN DEALERHANDVALUE >21 (WIN 1)");
            Win().ConfigureAwait(false);
              
        }  
        
        if (playerHandValue <21 && playerHandValue > dealerHandValue) //Gana
        {
            Debug.Log(dealerHandValue);
            Debug.Log(playerHandValue);
            if (isChupando) return;
            Debug.Log("WIN (WIN 2)");
            Win().ConfigureAwait(false);
           
        }
        if (playerHandValue <21 && playerHandValue < dealerHandValue && dealerHandValue < 22) //pierde
        {                   
            Debug.Log("LOSE3 "+dealerHandValue);
            Debug.Log("LOSE3 "+playerHandValue);
            if (isChupando) return;
            Debug.Log("LOSE (LOSE 3)");
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

        if (playerHandValue > 21 && dealerHandValue > 21)
        {
            Debug.Log("Ambos pierden LOSE 4");
            Lose().ConfigureAwait(false);
        }
        isStanding = false;
    }

    void DoubleClicked()
    {
        if (hitClicks > 0) // No permitir el double si ya se ha tomado otra carta
        {
            Debug.Log("No se puede doblar después de haber pedido una carta");
            return;
        }

        hitClicks++;
        DrawCard(false);
        StartCoroutine(AddDistanceBetweenCards());

        // Ocultar botones después del double
        doubleButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
    
        // Duplicar la apuesta
        moneyLeft -= currentBet;
        currentBet *= 2;
        cashText.text = moneyLeft.ToString();
    
        // Llamar a la lógica de Stand después de doblar
        StandClicked();
    }


    private bool isLosing = false;

    private async Task Lose()
    {
        standButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        GameObject hiddenCard = GameObject.FindWithTag("HiddenCard");
        if (hiddenCard != null)
        {
            hiddenCard.transform.rotation = Quaternion.Euler(0, -90, 0);

            // Ahora sí, sumamos su valor a la mano del dealer
            dealerHandValue += hiddenCardValue;
            AdjustAceValue(ref dealerHandValue, dealerHandCards);
            //dealerScoreText.text = "Dealer: " + dealerHandValue;
            scoreDealer.text =""+ dealerHandValue;

        } 
        await Task.Delay(delay * 1000);
        if (mainText != null)
        {
          Debug.Log("LOSETASK");
        }
        await Task.Delay(delay * 500);

        EndRound();

        isLosing = false;
    }


    private async Task Win()
    {

        await Task.Delay(delay*1000);

        moneyLeft += (currentBet*2);
       // mainText.gameObject.SetActive(true);
       // mainText.text = "Felicitaciones, ganaste:" + (currentBet*2).ToString();
        panelbetwin.gameObject.SetActive(true);
        Debug.Log(currentBet);
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
            if (playerHandValue != 21)
            {
                await Task.Delay(delay*1000);
                Debug.Log("BLACKJACK PLAYER LOSE");
                Lose();
            }

            else if (playerHandValue == 21)
            {
                standButton.gameObject.SetActive(false);
                hitButton.gameObject.SetActive(false);
                Debug.Log("BLACKJACK TIE");
                Tie();
                
            }
        }
        else if (dealerHandValue > 21)
        {
            GameObject hiddenCard = GameObject.FindWithTag("HiddenCard");
            if (hiddenCard != null)
            {
                standButton.gameObject.SetActive(false);
                hitButton.gameObject.SetActive(false);
                hiddenCard.transform.rotation = Quaternion.Euler(0, -90, 0);

                // Ahora sí, sumamos su valor a la mano del dealer
                dealerHandValue += hiddenCardValue;
                AdjustAceValue(ref dealerHandValue, dealerHandCards);
                //dealerScoreText.text = "Dealer: " + dealerHandValue;
                scoreDealer.text =""+ dealerHandValue;
                panelbetwin.gameObject.SetActive(true);
                moneyLeft += currentBet*3;
                betText.text = "$ " + (currentBet*3) + " COP".ToString();
                audioSource.Play();

                Debug.Log("BLACKJACK dealer bust");
                
            } 
            await Task.Delay(delay*1000);
            panelbetwin.gameObject.SetActive(false);

            EndRound();
        }
        else if(dealerHandValue < playerHandValue &&
                playerHandValue == 21)
        {
            GameObject hiddenCard = GameObject.FindWithTag("HiddenCard");
            if (hiddenCard != null)
            {
                hiddenCard.transform.rotation = Quaternion.Euler(0, -90, 0);
                dealerHandValue += hiddenCardValue;
                AdjustAceValue(ref dealerHandValue, dealerHandCards);
                //dealerScoreText.text = "Dealer: " + dealerHandValue;
                scoreDealer.text =""+ dealerHandValue;
                
                if (dealerHandValue >= 17) //Solo se voltea la carta
                {
                    standButton.gameObject.SetActive(false);
                    hitButton.gameObject.SetActive(false);
                    
                    panelbetwin.gameObject.SetActive(true);
                    moneyLeft += currentBet*3;
                    betText.text = "$ " + (currentBet*3) + " COP".ToString();
                    audioSource.Play();
                    
                    Debug.Log("BLACKJACK entre 17 y 20");
                    await Task.Delay(delay*1000);
                    panelbetwin.gameObject.SetActive(false);
                    EndRound();
                }
                else if (dealerHandValue < 17) //Se voltea la carta y pide hasta empatar o perder
                {
                    
                    DrawCard(isDealer: true, isHidden: false);
                    BlackJack();
                    panelbetwin.gameObject.SetActive(true);
                    moneyLeft += currentBet*3;
                    betText.text = "$ " + (currentBet*3) + " COP".ToString();
                    audioSource.Play();
                    Debug.Log("BLACKJACK menos a 17 ");

                    await Task.Delay(delay*1000);
                    panelbetwin.gameObject.SetActive(false);
                    EndRound();
                }


            } 
        
        }
    }

    private async Task Tie()
    {
        GameObject hiddenCard = GameObject.FindWithTag("HiddenCard");
        if (hiddenCard != null)
        {
            hiddenCard.transform.rotation = Quaternion.Euler(0, -90, 0);

            // Ahora sí, sumamos su valor a la mano del dealer
            dealerHandValue += hiddenCardValue;
            AdjustAceValue(ref dealerHandValue, dealerHandCards);
            //dealerScoreText.text = "Dealer: " + dealerHandValue;
            scoreDealer.text =""+ dealerHandValue;
            panelbettie.gameObject.SetActive(true);
            moneyLeft += currentBet;
            betText2.text = "$ " + (currentBet) + " COP".ToString();
            audioSource.Play();

        } 
        await Task.Delay(delay*1000);
        panelbettie.gameObject.SetActive(false);

        EndRound();
        
    }

    public async Task EndRound()
    {
        
        dealerHandCards.Clear();
        playerHandCards.Clear();
        isChupando = false;
        playerHandValue = 0;
        //scoreText.text = playerHandValue.ToString();
        scorePlayer.text =playerHandValue.ToString();


        dealerHandValue = 0;
        //dealerScoreText.text = dealerHandValue.ToString();
        scoreDealer.text =dealerHandValue.ToString();

        hiddenCardValue = 0;

        counter = 0;
        hitClicks = 0;
        distance = 0;
        await Task.Delay(delay*1000);
        DestroyAllSpawnedObjects();
    }
    
    private void AdjustAceValue(ref int handValue, List<int> handCards)
    {
        int aceCount = 0;
        int tempHandValue = 0;

        foreach (int card in handCards)
        {
            tempHandValue += card;
            if (card == 11) 
            {
                Debug.Log("carta "+card);
                aceCount++;
            }
        }

        while (tempHandValue > 21 && aceCount > 0)
        {
            tempHandValue -= 10;
            aceCount--;
        }
        Debug.Log("tempHandValue "+tempHandValue);
        handValue = tempHandValue;
    }


    private async Task EndGame()
    {
        mainText.text = "No te queda dinero LUDOPATA, Chau";
        await Task.Delay(5000);
        Application.Quit();

    }   
}
