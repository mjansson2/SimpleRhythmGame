using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Events;
using EventHandler = GameTools.Events.EventHandler;
using SimpleRhythmGame.Events;
using UnityEngine.UI;
using SimpleRhythmGame.UI;

namespace SimpleRhythmGame.Core
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform      m_hitLine;
        [SerializeField] private RectTransform      m_noteContainer;
        [SerializeField] private GameObject         m_notePrefab;
        [SerializeField] private GameObject         m_hitEffectPrefab;
     
        [Header("Game Settings")]
        [SerializeField] private int                m_iNumberOfLanes = 4;
        [SerializeField] private float              m_fNoteSpeed = 500f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip          m_songTrack;
        [SerializeField] private float              m_bpm = 120f;
        [SerializeField] private float              m_endFadeOutDuration = 25f;

        [Header("Input Settings")]
        [SerializeField] private float              m_fPerfectWindow = 20f;
        [SerializeField] private float              m_fGoodWindow = 100f;

        [Header("UI Canvases")]
        [SerializeField] private Canvas             m_mainMenuCanvas;
        [SerializeField] private Canvas             m_gameplayCanvas;
        [SerializeField] private Canvas             m_gameOverCanvas;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button             m_playGameButton;
        [SerializeField] private Button             m_mainMenuQuitButton;

        [Header("Game Over Buttons")]
        [SerializeField] private Button             m_mainMenuButton;
        [SerializeField] private Button             m_gameOverQuitButton;

        private void Awake()
        {
            EventHandler eventHandler = EventHandler.Main;

            if (GameManager.Instance == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
                gameManager.Initialize(m_iNumberOfLanes, m_fNoteSpeed, m_hitLine, m_noteContainer, m_notePrefab, m_hitEffectPrefab);
                DontDestroyOnLoad(gameManagerObject);
                Debug.Log("GameInitializer: GameManager created and initialized");
            }

            if (AudioManager.Instance == null)
            {
                GameObject audioManagerObject = new GameObject("AudioManager");
                AudioManager audioManager = audioManagerObject.AddComponent<AudioManager>();
                if (m_songTrack == null)
                {
                    Debug.LogError("GameInitializer: Missing AudioClip for AudioManager!");
                }
                else
                {
                    audioManager.Initialize(m_songTrack, m_bpm, m_endFadeOutDuration);
                }

                DontDestroyOnLoad(audioManagerObject);
                Debug.Log("GameInitializer: AudioManager created and initialized");
            }

            if (InputManager.Instance == null)
            {
                GameObject inputManagerObject = new GameObject("InputManager");
                InputManager inputManager = inputManagerObject.AddComponent<InputManager>();
                inputManager.Initialize(m_fPerfectWindow, m_fGoodWindow);
                DontDestroyOnLoad(inputManagerObject);
                Debug.Log("GameInitializer: InputManager created and initialized");
            }

            if (UIManager.Instance == null)
            {
                GameObject uiManagerObject = new GameObject("UIManager");
                UIManager uiManager = uiManagerObject.AddComponent<UIManager>();
                uiManager.Initialize(m_mainMenuCanvas, m_gameplayCanvas, m_gameOverCanvas);
                uiManager.RegisterButton(UIButton.PlayGame, m_playGameButton);
                uiManager.RegisterButton(UIButton.MainMenuQuit, m_mainMenuQuitButton);
                uiManager.RegisterButton(UIButton.MainMenu, m_mainMenuButton);
                uiManager.RegisterButton(UIButton.GameOverQuit, m_gameOverQuitButton);

                DontDestroyOnLoad(uiManagerObject);
                Debug.Log("GameInitializer: UIManager created and initialized");
            }

            eventHandler.PushEvent(new MainMenuEvent());
            Debug.Log("GameInitializer: Initialization complete, MainMenuEvent pushed");
        }
    }
}
