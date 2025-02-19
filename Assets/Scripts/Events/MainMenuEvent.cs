using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Events;
using SimpleRhythmGame.Core;
using SimpleRhythmGame.UI;
using UnityEditor;

namespace SimpleRhythmGame.Events
{
    public class MainMenuEvent : EventHandler.GameEvent
    {
        public override void OnBegin(bool bFirstTime)
        {
            Debug.Log("MainMenuEvent: Started. First Time? " + bFirstTime);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(UIScreen.MainMenu);

                UIManager.Instance.AssignListener(UIButton.PlayGame, () =>
                {
                    Debug.Log("Play Game Button Clicked!");
                    EventHandler.Main.PushEvent(new GameplayEvent());
                });

                UIManager.Instance.AssignListener(UIButton.MainMenuQuit, () =>
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
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("MainMenuEvent: Transitioning to GameplayEvent...");
                EventHandler.Main.PushEvent(new GameplayEvent());
            }
        }

        public override void OnEnd()
        {
            Debug.Log("MainMenuEvent: Ended.");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Hide(UIScreen.MainMenu);

                UIManager.Instance.UnregisterButton(UIButton.PlayGame);
                UIManager.Instance.UnregisterButton(UIButton.MainMenuQuit);
            }
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}