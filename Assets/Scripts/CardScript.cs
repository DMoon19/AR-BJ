using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] private string number;
    [SerializeField] private int value = 0;

    [SerializeField] private Renderer cardRenderer; // Renderer de la carta
    [SerializeField] private Material cardFrontMaterial; // Material para la cara de la carta
    [SerializeField] private Material cardBackMaterial; // Material para el dorso de la carta

    [SerializeField] private DeckNewScript deckScript;

    public int GetValueOfCard()
    {
        return value;
    }

    public void SetValue(int newValue)
    {
        value = newValue;
    }

    public void SetCardTexture(Texture newTexture)
    {
        cardFrontMaterial.mainTexture = newTexture; // Cambia el albedo de la carta
        cardRenderer.material = cardFrontMaterial; // Asigna el material de la carta
    }

    public void SetCardBack()
    {
        cardRenderer.material = cardBackMaterial; // Asigna el dorso de la carta
    }
}