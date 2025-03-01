using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BetScript : MonoBehaviour
{
    [SerializeField] private Button _500Button;
    [SerializeField] private Button _1000Button;
    [SerializeField] private Button _2500Button;
    [SerializeField] private Button _5000Button;
    [SerializeField] private Button _10000Button;
    [SerializeField] private Button _50000Button;
    [SerializeField] private Button _x2Button;
    [SerializeField] private Button _returnButton;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _timerText;  
    [SerializeField] private TextMeshProUGUI _betText;
    [SerializeField] private Button _dealButton;
    [SerializeField] private Button _betButton;
    [SerializeField] private Button _confirmButton;

    [SerializeField] private GameObject betCanvas;
    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    
    bool betConfirmed = false;

    private int bet = 0;
    private Coroutine timerCoroutine;

    private void Start()
    {
        _500Button.onClick.AddListener(() => IncreaseBet(500));
        _1000Button.onClick.AddListener(() => IncreaseBet(1000));
        _2500Button.onClick.AddListener(() => IncreaseBet(2500));
        _5000Button.onClick.AddListener(() => IncreaseBet(5000));
        _10000Button.onClick.AddListener(() => IncreaseBet(10000));
        _50000Button.onClick.AddListener(() => IncreaseBet(50000));
        _x2Button.onClick.AddListener(DoubleBet);
        _returnButton.onClick.AddListener(ResetBet);
        _confirmButton.gameObject.SetActive(false);

    }

    private void IncreaseBet(int amount)
    {
        if (bet + amount <= _gameManager.moneyLeft)
        {
            bet += amount;
            _betText.text = $"Apuesta: {bet}";
        }
        else
        {
            Debug.Log("No tienes suficiente dinero para esta apuesta.");
        }
    }

    private void DoubleBet()
    {
        if (bet * 2 <= _gameManager.moneyLeft)
        {
            bet *= 2;
            _betText.text = $"Apuesta: {bet}";
        }
        else
        {
            Debug.Log("No tienes suficiente dinero para duplicar la apuesta.");
        }
    }

    public void ResetBet()
    {
        bet = 0;
        _betText.text = "Apuesta: 0";
    }

    public IEnumerator Bet()
    {
        betConfirmed = false;
        _betButton.gameObject.SetActive(false);
        _dealButton.gameObject.SetActive(false);

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(StartTimer());

        float elapsedTime = 0f;
        while (elapsedTime < 10f && !betConfirmed)
        {
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
        }

        if (!betConfirmed) ConfirmBet(); // Si no se confirmó manualmente, ejecuta ConfirmBet

       
    }

    private IEnumerator StartTimer()
    {
        int timeLeft = 10;
        while (timeLeft > 0 && !betConfirmed)
        {
            if (bet > 0)
                _confirmButton.gameObject.SetActive(true);
            else
                _confirmButton.gameObject.SetActive(false);

            _timerText.text = $"{timeLeft}";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        _timerText.text = "0";
        betCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        timeText.gameObject.SetActive(false);
        _gameManager.EndRound();
    }

    public void ConfirmBet()
    {
        betConfirmed = true;
        print(bet + " " + _gameManager.moneyLeft);
        _dealButton.gameObject.SetActive(true);
        betCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        timeText.gameObject.SetActive(false);
        if (bet <= _gameManager.moneyLeft)
        {
            _gameManager.currentBet = bet;
            Debug.Log("Apuesta confirmada"+_gameManager.currentBet);
            _gameManager.moneyLeft -= _gameManager.currentBet;
            Debug.Log("Apuesta confirmada 2 "+_gameManager.currentBet);


            _gameManager.cashText.text = _gameManager.moneyLeft.ToString();
            Debug.Log("Apuesta confirmada 3 "+_gameManager.currentBet);

        }
        else
        {
            Debug.Log("Error: Apuesta mayor al dinero disponible. Reiniciando apuesta.");
            ResetBet();
        }
    }
}
