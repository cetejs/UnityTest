using UnityEngine;

namespace DataStructure
{
    public class CustomMatrix4x4
    {
        public CustomVector4 c0;
        public CustomVector4 c1;
        public CustomVector4 c2;
        public CustomVector4 c3;

        private bool isInverse;

        public static readonly CustomMatrix4x4 identity = new CustomMatrix4x4(
            new CustomVector4(1, 0, 0, 0),
            new CustomVector4(0, 1, 0, 0),
            new CustomVector4(0, 0, 1, 0),
            new CustomVector4(0, 0, 0, 1));

        public CustomMatrix4x4()
        {
        }

        public CustomMatrix4x4(CustomVector4 c0, CustomVector4 c1, CustomVector4 c2, CustomVector4 c3)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
        }

        public CustomVector4 MultiplyPoint(CustomVector4 v)
        {
            var r = new CustomVector4();
            r.x = CustomVector4.Dot(c0, v);
            r.y = CustomVector4.Dot(c1, v);
            r.z = CustomVector4.Dot(c2, v);
            r.w = CustomVector4.Dot(c3, v);
            return r;
        }

        public Vector3 MultiplyPoint3X4(Vector3 v)
        {
            var r = MultiplyPoint(new CustomVector4(v.x, v.y, v.z, 1));
            return new Vector3(r.x, r.y, r.z);
        }

        public override string ToString()
        {
            return $"{c0.ToMatrixString()}\n{c1.ToMatrixString()}\n{c2.ToMatrixString()}\n{c3.ToMatrixString()}";
        }

        private static CustomVector4 Multiply(CustomVector4 v, CustomMatrix4x4 m)
        {
            var r = new CustomVector4();
            r.x = v.x * m.c0.x + v.y * m.c1.x + v.z * m.c2.x + v.w * m.c3.x;
            r.y = v.x * m.c0.y + v.y * m.c1.y + v.z * m.c2.y + v.w * m.c3.y;
            r.z = v.x * m.c0.z + v.y * m.c1.z + v.z * m.c2.z + v.w * m.c3.z;
            r.w = v.x * m.c0.w + v.y * m.c1.w + v.z * m.c2.w + v.w * m.c3.w;
            return r;
        }

        public static CustomMatrix4x4 Multiply(CustomMatrix4x4 a, CustomMatrix4x4 b)
        {
            var m = new CustomMatrix4x4();
            m.c0 = Multiply(a.c0, b);
            m.c1 = Multiply(a.c1, b);
            m.c2 = Multiply(a.c2, b);
            m.c3 = Multiply(a.c3, b);
            return m;
        }

        public static CustomMatrix4x4 Transpose(CustomMatrix4x4 m)
        {
            var r = new CustomMatrix4x4();
            r.c0 = new CustomVector4(m.c0.x, m.c1.x, m.c2.x, m.c3.x);
            r.c1 = new CustomVector4(m.c0.y, m.c1.y, m.c2.y, m.c3.y);
            r.c2 = new CustomVector4(m.c0.z, m.c1.z, m.c2.z, m.c3.z);
            r.c3 = new CustomVector4(m.c0.w, m.c1.w, m.c2.w, m.c3.w);
            return r;
        }

