using DataStructure;
using UnityEngine;

public class QuaternionSlerpTest : MonoBehaviour {
    public Transform target;
    public LerpType slerpType;
    public float lerpSpeed = 1;

    public void Update() {
        var selfRot = transform.rotation;
        var targetRot = target.rotation;
        var selfCustomRot = selfRot.ToCustomQuaternion();
        var targetCustomRot = targetRot.ToCustomQuaternion();
        var t = lerpSpeed * Time.deltaTime;

        switch (slerpType) {
            case LerpType.Lerp:
                transform.rotation = Quaternion.Lerp(selfRot, targetRot, t);
                break;
            case LerpType.CustomLerp:
                transform.rotation = CustomQuaternion.Lerp(selfCustomRot, targetCustomRot, t).ToQuaternion();
                break;
            case LerpType.Slerp:
                transform.rotation = Quaternion.Slerp(selfRot, targetRot, t);
                break;
            case LerpType.CustomSlerp:
                transform.rotation = CustomQuaternion.Slerp(selfCustomRot, targetCustomRot, t).ToQuaternion();
                break;
            case LerpType.CustomSlerpTheory:
                transform.rotation = CustomQuaternion.SlerpTheory(selfCustomRot, targetCustomRot, t).ToQuaternion();
                break;
        }
    }

    public enum LerpType {
        Lerp,
        CustomLerp,
        Slerp,
        CustomSlerp,
        CustomSlerpTheory,
    }
}