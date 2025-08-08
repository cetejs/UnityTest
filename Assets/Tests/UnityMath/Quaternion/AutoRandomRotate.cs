using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class AutoRandomRotate : MonoBehaviour {
    public bool rotateX = true;
    public bool rotateY = true;
    public bool rotateZ = true;
    public float duration = 1;
    public UnityEvent onRotateFinish;
    
    private float lastTime;

    private void Start() {
        Rotate();
    }

    private void Update() {
        if (Time.time - lastTime > duration) {
            lastTime = Time.time;
            Rotate();
        }
    }

    private void Rotate() {
        var euler = new Vector3();
        if (rotateX) {
            euler.x = Random.Range(0, 360);
        }
            
        if (rotateY) {
            euler.y = Random.Range(0, 360);
        }
            
        if (rotateZ) {
            euler.z = Random.Range(0, 360);
        }

        transform.eulerAngles = euler;
        onRotateFinish?.Invoke();
    }
}