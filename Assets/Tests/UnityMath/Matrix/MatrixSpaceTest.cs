using UnityEngine;

public class MatrixSpaceTest : MonoBehaviour {
    public Transform testGo;
    public bool isRandom = true;
    [TextArea]
    public string describe = "演示矩阵空间变换 对象空间 <=> 世界空间 => 观察空间 => 裁剪空间 => 屏幕空间。";

    private void OnEnable() {
        var cam = Camera.main;
        var parent = testGo.parent;
        if (isRandom) {
            parent.position = Vector4Util.RandomVector3(100);
            parent.eulerAngles = Vector4Util.RandomVector3(180);
            parent.localScale = Vector4Util.RandomVector3(10);
            testGo.localPosition = Vector4Util.RandomVector3(100);
        }

        var tm = MatrixUtil.GetWorldMatrix(parent);
        var om = MatrixUtil.GetObjectMatrix(parent);
        var vm = MatrixUtil.GetViewMatrix(cam);
        var pm = MatrixUtil.GetProjectionMatrix(cam);
        var testWorldPos = tm * testGo.localPosition;
        var testLocalPos = om * testGo.position;
        var testViewPos = vm * new Vector4(testWorldPos, 1);
        var testClipPos = pm * testViewPos;
        var testScreenPos = MatrixUtil.GetScreenPos(cam, testClipPos);

        Debug.Log($"tm: {parent.localToWorldMatrix}\n{tm}");
        Debug.Log($"om: {parent.worldToLocalMatrix}\n{om}");
        Debug.Log($"vm: {cam.worldToCameraMatrix}\n{vm}");
        Debug.Log($"pm: {cam.projectionMatrix}\n{pm}");
        Debug.Log($"testWorldPos: {testGo.position} {testWorldPos}");
        Debug.Log($"testLocalPos: {testGo.localPosition} {testLocalPos}");
        Debug.Log($"testViewPos: {cam.worldToCameraMatrix * new Vector4(testWorldPos, 1).ToVector4()} {testViewPos}");
        Debug.Log($"testClipPos: {cam.projectionMatrix * cam.worldToCameraMatrix * new Vector4(testWorldPos, 1).ToVector4()} {testClipPos}");
        Debug.Log($"testClipPos: {cam.WorldToScreenPoint(testWorldPos)} {new Vector3(testScreenPos.x, testScreenPos.y, -testViewPos.z)}");
    }
}