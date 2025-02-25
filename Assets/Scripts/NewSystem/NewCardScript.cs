using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardScript : MonoBehaviour
{
    [SerializeField] private string number;
    [SerializeField] private int value = 0;

    [SerializeField] private Renderer cardRenderer; // Renderer de la carta
    [SerializeField] private Material cardFrontOriginalMaterial; // Material original para la cara de la carta
    [SerializeField] private Material cardBackMaterial; // Material para el dorso de la carta
    
    private Material cardFrontMaterial;

    [SerializeField] private DeckScript deckScript;

    private void Awake()
    {
        cardFrontMaterial = new Material(cardFrontOriginalMaterial); // Crea una copia independiente
    }


    public void SetValue(int newValue)
    {
        value = newValue;
    }
    
    public int GetValueOfCard()
    {
        return value;
    }
    
    public void SetCardTexture(Texture frontTexture)
    {
        cardFrontMaterial.mainTexture = frontTexture; // Asigna la textura directamente
        cardRenderer.material = cardFrontMaterial; // Usa el material existente
    }


    public void SetCardBack()
    {
        cardRenderer.material = cardBackMaterial; // Asigna el dorso de la carta
    }
}