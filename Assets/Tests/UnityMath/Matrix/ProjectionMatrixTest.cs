using UnityEngine;

public class ProjectionMatrixTest : MonoBehaviour {
    [TextArea]
    public string describe = "支持（透视投影）和 （正交投影）的演示，可在编辑器非运行状态下，修改相机的投影类型。";
    
    private Vector3[] camVertexPosArray;
    private Transform[] camVertexGoArray;
    private Transform[] camClipGoArray;
    private Transform worldGo;
    private Transform clipGo;

    private Camera cam;
    private Transform camTrs;

    private void Start() {
        cam = Camera.main;
        camTrs = cam.transform;
        camVertexPosArray = GetCamVertexPos();
        camVertexGoArray = new Transform[camVertexPosArray.Length];
        camClipGoArray = new Transform[camVertexPosArray.Length];
        var vertexGos = new GameObject("VertexGos").transform;
        var clipGos = new GameObject("ClipGos").transform;
        for (int i = 0; i < camVertexPosArray.Length; i++) {
            var trs = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            camVertexGoArray[i] = trs;
            trs.position = camTrs.position + camTrs.rotation * camVertexPosArray[i];
            trs.SetParent(vertexGos);
        }
        
        for (int i = 0; i < camClipGoArray.Length; i++) {
            var trs = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            camClipGoArray[i] = trs;
            var pos = camTrs.position + camTrs.rotation * camVertexPosArray[i];
            trs.position = MatrixUtil.WorldToClipPoint(cam, pos);
            trs.localScale = Vector3.one * 0.5f;
            trs.SetParent(clipGos);
        }
        
        worldGo = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        worldGo.name = "WorldGo (Move This)";
        worldGo.GetComponent<MeshRenderer>().material.color = Color.red;
        clipGo = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        clipGo.name = "ClipGo (Watch this)";
        clipGo.localScale = Vector3.one * 0.5f;
        clipGo.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    private void Update() {
        for (int i = 0; i < camVertexPosArray.Length; i++) {
            var trs = camVertexGoArray[i];
            trs.position = camTrs.position + camTrs.rotation * camVertexPosArray[i];
        }
        
        for (int i = 0; i < camVertexPosArray.Length; i++) {
            var trs = camClipGoArray[i];
            var pos = camTrs.position + camTrs.rotation * camVertexPosArray[i];
            trs.position = MatrixUtil.WorldToClipPoint(cam, pos);
        }

        clipGo.position = MatrixUtil.WorldToClipPoint(cam, worldGo.position);
    }

    private Vector3[] GetCamVertexPos() {
        var vertexes = new Vector3[8];
        if (cam.orthographic) {
            var n = cam.nearClipPlane;
            var f = cam.farClipPlane;
            var t = cam.orthographicSize;
            var r = t * cam.aspect;

            vertexes[0] = new Vector3(-r, -t, n);
            vertexes[1] = new Vector3(-r, t, n);
            vertexes[2] = new Vector3(r, t, n);
            vertexes[3] = new Vector3(r, -t, n);
            vertexes[4] = new Vector3(-r, -t, f);
            vertexes[5] = new Vector3(-r, t, f);
            vertexes[6] = new Vector3(r, t, f);
            vertexes[7] = new Vector3(r, -t, f);
        } else {
            var n = cam.nearClipPlane;
            var f = cam.farClipPlane;
            var nt = Mathf.Tan(cam.fieldOfView / 2 * Mathf.Deg2Rad) * n;
            var nr = nt * cam.aspect;
            var ft = Mathf.Tan(cam.fieldOfView / 2 * Mathf.Deg2Rad) * f;
            var fr = ft * cam.aspect;

            vertexes[0] = new Vector3(-nr, -nt, n);
            vertexes[1] = new Vector3(-nr, nt, n);
            vertexes[2] = new Vector3(nr, nt, n);
            vertexes[3] = new Vector3(nr, -nt, n);
            vertexes[4] = new Vector3(-fr, -ft, f);
            vertexes[5] = new Vector3(-fr, ft, f);
            vertexes[6] = new Vector3(fr, ft, f);
            vertexes[7] = new Vector3(fr, -ft, f);
        }

        return vertexes;
    }

    private void OnDrawGizmos() {
        var cam = Camera.main;
        if (cam.orthographic) {
            Gizmos.color = Color.yellow;
            var nCenter = new Vector3(0, 0, cam.nearClipPlane / 2);
            var nSize = new Vector3(cam.orthographicSize * 2 * cam.aspect, cam.orthographicSize * 2, cam.nearClipPlane);
            Gizmos.DrawWireCube(nCenter, nSize);
            Gizmos.color = Color.red;
            var fCenter = new Vector3(0, 0, cam.nearClipPlane + (cam.farClipPlane - cam.nearClipPlane) / 2);
            var fSize = new Vector3(cam.orthographicSize * 2 * cam.aspect, cam.orthographicSize * 2, cam.farClipPlane - cam.nearClipPlane);
            Gizmos.DrawWireCube(fCenter, fSize);
        } else {
            // 相机的位置需要归零，否则 Gizmos.DrawFrustum 会受到影响，变得很奇怪
            Gizmos.color = Color.yellow;
            Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.nearClipPlane, 0, cam.aspect);
            Gizmos.color = Color.red;
            Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        }
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2);
    }
}