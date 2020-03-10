using UnityEngine;
using UnityEngine.Rendering;

public class EchoRenderer : MonoBehaviour
{
    private static readonly int sLightDirID = Shader.PropertyToID("_LightDir");
    private static readonly int sInteractorPosID = Shader.PropertyToID("_InteractorPos");
    private static readonly int sInteractorStrengthID = Shader.PropertyToID("_InteractorStrength");
    private static readonly int sInteractorRadiusID = Shader.PropertyToID("_InteractorRadius");

    public EchoData Echo;
    public Material Material;

    private Mesh mMesh;

    // ------------------------------------------------------------------

    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(Echo, "Echo data is null");

        InitMesh();
    }

    // --------------------------------------------------------------------

    private void InitMesh()
    {
        mMesh = new Mesh();
        mMesh.vertices = Echo.Vertices;
        mMesh.normals = Echo.Normals;
        int[] indices = new int[mMesh.vertices.Length];
        for (int i = 0; i < indices.Length; ++i)
            indices[i] = i;
        mMesh.SetIndices(indices, MeshTopology.Points, 0);
    }

    // ------------------------------------------------------------------

    private void OnEnable()
    {
        EchoManager.Instance.Register(this);
    }

    // ------------------------------------------------------------------

    public void AddToCommandBuffer(CommandBuffer buffer)
    {
        UpdateMaterial();
        buffer.DrawMesh(mMesh, transform.localToWorldMatrix, Material);
    }

    // ------------------------------------------------------------------

    private void UpdateMaterial()
    {
        Material.SetPass(0);
        Material.SetVector(sLightDirID, Echo.LightDirection);

        // In a real world implementation this should probably be a buffer of interactors
        EchoInteractor interactor = EchoManager.Instance.Interactor;
        if (interactor)
        {
            Material.SetVector(sInteractorPosID, interactor.transform.position);
            Material.SetFloat(sInteractorStrengthID, interactor.Strength);
            Material.SetFloat(sInteractorRadiusID, interactor.Radius);
        }
    }

    // ------------------------------------------------------------------

    public void OnDisable()
    {
        EchoManager.Instance.Unregister(this);
    }

    // --------------------------------------------------------------------

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        InitMesh();
        UpdateMaterial();
        Graphics.DrawMeshNow(mMesh, transform.localToWorldMatrix);
    }

#endif
}