        // 2D 旋转矩阵推导
        // x = r * cos(a), y = r * sin(a)
        // x` = r * cos(a + b), y` = r * cos(a + b)
        // x` = r * cos(a) * cos(b) - r * sin(a) * sin(b), y` = r * sin(a) * cos(b) + r * sin(b) * cos(a), 
        // 代入 x, y 得
        // x` = x * cos(b) - y * sin(b), y` = x * sin(b) + y * cos(b)
        // 使用矩阵形式
        // [cos(b), -sin(b)] [x]
        // [sin(b),  cos(b)] [y]
        // 注意 2D 旋转的正方向为逆时针, 与绕 x、z 轴旋转的正方向一致, y 轴旋转正方向相反, 可推导出 3D 旋转矩阵
        //       [1,   0,       0   ]          [ cos(y), 0, sin(y)]          [cos(z), -sin(z), 0]
        // rxm = [0, cos(x), -sin(x)]    rym = [   0,    1,   0   ]    rzm = [sin(z),  cos(z), 0]
        //       [0, sin(x),  cos(x)]          [-sin(y), 0, cos(y)]          [  0,       0,    1]
        //
        // rm  = rym * rxm * rzm
        //       [cy * cz + sy * sx * sz,  -cy * sz + sy * sx * cz, sy * cx]
        //     = [      cx * sz,                 cx * cz,             -sx  ]
        //       [-sy * cz + cy * sx * sz, sy * sz + cy * sx * cz,  cy * cx]
        public static CustomMatrix4x4 Rotate(Vector3 rot)
        {
            // var sinX = Mathf.Sin(rot.x * Mathf.Deg2Rad);
            // var cosX = Mathf.Cos(rot.x * Mathf.Deg2Rad);
            // var rxm = new CustomMatrix4x4();
            // rxm.c0 = new CustomVector4(1, 0, 0, 0);
            // rxm.c1 = new CustomVector4(0, cosX, -sinX, 0);
            // rxm.c2 = new CustomVector4(0, sinX, cosX, 0);
            // rxm.c3 = new CustomVector4(0, 0, 0, 1);

            // var sinY = Mathf.Sin(rot.y * Mathf.Deg2Rad);
            // var cosY = Mathf.Cos(rot.y * Mathf.Deg2Rad);
            // var rym = new CustomMatrix4x4();
            // rym.c0 = new CustomVector4(cosY, 0, sinY, 0);
            // rym.c1 = new CustomVector4(0, 1, 0, 0);
            // rym.c2 = new CustomVector4(-sinY, 0, cosY, 0);
            // rym.c3 = new CustomVector4(0, 0, 0, 1);

            // var sinZ = Mathf.Sin(rot.z * Mathf.Deg2Rad);
            // var cosZ = Mathf.Cos(rot.z * Mathf.Deg2Rad);
            // var rzm = new CustomMatrix4x4();
            // rzm.c0 = new CustomVector4(cosZ, -sinZ, 0, 0);
            // rzm.c1 = new CustomVector4(sinZ, cosZ, 0, 0);
            // rzm.c2 = new CustomVector4(0, 0, 1, 0);
            // rzm.c3 = new CustomVector4(0, 0, 0, 1);

            // var rm = rym * rxm * rzm;
            var sx = Mathf.Sin(rot.x * Mathf.Deg2Rad);
            var cx = Mathf.Cos(rot.x * Mathf.Deg2Rad);
            var sy = Mathf.Sin(rot.y * Mathf.Deg2Rad);
            var cy = Mathf.Cos(rot.y * Mathf.Deg2Rad);
            var sz = Mathf.Sin(rot.z * Mathf.Deg2Rad);
            var cz = Mathf.Cos(rot.z * Mathf.Deg2Rad);
            var rm = new CustomMatrix4x4();
            rm.c0 = new CustomVector4(cy * cz + sy * sx * sz, -cy * sz + sy * sx * cz, sy * cx, 0);
            rm.c1 = new CustomVector4(cx * sz, cx * cz, -sx, 0);
            rm.c2 = new CustomVector4(-sy * cz + cy * sx * sz, sy * sz + cy * sx * cz, cy * cx, 0);
            rm.c3 = new CustomVector4(0, 0, 0, 1);
            return rm;
        }

        public static CustomMatrix4x4 Rotate(CustomQuaternion rot)
        {
            return CustomQuaternion.ToMatrix(rot);
        }

