using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleRhythmGame.Game
{
    public class HitEffect : MonoBehaviour
    {
        [SerializeField] private float              m_effectDuration = 0.5f;
        [SerializeField] private AnimationCurve     m_scaleCurve;
        [SerializeField] private Color              m_perfectHitColor = new Color(0.06959774f, 0.5754527f, 0.6415094f, 0.5f);
        [SerializeField] private Color              m_goodHitColor = new Color(0.01668743f, 0.7075472f, 0.08922558f, 0.5f);
        [SerializeField] private Color              m_missHitColor = new Color(0.8f, 0.1f, 0.1f, 0.5f);

        private Image                               m_hitEffectImage;
        private float                               m_elapsedTime = 0f;

        void Start()
        {
            m_hitEffectImage = GetComponent<Image>();
            transform.localScale = Vector3.zero;
        }

        public void Initialize(bool isPerfect, bool isMissed = false)
        {
            if (m_hitEffectImage == null)
            {
                m_hitEffectImage=GetComponent<Image>();
            }

            if (isMissed)
            {
                m_hitEffectImage.color = m_missHitColor;
            }
            else
            {
                m_hitEffectImage.color = isPerfect ? m_perfectHitColor : m_goodHitColor;
            }
        }

        void Update()
        {
            m_elapsedTime += Time.deltaTime;
            float normalizedTime = m_elapsedTime / m_effectDuration;
            float scale = m_scaleCurve.Evaluate(normalizedTime);
            transform.localScale = Vector3.one * scale;

            Color currentColor = m_hitEffectImage.color;
            currentColor.a = 1f - normalizedTime;
            m_hitEffectImage.color = currentColor;

            if (normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }
    }
}