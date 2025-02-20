using UnityEngine;
using System.Collections.Generic;
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
    [SerializeField] Text dealerScoreText;
    [SerializeField] Text betsText;
    [SerializeField] Text cashText;
    [SerializeField] Text mainText;
    
    [Header("Juego")]
    [SerializeField] PlayerScript playerScript;
    [SerializeField] PlayerScript dealerScript;
    [SerializeField] DeckNewScript deckScript;
    
    [Header("Apuestas")]
    [SerializeField] int moneyLeft = 1000;
    [SerializeField] int currentBet = 0;
    
    private int standClicks = 0;

    void Start()
    {
        dealButton.onClick.AddListener(DealClicked);
        hitButton.onClick.AddListener(HitClicked);
        standButton.onClick.AddListener(StandClicked);
        betButton.onClick.AddListener(BetClicked);
        doubleButton.onClick.AddListener(DoubleClicked);
        splitButton.onClick.AddListener(SplitClicked);
    }

    void DealClicked()
    {
        playerScript.ResetHand();
        dealerScript.ResetHand();
        deckScript.Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();

        scoreText.text = "Jugador: " + playerScript.handValue;
        dealerScoreText.text = "Dealer: " + dealerScript.handValue;

        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        doubleButton.gameObject.SetActive(true);
        //splitButton.gameObject.SetActive(playerScript.CanSplit());
    }

    void HitClicked()
    {
        playerScript.GetCard();
        scoreText.text = "Jugador: " + playerScript.handValue;
        if (playerScript.handValue > 21) RoundOver();
    }

    void StandClicked()
    {
        standClicks++;
        if (standClicks > 1) RoundOver();
        else HitDealer();
    }

    void HitDealer()
    {
        while (dealerScript.handValue < 17)
        {
            dealerScript.GetCard();
        }
        dealerScoreText.text = "Dealer: " + dealerScript.handValue;
        RoundOver();
    }

    void RoundOver()
    {
        string result = "";
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        
        if (playerBust) result = "Perdiste";
        else if (dealerBust || playerScript.handValue > dealerScript.handValue) result = "Ganaste";
        else if (playerScript.handValue == dealerScript.handValue) result = "Empate";
        else result = "Perdiste";
        
        mainText.text = result;
        dealButton.gameObject.SetActive(true);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        doubleButton.gameObject.SetActive(false);
        splitButton.gameObject.SetActive(false);
    }

    void BetClicked()
    {
        currentBet += 20;
        moneyLeft -= 20;
        betsText.text = "Apuesta: " + currentBet;
        cashText.text = "$" + moneyLeft;
    }
    
    void DoubleClicked()
    {
        if (moneyLeft >= currentBet)
        {
            moneyLeft -= currentBet;
            currentBet *= 2;
            betsText.text = "Apuesta: " + currentBet;
            cashText.text = "$" + moneyLeft;
            HitClicked();
            StandClicked();
        }
    }
    
    void SplitClicked()
    {
        // Implementar lógica para dividir manos
    }
}
