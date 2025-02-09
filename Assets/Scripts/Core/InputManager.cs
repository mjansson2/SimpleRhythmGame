using SimpleRhythmGame.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleRhythmGame.Core
{
    public class InputManager : MonoBehaviour
    {
        [Header("Timing Windows")]
        [SerializeField] private float  m_fPerfectWindow = 20f;
        [SerializeField] private float  m_fGoodWindow = 100f;

        private GameManager             m_gameManager;
        private float                   m_fLaneWidth;

        private void Start()
        {
            m_gameManager = GameManager.Instance;
            m_fLaneWidth = Screen.width / m_gameManager.NumberOfLanes;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(Input.mousePosition.x);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    HandleTouch(touch.position.x);
                }
            }
        }

        private void HandleTouch(float xPos)
        {
            int lane = Mathf.FloorToInt(xPos / m_fLaneWidth);        
            CheckHit(lane);
        }

        private void CheckHit(int lane)
        {
            Note closestNote = m_gameManager.GetClosestNoteInLane(lane);
            if (closestNote == null)
            {
                return;
            }

            float noteY = closestNote.GetComponent<RectTransform>().anchoredPosition.y;
            float hitLineY = m_gameManager.HitLineY;
            float distance = Mathf.Abs(noteY - hitLineY);

            if (distance <= m_fPerfectWindow)
            {
                m_gameManager.HandleNoteHit(true);
                ShowHitEffect(lane, true);
                Destroy(closestNote.gameObject);
            }
            else if (distance <= m_fGoodWindow)
            {
                m_gameManager.HandleNoteHit(false);
                ShowHitEffect(lane, false);
                Destroy(closestNote.gameObject);
            }
            else
            {
                m_gameManager.HandleNoteMiss(closestNote.Lane);
                Destroy(closestNote.gameObject);
            }
        }

        private void ShowHitEffect(int lane, bool perfect)
        {
            GameObject hitEffect = Instantiate(m_gameManager.HitEffectPrefab, m_gameManager.NoteContainer);
            RectTransform effectRect = hitEffect.GetComponent<RectTransform>();
            effectRect.anchoredPosition = new Vector2(
                m_gameManager.LaneXPositions[lane],
                m_gameManager.HitLineY
            );

            HitEffect spawnedHitEffect = hitEffect.GetComponent<HitEffect>();
            spawnedHitEffect.Initialize(perfect, false);
        }
    }
}