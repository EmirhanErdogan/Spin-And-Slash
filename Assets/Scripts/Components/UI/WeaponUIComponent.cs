using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public struct ExistingWeaponData
{
    public int Id;
    public EWeaponType WeaponType;
}

public class WeaponUIComponent : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private EWeaponType typeWeapon;
    [SerializeField] private bool IsEmpty;
    [SerializeField] private int Level;
    [SerializeField] private Image Sprite;
    [SerializeField] private GridComponent CurrentGrid;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private List<GameObject> Weapons = new List<GameObject>();

    #endregion

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        DisableAllWeapons();
        EnableTargetWeapon(GetLevel());
        ResetText();
    }

    public void ResetText()
    {
        if (GetLevel() == 9)
        {
            levelText.text = "Max";
        }
        else
        {
            levelText.text = (GetLevel() + 1).ToString();
        }
    }

    public void Merge(bool value)
    {
        if (value is true)
        {
            int TargetLevel = GetLevel() + 1;
            SetLevel(TargetLevel);
            Initialize();
        }
        else
        {
            ShopComponent.Instance.GetWeapons().Remove(this);
            if (GetGrid() is not null)
            {
                GetGrid().SetIsEmpty(false);
                GetGrid().SetWeaponUI(null);
                GetGrid().SetIsEmpty(false);
            }

            Destroy(this.gameObject);
        }
    }

    public void DisableAllWeapons()
    {
        foreach (var weapon in Weapons)
        {
            weapon.SetActive(false);
        }
    }

    public void EnableTargetWeapon(int TargetLevelWeapon)
    {
        Weapons[TargetLevelWeapon].SetActive(true);
    }

    #region Getters

    public RectTransform GetRectTransform()
    {
        return _transform;
    }

    public bool GetIsEmpty()
    {
        return IsEmpty;
    }

    public GridComponent GetGrid()
    {
        return CurrentGrid;
    }

    public List<GameObject> GetWeapons()
    {
        return Weapons;
    }

    public int GetLevel()
    {
        return Level;
    }

    public EWeaponType GetType()
    {
        return typeWeapon;
    }

    public Image GetImage()
    {
        return Sprite;
    }

    #endregion

    #region Setters

    public void SetIsEmpty(bool value)
    {
        IsEmpty = value;
    }

    public void SetGrid(GridComponent TargetGrid)
    {
        CurrentGrid = TargetGrid;
    }

    public void SetLevel(int value)
    {
        Level = value;
    }


    public void ChangeUIMask(bool value)
    {
        GetImage().raycastTarget = value;
        GetImage().maskable = value;
    }

    public void ResetPos(Vector2 TargetPos)
    {
        GetRectTransform().anchoredPosition = TargetPos;
    }

    #endregion
}