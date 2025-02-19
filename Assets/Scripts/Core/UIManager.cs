using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleRhythmGame.UI;
using UnityEngine.UI;
using UnityEditor;

namespace SimpleRhythmGame.Core
{
    public class UIManager : MonoBehaviour
    {
        private Canvas      m_mainMenuCanvas;
        private Canvas      m_gameplayCanvas;
        private Canvas      m_gameOverCanvas;

        private Canvas      m_currentCanvas;

        private Dictionary<UIButton, List<Button>> m_buttonMapping = new Dictionary<UIButton, List<Button>>();

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Initialize(Canvas mainMenuCanvas, Canvas gameplayCanvas, Canvas gameOverCanvas)
        {
            m_mainMenuCanvas = mainMenuCanvas;
            m_gameplayCanvas = gameplayCanvas;
            m_gameOverCanvas = gameOverCanvas;
        }

        public void Show(UIScreen screen)
        {
            HideAll();

            switch (screen)
            {
                case UIScreen.MainMenu:
                    m_currentCanvas = m_mainMenuCanvas;
                    break;
                case UIScreen.Gameplay:
                    m_currentCanvas = m_gameplayCanvas;
                    break;
                case UIScreen.GameOver:
                    m_currentCanvas = m_gameOverCanvas;
                    break;
                default:
                    Debug.LogError($"UIManager: Unknown UIScreen {screen}");
                    return;
            }

            if (m_currentCanvas != null)
            {
                m_currentCanvas.enabled = true;
            }
        }

        public void Hide(UIScreen screen)
        {
            switch (screen)
            {
                case UIScreen.MainMenu:
                    if (m_mainMenuCanvas != null) m_mainMenuCanvas.enabled = false;
                    break;
                case UIScreen.Gameplay:
                    if (m_gameplayCanvas != null) m_gameplayCanvas.enabled = false;
                    break;
                case UIScreen.GameOver:
                    if (m_gameOverCanvas != null) m_gameOverCanvas.enabled = false;
                    break;
                default:
                    Debug.LogError($"UIManager: Unknown UIScreen {screen}");
                    return;
            }

            if (m_currentCanvas == GetCanvas(screen))
            {
                m_currentCanvas = null;
            }
        }

        public void RegisterButton(UIButton buttonID, Button button)
        {
            if (!m_buttonMapping.ContainsKey(buttonID))
            {
                m_buttonMapping[buttonID] = new List<Button>();
            }

            if (!m_buttonMapping[buttonID].Contains(button))
            {
                m_buttonMapping[buttonID].Add(button);
            }
        }

        public List<Button> GetButtons(UIButton buttonID)
        {
            if (m_buttonMapping.TryGetValue(buttonID, out List<Button> buttons))
            {
                return buttons;
            }

            Debug.LogError($"No buttons registered for {buttonID}.");
            return null;
        }

        public void AssignListener(UIButton buttonID, System.Action callback)
        {
            List<Button> buttons = GetButtons(buttonID);

            if (buttons == null) 
                return;

            foreach (Button button in buttons)
            {
                button.onClick.AddListener(() => callback?.Invoke());
            }
        }

        public void UnregisterButton(UIButton buttonID)
        {
            List<Button> buttons = GetButtons(buttonID);

            if (buttons == null)
                return;

            foreach(Button button in buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        private void HideAll()
        {
            if (m_mainMenuCanvas != null) m_mainMenuCanvas.enabled = false;
            if (m_gameplayCanvas != null) m_gameplayCanvas.enabled = false;
            if (m_gameOverCanvas != null) m_gameOverCanvas.enabled = false;

            m_currentCanvas = null;
        }

        private Canvas GetCanvas(UIScreen screen)
        {
            return screen switch
            {
                UIScreen.MainMenu => m_mainMenuCanvas,
                UIScreen.Gameplay => m_gameplayCanvas,
                UIScreen.GameOver => m_gameOverCanvas,
                _ => null
            };
        }
    }
}