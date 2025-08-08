using DataStructure;
using UnityEngine;

public class QuaternionEulerTest : MonoBehaviour {
    public AutoRandomRotate target;
    public EulerType eulerType;

    private void OnEnable() {
        target.onRotateFinish.AddListener(OnRotateFinish);
    }

    private void OnDisable() {
        target.onRotateFinish.RemoveListener(OnRotateFinish);
    }

    private void OnRotateFinish() {
        switch (eulerType) {
            case EulerType.Euler:
                transform.rotation = Quaternion.Euler(target.transform.eulerAngles);
                break;
            case EulerType.CustomEuler:
                transform.rotation = CustomQuaternion.Euler(target.transform.eulerAngles).ToQuaternion();
                break;
        }
    }

    private void OnGUI() {
        var scenePos = Camera.main.WorldToScreenPoint(transform.position);
        scenePos.x -= 60;
        scenePos.y = Camera.main.pixelHeight - scenePos.y - 60;
        var rect = new Rect(scenePos.x, scenePos.y, 120, 120);
        switch (eulerType) {
            case EulerType.ToEuler:
                var euler = target.transform.eulerAngles;
                GUI.TextField(rect, new Vector3(QuaternionUtil.WrapPi(euler.x), QuaternionUtil.WrapPi(euler.y), QuaternionUtil.WrapPi(euler.z)).ToString());
                break;
            case EulerType.ToCustomEuler:
                GUI.TextField(rect, target.transform.rotation.ToCustomQuaternion().eulerAngles.ToString());
                break;
        }
    }

    public enum EulerType {
        Euler,
        CustomEuler,
        ToEuler,
        ToCustomEuler
    }
}