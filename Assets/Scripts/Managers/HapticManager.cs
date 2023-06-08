using System.Collections;
using System.Collections.Generic;
using Emir;
using UnityEngine;

public class HapticManager : Singleton<HapticManager>
{
    #region Private Fields

    private bool IsNoHaptic = true;

    #endregion


    public void PlayHaptic(HapticTypes Type)
    {
        if (!IsNoHaptic) return;

        if (Type == HapticTypes.LightImpact)
        {
            Taptic.Light();
        }
        else if (Type == HapticTypes.MediumImpact)
        {
            Taptic.Medium();
        }
    }

    public void ChangeHapticState(bool value)
    {
        IsNoHaptic = value;
    }
}