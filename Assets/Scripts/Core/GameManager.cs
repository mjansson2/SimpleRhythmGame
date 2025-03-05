using SimpleRhythmGame.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace SimpleRhythmGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public delegate void ScoreUpdateHandler(int score, int combo);
        public event ScoreUpdateHandler OnScoreUpdated;

        private RectTransform                   m_noteContainer;
        private RectTransform                   m_hitLine;
        private GameObject                      m_notePrefab;
        private GameObject                      m_hitEffectPrefab;
        private int                             m_iCurrentBeatIndex = 0;

        private int                             m_iNumberOfLanes;
        private float                           m_fNoteSpeed;

        private float[]                         m_fLaneXPositions;
        private int                             m_iScore;
        private int                             m_iCombo;
        private int                             m_iMaxCombo = 0;
        private float                           m_fTopOfScreen;
        private float                           m_fBottomOfScreen;
        private float                           m_fScreenOffset = 100f;

        private int                             m_iPerfectHits = 0;
        private int                             m_iGoodHits = 0;
        private int                             m_iMisses = 0;

        public static GameManager Instance { get; private set; }

        #region Properties
        public float[] LaneXPositions => m_fLaneXPositions;
        public GameObject HitEffectPrefab => m_hitEffectPrefab;
        public RectTransform NoteContainer => m_noteContainer;
        public float HitLineY { get; private set; }
        public float NoteSpeed => m_fNoteSpeed;
        public int NumberOfLanes => m_iNumberOfLanes;
        public float BottomOfScreen => m_fBottomOfScreen;
        public float ScreenOffset => m_fScreenOffset;   
        public int Score => m_iScore;  
        public int MaxCombo => m_iMaxCombo; 

        public int PerfectHits => m_iPerfectHits;
        public int GoodHits => m_iGoodHits;
        public int Misses => m_iMisses; 
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

        public void Initialize(int numberOfLanes, float noteSpeed, RectTransform hitLine, RectTransform noteContainer, GameObject notePrefab, GameObject hitEffectPrefab)
        {
            m_iNumberOfLanes = numberOfLanes;
            m_fNoteSpeed = noteSpeed;
            m_hitLine = hitLine;
            m_noteContainer = noteContainer;
            m_notePrefab = notePrefab;
            m_hitEffectPrefab = hitEffectPrefab;
            CalculateLanePositions();
            m_fTopOfScreen = GetScreenTop() + m_fScreenOffset;
            m_fBottomOfScreen = GetScreenBottom();

            Canvas parentCanvas = m_hitLine.GetComponentInParent<Canvas>();
            float screenHeight = Screen.height;
            float targetDistance = screenHeight * 0.2f;
            if (parentCanvas && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                float hitLinePosition = -screenHeight / 2 + targetDistance;
                HitLineY = hitLinePosition;

                Vector2 newPosition = m_hitLine.anchoredPosition;
                newPosition.y = hitLinePosition;
                m_hitLine.anchoredPosition = newPosition;
            }

            else
            {
                Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, m_hitLine.position);
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    m_noteContainer, screenPosition, parentCanvas.worldCamera, out localPosition 
                    );

                Vector2 newPosition = m_hitLine.anchoredPosition;
                newPosition.y = localPosition.y;
                m_hitLine.anchoredPosition = newPosition;
            }
        }

        public void StartGame()
        {
            Debug.Log("GameManager: Starting game...");

            ResetGame();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySong();
                StartCoroutine(SpawnNotes());
            }
        }

        private IEnumerator SpawnNotes()
        {
            float beatInterval = 60f / AudioManager.Instance.BPM;
            float noteTravelTime = HitLineY / m_fNoteSpeed;

            foreach (float beatTime in AudioManager.Instance.BeatTimestamps)
            {
                float spawnTime = beatTime - noteTravelTime;

                while (AudioManager.Instance.AudioSource.time < spawnTime)
                {
                    yield return null;
                }

                int lane = Random.Range(0, m_iNumberOfLanes);
                SpawnNote(lane);
            }
        }

        public void SpawnNote(int lane)
        {
            GameObject note = Instantiate(m_notePrefab, m_noteContainer);
            RectTransform noteRect = note.GetComponent<RectTransform>();
            noteRect.anchoredPosition = new Vector2(m_fLaneXPositions[lane], m_fTopOfScreen);

            // Set values to spawned note
            Note spawnedNote = note.GetComponent<Note>();
            spawnedNote.Initialize(m_fNoteSpeed, lane);
        }

        private void CalculateLanePositions()
        {
            float totalPadding = Screen.width * 0.1f;
            float paddingPerSide = totalPadding * 0.5f;
            float availableWidth = Screen.width - totalPadding;

            m_fLaneXPositions = new float[m_iNumberOfLanes];
            float laneWidth = availableWidth / m_iNumberOfLanes;
            float halfScreenWidth = Screen.width * 0.5f;

            for (int i = 0; i < m_iNumberOfLanes; ++i)
            {
                m_fLaneXPositions[i] = (-halfScreenWidth + paddingPerSide + (laneWidth * i) + (laneWidth * 0.5f));
            }
        }

        private float GetScreenTop()
        {
            return Screen.height * 0.5f;
        }

        private float GetScreenBottom()
        {
            return -Screen.height * 0.5f;
        }

        public Note GetClosestNoteInLane(int lane)
        {
            Note closestNote = null;
            float closestDistance = float.MaxValue;
            float hitLineY = m_hitLine.anchoredPosition.y;

            foreach (RectTransform noteRect in m_noteContainer)
            {
                Note note = noteRect.GetComponent<Note>();
                if (note == null || note.Lane != lane)
                {
                    continue;
                }

                float noteDistance = Mathf.Abs(noteRect.anchoredPosition.y - hitLineY);
                if (noteDistance < closestDistance)
                {
                    closestDistance = noteDistance;
                    closestNote = note;
                }
            }

            return closestNote;
            
        }

        public void SpawnMissEffect(int lane, Vector2 position)
        {
            GameObject missEffect = Instantiate(m_hitEffectPrefab, m_noteContainer);
            RectTransform effectRect = missEffect.GetComponent<RectTransform>();
            effectRect.anchoredPosition = position;

            HitEffect spawnedMissEffect = missEffect.GetComponent<HitEffect>();
            spawnedMissEffect.Initialize(false, true);
        }

        public void HandleNoteHit(bool perfect)
        {
            m_iMaxCombo = Mathf.Max(m_iMaxCombo, m_iCombo + 1);

            if (perfect)
            {
                m_iPerfectHits++;
                m_iScore += 100;
            }
            else
            {
                m_iGoodHits++;
                m_iScore += 50;
            }
            m_iCombo++;
            OnScoreUpdated?.Invoke(m_iScore, m_iCombo);
        }

        public void HandleNoteMiss(int lane)
        {
            m_iMisses++;
            m_iCombo = 0;

            Note closestNote = GetClosestNoteInLane(lane);
            if (closestNote != null)
            {
                RectTransform noteRect = closestNote.GetComponent<RectTransform>();
                SpawnMissEffect(lane, noteRect.anchoredPosition);
            }
        }

        public void ResetGame()
        {
            m_iScore = 0;
            m_iCombo = 0;
            m_iMaxCombo = 0;
            m_iPerfectHits = 0;
            m_iGoodHits = 0;
            m_iMisses = 0;

            foreach (Transform child in m_noteContainer)
            {
                Destroy(child.gameObject);
            }

            OnScoreUpdated?.Invoke(m_iScore, m_iCombo);
        }
    }
}