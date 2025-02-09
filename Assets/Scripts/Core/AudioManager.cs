using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRhythmGame.Core
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioClip      m_songTrack;
        [SerializeField] private AudioSource    m_audioSource;

        private float[]                         m_fBeatTimestamps;
        private int                             m_iCurrentBeatIndex = 0;
        private float                           m_fBPM = 120f;
        private GameManager                     m_gameManager;
        private GameStateManager                m_gameStateManager;

        private float                           m_fSongDuration;
        private float                           m_fEndFadeOutDuration = 25f;

        public static AudioManager Instance {  get; private set; }

        #region Properties
        public AudioClip SongTrack => m_songTrack;
        public AudioSource AudioSource => m_audioSource;
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_gameManager = GameManager.Instance;
            m_gameStateManager = GameStateManager.Instance;
            GenerateBeatTimestamps();
            m_fSongDuration = m_songTrack.length;
            Debug.Log($"Song duration set to: {m_fSongDuration}");
        }

        private void Update()
        {
            if (m_gameStateManager.CurrentState != GameState.Playing)
            {
                if (m_audioSource.isPlaying)
                {
                    m_audioSource.Stop();
                }

                m_iCurrentBeatIndex++;
                return;
            }
            
            if (m_iCurrentBeatIndex < m_fBeatTimestamps.Length)
            {
                float noteSpeed = m_gameManager.NoteSpeed;
                float hitLineY = m_gameManager.HitLineY;
                float noteTravelTime = hitLineY / noteSpeed;
                float spawnTime = m_fBeatTimestamps[m_iCurrentBeatIndex] - noteTravelTime;

                if (m_audioSource.time >= spawnTime)
                {
                    if (SpawnNoteWithFadeOut())
                    {
                        int lane = Random.Range(0, m_gameManager.NumberOfLanes);
                        m_gameManager.SpawnNote(lane);
                    }

                    m_iCurrentBeatIndex++;
                }
            }

            else
            {
                if (m_audioSource.time >= m_fSongDuration)
                {
                    Debug.Log($"Song ended. Time: {m_audioSource.time}, Duration: {m_fSongDuration}");
                    m_audioSource.Stop();
                    m_audioSource.time = 0f;
                    m_gameStateManager.TransitionToState(GameState.GameOver);
                }
            }

        }

        bool SpawnNoteWithFadeOut()
        {
            float remainingTime = m_fSongDuration - m_audioSource.time;

            if (remainingTime <= 5f)
            {
                return false;
            }

            if (remainingTime > m_fEndFadeOutDuration)
            {
                return true;
            }

            float fadeProgress = 1f - (remainingTime / m_fEndFadeOutDuration);
            return Random.value > fadeProgress;
        }

        private void GenerateBeatTimestamps()
        {
            float beatInterval = 60f / m_fBPM;
            float songDuration = m_songTrack.length;
            int totalBeats = Mathf.FloorToInt(songDuration / beatInterval);

            m_fBeatTimestamps = new float[totalBeats];
            for (int i = 0; i < totalBeats; ++i)
            {
                m_fBeatTimestamps[i] = i * beatInterval;
            }
        }

        public void PlaySong()
        {
            m_iCurrentBeatIndex = 0;
            m_audioSource.clip = m_songTrack;
            m_audioSource.time = 0f;

            if (m_gameStateManager.CurrentState == GameState.Playing)
            {
                m_audioSource.Play();
            }
        }
    }
}