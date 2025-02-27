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

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] public TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI mainText;
    
    [Header("Scripts")]
    [SerializeField] private NewCardScript cardScript;
    [SerializeField] DeckScript deckScript;
    [SerializeField] private BetScript betScript;

    [Header("Dinero")] 
    [SerializeField] public int startMoney = 1000;
    [SerializeField] public int moneyLeft;
    [SerializeField] public int currentBet; 
    
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

        DrawCard(true);
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
        
        /*if () //LOGICA SPLIT
        {
            splitButton.gameObject.SetActive(true);

        }*/
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
    void DrawCard(bool isDealer)
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
        if (counter == 3)
        {
            newCard.tag = "Mierda";
        }

        
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
        
    }


    void HitClicked()
    {
        doubleButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(false);
        DrawCard(false);
        StartCoroutine(AddDistanceBetweenCards());

        if (playerHandValue >21) //Se pasa
        {
            Debug.Log("LOSE DESDE HIT");
            Lose();
        }

        if (playerHandValue == 21)//BLACKJACK>>>>>>
        {
            Debug.Log("BLACKJACK DESDE HIT");

            BlackJack();
        }

        hitClicks++;
    }
    
    IEnumerator DealerHitCard()
    {
        DrawCard(true);
        StartCoroutine(AddDistanceBetweenCards());
        yield return new WaitForSeconds(0.05f);
        StandClicked();
        
    }
    int count;
    private bool isChupando=false;
    public void StandClicked()
    {
        dealButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
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
        else if(dealerHandValue >= 17)
        {

            
            
            if (dealerHandValue == 21)
            {
                if (isChupando) return;
                BlackJack();
               
            }
            if (dealerHandValue > 21)
            {
                if (isChupando) return;
                Debug.Log("WIN DEALERHANDVALUE >21");
                Win().ConfigureAwait(false);
              
            }  
        
            if (playerHandValue <21 && playerHandValue > dealerHandValue) //Gana
            {
                if (isChupando) return;
                Debug.Log("WIN NORMALITO");
                Win().ConfigureAwait(false);
           
            }
            if (playerHandValue <21 && playerHandValue < dealerHandValue && dealerHandValue < 22) //pierde
            {                                   //Si mano de jugador es menor a 21 
                if (isChupando) return;
                print(playerHandValue <21 && playerHandValue < dealerHandValue && dealerHandValue < 22);
                Debug.Log("LOSE NORMALITO");   //Si mano de jugador es menor a mano de dealer y es menor a 21
                Lose().ConfigureAwait(false);  //Si mano de jugador es menor a mano de dealer y es menor a 21;                       //  
            }

            if (playerHandValue == dealerHandValue)
            {
                if (isChupando) return;
                Debug.Log("TIE");
                Tie().ConfigureAwait(false);
            }
        }
        
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
            mainText.gameObject.SetActive(true);
            mainText.text = "Perdiste :c, LOOOSERRRR";
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
        mainText.gameObject.SetActive(true);
        mainText.text = "Felicitaciones, ganaste:" + moneyLeft.ToString();
        await Task.Delay(delay*1000);

        EndRound();
    }

    private async Task BlackJack()
    {
        if (dealerHandValue == 21)
        {
            mainText.text = "21 DEL DEALER, PERDISTE";
            await Task.Delay(delay*1000);
            Lose();
        }
        else if (playerHandValue == 21)
        {
            moneyLeft = moneyLeft + (currentBet*3);
            mainText.text = "BLACKJACKKKK FELICITACIONES, Ganaste: " + moneyLeft.ToString();
            await Task.Delay(delay * 1000);

        EndRound();
        }
    }

    private async Task Tie()
    {
        moneyLeft += currentBet;
        mainText.text = "Empate, No perdiste ni ganaste";
        EndRound();
    }

    private async Task EndRound()
    {
        isChupando = false;
        playerHandValue = 0;
        scoreText.text = playerHandValue.ToString();

        dealerHandValue = 0;
        dealerScoreText.text = dealerHandValue.ToString();

        counter = 0;
        hitClicks = 0;
        distance = 0;
        await Task.Delay(delay*1000);
        DestroyAllSpawnedObjects();
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
