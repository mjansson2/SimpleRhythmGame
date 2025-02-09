using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SimpleRhythmGame.Core
{
    public enum GameState
    {
        StartScreen,
        Playing,
        GameOver
    }

    public class GameStateManager : MonoBehaviour
    {

        [SerializeField] private GameObject         m_startScreen;
        [SerializeField] private GameObject         m_gameplay;
        [SerializeField] private GameObject         m_gameOver;

        [SerializeField] private TextMeshProUGUI    m_finalScoreText;
        [SerializeField] private TextMeshProUGUI    m_finalComboText;
        [SerializeField] private TextMeshProUGUI    m_perfectHitText;
        [SerializeField] private TextMeshProUGUI    m_goodHitText;
        [SerializeField] private TextMeshProUGUI    m_missesText;

        private GameState                           m_currentState;
        private GameManager                         m_gameManager;

        public static GameStateManager Instance {  get; private set; }

        #region Properties
        public GameState CurrentState => m_currentState;
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            m_gameManager = GameManager.Instance;

            TransitionToState(GameState.StartScreen);
        }

        public void TransitionToState(GameState newState)
        {
            Debug.Log($"Transitioning to state: {newState}");
            m_startScreen.SetActive(false);
            m_gameplay.SetActive(false);
            m_gameOver.SetActive(false);

            switch (newState)
            {
                case GameState.StartScreen:
                    m_startScreen.SetActive(true);
                    break;
                case GameState.Playing:
                    m_gameplay.SetActive(true);
                    break;
                case GameState.GameOver:
                    Debug.Log("Entering GameOver state");
                    m_gameOver.SetActive(true);
                    ShowGameResults();
                    break;
            }

            m_currentState = newState;
        }

        private void ShowGameResults()
        {
            int finalScore = m_gameManager.Score;
            int finalCombo = m_gameManager.MaxCombo;
            int perfectHits = m_gameManager.PerfectHits;
            int goodHits = m_gameManager.GoodHits;
            int misses = m_gameManager.Misses;

            m_finalScoreText.text = $"Score: {finalScore}";
            m_finalComboText.text = $"Max Combo: {finalCombo}";
            m_perfectHitText.text = $"Perfect Hits: {perfectHits}";
            m_goodHitText.text = $"Good Hits: {goodHits}"; 
            m_missesText.text = $"Misses: {misses}";
        }

        public void OnPlayButtonClicked()
        {
            m_gameManager.ResetGame();
            TransitionToState(GameState.Playing);
            AudioManager.Instance.PlaySong();
        }

        public void OnRestartButtonClicked()
        {
            TransitionToState(GameState.StartScreen);
        }

        public void OnQuitButtonClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}