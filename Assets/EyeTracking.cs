using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EyeTracking : MonoBehaviour
{
    [SerializeField] float eyeTrackRange = 4;
    [SerializeField] float eyeTrackingSpeed = 0.25f;
    [Space]
    [SerializeField] MultiAimConstraint multiAimConstraintL;
    [SerializeField] MultiAimConstraint multiAimConstraintR;
    [SerializeField] RigBuilder rigBuilder;

    bool playerInRange = false;
    Tweener weightTweener = null;

    private void Start()
    {
        var data = multiAimConstraintL.data.sourceObjects;
        data.SetTransform(0, Camera.main.transform);

        multiAimConstraintR.data.sourceObjects = data;
        multiAimConstraintL.data.sourceObjects = data;
        rigBuilder.Build();
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (distance < eyeTrackRange && !playerInRange)
        {
            playerInRange = true;
            TrackCamera(true);

        }
        else if (distance > eyeTrackRange && playerInRange)
        {
            playerInRange = false;
            TrackCamera(false);
        }
    }

    public void TrackCamera(bool trackCam)
    {
        float targetValue = trackCam ? 1 : 0;

        if (weightTweener != null)
        {
            if (weightTweener.active)
            {
                weightTweener.Kill();
            }
        }

        weightTweener = DOVirtual.Float(multiAimConstraintL.weight, targetValue, eyeTrackingSpeed, value => { multiAimConstraintL.weight = value; multiAimConstraintR.weight = value; });
    }
}
