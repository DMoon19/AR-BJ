using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckNewScript : MonoBehaviour
{
    [Header("Sprites de Cartas")]
    [SerializeField] private Sprite[] normalCardSprites;
    [SerializeField] private Sprite[] asesSprites;
    [SerializeField] private Sprite[] JKQCardSprites;
    
    [Header("Cantidad de Barajas")]
    [SerializeField] private int decks;

    private List<Sprite> deckSprites = new List<Sprite>(); 
    private List<int> deckValues = new List<int>();
    private List<string> discardedCards = new List<string>();

    private int currentIndex = 0;

    void Start()
    {
        InitializeDeck();
        Shuffle();
    }

    void InitializeDeck()
    {
        //int cardsAmount = 52*decks;
        deckSprites.Clear();
        deckValues.Clear();
        discardedCards.Clear();

        for (int deckCount = 0; deckCount < decks; deckCount++) 
        {
            foreach (var sprite in normalCardSprites) { deckSprites.Add(sprite); deckValues.Add(GetValueFromSprite(sprite)); }
            foreach (var sprite in JKQCardSprites) { deckSprites.Add(sprite); deckValues.Add(10); }
            foreach (var sprite in asesSprites) { deckSprites.Add(sprite); deckValues.Add(11); }
        }

        Debug.Log("Mazo inicializado con " + deckSprites.Count + " cartas.");
    }

    public void Shuffle()
    {
        for (int i = deckSprites.Count - 1; i > 0; --i)
        {
            int j = Random.Range(0, i + 1);
            (deckSprites[i], deckSprites[j]) = (deckSprites[j], deckSprites[i]);
            (deckValues[i], deckValues[j]) = (deckValues[j], deckValues[i]);
        }

        currentIndex = 0;
        discardedCards.Clear();
        Debug.Log("Mazo barajado.");
    }

    public int DealCard(CardScript cardScript)
    {
        if (currentIndex >= deckSprites.Count)
        {
            Debug.LogWarning("¡No quedan cartas en el mazo! Barajando de nuevo...");
            Shuffle();
        }

        Sprite drawnSprite = deckSprites[currentIndex];
        int cardValue = deckValues[currentIndex];

        cardScript.SetCardTexture(drawnSprite.texture);
        cardScript.SetValue(cardValue);

        discardedCards.Add(drawnSprite.name + " (Valor: " + cardValue + ")");
        Debug.Log("Carta retirada: " + drawnSprite.name + " (Valor: " + cardValue + ")");

        currentIndex++;

        Debug.Log("Cartas restantes en el mazo: " + (deckSprites.Count - currentIndex));

        return cardValue;
    }

    private int GetValueFromSprite(Sprite sprite)
    {
        string spriteName = sprite.name.ToLower();

        if (spriteName.Contains("j") || spriteName.Contains("q") || spriteName.Contains("k")) return 10;
        if (spriteName.Contains("a")) return 11;

        int parsedValue;
        if (int.TryParse(spriteName, out parsedValue)) return parsedValue;

        return 0; // Error
    }
}
