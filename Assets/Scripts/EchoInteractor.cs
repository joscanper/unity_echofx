using UnityEngine;

public class EchoInteractor : MonoBehaviour
{
    public float Radius;
    public float Strength;

    private void OnEnable()
    {
        EchoManager.Instance.Interactor = this;
    }
}