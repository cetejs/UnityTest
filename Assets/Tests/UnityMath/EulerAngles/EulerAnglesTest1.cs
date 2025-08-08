using UnityEngine;

/// <summary>
/// 欧拉角旋转顺序 z-x-y
/// 使用父子节点描述轴的层级关系 y-x-z
/// y 轴影响 x 轴 、z 轴, x 轴影响 z 轴
/// 所以最先旋转影响最小的 z 轴, 其次 x 轴, 再是 y 轴
/// </summary>
public class EulerAnglesTest1 : MonoBehaviour {
    public TestType testType;
    public Vector3 eulerAngles;
    
    private void OnEnable() {
        var x = new Vector3(eulerAngles.x, 0, 0);
        var y = new Vector3(0, eulerAngles.y, 0);
        var z = new Vector3(0, 0, eulerAngles.z);

        // 下面三种旋转的结果一致
        switch (testType) {
            case TestType.One:
                transform.Rotate(eulerAngles);
                return;
            // 静态旋转(世界空间)执行顺序 z-x-y
            case TestType.Two:
                transform.Rotate(z, Space.World);
                transform.Rotate(x, Space.World);
                transform.Rotate(y, Space.World);
                return;
            // 动态旋转(本地空间)执行顺序 y-x-z
            case TestType.Three:
                transform.Rotate(y, Space.Self);
                transform.Rotate(x, Space.Self);
                transform.Rotate(z, Space.Self);
                return;
        }
    }
    
    public enum TestType {
        One,
        Two,
        Three
    }
}