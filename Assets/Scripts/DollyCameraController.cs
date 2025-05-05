using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class DollyCameraController : MonoBehaviour
{

    [SerializeField] CinemachineSplineDolly dolly;
    [SerializeField] float dollyMoveTime = 1;
     
    Tweener currentTweener = null;

    public void SetMoveTime(float time)
    {
        dollyMoveTime = time;
    }

    public void MoveTo(float position)
    {
        if (currentTweener != null)
        {
            currentTweener.Kill();
        }

        currentTweener = DOVirtual.Float(dolly.CameraPosition, position, dollyMoveTime, (value) => dolly.CameraPosition = value);
    }
}
