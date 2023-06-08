using System;
using System.Collections;
using System.Collections.Generic;
using Emir;
using JetBrains.Annotations;
using UnityEngine;

public class OnBoardingComponent : Singleton<OnBoardingComponent>
{
    #region Serializable Fields

    [SerializeField] private Animator m_animator;
    [SerializeField] private GameObject m_onboardingobj;

    #endregion

    private void Start()
    {
        Debug.Log(LevelService.GetCachedLevel() + "şuan oynanan level");
    }

    public Animator GetAnimator()
    {
        return m_animator;
    }

    public GameObject GetOnBoardingObj()
    {
        return m_onboardingobj;
    }

    public void SwitchAnimation(string key)
    {
        m_animator.SetBool(key, true);
    }
    public void NonSwitchAnimation(string key)
    {
        m_animator.SetBool(key, false);
    }
}