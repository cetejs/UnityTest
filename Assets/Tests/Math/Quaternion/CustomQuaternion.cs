using UnityEngine;

namespace Tests
{
    public class CustomQuaternion
    {
        public float w;
        public float x;
        public float y;
        public float z;

        public static readonly CustomQuaternion identity = new CustomQuaternion(0, 0, 0, 1);

        public Vector3 eulerAngles
        {
            get { return ToEuler(this); }
            set { Set(Euler(x, y, z)); }
        }

        public CustomQuaternion()
        {
        }

        public CustomQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        // w = a / 2
        // q = [cos(w), n * sin(w)]
        // q = [cos(w), x * sin(w), y * sin(w), z * sin(w)]
        public CustomQuaternion(float a, Vector3 n)
        {
            var halfRadA = (a / 2) * Mathf.Deg2Rad;
            var sinHalfA = Mathf.Sin(halfRadA);
            var cosHalfA = Mathf.Cos(halfRadA);
            n = n.normalized;

            w = cosHalfA;
            x = sinHalfA * n.x;
            y = sinHalfA * n.y;
            z = sinHalfA * n.z;
        }

        public void Set(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Set(CustomQuaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public override string ToString()
        {
            return $"({x:F5}, {y:F5}, {z:F5}, {w:F5})";
        }

        // 由 p(x, 0, 0), h(0, y, 0), b(0, 0, z) 组成三个四元数, 再按顺序连接
        public static CustomQuaternion Euler(float x, float y, float z)
        {
            x *= Mathf.Deg2Rad;
            y *= Mathf.Deg2Rad;
            z *= Mathf.Deg2Rad;
            var qx = new CustomQuaternion(Mathf.Sin(x / 2f), 0, 0, Mathf.Cos(x / 2f));
            var qy = new CustomQuaternion(0, Mathf.Sin(y / 2f), 0, Mathf.Cos(y / 2f));
            var qz = new CustomQuaternion(0, 0, Mathf.Sin(z / 2f), Mathf.Cos(z / 2f));
            return qy * qx * qz;
        }

        public static CustomQuaternion Euler(Vector3 euler)
        {
            return Euler(euler.x, euler.y, euler.z);
        }

        public static CustomQuaternion AngleAxis(float angle, Vector3 axis)
        {
            return new CustomQuaternion(angle, axis);
        }

        // 变负, 表示的角位移不变, 只是不同的表示方式
        // 旋转轴取反, 同时角度也取反, 负负的正
        public static CustomQuaternion Negative(CustomQuaternion q)
        {
            return new CustomQuaternion(-q.x, -q.y, -q.z, -q.w);
        }

        // q* = (w, -n)
        // q-1 = q* / |q|, 由于定义的是单位四元数, 所以其大小始终为 1
        public static CustomQuaternion Inverse(CustomQuaternion q)
        {
            return new CustomQuaternion(-q.x, -q.y, -q.z, q.w);
        }

        // exp(q, t) = (cos(a * t), n * sin(a * t)) 
        public static CustomQuaternion Exp(CustomQuaternion q, float t)
        {
            // 如果 cos(a) = +-1 则 sin(a) = 0, 避免计算 mult 时, 0 为除数
            // 此时角位移为 0/360 的倍数, 对其求指数无效
            if (Mathf.Abs(q.w) < 0.9999f)
            {
                var a = Mathf.Acos(q.w);
                var b = a * t;
                var sinA = Mathf.Sin(a);
                var sinB = Mathf.Sin(b);
                var mult = sinB / sinA;

                CustomQuaternion r = new CustomQuaternion();
                r.w = Mathf.Cos(b);
                r.x = q.x * mult;
                r.y = q.y * mult;
                r.z = q.z * mult;
                return r;
            }

            return q;
        }

        // w = w1 * w2 - (n1 * n2)
        // v = w1 * v2 + w2 * v1 + (v1 x v2)
        public static CustomQuaternion Multiply(CustomQuaternion a, CustomQuaternion b)
        {
            var q = new CustomQuaternion();
            q.w = a.w * b.w - (a.x * b.x + a.y * b.y + a.z * b.z);
            q.x = a.x * b.w + a.w * b.x + (a.y * b.z - a.z * b.y);
            q.y = a.y * b.w + a.w * b.y + (a.z * b.x - a.x * b.z);
            q.z = a.z * b.w + a.w * b.z + (a.x * b.y - a.y * b.x);
            return q;
        }

        // 结果与 diff(a, b).w 一致, 绝对值越大 角位移越相识
        public static float Dot(CustomQuaternion a, CustomQuaternion b)
        {
            return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
        }

        // d = b * a-1
        public static CustomQuaternion Diff(CustomQuaternion a, CustomQuaternion b)
        {
            return Multiply(b, Inverse(a));
        }

        public static CustomQuaternion Lerp(CustomQuaternion a, CustomQuaternion b, float t)
        {
            t = Mathf.Clamp01(t);
            t = Mathf.Clamp01(t);
            // 点积恰好为四元数之间角度的余弦
            var cosW = Dot(a, b);

            // 如果点积为负, 则将其中一个输入的四元数变负
            // 以取得最短的弧
            if (cosW < 0)
            {
                b = Negative(b);
            }

            var k0 = 1 - t;
            var k1 = t;
            CustomQuaternion q = new CustomQuaternion();
            q.w = k0 * a.w + k1 * b.w;
            q.x = k0 * a.x + k1 * b.x;
            q.y = k0 * a.y + k1 * b.y;
            q.z = k0 * a.z + k1 * b.z;
            return q;
        }

        // slerp(a, b, t) = d(t) * a = (b * a-1)(t) * a
        public static CustomQuaternion SlerpTheory(CustomQuaternion a, CustomQuaternion b, float t)
        {
            t = Mathf.Clamp01(t);
            var d = Diff(a, b);

            // 如果点积为负, 则将其中一个输入的四元数变负
            // 以取得最短的弧
            if (d.w < 0)
            {
                d = Negative(d);
            }

            return Exp(d, t) * a;
        }

        // w 是弧从 v0 到 v1 截取的角度
        // slerp(a, b, t) = sin[(1 - t) * w] / sin(w) * a + sin(t * w) / sin(w) * b
        public static CustomQuaternion Slerp(CustomQuaternion a, CustomQuaternion b, float t)
        {
            t = Mathf.Clamp01(t);
            // 点积恰好为四元数之间角度的余弦
            var cosW = Dot(a, b);

            // 如果点积为负, 则将其中一个输入的四元数变负
            // 以取得最短的弧
            if (cosW < 0)
            {
                b = Negative(b);
                cosW = -cosW;
            }

            float k0, k1;
            // 检测是否相识, 避免出现除零情况
            if (cosW > 0.9999f)
            {
                // 如果相识, 则使用线性插值
                k0 = 1 - t;
                k1 = t;
            }
            else
            {
                // k0 = sin[(1 - t) * w] / sin(w)
                // k1 = sin(t * w) / sin(w)
                var sinW = Mathf.Sqrt(1 - cosW * cosW);
                var w = Mathf.Atan2(sinW, cosW);
                k0 = Mathf.Sin((1 - t) * w) / sinW;
                k1 = Mathf.Sin(t * w) / sinW;
            }

            CustomQuaternion q = new CustomQuaternion();
            q.w = k0 * a.w + k1 * b.w;
            q.x = k0 * a.x + k1 * b.x;
            q.y = k0 * a.y + k1 * b.y;
            q.z = k0 * a.z + k1 * b.z;
            return q;
        }

        // 由 CustomMatrix4x4.ToEuler 可知由欧拉角表示的旋转矩阵
        //     [cy * cz + sy * sx * sz,  -cy * sz + sy * sx * cz, sy * cx]
        // m = [      cx * sz,                 cx * cz,             -sx  ]
        //     [-sy * cz + cy * sx * sz, sy * sz + cy * sx * cz,  cy * cx]
        //
        // 如果 x == +-90, 出现万向节死锁, cx = 0, z = 0, sz = 0, cz = 1
        //     [cy,  sy * sx, 0]
        // m = [0,      0,  -sx]
        //     [-sy, cy * sx, 0]
        //
        // x = asin(-m12), y = atan2(m02, m22), z = atan2(m10, m12)
        // 如果 x == +-90, y = atan2(-m20, m00), z = 0
        //
        // 由 CustomQuaternion.ToMatrix 可知由四元数表示的旋转矩阵
        //     [1 - 2 * y * y - 2 * z * z, 2 * x * y - 2 * w * z,     2 * x * z + 2 * w * y    ]
        // m = [2 * x * y + 2 * w * z,     1 - 2 * x * x - 2 * z * z, 2 * y * z - 2 * w * x    ]
        //     [2 * x * z - 2 * w * y,     2 * y * z + 2 * w * x,     1 - 2 * x * x - 2 * y * y]
        //
        // x = asin(-m12) = asin(-2yz + 2wx) = asin[-2(yz - wx)]
        // y = atan2(m02, m22) = atan2(2xz + 2wy, 1 - 2xx - 2yy) = atan2(xz + wy, 0.5 - xx - yy)
        // z = atan2(m10, m12) = atan2(2xy + 2wz, 1 - 2xx - 2zz) = atan2(xy + wz, 0.5 - xx - zz)
        // x = +- 90
        // y = atan2(-m20, m00) = atan2(-(2xz - 2wy), 1 - 2yy - 2zz) = atan2(-xz + wy, 0.5 - yy - zz)
        // z = 0
        public static Vector3 ToEuler(CustomQuaternion q)
        {
            var sinX = -2f * (q.y * q.z - q.w * q.x);
            float x, y, z;

            //如果 sinX == +-1 时 x = 90, 出现了万向节死锁的情况
            if (Mathf.Abs(sinX) > 0.9999f)
            {
                x = sinX * Mathf.PI / 2;
                y = Mathf.Atan2(-q.x * q.z + q.w * q.y, 0.5f - q.y * q.y - q.z * q.z);
                z = 0.0f;
            }
            else
            {
                x = Mathf.Asin(sinX);
                y = Mathf.Atan2(q.x * q.z + q.w * q.y, 0.5f - q.x * q.x - q.y * q.y);
                z = Mathf.Atan2(q.x * q.y + q.w * q.z, 0.5f - q.x * q.x - q.z * q.z);
            }

            return new Vector3(x * Mathf.Rad2Deg, y * Mathf.Rad2Deg, z * Mathf.Rad2Deg);
        }

        // 由 CustomMatrix4x4.AngleAxis 可知
        //           [ |   |   | ]   [nx * nx * (1 - ca) + ca,      nx * ny * (1 - ca) - nz * sa, nx * nz * (1 - ca) + ny * sa]
        // R(n, a) = [ p`, q`, r`] = [nx * ny * (1 - ca) + nz * sa, ny * ny * (1 - ca) + ca,      ny * nz * (1 - ca) - nx * sa]
        //           [ |   |   | ]   [nx * nz * (1 - ca) - ny * sa, ny * nz * (1 - ca) + nx * sa, nz * nz * (1 - ca) + ca     ]
        //
        // b = a / 2
        // w = cb, x = nx * sb, y = ny * sb, z = nz * sb
        // c2b = cb * cb - sb * sb
        //     = (1 - sb * sb) - sb * sb
        // ca  = 1 - 2 * sb * sb
        // nx * nx = 1 - ny * ny - nz * nz
        // m11 = nx * nx * (1 - ca) + ca
        //     = nx * nx - nx * nx * ca + ca
        //     = 1 - 1 + nx * nx - nx * nx * ca + ca
        //     = 1 - (1 - nx * nx + nx * nx * ca - ca)
        //     = 1 - (1 - ca - nx * nx * (1 - ca))
        //     = 1 - (1 - ca)(1 - nx * nx)
        //     = 1 - (1 - 1 + 2 * sb * sb)(1 - 1 + ny * ny + nz * nz)
        //     = 1 - 2 * sb * sb * (ny * ny + nz * nz)
        //     = 1 - 2 * ny * sb * ny * sb - 2 * nz * sb * nz * sb
        //     = 1 - 2 * y * y - 2 * z * z
        // 依次推导可得最终旋转矩阵
        //     [1 - 2 * y * y - 2 * z * z, 2 * x * y - 2 * w * z,     2 * x * z + 2 * w * y    ]
        // m = [2 * x * y + 2 * w * z,     1 - 2 * x * x - 2 * z * z, 2 * y * z - 2 * w * x    ]
        //     [2 * x * z - 2 * w * y,     2 * y * z + 2 * w * x,     1 - 2 * x * x - 2 * y * y]
        public static CustomMatrix4x4 ToMatrix(CustomQuaternion q)
        {
            var w = q.w;
            var x = q.x;
            var y = q.y;
            var z = q.z;
            var m = new CustomMatrix4x4();
            m.c0 = new CustomVector4(1 - 2 * y * y - 2 * z * z, 2 * x * y - 2 * w * z, 2 * x * z + 2 * w * y, 0);
            m.c1 = new CustomVector4(2 * x * y + 2 * w * z, 1 - 2 * x * x - 2 * z * z, 2 * y * z - 2 * w * x, 0);
            m.c2 = new CustomVector4(2 * x * z - 2 * w * y, 2 * y * z + 2 * w * x, 1 - 2 * x * x - 2 * y * y, 0);
            m.c3 = new CustomVector4(0, 0, 0, 1);
            return m;
        }

        public static CustomQuaternion operator *(CustomQuaternion a, CustomQuaternion b)
        {
            return Multiply(a, b);
        }

        // p` = q * p * q-1
        public static Vector3 operator *(CustomQuaternion q, Vector3 v)
        {
            var p = new CustomQuaternion(v.x, v.y, v.z, 0);
            var r = q * p * Inverse(q);
            return new Vector3(r.x, r.y, r.z);
        }
    }
}