        public static CustomMatrix4x4 TRS(Vector3 pos, Vector3 rot, Vector3 scl)
        {
            var tm = new CustomMatrix4x4();
            tm.c0 = new CustomVector4(1, 0, 0, pos.x);
            tm.c1 = new CustomVector4(0, 1, 0, pos.y);
            tm.c2 = new CustomVector4(0, 0, 1, pos.z);
            tm.c3 = new CustomVector4(0, 0, 0, 1);

            var rm = Rotate(rot);

            var sm = new CustomMatrix4x4();
            sm.c0 = new CustomVector4(scl.x, 0, 0, 0);
            sm.c1 = new CustomVector4(0, scl.y, 0, 0);
            sm.c2 = new CustomVector4(0, 0, scl.z, 0);
            sm.c3 = new CustomVector4(0, 0, 0, 1);

            return tm * rm * sm;
        }

        public static CustomMatrix4x4 TRS(Vector3 pos, CustomQuaternion rot, Vector3 scl)
        {
            var tm = new CustomMatrix4x4();
            tm.c0 = new CustomVector4(1, 0, 0, pos.x);
            tm.c1 = new CustomVector4(0, 1, 0, pos.y);
            tm.c2 = new CustomVector4(0, 0, 1, pos.z);
            tm.c3 = new CustomVector4(0, 0, 0, 1);

            var rm = CustomQuaternion.ToMatrix(rot);

            var sm = new CustomMatrix4x4();
            sm.c0 = new CustomVector4(scl.x, 0, 0, 0);
            sm.c1 = new CustomVector4(0, scl.y, 0, 0);
            sm.c2 = new CustomVector4(0, 0, scl.z, 0);
            sm.c3 = new CustomVector4(0, 0, 0, 1);

            return tm * rm * sm;
        }

        // 旋转矩阵是正交矩阵, 求旋转矩阵的逆可使用其转置
        //      [cy * cz + sy * sx * sz,   cx * sz, -sy * cz + cy * sx * sz]
        // rm = [-cy * sz + sy * sx * cz,  cx * cz,  sy * sz + cy * sx * cz]
        //      [sy * cx,                   -sx ,          cy * cx         ]
        public static CustomMatrix4x4 InverseTRS(Vector3 pos, Vector3 rot, Vector3 scl)
        {
            pos = -pos;
            scl = new Vector3(scl.x == 0 ? 0 : 1 / scl.x, scl.y == 0 ? 0 : 1 / scl.y, scl.z == 0 ? 0 : 1 / scl.z);

            var tm = new CustomMatrix4x4();
            tm.c0 = new CustomVector4(1, 0, 0, pos.x);
            tm.c1 = new CustomVector4(0, 1, 0, pos.y);
            tm.c2 = new CustomVector4(0, 0, 1, pos.z);
            tm.c3 = new CustomVector4(0, 0, 0, 1);

            // var sx =  Mathf.Sin(rot.x * Mathf.Deg2Rad);
            // var cx = Mathf.Cos(rot.x * Mathf.Deg2Rad);
            // var sy = Mathf.Sin(rot.y * Mathf.Deg2Rad);
            // var cy = Mathf.Cos(rot.y * Mathf.Deg2Rad);
            // var sz = Mathf.Sin(rot.z * Mathf.Deg2Rad);
            // var cz = Mathf.Cos(rot.z * Mathf.Deg2Rad);
            // var rm = new CustomMatrix4x4();
            // rm.c0 = new CustomVector4(cy * cz + sy * sx * sz,   cx * sz, -sy * cz + cy * sx * sz, 0);
            // rm.c1 = new CustomVector4(-cy * sz + sy * sx * cz,  cx * cz,  sy * sz + cy * sx * cz, 0);
            // rm.c2 = new CustomVector4(sy * cx, -sx, cy * cx, 0);
            // rm.c3 = new CustomVector4(0, 0, 0, 1);
            var rm = Transpose(Rotate(rot));

            var sm = new CustomMatrix4x4();
            sm.c0 = new CustomVector4(scl.x, 0, 0, 0);
            sm.c1 = new CustomVector4(0, scl.y, 0, 0);
            sm.c2 = new CustomVector4(0, 0, scl.z, 0);
            sm.c3 = new CustomVector4(0, 0, 0, 1);

            var m = sm * rm * tm;
            m.isInverse = true;
            return m;
        }

