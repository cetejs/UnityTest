using UnityEngine;

public class QuaternionRotateVectorTest : MonoBehaviour {
    public AutoRandomRotate target;
    public RotateVectorType vectorType;

    private void OnEnable() {
        target.onRotateFinish.AddListener(OnRotateFinish);
    }

    private void OnDisable() {
        target.onRotateFinish.RemoveListener(OnRotateFinish);
    }

    private void OnRotateFinish() {
        switch (vectorType) {
            case RotateVectorType.RotateVector:
                transform.forward = target.transform.rotation * Vector3.forward;
                break;
            case RotateVectorType.CustomRotateVector:
                transform.forward = target.transform.rotation.ToCustomQuaternion() * Vector3.forward;
                break;
        }
    }
    
    public enum RotateVectorType {
        RotateVector,
        CustomRotateVector
    }
}