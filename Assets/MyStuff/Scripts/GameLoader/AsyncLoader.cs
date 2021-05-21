using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AsyncLoader : MonoBehaviour
{
    private class RoutineInfo
    {
        public RoutineInfo(IEnumerator routine, int weight, Func<float> progress)
        {
            this.routine = routine;
            this.weight = weight;
            this.progress = progress;
        }

        public readonly IEnumerator routine;
        public readonly int weight;
        public readonly Func<float> progress;
    }

    protected virtual void ProgressUpdated(float percentComplete) { }
    protected virtual void InitError(int reasonCode, string reasonDebug) { }

    private Queue<RoutineInfo> _pending = new Queue<RoutineInfo>();
    private bool _completedWithoutError = true;
    private static event Action _loadingCompleted;

    public static bool Complete { get; private set; } = false;
    public static float Progress { get; private set; } = 0.0f;

    protected void Enqueue(IEnumerator routine, int weight, Func<float> progress = null)
    {
        _pending.Enqueue(new RoutineInfo(routine, weight, progress));
    }

    protected abstract void Awake();

    private IEnumerator Start()
    {
        if (Complete)
        {
            Progress = 1.0f;
            _pending.Clear();
            yield break;
        }

        float percentCompleteByFullSections = 0.0f;
        int outOf = 0;

        var running = new Queue<RoutineInfo>(_pending);
        _pending.Clear();

        foreach (var routineInfo in running)
        {
            outOf += routineInfo.weight;
        }

        while (running.Count != 0)
        {
            var routineInfo = running.Dequeue();
            var routine = routineInfo.routine;

            while (routine.MoveNext())
            {
                if (routineInfo.progress != null)
                {
                    var routinePercent = routineInfo.progress() * (float)routineInfo.weight / (float)outOf;
                    Progress = percentCompleteByFullSections + routinePercent;
                    ProgressUpdated(Progress);
                }

                yield return routine.Current;
            }

            percentCompleteByFullSections += (float)routineInfo.weight / (float)outOf;
            Progress = percentCompleteByFullSections;
            ProgressUpdated(Progress);
        }

        if (!_completedWithoutError)
            Debug.Log("A fatal error occurred while running initialization. Please check your logs and fix the error.");

        Complete = true;
        _loadingCompleted?.Invoke();
    }

    public static void CallOnComplete(Action callback)
    {
        if (Complete)
        {
            callback();
        }
        else
        {
            _loadingCompleted += callback;
        }
    }
}