        public static CustomMatrix4x4 InverseTRS(Vector3 pos, CustomQuaternion rot, Vector3 scl)
        {
            pos = -pos;
            scl = new Vector3(scl.x == 0 ? 0 : 1 / scl.x, scl.y == 0 ? 0 : 1 / scl.y, scl.z == 0 ? 0 : 1 / scl.z);

            var tm = new CustomMatrix4x4();
            tm.c0 = new CustomVector4(1, 0, 0, pos.x);
            tm.c1 = new CustomVector4(0, 1, 0, pos.y);
            tm.c2 = new CustomVector4(0, 0, 1, pos.z);
            tm.c3 = new CustomVector4(0, 0, 0, 1);

            var rm = Transpose(CustomQuaternion.ToMatrix(rot));

            var sm = new CustomMatrix4x4();
            sm.c0 = new CustomVector4(scl.x, 0, 0, 0);
            sm.c1 = new CustomVector4(0, scl.y, 0, 0);
            sm.c2 = new CustomVector4(0, 0, scl.z, 0);
            sm.c3 = new CustomVector4(0, 0, 0, 1);


            var m = sm * rm * tm;
            m.isInverse = true;
            return m;
        }

        // 在透视投影中, 先把观察空间的点转换到规则观察体, 再使用透视除法转换到标准化设备坐标(DNC), 在 OpenGL 中 DNC 的范围在 x, y, z <= [-1, 1] 一个立方体
        // 注意观察空间使用的是右手坐标, z 轴方向相反
        // 假设视锥体定义左右下上前后 (l, r, b , t, n, f)
        // 假设观察空间中有一点 p(x, y, z), 需要把 p 映射到近平面得到 pn
        // 根据三角形比率计算 px`/ px = pz` / pz
        // xn = -n * x / z 同理 yn = -n * y / z
        // 把 pn 映射到 DNC 空间中得到 pc, 由于 xn <= [l, r], xc <= [-1, 1]
        // (r - x) / (r - l) = (1 - xc) / (1 - (-1))
        // xc = 2 * xn / (r - l) - (r + l) / (r - l)
        // 同理可得 yc = 2 * yn / (t - b) - (t + b) / (t - b)
        // 将 xn, yn 代入方程得
        // xc = [2n / (r - l) * x + (r + l) / (r - l) * z] / -z
        // yc = [2n / (t - b) * y + (t + b) / (t - b) * z] / -z
        // 最后会使用透视除法(x / w, y / w), 因此 w = -z
        // [xc]   [2n / (r - l), 0,            (r + l) / (r - l), 0] [x]
        // [yc] = [0,            2n / (t - b), (t + b) / (t - b), 0] [y]
        // [zc]   [0,            0,             A,                B] [z]
        // [wc]   [0,            0,             -1,               0] [w]
        // z 值的变换不依赖与 xy, 则可借用 w 分量来找到 z` 与 z 的关系
        // zc = (Az + Bw) / -z, 在观察空间 w = 1
        //    = (Az + B) / -z
        // 使用 z <= [-n, -f] zc <= [-1, 1] 映射关系
        // -1 = (A * (-n) + B) / -z
        // 1  = (A * (-f) + B) / -z
        // 求出 A = -(f + n) / (f - n), B = -2fn / (f - n), 可得投影矩阵
        //     [2n / (r - l), 0,            (r + l) / (r - l),   0             ]
        // m = [0,            2n / (t - b), (t + b) / (t - b),   0             ]
        //     [0,            0,             -(f + n) / (f - n), -2fn / (f - n)]
        //     [0,            0,             -1,                 0             ]
        // 因 r = -l, t = -b 则 r + l = 0, r - l = 2r, 同理 t + b = 0, t - b = 2t, 可简化投影矩阵
        //     [n / r, 0,     0,                  0             ]
        // m = [0,     n / t, 0,                  0             ]
        //     [0,     0,     -(f + n) / (f - n), -2fn / (f - n)]
        //     [0,     0,     -1,                 0             ]
        // 因 tan(fov / 2) = t / n, aspect = r / t 则 n / r = cot(fov / 2) / aspect, n / t = cot(fov / 2), 则最终投影矩阵
        //     [cot(fov / 2) / aspect, 0,          0,                  0             ]
        // m = [0,                     cot(fov / 2), 0,                  0             ]
        //     [0,                     0,            -(f + n) / (f - n), -2fn / (f - n)]
        //     [0,                     0,            -1,                 0             ]
        public static CustomMatrix4x4 Perspective(float fov, float aspect, float zNear, float zFar)
        {
            var m = new CustomMatrix4x4();
            var cotHalfFov = 1 / Mathf.Tan(fov / 2 * Mathf.Deg2Rad);
            m.c0 = new CustomVector4(cotHalfFov / aspect, 0, 0, 0);
            m.c1 = new CustomVector4(0, cotHalfFov, 0, 0);
            m.c2 = new CustomVector4(0, 0, -(zFar + zNear) / (zFar - zNear), -2 * zFar * zNear / (zFar - zNear));
            m.c3 = new CustomVector4(0, 0, -1, 0);
            return m;
        }

