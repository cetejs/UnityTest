using System;
using UnityEngine;

namespace Tests
{
    public class Collision2DTest : MonoBehaviour
    {
        public CustomCollider2D player;
        public float speed = 1;
        private Vector3 lastPos;
        
        private CustomCollider2D[] cs;

        private void Awake()
        {
            cs = FindObjectsOfType<CustomCollider2D>();
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector2(h, v);
            lastPos = player.transform.position;
            player.transform.Translate(input * speed * Time.deltaTime);

            for (int i = 0; i < cs.Length; i++)
            {
                cs[i].IsTrigger = false;
            }
            
            for (int i = 0; i < cs.Length; i++)
            {
                if (cs[i] == player)
                {
                    continue;
                }

                if (Physics2DUtils.IsIntersect(player, cs[i]))
                {
                    player.IsTrigger = cs[i].IsTrigger = true;
                    player.HitInfo = new HitInfo2D()
                    {
                        other = cs[i]
                    };

                    cs[i].HitInfo = new HitInfo2D()
                    {
                        other = player
                    };
                    
                    break;
                }
            }

            if (player.IsTrigger)
            {
                player.transform.position = lastPos;
            }
        }
    }
}