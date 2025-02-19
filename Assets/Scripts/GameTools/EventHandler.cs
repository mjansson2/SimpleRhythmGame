using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace GameTools.Events
{
    public class EventHandler : MonoBehaviour
    {
        public interface IEvent
        {
            void OnBegin(bool bFirstTime);
            void OnUpdate();
            void OnEnd();
            bool IsDone();
        }

        public abstract class GameEvent : IEvent
        {
            public virtual void OnBegin(bool bFirstTime) { }
            public virtual void OnUpdate() { }
            public virtual void OnEnd() { }
            public virtual bool IsDone()
            {
                return true;
            }
        }

        public abstract class GameEventBehaviour : MonoBehaviour, IEvent
        {
            public virtual void OnBegin(bool bFirstTime) { }
            public virtual void OnUpdate() { }
            public virtual void OnEnd() { }
            public virtual bool IsDone()
            {
                return true;
            }
        }

        private List<IEvent>            m_eventStack = new List<IEvent>();
        private HashSet<IEvent>         m_startedEvents = new HashSet<IEvent>();
        private IEvent                  m_currentEvent;

        private static EventHandler     sm_main = null;

        #region Properties

        public IEvent CurrentEvent => m_currentEvent;
        public List<IEvent> EventStack => m_eventStack;

        public static EventHandler Main
        {
            get
            {
                if (sm_main == null && Application.isPlaying)
                {
                    GameObject gameObject = new GameObject("MainEventHandler");
                    DontDestroyOnLoad(gameObject);
                    sm_main = gameObject.AddComponent<EventHandler>();
                }

                return sm_main;
            }
        }

        #endregion

        public void PushEvent(IEvent evt) 
        {
            if (evt != null)
            {
                m_eventStack.RemoveAll(e => e == evt); 
                m_eventStack.Insert(0, evt);
                if (m_currentEvent != null && m_currentEvent != evt)
                {
                    m_currentEvent = null;
                }
            }
        }

        public void RemoveEvent(IEvent evt)
        {
            if (evt == null || !m_eventStack.Contains(evt))
            {
                return;
            }

            if (evt == m_currentEvent || m_startedEvents.Contains(evt))
            {
                evt.OnEnd();
                m_currentEvent = null;
            }

            m_eventStack.Remove(evt);
        }

        protected virtual void Update()
        {
            UpdateEvents();
        }

        protected virtual void UpdateEvents()
        {
            if (m_eventStack.Count == 0)
            {
                return;
            }

            if (m_currentEvent == null)
            {
                m_startedEvents.RemoveWhere(evt => evt == null);
                m_currentEvent = m_eventStack[0];
                bool bFirstTime = !m_startedEvents.Contains(m_currentEvent);
                m_startedEvents.Add(m_currentEvent);
                m_currentEvent.OnBegin(bFirstTime);

                if (m_eventStack != null)
                {
                    if (m_eventStack.Count > 0 && m_currentEvent != m_eventStack[0])
                    {
                        m_currentEvent = null;
                        UpdateEvents();
                    }
                }
            }

            if (m_currentEvent != null)
            {
                m_currentEvent.OnUpdate();
                
                if (m_eventStack.Count > 0 && m_currentEvent == m_eventStack[0])
                {
                    if (m_currentEvent.IsDone())
                    {
                        m_eventStack.RemoveAt(0);
                        m_currentEvent.OnEnd();
                        m_startedEvents.Remove(m_currentEvent);
                        m_currentEvent = null;
                    }
                }
            }
        }

        protected virtual void OnGUI()
        {
            if (this != sm_main)
            {
                return;
            }

            #if UNITY_EDITOR
            const float LINE_HEIGHT = 32.0f;

            GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.7f); // Dark grey
            Rect r = new Rect(0, 0, 250.0f, LINE_HEIGHT * m_eventStack.Count);
            GUI.DrawTexture(r, Texture2D.whiteTexture);

            Rect line = new Rect(10, 0, r.width - 20, LINE_HEIGHT);
            for (int i = 0; i < m_eventStack.Count; ++i)
            {
                GUI.color = m_eventStack[i] == m_currentEvent ? Color.green : Color.white;
                GUI.Label(line, "#" + i + ": " + m_eventStack[i].ToString(), i == 0 ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label);
                line.y += line.height;
            }

            #endif
        }
    }
}