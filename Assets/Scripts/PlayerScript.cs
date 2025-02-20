using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    [SerializeField] private DeckNewScript deckScript;
    [SerializeField] private int money = 1000;

    [Header("Cartas ")]
    [SerializeField] private List<GameObject> playerCards;
    [SerializeField] private List<GameObject> hitCards;
    [SerializeField] private List<GameObject> dealerInitialCards;
    


    public int handValue = 0;
    public GameObject[] hand;
    public int cardIndex = 0;


    List<CardScript> aceList = new List<CardScript>();

    public void StartHand()
    {

        GetCard();
        GetCard();
    }

    public int GetCard()
    {
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());

        Renderer cardRenderer = hand[cardIndex].GetComponent<Renderer>();
        if (cardRenderer != null && cardRenderer.materials.Length > 1)
        {
            // El material 1 es el de la carta, el 0 es el dorso
            //cardRenderer.materials[1].mainTexture = hand[cardIndex].GetComponent<CardScript>().SetCardTexture();
        }

        cardRenderer.enabled = true;

        handValue += cardValue;

        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }

        AceCheck();
        cardIndex++;
        return handValue;
    }

    public void AceCheck()
    {
        // for each ace in the list check
        foreach (CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                ace.SetValue(11);
                handValue += 10;
            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    public int GetMoney()
    {
        return money;
    }

    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            Renderer cardRenderer = hand[i].GetComponent<Renderer>();
            if (cardRenderer != null && cardRenderer.materials.Length > 1)
            {
                // Se asigna la textura del dorso en lugar de usar ResetCard()
                //cardRenderer.materials[1].mainTexture = GameObject.Find("Deck").GetComponent<DeckNewScript>().GetCardBack();
            }
            cardRenderer.enabled = false;
        }

        cardIndex = 0;
        handValue = 0;
        aceList.Clear();
    }
}
