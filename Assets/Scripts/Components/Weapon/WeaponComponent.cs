using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private int Level;

    #endregion


    #region Setters

    public void SetLevel(int value)
    {
        Level = value;
    }

    #endregion


    #region Getters

    public MeshRenderer GetMesh()
    {
        return _meshRenderer;
    }

    public int GetLevel()
    {
        return Level;
    }

    #endregion
}