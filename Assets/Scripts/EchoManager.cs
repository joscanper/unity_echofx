using System.Collections.Generic;
using UnityEngine.Rendering;

public class EchoManager
{
    #region Singleton

    private static EchoManager _instance;

    public static EchoManager Instance
    {
        get
        {
            return _instance = _instance ?? new EchoManager();
        }
    }

    #endregion Singleton

    private List<EchoRenderer> mEchoEffects = new List<EchoRenderer>();
    public EchoInteractor Interactor { get; set; }

    public void Register(EchoRenderer echoEffect)
    {
        mEchoEffects.Add(echoEffect);
    }

    public void Unregister(EchoRenderer echoEffect)
    {
        mEchoEffects.Remove(echoEffect);
    }

    public void PopulateCommandBuffer(CommandBuffer commandBuffer)
    {
        for (int i = 0, len = mEchoEffects.Count; i < len; i++)
        {
            var effect = mEchoEffects[i];
            if (effect == null || !effect.isActiveAndEnabled)
            {
                mEchoEffects.Remove(effect);
                --i;
            }

            effect.AddToCommandBuffer(commandBuffer);
        }
    }
}