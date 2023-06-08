using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private bool IsEmpty;
    [SerializeField] private WeaponUIComponent Weapon;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private bool IsMainGrid;
    [SerializeField] private EWeaponType type;

    #endregion


    #region Setters

    public void SetIsEmpty(bool value)
    {
        IsEmpty = value;
    }

    public void SetWeaponUI(WeaponUIComponent TargetWeaponUI)
    {
        Weapon = TargetWeaponUI;
    }

    #endregion


    #region Getters

    public bool GetIsEmpty()
    {
        return IsEmpty;
    }

    public WeaponUIComponent GetWeaponUI()
    {
        return Weapon;
    }

    public RectTransform GetTransform()
    {
        return _transform;
    }

    public EWeaponType GetWeaponType()
    {
        return type;
    }

    public bool GetIsMain()
    {
        return IsMainGrid;
    }
    #endregion
}