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

    [SerializeField] private GameObject betCanvas;
    [SerializeField] private GameObject timerCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    
    

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
    }

    private void IncreaseBet(int amount)
    {
        bet += amount;
        _betText.text = $"Apuesta: {bet}";
    }

    private void DoubleBet()
    {
        bet *= 2;
        _betText.text = $"Apuesta: {bet}";
    }

    private void ResetBet()
    {
        bet = 0;
        _betText.text = "Apuesta: 0";
    }

    public IEnumerator Bet()
    {
        _betButton.gameObject.SetActive(false);
        _dealButton.gameObject.SetActive(false);

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(StartTimer());

        yield return new WaitForSeconds(10f);

        _dealButton.gameObject.SetActive(true);

        betCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        timeText.gameObject.SetActive(false);
        
        _gameManager.currentBet = bet;
        _gameManager.moneyLeft -= _gameManager.currentBet;
        _gameManager.cashText.text = _gameManager.moneyLeft.ToString();
    }

    private IEnumerator StartTimer()
    {
        int timeLeft = 10;
        while (timeLeft > 0)
        {
            _timerText.text = $"{timeLeft}";
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        _timerText.text = "0";
    }
}
