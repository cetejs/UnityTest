using UnityEngine;

namespace Tests
{
    /// <summary>
    /// 欧拉角旋转顺序 z-x-y
    /// 使用父子节点描述轴的层级关系 y-x-z
    /// y 轴影响 x 轴 、z 轴, x 轴影响 z 轴
    /// 所以最先旋转影响最小的 z 轴, 其次 x 轴, 再是 y 轴
    /// </summary>
    public class EulerAnglesTest2 : MonoBehaviour
    {
        public float angleSpeed = 30;
        public float wrapPiAngle = 0;

        // 当拖到 Inspector 面板上 Rotation 参数时, 可以肉眼观察到既不是围绕本地坐标轴、也不是世界坐标轴, 那么到底是围绕着什么轴在旋转？
        // 下面做个实验, 当围绕描述的指定轴旋转时, 查看 Inspector 面板的 Rotation 参数的变换是否只影响到对应轴的旋转
        // 可以发现当 x 轴旋转到 90 的倍数就无法继续旋转, 同时 y 轴的旋转值会加入到 z 轴上, y 轴置零
        // 这时可以观察 y 轴与 z 轴共面, 失去了一个自由度, 导致万向节死锁, 再去旋转 y 轴时, 属性变化会反应到 z 轴上
        // 所以我们需要限制欧拉角的范围,  x => (-90, 90), y => (-180, 180],  z => (-180, 180]
        // 限制到 [-180, 180] => warpPi(x) = x - 360[(x + 180)/360]
        private void Update()
        {
            if (Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.LeftShift))
            {
                transform.RotateAround(transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.right, -angleSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.X))
            {
                transform.RotateAround(transform.position, Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.right, angleSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.LeftShift))
            {
                transform.RotateAround(transform.position, Vector3.up, -angleSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Y))
            {
                transform.RotateAround(transform.position, Vector3.up, angleSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Z) && Input.GetKey(KeyCode.LeftShift))
            {
                transform.RotateAround(transform.position, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * Vector3.forward, -angleSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                transform.RotateAround(transform.position, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * Vector3.forward, angleSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(WrapPi(wrapPiAngle));
            }
        }

        private void OnDrawGizmos()
        {
            // y 轴不受到其他两个轴的影响, 所以始终朝向直立坐标的 y 方向
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
            Gizmos.color = Color.white;

            // x 轴受到 y 轴的影响, 所以需要加上 y 轴旋转
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.right);
            Gizmos.color = Color.white;

            // z 轴受到 x 、y 轴的影响, 所以需要加上 x、y 轴旋转
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0) * Vector3.forward);
            Gizmos.color = Color.white;
        }

        public static float WrapPi(float angle)
        {
            if (Mathf.Abs(angle) > 180)
            {
                return angle - 360 * Mathf.Floor((angle + 180) / 360f);
            }

            return angle;
        }
    }
}