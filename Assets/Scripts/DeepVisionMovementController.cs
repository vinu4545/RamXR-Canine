using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeepVisionMovementController : MonoBehaviour
{
    public DeepVisionMovementSequence sequence;
    public IntEventChannelSO stepChangedEvent;
    public DeepVisionSceneBinder binder;

    private Dictionary<string, TransformData> initialStates = new();
    private Coroutine currentRoutine;

    void Awake()
    {
        CacheInitialStates();
    }

    void OnEnable()
    {
        stepChangedEvent.OnEventRaised += OnStepChanged;
    }

    void OnDisable()
    {
        stepChangedEvent.OnEventRaised -= OnStepChanged;
    }

    void CacheInitialStates()
    {
        foreach (var step in sequence.steps)
        {
            foreach (var t in step.targets)
            {
                if (!initialStates.ContainsKey(t.targetId))
                {
                    Transform obj = binder.Get(t.targetId);

                    if (obj != null)
                    {
                        initialStates[t.targetId] = new TransformData(obj);
                    }
                    else
                    {
                        Debug.LogWarning($"Binder missing target for ID: {t.targetId}");
                    }
                }
            }
        }
    }

    void OnStepChanged(int index)
    {
        if (index < 0 || index >= sequence.steps.Length)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateStep(index));
    }

    IEnumerator AnimateStep(int stepIndex)
    {
        var step = sequence.steps[stepIndex];

        float time = 0f;

        Dictionary<string, TransformData> startStates = new();

        // Capture current state
        foreach (var t in step.targets)
        {
            Transform obj = binder.Get(t.targetId);

            if (obj == null)
                continue;

            startStates[t.targetId] = new TransformData(obj);
        }

        while (time < step.duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, time / step.duration);

            foreach (var target in step.targets)
            {
                Transform obj = binder.Get(target.targetId);
                if (obj == null) continue;

                var start = startStates[target.targetId];

                obj.localPosition = Vector3.Lerp(start.position, target.position, t);
                obj.localRotation = Quaternion.Lerp(start.rotation, Quaternion.Euler(target.rotation), t);
                obj.localScale = Vector3.Lerp(start.scale, target.scale, t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Snap final state
        foreach (var target in step.targets)
        {
            Transform obj = binder.Get(target.targetId);
            if (obj == null) continue;

            obj.localPosition = target.position;
            obj.localRotation = Quaternion.Euler(target.rotation);
            obj.localScale = target.scale;
        }
    }

    public void ResetAll()
    {
        foreach (var kvp in initialStates)
        {
            Transform obj = binder.Get(kvp.Key);
            if (obj == null) continue;

            obj.localPosition = kvp.Value.position;
            obj.localRotation = kvp.Value.rotation;
            obj.localScale = kvp.Value.scale;
        }
    }

    class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Transform t)
        {
            position = t.localPosition;
            rotation = t.localRotation;
            scale = t.localScale;
        }
    }
}