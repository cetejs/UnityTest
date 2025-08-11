using UnityEngine;

namespace Tests
{
    public class FreeMovement : MonoBehaviour
    {
        public float speed = 1;

        private void Update()
        {
            var t = speed * Time.deltaTime;
            var position = transform.position;

            if (Input.GetKey(KeyCode.W))
            {
                position = Vector3.Lerp(position, position + transform.up, t);
            }

            if (Input.GetKey(KeyCode.S))
            {
                position = Vector3.Lerp(position, position - transform.up, t);
            }

            if (Input.GetKey(KeyCode.A))
            {
                position = Vector3.Lerp(position, position - transform.right, t);
            }

            if (Input.GetKey(KeyCode.D))
            {
                position = Vector3.Lerp(position, position + transform.right, t);
            }

            if (Input.GetKey(KeyCode.E))
            {
                position = Vector3.Lerp(position, position + transform.forward, t);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                position = Vector3.Lerp(position, position - transform.forward, t);
            }

            transform.position = position;
        }
    }
}