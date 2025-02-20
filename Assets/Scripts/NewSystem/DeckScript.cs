using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    [Header("Sprites de Cartas")]
    [SerializeField] private Sprite[] normalCardSprites; // Cartas normales (2-10)
    [SerializeField] private Sprite[] asesSprites;      // Ases (A)
    [SerializeField] private Sprite[] JKQCardSprites;   // Figuras (J, Q, K)

    [Header("Cantidad de Barajas")]
    [SerializeField] private int decks = 8; // Número de barajas (8 por defecto)

    private Dictionary<Sprite, int> deck = new Dictionary<Sprite, int>(); // Diccionario para almacenar cartas y sus valores
    private List<Sprite> availableCards = new List<Sprite>(); // Lista de cartas disponibles para repartir

    void Start()
    {
        InitializeDeck();
        Shuffle();
    }

    // Inicializa el mazo con 8 barajas
    void InitializeDeck()
    {
        deck.Clear();
        availableCards.Clear();

        for (int deckCount = 0; deckCount < decks; deckCount++)
        {
            // Agregar cartas normales (2-10)
            foreach (var sprite in normalCardSprites)
            {
                deck[sprite] = GetValueFromSprite(sprite);
                availableCards.Add(sprite);
            }

            // Agregar figuras (J, Q, K)
            foreach (var sprite in JKQCardSprites)
            {
                deck[sprite] = 10; // Las figuras valen 10
                availableCards.Add(sprite);
            }

            // Agregar ases (A)
            foreach (var sprite in asesSprites)
            {
                deck[sprite] = 11; // Los ases valen 11 por defecto
                availableCards.Add(sprite);
            }
        }

        Debug.Log("Mazo inicializado con " + availableCards.Count + " cartas.");
    }

    // Baraja las cartas disponibles
    public void Shuffle()
    {
        for (int i = availableCards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = availableCards[i];
            availableCards[i] = availableCards[j];
            availableCards[j] = temp;
        }

        Debug.Log("Mazo barajado.");
    }

    // Reparte una carta y la elimina del mazo disponible
    public int DealCard(NewCardScript cardScript)
    {
        if (availableCards.Count == 0)
        {
            Debug.LogWarning("¡No quedan cartas en el mazo! Barajando de nuevo...");
            InitializeDeck(); 
            Shuffle();
        }

        int randomIndex = Random.Range(0, availableCards.Count); // Seleccionar una carta aleatoria
        Sprite drawnSprite = availableCards[randomIndex];
        int cardValue = deck[drawnSprite];

        cardScript.SetCardTexture(drawnSprite.texture);
        cardScript.SetValue(cardValue);

        availableCards.RemoveAt(randomIndex); // Eliminar la carta

        return cardValue;
    }


    // Obtiene el valor de una carta basado en su nombre
    private int GetValueFromSprite(Sprite sprite)
    {
        string spriteName = sprite.name.ToLower();

        if (spriteName.Contains("j") || spriteName.Contains("q") || spriteName.Contains("k")) return 10;
        if (spriteName.Contains("a")) return 11;

        // Extraer número directamente del nombre (Ejemplo: "card_7")
        foreach (char c in spriteName)
        {
            if (char.IsDigit(c))
            {
                return int.Parse(c.ToString());
            }
        }

        return 0;
    }
}