        // 假设观察空间中有一点 p(x, y, z), 需要把 p 映射到 DNC 空间
        // 假设视锥体定义左右下上前后 (l, r, b , t, n, f)
        // 因 x <= [l, r], xc <= [-1, 1]
        // (r - x) / (r - l) = (1 - xc) / (1 - (-1))
        // xc = 2x / (r - l) - (r + l) / (r - l) 
        // 同理可得 yc = 2y / (t - b) - (t + b) / (t - b)
        // 因 z <= [-n, -f], xc <= [-1, 1]
        // (-f - x) / (-f - (-n)) = (1 - zc) / (1 - (-1))
        // zc = -2z / (f - n) - (f + n) / (f - n)
        // pc(xc, yc ,zc) 可得投影矩阵, 正交投影不需要进行透视除法则 w = 1
        //     [2 / (r - l), 0,           0,            -(r + l) / (r - l)]
        // m = [0,           2 / (t - b), 0,            -(t + b) / (t - b)]
        //     [0,           0,           -2 / (f - n), -(f + n) / (f - n)]
        //     [0,           0,           0,                             1]
        // 因 r = -l, t = -b 则 r + l = 0, r - l = 2r, 同理 t + b = 0, t - b = 2t, 可简化投影矩阵
        //     [1 / r, 0,     0,            0                 ]
        // m = [0,     1 / t, 0,            0                 ]
        //     [0,     0,     -2 / (f - n), -(f + n) / (f - n)]
        //     [0,     0,     0,            1                 ]
        // 因 size = t, aspect = r / t, 则 r = aspect * size, 则最终投影矩阵
        //     [1 / (aspect * size), 0,        0,            0                 ]
        // m = [0,                   1 / size, 0,            0                 ]
        //     [0,                   0,        -2 / (f - n), -(f + n) / (f - n)]
        //     [0,                   0,        0,            1                 ]
        public static CustomMatrix4x4 Ortho(float size, float aspect, float zNear, float zFar)
        {
            var m = new CustomMatrix4x4();
            m.c0 = new CustomVector4(1 / (aspect * size), 0, 0, 0);
            m.c1 = new CustomVector4(0, 1 / size, 0, 0);
            m.c2 = new CustomVector4(0, 0, -2 / (zFar - zNear), -(zFar + zNear) / (zFar - zNear));
            m.c3 = new CustomVector4(0, 0, 0, 1);
            return m;
        }

