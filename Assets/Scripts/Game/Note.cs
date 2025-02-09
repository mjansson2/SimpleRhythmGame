using SimpleRhythmGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRhythmGame.Game
{
    public class Note : MonoBehaviour
    {
        private RectTransform   m_rectTransform;
        private float           m_fSpeed;
        private int             m_iLane;
        private GameManager     m_gameManager;
        private float           m_fBottomOfScreen;
        private float           m_fScreenOffset;

        #region Properties
        public int Lane => m_iLane;
        #endregion

        public void Initialize(float noteSpeed, int noteLane)
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_fSpeed = noteSpeed;
            m_iLane = noteLane;
        }

        private void Start()
        {
            m_gameManager = GameManager.Instance;
            m_fBottomOfScreen = m_gameManager.BottomOfScreen;
            m_fScreenOffset = m_gameManager.ScreenOffset;
        }

        private void Update()
        {
            Vector2 pos = m_rectTransform.anchoredPosition;
            pos.y -= m_fSpeed * Time.deltaTime;
            m_rectTransform.anchoredPosition = pos;

            if (pos.y < m_fBottomOfScreen + m_fScreenOffset)
            {
                GameManager.Instance.HandleNoteMiss(m_iLane);
                Destroy(gameObject);
            }
        }
    }
}