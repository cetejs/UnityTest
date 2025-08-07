using System;
using System.Collections.Generic;
using UnityEngine;

namespace Algorithm
{
    public class PathFindingSample : MonoBehaviour
    {
        [SerializeField]
        private PathFinding pathFinding;
        [SerializeField]
        private float moveSpeed = 5f;
        [SerializeField]
        private float angleSpeed = 10f;
        private List<Vector3> path = new List<Vector3>();
        private Vector3 nextPoint;
        private bool findNextPoint;
        private GameObject endPoint;
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                List<Vector3> results = pathFinding.FindPath(transform.position, hit.point);
                if (results != null)
                {
                    path.Clear();
                    path.AddRange(results);
                    findNextPoint = true;

                    if (endPoint == null)
                    {
                        endPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        endPoint.transform.localScale = Vector3.one * 0.5f;
                        endPoint.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    
                    endPoint.transform.position = hit.point;
                }
            }

            if (path.Count > 0 && findNextPoint)
            {
                nextPoint = path[0];
                path.RemoveAt(0);
                findNextPoint = false;
            }

            if (nextPoint != Vector3.zero)
            {
                Vector3 offset = nextPoint - transform.position;
                offset.y = 0;
                float speed = moveSpeed * Time.deltaTime;
                if (Vector3.SqrMagnitude(offset) <= speed * speed)
                {
                    transform.position = nextPoint;
                    nextPoint = Vector3.zero;
                    findNextPoint = true;
                }
                else
                {
                    Vector3 direction = offset.normalized;
                    transform.position += direction * speed;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), angleSpeed * Time.deltaTime);
                }
            }
        }
    } 
}
