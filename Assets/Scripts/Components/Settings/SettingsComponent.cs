using System.Collections;
using System.Collections.Generic;
using Emir;
using UnityEngine;
using UnityEngine.UI;

public class SettingsComponent : Singleton<SettingsComponent>
{
    #region Serializable Fields

    [Header("Images")] [SerializeField] private Image m_soundOn;
    [SerializeField] private Image m_soundOff;
    [SerializeField] private Image m_vibrateOn;
    [SerializeField] private Image m_vibrateOff;
    [SerializeField] private RectTransform MaskObj;

    #endregion

    #region Private Fields

    private bool IsSound = true;
    private bool IsHaptic = true;

    #endregion

    // private void Start()
    // {
    //     ChangeValues();
    // }

    public void SoundButtonClick()
    {
        IsSound = !IsSound;
        //icon düzeltme
        if (IsSound is true)
        {
            m_soundOn.enabled = true;
            m_soundOff.enabled = false;
        }
        else if (IsSound is false)
        {
            m_soundOn.enabled = false;
            m_soundOff.enabled = true;
        }

        ChangeValues();
    }

    public void VibrationButtonClick()
    {
        IsHaptic = !IsHaptic;
        if (IsHaptic is true)
        {
            m_vibrateOn.enabled = true;
            // Taptic.tapticOn = true;
            m_vibrateOff.enabled = false;
        }
        else if (IsHaptic is false)
        {
            m_vibrateOn.enabled = false;
            // Taptic.tapticOn = false;
            m_vibrateOff.enabled = true;
        }

        ChangeValues();
    }

    public void ChangeValues()
    {
        Emir.SoundManager.Instance.SetMuteState(IsSound);
        HapticManager.Instance.ChangeHapticState(IsHaptic);
        Emir.SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
        HapticManager.Instance.PlayHaptic(HapticTypes.HeavyImpact);
    }

    public RectTransform GetMaskObj()
    {
        return MaskObj;
    }
}