        // 绕任意轴的三维旋转
        // v1 表示 n 的垂直向量, v2 表示 n 的平行向量, w 向量与 v1、v2 互相垂直, 相当于形成了三维基坐标
        // v` = vR(n, a) 为了求 R(n, a) 需要使用 v, n, a 表示 v`
        // v = v1 + v2, v` = v1` + v2
        // v2 = (v2 · n) * n, v1` = cos(a) * v1 + sin(a) * w
        // v1 = v - v2 = v - (v · n) * n
        // w = n x v1 = n x (v - v2) = n x v - n x v2 = n x v
        // v1` = cos(a) * v1 + sin(a) * w
        //     = cos(a) * [v - (v · n) * n] + sin(a) * (n x v) + (v · n) * n
        //
        // 将 p(1, 0, 0)、q(0, 1, 0)、 r(0, 0, 1) 代入可得
        // p` = ca * [(1, 0, 0) · n * n] + sa * [n x (1, 0, 0)] + (1, 0, 0) · n * n
        //    = ca * [(1, 0, 0) - nx * (nx, ny, nz))] + sa * [(nx, ny, nz) x (1, 0, 0)] + nx * (nx, ny, nz))
        //    = ca * (1 - nx * nx, -nx * ny, -nx * nz) + sa * (0, nz, -ny) + nx * (nx, ny, nz))
        //    = (ca - ca * nx * nx + nx * nx, -ca * nx * ny + sa * nz + nx * ny, -ca * nx * nz - sa * ny + nx * nz)T
        //    = (nx * nx * (1 - ca) + ca, nx * ny * (1 - ca) + nz * sa, nx * nz * (1 - ca) - ny * sa)T
        //
        //           [ |   |   | ]   [nx * nx * (1 - ca) + ca,      nx * ny * (1 - ca) - nz * sa, nx * nz * (1 - ca) + ny * sa]
        // R(n, a) = [ p`, q`, r`] = [nx * ny * (1 - ca) + nz * sa, ny * ny * (1 - ca) + ca,      ny * nz * (1 - ca) - nx * sa]
        //           [ |   |   | ]   [nx * nz * (1 - ca) - ny * sa, ny * nz * (1 - ca) + nx * sa, nz * nz * (1 - ca) + ca     ]
        public static CustomMatrix4x4 AngleAxis(float angle, Vector3 axis)
        {
            var nx = axis.x;
            var ny = axis.y;
            var nz = axis.z;
            var sa = Mathf.Sin(angle * Mathf.Deg2Rad);
            var ca = Mathf.Cos(angle * Mathf.Deg2Rad);
            var m = new CustomMatrix4x4();
            m.c0 = new CustomVector4(nx * nx * (1 - ca) + ca, nx * ny * (1 - ca) - nz * sa, nx * nz * (1 - ca) + ny * sa, 0);
            m.c1 = new CustomVector4(nx * ny * (1 - ca) + nz * sa, ny * ny * (1 - ca) + ca, ny * nz * (1 - ca) - nx * sa, 0);
            m.c2 = new CustomVector4(nx * nz * (1 - ca) - ny * sa, ny * nz * (1 - ca) + nx * sa, nz * nz * (1 - ca) + ca, 0);
            m.c3 = new CustomVector4(0, 0, 0, 1);
            return m;
        }

