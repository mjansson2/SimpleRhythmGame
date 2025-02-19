using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Events;
using SimpleRhythmGame.UI;
using SimpleRhythmGame.Core;

namespace SimpleRhythmGame.Events
{
    public class GameOverEvent : EventHandler.GameEvent
    {
        public override void OnBegin(bool bFirstTime)
        {
            Debug.Log("GameOverEvent: Started. First Time?" + bFirstTime);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(UIScreen.GameOver);

                UIManager.Instance.AssignListener(UIButton.MainMenu, () =>
                {
                    Debug.Log("Main Menu Button Clicked!");
                    EventHandler.Main.PushEvent(new MainMenuEvent());
                });

                UIManager.Instance.AssignListener(UIButton.GameOverQuit, () =>
                {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                });
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("GameOverEvent: Restarting game... Returning to Main Menu.");
                EventHandler.Main.PushEvent(new MainMenuEvent());
            }
        }

        public override void OnEnd()
        {
            Debug.Log("GameOverEvent: Ended.");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Hide(UIScreen.GameOver);

                UIManager.Instance.UnregisterButton(UIButton.MainMenu);
                UIManager.Instance.UnregisterButton(UIButton.GameOverQuit);
            }
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}