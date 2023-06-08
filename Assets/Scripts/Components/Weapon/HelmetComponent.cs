using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HelmetComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private int Level;
    [SerializeField] private float HealthValue;

    #endregion


    #region Setters

    public void SetLevel(int value)
    {
        Level = value;
    }

    #endregion


    #region Getters

    public float GetHealth()
    {
        return HealthValue;
    }

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