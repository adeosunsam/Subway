using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private Player player;

    private bool isGameStarted = false;
    private const int COIN_SCORE_AMOUNT = 5;

    [SerializeField]
    public TextMeshProUGUI scoreText, coinText, modifierText;

    private int lastScore;

    private float score, coinScore, modifierScore;
    private void Awake()
    {
        Instance = this;
        player = FindObjectOfType<Player>();
        modifierScore = 1.0f;
    }

    private void Update()
    {
        if(MobileInput.Instance.Tap && !isGameStarted)
        {
            player.StartRunning();
            isGameStarted = true;
        }
        if(isGameStarted)
        {
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
            
        }
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString();
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = $"x{modifierScore}";
    }
}
