using UnityEngine;

namespace Tests
{
    public class MatrixConvertTest : MonoBehaviour
    {
        public Transform testGo;
        public bool isRandom = true;
        [TextArea]
        public string describe = "演示 欧拉角-四元数-矩阵 相互转换。";

        private void OnEnable()
        {
            if (isRandom)
            {
                testGo.transform.eulerAngles = new Vector3(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            }

            var e = testGo.eulerAngles;
            var q = testGo.rotation;
            var m = Matrix4x4.Rotate(q);
            e.x = QuaternionUtil.WrapPi(e.x);
            e.y = QuaternionUtil.WrapPi(e.y);
            e.z = QuaternionUtil.WrapPi(e.z);

            var e2q = EulerToQuaternion(e);
            var e2m = EulerToMatrix(e);
            var q2e = QuaternionToEuler(q.ToCustomQuaternion());
            var q2m = QuaternionToMatrix(q.ToCustomQuaternion());
            var m2e = MatrixToEuler(m.ToCustomMatrix4x4());
            var m2q = MatrixToQuaternion(m.ToCustomMatrix4x4());

            Debug.Log($"q: {q}\te2q: {e2q}");
            Debug.Log($"m: {m}\ne2m: {e2m}");
            Debug.Log($"e: {e}\tq2e: {q2e}");
            Debug.Log($"m: {m}\nq2m: {q2m}");
            Debug.Log($"e: {e}\tm2e: {m2e}");
            Debug.Log($"q: {q}\tm2q: {m2q}");
        }

        private CustomQuaternion EulerToQuaternion(Vector3 e)
        {
            return CustomQuaternion.Euler(e);
        }

        private CustomMatrix4x4 EulerToMatrix(Vector3 euler)
        {
            return CustomMatrix4x4.Rotate(euler);
        }

        private Vector3 QuaternionToEuler(CustomQuaternion q)
        {
            return CustomQuaternion.ToEuler(q);
        }

        private CustomMatrix4x4 QuaternionToMatrix(CustomQuaternion q)
        {
            return CustomQuaternion.ToMatrix(q);
        }

        private Vector3 MatrixToEuler(CustomMatrix4x4 m)
        {
            return CustomMatrix4x4.ToEuler(m);
        }

        private CustomQuaternion MatrixToQuaternion(CustomMatrix4x4 m)
        {
            return CustomMatrix4x4.ToQuaternion(m);
        }
    }
}