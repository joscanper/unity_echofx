using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using System.Collections.Generic;

public class EchoCreationToolWindow : EditorWindow
{
    private static readonly int sLightDirID = Shader.PropertyToID("_LightDir");

    public Material Material;
    public EchoData Echo;
    public float Radius = 0.25f;
    public float Density = 100;

    private List<Vector3> mVertices = new List<Vector3>();
    private List<Vector3> mNormals = new List<Vector3>();
    private List<int> mIndices = new List<int>();
    private Mesh mMesh;

    [MenuItem("Tools/ECHO Creation")]
    private static void Init()
    {
        EchoCreationToolWindow window = EditorWindow.GetWindowWithRect<EchoCreationToolWindow>(new Rect(100, 100, 300, 300), false, "Echo Creation Tool");
        window.Show();
    }

    // --------------------------------------------------------------------

    private void OnGUI()
    {
        GUILayout.Label("Vertices: " + mVertices.Count);
        Material = EditorGUILayout.ObjectField(Material, typeof(Material), false) as Material;
        Light = EditorGUILayout.ObjectField(Light, typeof(Light), true) as Light;
        Echo = EditorGUILayout.ObjectField(Echo, typeof(EchoData), false) as EchoData;
        if (Echo)
        {
            Radius = EditorGUILayout.FloatField("Radius", Radius);
            Density = EditorGUILayout.FloatField("Density", Density);

            if (GUILayout.Button("Cast"))
            {
                GeneratePoints();
            }

            if (GUILayout.Button("Clear"))
            {
                if (EditorUtility.DisplayDialog("Delete all points", "Are you sure you want to remove all points?", "Yes", "No"))
                {
                    ClearAll();
                }
            }

            if (GUILayout.Button("Load"))
            {
                ClearAll();

                for (int i = 0; i < Echo.Vertices.Length; ++i)
                {
                    mVertices.Add(Echo.Vertices[i]);
                    mNormals.Add(Echo.Normals[i]);
                    mIndices.Add(mVertices.Count - 1);
                }
            }

            if (GUILayout.Button("Save"))
            {
                Echo.Vertices = mVertices.ToArray();
                Echo.Normals = mNormals.ToArray();
                EditorUtility.SetDirty(Echo);
            }
        }
    }

    // --------------------------------------------------------------------

    private void SaveDataToMesh(Mesh mesh)
    {
        mesh.SetVertices(mVertices);
        mesh.SetNormals(mNormals);
        mesh.SetIndices(mIndices.ToArray(), MeshTopology.Points, 0);
    }

    // --------------------------------------------------------------------

    private void ClearAll()
    {
        mVertices.Clear();
        mNormals.Clear();
        mIndices.Clear();
    }

    // --------------------------------------------------------------------

    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    // --------------------------------------------------------------------

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    // --------------------------------------------------------------------

    private void OnSceneGUI(SceneView sceneView)
    {
        GetRaycastedPoint(out Vector3 pos, out Vector3 normal, out Transform obj);
        Handles.color = Color.white;
        Handles.DrawWireDisc(pos, normal, Radius);
        Handles.color = Color.red;
        Handles.DrawWireDisc(pos, normal, Density / 100f);
        Handles.color = Color.cyan;
        Handles.DrawLine(pos, pos + normal);

        if (mVertices.Count > 0)
        {
            Material.SetPass(0);

            mMesh = new Mesh();
            SaveDataToMesh(mMesh);
            Graphics.DrawMeshNow(mMesh, Vector3.zero, Quaternion.identity);
        }
    }

    // --------------------------------------------------------------------

    private void GeneratePoints()
    {
        Camera cam = SceneView.lastActiveSceneView.camera;

        GetRaycastedPoint(out Vector3 hitPos, out Vector3 hitNormal, out Transform hitObj);

        Vector3 rel = Vector3.Dot(Vector3.up, hitNormal) > 0.75 ? Vector3.forward : Vector3.up;
        Vector3 right = Vector3.Cross(rel, hitNormal).normalized;
        Vector3 up = Vector3.Cross(right, hitNormal).normalized;

        Matrix4x4 tangentSpace = new Matrix4x4(right, up, hitNormal, Vector4.zero);

        for (int point = 0; point < Density; ++point)
        {
            Vector3 globalOffset = Random.insideUnitCircle * Radius;
            Vector3 localOffset = tangentSpace * globalOffset;
            Vector3 from = hitPos + hitNormal * 0.1f + localOffset;
            Vector3 to = hitPos - hitNormal + localOffset;
            if (GetRaycastedPoint(from, to, out Vector3 pointHitPos, out Vector3 pointHitNormal, out Transform pointHitObj, hitObj))
            {
                mVertices.Add(pointHitPos);
                mNormals.Add(pointHitNormal);
                mIndices.Add(mVertices.Count - 1);
            }
        }
    }

    // --------------------------------------------------------------------

    private bool GetRaycastedPoint(out Vector3 hitPos, out Vector3 hitNormal, out Transform hitObj)
    {
        Camera cam = SceneView.lastActiveSceneView.camera;
        return GetRaycastedPoint(cam.transform.position, cam.transform.position + cam.transform.forward * 100f, out hitPos, out hitNormal, out hitObj);
    }

    // --------------------------------------------------------------------

    private bool GetRaycastedPoint(Vector3 from, Vector3 to, out Vector3 hitPos, out Vector3 hitNormal, out Transform hitObj, Transform filterTransform = null)
    {
        hitObj = null;
        hitNormal = Vector3.up;
        hitPos = to;

        Vector3 dir = (to - from).normalized;

        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            Scene scene = prefabStage.scene;
            PhysicsScene physicScene = scene.GetPhysicsScene();
            if (physicScene.Raycast(from, dir, out RaycastHit hit, 100, ~0, QueryTriggerInteraction.Ignore))
            {
                hitPos = hit.point;
                hitObj = hit.transform;
                hitNormal = hit.normal;

                return (!filterTransform || filterTransform && hitObj == filterTransform);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Physics.Raycast(from, dir, out RaycastHit hit, 100, ~0, QueryTriggerInteraction.Ignore))
            {
                hitPos = hit.point;
                hitObj = hit.transform;
                hitNormal = hit.normal;

                return (!filterTransform || filterTransform && hitObj == filterTransform);
            }
            else
            {
                return false;
            }
        }
    }
}