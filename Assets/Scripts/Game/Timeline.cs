using SimpleRhythmGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleRhythmGame.Game
{
    public class Timeline : MonoBehaviour
    {
        [SerializeField] private Image m_timelineImage;
        [SerializeField] private AudioManager m_audioManager;

        private float m_songDuration;

        private void Start()
        {
            m_audioManager = AudioManager.Instance;
            m_timelineImage.type = Image.Type.Filled;
            m_timelineImage.fillMethod = Image.FillMethod.Horizontal;

            m_songDuration = m_audioManager.SongTrack.length;
        }

        private void Update()
        {
            float currentTime = m_audioManager.AudioSource.time;
            float progressPercentage = currentTime / m_songDuration;
            m_timelineImage.fillAmount = progressPercentage;
        }
    }
}