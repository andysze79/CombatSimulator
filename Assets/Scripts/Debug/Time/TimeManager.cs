using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public bool m_UseTimeControl = true;
    public float m_SpeedControlMax = 3;
    public enum TimeEventType {
        SkipFrame, SlowMotion
    }
    public enum TimerMode { 
        Duration, OnOff
    }
    [System.Serializable]
    public struct TimeEvent {
        public string Name;
        public TimeEventType EventType;
        [ShowIf("EventType", TimeEventType.SkipFrame)]
        public int Frames;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public float SlowDownSpeed;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public float TransitionTime;
        [ShowIf("EventType", TimeEventType.SlowMotion)]
        public TimerMode TimerMode;
        [ShowIf("TimerMode", TimerMode.OnOff)]
        public bool OnOff;
        [ShowIf("TimerMode", TimerMode.Duration)]
        public float Duration;
    }
    public List<TimeEvent> m_TimeEvents = new List<TimeEvent>();
    List<ITimeScaleChange> TimeScaleChangeObservers = new List<ITimeScaleChange>();
    List<ITimelinePlaying> ITimelinePlayingObservers = new List<ITimelinePlaying>();
    public static TimeManager m_Instance;
    public static TimeManager Instance {
        get {
            if (m_Instance == null)
                return GameObject.FindObjectOfType<TimeManager>();
            else
                return m_Instance;
        }
    }
    public delegate void TimeChangeEvent();
    public TimeChangeEvent WhenSlowMoStart;
    public TimeChangeEvent WhenSlowMoEnd;
    Coroutine Process { get; set; }

    public void StartTimeEvent(string name) {

        if (!m_UseTimeControl) return;

        var check = false;
        for (int i = 0; i < m_TimeEvents.Count; i++)
        {
            if (m_TimeEvents[i].Name == name) {
                CheckEventType(i);
                check = true;
            }
        }

        if (!check) {
            Debug.Log("Couldn't found the " + name + " time event");
        }
    }
    public void ResetTimeScale() {
        StopAllCoroutines();
        Time.timeScale = 1;
        WhenSlowMoEnd?.Invoke();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (Time.timeScale < m_SpeedControlMax)
            {
                Time.timeScale += 0.2f;
                NotifyObservers();
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (Time.timeScale > 0.1f)
            {
                Time.timeScale -= 0.2f; 
            }
            else
            {
                Time.timeScale = 0f;
            }
            NotifyObservers();
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Time.timeScale = 0f;
            NotifyObservers();
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Time.timeScale = 1f;
            NotifyObservers();
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Time.timeScale = 2f;
            NotifyObservers();
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Time.timeScale = 3f;
            NotifyObservers();
        }
    }
    private void CheckEventType(int index) {
        switch (m_TimeEvents[index].EventType)
        {
            case TimeEventType.SkipFrame:
                if (Process == null) {
                    Process = StartCoroutine(SkipFrames(m_TimeEvents[index].Frames));
                }
                else
                {
                    StopCoroutine(Process);
                    Process = StartCoroutine(SkipFrames(m_TimeEvents[index].Frames));
                }
                break;
            case TimeEventType.SlowMotion:
                if (Process == null)
                {
                    switch (m_TimeEvents[index].TimerMode)
                    {
                        case TimerMode.Duration:
                            Process = StartCoroutine(SlowMotion(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].Duration));
                            break;
                        case TimerMode.OnOff:
                            Process = StartCoroutine(SlowMotionOnOff(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].OnOff));
                            break;
                        default:
                            break;
                    }                    
                }
                else
                {
                    switch (m_TimeEvents[index].TimerMode)
                    {
                        case TimerMode.Duration:
                            StopCoroutine(Process);
                            Process = StartCoroutine(SlowMotion(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].Duration));
                            break;
                        case TimerMode.OnOff:
                            StopCoroutine(Process);
                            Process = StartCoroutine(SlowMotionOnOff(m_TimeEvents[index].SlowDownSpeed, m_TimeEvents[index].TransitionTime, m_TimeEvents[index].OnOff));
                            break;
                        default:
                            break;
                    }                    
                }
                break;
            default:
                break;
        }
    }
    public IEnumerator SkipFrames(int frames) {
        int counter = 0;

        Time.timeScale = 0;

        while (counter < frames) {
            ++counter; 
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1;
    }
    public IEnumerator SlowMotion(float Speed, float transitionTime, float Duration) {
        var startTime = Time.realtimeSinceStartup;
        var endTime = transitionTime;

        WhenSlowMoStart?.Invoke();

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(1, Speed, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = Speed;
        
        yield return new WaitForSecondsRealtime(Duration);

        startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(Speed, 1, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = 1;

        WhenSlowMoEnd?.Invoke();
    }
    public IEnumerator SlowMotionOnOff(float Speed, float transitionTime, bool onOff)
    {
        var startTime = Time.realtimeSinceStartup;
        var endTime = transitionTime;
        var from = Time.timeScale;
        var to = onOff ? Speed : 1;

        while (Time.realtimeSinceStartup - startTime < endTime)
        {
            Time.timeScale = Mathf.Lerp(from, to, (Time.unscaledTime - startTime) / endTime);
            yield return null;
        }

        Time.timeScale = to;        
    }
    #region TimeScaleObserver
    public void AddObserver(ITimeScaleChange target)
    {
        TimeScaleChangeObservers.Add(target);
    }
    public void RemoveObserver(ITimeScaleChange target)
    {
        TimeScaleChangeObservers.Remove(target);
    }
    private void NotifyObservers() {
        foreach (var item in TimeScaleChangeObservers)
        {
            item.NotifyTimeScaleChange();
        }
    }
    #endregion

    #region TimelinePlayingObserver
    public void AddObserver(ITimelinePlaying target)
    {
        ITimelinePlayingObservers.Add(target);
    }
    public void RemoveObserver(ITimelinePlaying target)
    {
        ITimelinePlayingObservers.Remove(target);
    }
    public void NotifyTimelineObservers(bool value)
    {
        foreach (var item in ITimelinePlayingObservers)
        {
            item.NotifyTimelinePlaying(value);
        }
    }
    #endregion
}
