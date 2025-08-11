using UnityEngine;

namespace Tests
{
    [RequireComponent(typeof(CustomCollider2D))]
    public class Collider2DMove : MonoBehaviour
    {
        public float speed = 3;
        public float ignoreHitTime = 0.3f;
        private CustomCollider2D col;
        private Vector3 vel;

        private float timer;

        private void Awake()
        {
            col = GetComponent<CustomCollider2D>();
        }

        private void Start()
        {
            vel = RandomVelocity(speed);
        }

        private void FixedUpdate()
        {
            transform.Translate(vel * Time.fixedDeltaTime);
            timer -= Time.deltaTime;
            if (col.IsTrigger && timer <= 0)
            {
                if (col.HitInfo.other.name == "Wall")
                {
                    vel = -vel;
                }
                else
                {
                    vel = (col.Center - col.HitInfo.other.Center).normalized * speed;
                }

                timer = ignoreHitTime;
            }
        }

        private Vector2 RandomVelocity(float speed)
        {
            return Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right * speed;
        }
    }
}