        // 由 CustomMatrix4x4.TRS 可知, 由欧拉角表示的旋转矩阵
        //     [cy * cz + sy * sx * sz,  -cy * sz + sy * sx * cz, sy * cx]
        // m = [      cx * sz,                 cx * cz,             -sx  ]
        //     [-sy * cz + cy * sx * sz, sy * sz + cy * sx * cz,  cy * cx]
        //
        // 如果 x == +-90, 出现万向节死锁, cx = 0, z = 0, sz = 0, cz = 1
        // 将这些参数代入矩阵中求得, 如果是逆矩阵则是其转置
        //     [cy,  sy * sx, 0]
        // m = [0,      0,  -sx]
        //     [-sy, cy * sx, 0]
        //
        // x = asin(-m12), y = atan2(m02, m22), z = atan2(m10, m12)
        // 如果 x == +-90, y = atan2(-m20, m00), z = 0
        public static Vector3 ToEuler(CustomMatrix4x4 m)
        {
            float x, y, z;

            var sinX = m.isInverse ? -m.c2.y : -m.c1.z;
            if (sinX >= 1f)
            {
                x = Mathf.PI / 2;
            }
            else if (sinX <= -1f)
            {
                x = -Mathf.PI / 2;
            }
            else
            {
                x = Mathf.Asin(sinX);
            }

            // 为什么不直接使用 asin 或者 acos, 因为 asin 的取值范围是 [-90, 90], acos 的取值范围是 [0, 180]
            // 而 atan2 的取值范围是 [-180. 180]
            if (m.isInverse)
            {
                if (Mathf.Abs(sinX) > 0.9999f)
                {
                    y = Mathf.Atan2(-m.c0.z, m.c0.x);
                    z = 0;
                }
                else
                {
                    y = Mathf.Atan2(m.c2.x, m.c2.z);
                    z = Mathf.Atan2(m.c0.y, m.c1.y);
                }
            }
            else
            {
                if (Mathf.Abs(sinX) > 0.9999f)
                {
                    y = Mathf.Atan2(-m.c2.x, m.c0.x);
                    z = 0;
                }
                else
                {
                    y = Mathf.Atan2(m.c0.z, m.c2.z);
                    z = Mathf.Atan2(m.c1.x, m.c1.y);
                }
            }

            return new Vector3(x, y, z) * Mathf.Rad2Deg;
        }

        // 由 CustomQuaternion.ToMatrix 可知, 由四元数表示的旋转矩阵
        //     [1 - 2 * y * y - 2 * z * z, 2 * x * y - 2 * w * z,     2 * x * z + 2 * w * y    ]
        // m = [2 * x * y + 2 * w * z,     1 - 2 * x * x - 2 * z * z, 2 * y * z - 2 * w * x    ]
        //     [2 * x * z - 2 * w * y,     2 * y * z + 2 * w * x,     1 - 2 * x * x - 2 * y * y]
        //
        // tr(m) = m11 + m22 + m33
        //       = 1 - 2yy - 2zz + 1 - 2xx - 2zz + 2xx - 2yy
        //       = 3 - 4(xx + yy + zz)
        //       = 3 - 4(1 - ww)
        //       = 4ww - 1
        // w = sqrt(m11 + m22 + m33 + 1) / 2
        // m11 - m22 - m33  = 4xx - 1
        // -m11 + m22 - m33 = 4yy - 1
        // -m11 - m22 + m33 = 4zz - 1
        // x = sqrt( m11 - m22 - m33 + 1) / 2
        // y = sqrt(-m11 + m22 - m33 + 1) / 2
        // z = sqrt(-m11 - m22 + m33 + 1) / 2
        public static CustomQuaternion ToQuaternion(CustomMatrix4x4 m)
        {
            var q = new CustomQuaternion();
            q.w = Mathf.Sqrt(m.c0.x + m.c1.y + m.c2.z + 1) / 2;
            q.x = Mathf.Sqrt(m.c0.x - m.c1.y - m.c2.z + 1) / 2;
            q.y = Mathf.Sqrt(-m.c0.x + m.c1.y - m.c2.z + 1) / 2;
            q.z = Mathf.Sqrt(-m.c0.x - m.c1.y + m.c2.z + 1) / 2;
            return q;
        }

        public static CustomMatrix4x4 operator *(CustomMatrix4x4 a, CustomMatrix4x4 b)
        {
            return Multiply(a, b);
        }

        public static CustomVector4 operator *(CustomMatrix4x4 m, CustomVector4 v)
        {
            return m.MultiplyPoint(v);
        }

        public static Vector3 operator *(CustomMatrix4x4 m, Vector3 v)
        {
            return m.MultiplyPoint3X4(v);
        }
    }
}