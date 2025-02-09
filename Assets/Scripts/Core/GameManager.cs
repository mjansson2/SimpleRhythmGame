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
        [Header("Game Settings")]
        private int                                     m_iNumberOfLanes = 4;
        private float                                   m_fNoteSpeed = 500f;

        [Header("References")]
        [SerializeField] private RectTransform          m_noteContainer;
        [SerializeField] private RectTransform          m_hitLine;
        [SerializeField] private GameObject             m_notePrefab;
        [SerializeField] private GameObject             m_hitEffectPrefab;
        [SerializeField] private TextMeshProUGUI        m_scoreText;
        [SerializeField] private TextMeshProUGUI        m_comboText;

        private float[]                                 m_fLaneXPositions;
        private int                                     m_iScore;
        private int                                     m_iCombo;
        private int                                     m_iMaxCombo = 0;
        private float                                   m_fTopOfScreen;
        private float                                   m_fBottomOfScreen;
        private float                                   m_fScreenOffset = 100f;

        private int                                     m_iPerfectHits = 0;
        private int                                     m_iGoodHits = 0;
        private int                                     m_iMisses = 0;

        public static GameManager Instance { get; private set; }

        #region Properties
        public float[] LaneXPositions => m_fLaneXPositions;
        public GameObject HitEffectPrefab => m_hitEffectPrefab;
        public RectTransform NoteContainer => m_noteContainer;
        public float HitLineY => m_hitLine.anchoredPosition.y;
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

        private void Start()
        {
            CalculateLanePositions();
            m_fTopOfScreen = GetScreenTop() + m_fScreenOffset;
            m_fBottomOfScreen = GetScreenBottom();
            UpdateUI();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnNote(Random.Range(0, m_iNumberOfLanes));
            }
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

        public void SpawnNote(int lane)
        {
            GameObject note = Instantiate(m_notePrefab, m_noteContainer);
            RectTransform noteRect = note.GetComponent<RectTransform>();
            noteRect.anchoredPosition = new Vector2(m_fLaneXPositions[lane], m_fTopOfScreen);

            // Set values to spawned note
            Note spawnedNote = note.GetComponent<Note>();
            spawnedNote.Initialize(m_fNoteSpeed, lane);
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
            UpdateUI();
        }

        public void HandleNoteMiss(int lane)
        {
            m_iMisses++;
            m_iCombo = 0;
            UpdateUI();

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
            UpdateUI();
        }

        private void UpdateUI()
        {
            m_scoreText.text = m_iScore.ToString("D6");
            m_comboText.text = m_iCombo > 1 ? $"{m_iCombo} COMBO!" : "";
        }
    }
}