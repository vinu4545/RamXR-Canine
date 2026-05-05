using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeparateViewMovementController : MonoBehaviour
{
    public SeparateViewDataSO data;
    public VoidEventChannelSO activateEvent;
    public VoidEventChannelSO deactivateEvent;
    public SeparateViewSceneBinder binder;

    private Coroutine currentRoutine;

    void OnEnable()
    {
        activateEvent.OnEventRaised += ActivateView;
        deactivateEvent.OnEventRaised += DeactivateView;
    }

    void OnDisable()
    {
        activateEvent.OnEventRaised -= ActivateView;
        deactivateEvent.OnEventRaised -= DeactivateView;
    }

    void ActivateView()
    {
        StartTransition(true);
    }

    void DeactivateView()
    {
        StartTransition(false);
    }

    void StartTransition(bool toActive)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Animate(toActive));
    }

    IEnumerator Animate(bool toActive)
    {
        float time = 0f;
        float duration = data.transitionDuration;

        Dictionary<Transform, TransformSnapshot> startStates = new();

        // Capture initial state
        foreach (var element in data.elements)
        {
            var t = binder.GetTransform(element.targetId);
            if (t == null) continue;

            startStates[t] = new TransformSnapshot
            {
                position = t.localPosition,
                rotation = t.localRotation,
                scale = t.localScale
            };
        }

        while (time < duration)
        {
            float tNorm = time / duration;

            foreach (var element in data.elements)
            {
                var t = binder.GetTransform(element.targetId);
                if (t == null) continue;

                var start = startStates[t];

                Vector3 targetPos = toActive ? element.activePosition : element.inactivePosition;
                Vector3 targetRot = toActive ? element.activeRotation : element.inactiveRotation;
                Vector3 targetScale = toActive ? element.activeScale : element.inactiveScale;

                t.localPosition = Vector3.Lerp(start.position, targetPos, tNorm);
                t.localRotation = Quaternion.Lerp(start.rotation, Quaternion.Euler(targetRot), tNorm);
                t.localScale = Vector3.Lerp(start.scale, targetScale, tNorm);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Snap to final
        foreach (var element in data.elements)
        {
            var t = binder.GetTransform(element.targetId);
            if (t == null) continue;

            t.localPosition = toActive ? element.activePosition : element.inactivePosition;
            t.localRotation = Quaternion.Euler(toActive ? element.activeRotation : element.inactiveRotation);
            t.localScale = toActive ? element.activeScale : element.inactiveScale;
        }
    }

    struct TransformSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }
}