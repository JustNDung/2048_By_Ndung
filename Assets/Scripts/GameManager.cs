using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiScoreText;
    public TileBoard board;
    public CanvasGroup gameOver;
    private int score;
    private void Start() {
        NewGame();
    }

    public void NewGame() {
        SetScore(0);
        hiScoreText.text = LoadHiScore().ToString();
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;

    }

    public void GameOver() {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay) {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duraion = 0.5f;
        float from = canvasGroup.alpha;
        while (elapsed < duraion) {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duraion);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int point) {
        SetScore(score + point);
    }
    private void SetScore(int score) {
        this.score = score;
        scoreText.text = score.ToString();
        SaveHiScore();
    }

    private void SaveHiScore() {
        int hiscore = LoadHiScore();
        if (score > hiscore) {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiScore() {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
