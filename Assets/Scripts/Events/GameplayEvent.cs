using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameTools.Events;
using SimpleRhythmGame.UI;
using UnityEditor;
using SimpleRhythmGame.Core;

namespace SimpleRhythmGame.Events
{
    public class GameplayEvent : EventHandler.GameEvent
    {
        public override void OnBegin(bool bFirstTime)
        {
            Debug.Log("GameplayEvent: Started. First Time?" + bFirstTime);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Show(UIScreen.Gameplay);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("GameplayEvent: Game Over! Transitioning to GameOverEvent...");
                EventHandler.Main.PushEvent(new GameOverEvent());
            }
        }

        public override void OnEnd()
        {
            Debug.Log("GameplayEvent: Ended.");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.Hide(UIScreen.Gameplay);
            }
        }

        public override bool IsDone()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("GameplayEvent: Finished! Ready to transition");
                return true;
            }

            return false;
        }
    }
}