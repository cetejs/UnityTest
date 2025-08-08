using UnityEngine;

namespace DataStructure
{
    public static class MatrixUtil
    {
        public static CustomMatrix4x4 ToCustomMatrix4x4(this Matrix4x4 m)
        {
            CustomMatrix4x4 r = new CustomMatrix4x4();
            r.c0 = new CustomVector4(m.m00, m.m01, m.m02, m.m03);
            r.c1 = new CustomVector4(m.m10, m.m11, m.m12, m.m13);
            r.c2 = new CustomVector4(m.m20, m.m21, m.m22, m.m23);
            r.c3 = new CustomVector4(m.m30, m.m31, m.m32, m.m33);
            return r;
        }

        public static Matrix4x4 ToMatrix4x4(CustomMatrix4x4 m)
        {
            return new Matrix4x4(m.c0.ToVector4(), m.c1.ToVector4(), m.c2.ToVector4(), m.c3.ToVector4());
        }

        public static CustomMatrix4x4 GetWorldMatrix(Transform trs)
        {
            return CustomMatrix4x4.TRS(trs.position, trs.eulerAngles, trs.lossyScale);
        }

        public static CustomMatrix4x4 GetObjectMatrix(Transform trs)
        {
            return CustomMatrix4x4.InverseTRS(trs.position, trs.eulerAngles, trs.lossyScale);
        }

        // 观察空间选中的是右手坐标系, 需要对 z 取反
        public static CustomMatrix4x4 GetViewMatrix(Camera cam)
        {
            var camT = cam.transform;
            var vm = CustomMatrix4x4.InverseTRS(camT.position, camT.eulerAngles, Vector3.one);
            var im = CustomMatrix4x4.identity;
            im.c2.z = -1;
            vm = im * vm;
            return vm;
        }

        public static CustomMatrix4x4 GetProjectionMatrix(Camera cam)
        {
            if (cam.orthographic)
            {
                return CustomMatrix4x4.Ortho(cam.orthographicSize, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
            }

            return CustomMatrix4x4.Perspective(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
        }

        public static bool IsInClipArea(CustomVector4 cp)
        {
            if (cp.x < -cp.w || cp.x > cp.w ||
                cp.y < -cp.w || cp.y > cp.w ||
                cp.z < -cp.w || cp.z > cp.w)
            {
                return false;
            }

            return true;
        }

        // cp.xy / cp.w 获得 DNC 空间中的点, 把点从 DNC 空间转换到屏幕空间, x = [-1, 1] => [0, width], y = [-1, 1] => [0, height]
        public static Vector2 GetScreenPos(Camera cam, CustomVector4 cp)
        {
            if (cp.w == 0)
            {
                return Vector2.zero;
            }

            var pos = Vector2.zero;
            pos.x = (cp.x / cp.w + 1) / 2 * cam.pixelWidth;
            pos.y = (cp.y / cp.w + 1) / 2 * cam.pixelHeight;
            return pos;
        }

        public static Vector3 WorldToViewPoint(Camera cam, Vector3 pos)
        {
            return GetViewMatrix(cam) * pos;
        }

        public static Vector3 WorldToClipPoint(Camera cam, Vector3 pos)
        {
            var vp = GetViewMatrix(cam) * new CustomVector4(pos, 1);
            var cp = GetProjectionMatrix(cam) * vp;
            if (cp.w == 0)
            {
                return Vector3.zero;
            }

            return new Vector3(cp.x, cp.y, cp.z) / cp.w;
        }

        public static Vector3 WorldToScreenPoint(Camera cam, Vector3 pos)
        {
            var vp = GetViewMatrix(cam) * new CustomVector4(pos, 1);
            var cp = GetProjectionMatrix(cam) * vp;
            var sp = GetScreenPos(cam, cp);
            return new Vector3(sp.x, sp.y, -vp.z);
        }
    }
}