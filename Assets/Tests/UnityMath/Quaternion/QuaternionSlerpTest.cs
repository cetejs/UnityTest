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
                transform.rotation = UnityEngine.Quaternion.Lerp(selfRot, targetRot, t);
                break;
            case LerpType.CustomLerp:
                transform.rotation = Quaternion.Lerp(selfCustomRot, targetCustomRot, t).ToQuaternion();
                break;
            case LerpType.Slerp:
                transform.rotation = UnityEngine.Quaternion.Slerp(selfRot, targetRot, t);
                break;
            case LerpType.CustomSlerp:
                transform.rotation = Quaternion.Slerp(selfCustomRot, targetCustomRot, t).ToQuaternion();
                break;
            case LerpType.CustomSlerpTheory:
                transform.rotation = Quaternion.SlerpTheory(selfCustomRot, targetCustomRot, t).ToQuaternion();
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