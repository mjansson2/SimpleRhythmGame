using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRhythmGame.Core
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource                     m_audioSource;
        private AudioClip                       m_songTrack;
        private float[]                         m_fBeatTimestamps;
        private int                             m_iCurrentBeatIndex = 0;
        private float                           m_fBPM = 120f;
        private float                           m_fSongDuration;
        private float                           m_fEndFadeOutDuration = 25f;

        public static AudioManager Instance {  get; private set; }

        #region Properties
        public AudioClip SongTrack => m_songTrack;
        public AudioSource AudioSource => m_audioSource;
        public float[] BeatTimestamps => m_fBeatTimestamps;
        public int CurrentBeatIndex => m_iCurrentBeatIndex;
        public float BPM => m_fBPM; 
        #endregion

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            InitializeAudioSource();
        }

        private void InitializeAudioSource()
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
            m_audioSource.playOnAwake = false;
            m_audioSource.loop = false;
            m_audioSource.volume = 1.0f;
        }

        public void Initialize(AudioClip songTrack, float bpm, float endFadeOutDuration)
        {
            m_songTrack = songTrack;
            m_fBPM = bpm;
            m_fEndFadeOutDuration = endFadeOutDuration;
            m_audioSource.clip = m_songTrack;

            GenerateBeatTimestamps();
            m_fSongDuration = m_songTrack.length;
        }

      /*  private void Update()
        {
            if (!m_audioSource.isPlaying)
            {
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
                    m_audioSource.Stop();
                    m_audioSource.time = 0f;
                }
            }

        } */

       /* bool SpawnNoteWithFadeOut()
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
        } */

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
            m_audioSource.Play();
        }

        public bool IsSongPlaying()
        {
            return m_audioSource.isPlaying;
        }

        public float GetCurrentTime()
        {
            return m_audioSource.time;
        }
    }
}