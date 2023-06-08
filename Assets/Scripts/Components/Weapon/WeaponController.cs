using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Emir;
using UnityEngine;

public class WeaponController : Singleton<WeaponController>
{
    #region Serializable Fields

    [SerializeField] private List<WeaponComponent> Weapons = new List<WeaponComponent>();
    [SerializeField] private Rigidbody _rigidbody;

    #endregion

    #region Privatee Fields

    private bool IsMove = false;

    #endregion

    private void Update()
    {
        WeaponIsMove();
    }

    private void WeaponIsMove()
    {
        if (_rigidbody.velocity.magnitude >= 9f)
        {
            IsMove = true;
        }
        else if (_rigidbody.velocity.magnitude < 9)
        {
            IsMove = false;
        }
    }

    public void EnableTargetMeshes(int TargetLevel)
    {
        WeaponComponent helmet = GetWeapon().FirstOrDefault(x => x.GetLevel() == TargetLevel);
        helmet.gameObject.SetActive(true);
    }

    public void DisableMeshes()
    {
        foreach (var Weapon in Weapons)
        {
            Weapon.gameObject.SetActive(false);
        }
    }

    #region Setters

    #endregion

    #region Getters

    public bool GetIsMove()
    {
        return IsMove;
    }

    public List<WeaponComponent> GetWeapon()
    {
        return Weapons;
    }

    #endregion
}