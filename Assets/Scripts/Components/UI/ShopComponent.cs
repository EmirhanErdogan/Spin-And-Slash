using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using DG.Tweening;
using Emir;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum EWeaponType
{
    NONE,
    HAT,
    WEAPON,
}

public class ShopComponent : Emir.Singleton<ShopComponent>
{
    #region Serializable Fields

    [SerializeField] private TextMeshProUGUI AddButtonText;
    [SerializeField] private List<GridComponent> Grids = new List<GridComponent>();
    [SerializeField] private WeaponUIComponent DragableObj;
    [SerializeField] private List<WeaponUIComponent> Weapons = new List<WeaponUIComponent>();
    [SerializeField] private Image exclamation;

    #endregion

    private void Start()
    {
        DOVirtual.DelayedCall(0.25f, () =>
        {
            if (GameManager.Instance.GetCurreny() > 499)
            {
                SwitchExclamation(true);
            }
            else
            {
                SwitchExclamation(false);
            }
        });
    }

    #region Drag

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (Input.touchCount > 0)
        {
            if (LevelService.GetCachedLevel() == 1)
            {
                if (OnBoardingComponent.Instance.GetAnimator().GetBool(CommonTypes.ONBOARDING_ADD_BUTTON) ==
                    true) return;
            }

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (TouchManager.Instance.GetTouch().GetPointerObject() != null)
                {
                    if (TouchManager.Instance.GetTouch().GetPointerObject().CompareTag(CommonTypes.DRAGABLE_TAG))
                    {
                        UISelect();
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (DragableObj is not null)
                {
                    if (DragableObj.GetImage().raycastTarget is true)
                    {
                        DragableObj.ChangeUIMask(false);
                    }

                    DragObject(touch);
                    TrashControl();
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (TouchManager.Instance.GetTouch().GetPointerObject() is not null)
                {
                    if (TouchManager.Instance.GetTouch().GetPointerObject().CompareTag(CommonTypes.GRID_TAG))
                    {
                        BreakGrid();
                    }
                    else if (TouchManager.Instance.GetTouch().GetPointerObject().CompareTag(CommonTypes.DRAGABLE_TAG))
                    {
                        BreakDragable();
                    }
                    else if (TouchManager.Instance.GetTouch().GetPointerObject().CompareTag(CommonTypes.TRASH_TAG))
                    {
                        BreakTrash();
                    }
                    else
                    {
                        ReturnGrid();
                    }
                }
                else
                {
                    ReturnGrid();
                }

                TouchManager.Instance.GetTouch().SetPointerObject(null);
            }
        }
    }

    #endregion


    #region UI Select

    private void UISelect()
    {
        WeaponUIComponent TargetWeapon = GetWeapons()
            .FirstOrDefault(x => x.gameObject == TouchManager.Instance.GetTouch().GetPointerObject());

        TargetWeapon.GetGrid().SetIsEmpty(false);
        TargetWeapon.GetGrid().SetWeaponUI(null);

        DragableObj = TargetWeapon;
        DragableObj.GetRectTransform().SetParent(InterfaceManager.Instance.GetSWeaponUIRoot());
    }

    public void SwitchExclamation(bool value)
    {
        exclamation.enabled = value;
    }

    #endregion

    #region UI Move

    private void DragObject(Touch touch)
    {
        DragableObj.transform.position = touch.position;
        TouchManager.Instance.GetTouch().SetDeltaPos(Vector2.zero);
    }

    private void TrashControl()
    {
        if (TouchManager.Instance.GetTouch().GetPointerObject() != null)
        {
            if (TouchManager.Instance.GetTouch().GetPointerObject().CompareTag(CommonTypes.TRASH_TAG))
            {
                //çöp kutusu rengini değiştir
                InterfaceManager.Instance.GetTrashImage().color =
                    GameManager.Instance.GetGameSettings().UnselectedColor;
            }
            else
            {
                //çöp kutusu rengini eski haline getir
                InterfaceManager.Instance.GetTrashImage().color =
                    GameManager.Instance.GetGameSettings().SelectedColor;
            }
        }
    }

    #endregion

    #region UI Break

    private void BreakGrid()
    {
        GridComponent TargetGrid = GetGrids()
            .FirstOrDefault(x => x.gameObject == TouchManager.Instance.GetTouch().GetPointerObject());

        if (TargetGrid.GetIsMain() is true)
        {
            if (TargetGrid.GetWeaponType() == DragableObj.GetType())
            {
                TargetGrid.SetIsEmpty(true);
                TargetGrid.SetWeaponUI(DragableObj);
                DragableObj.SetGrid(TargetGrid);
                DragableObj.GetRectTransform().SetParent(TargetGrid.GetTransform());
                DragableObj.GetRectTransform().anchoredPosition = Vector2.zero + Vector2.down * 20;
                DragableObj.ChangeUIMask(true);
                DragableObj = null;
                SoundManager.Instance.Play(CommonTypes.SOUND_DRAG);
                HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                return;
            }
            else
            {
                //geri döndür
                ReturnGrid();
                return;
            }
        }

        TargetGrid.SetIsEmpty(true);
        TargetGrid.SetWeaponUI(DragableObj);
        DragableObj.SetGrid(TargetGrid);
        DragableObj.GetRectTransform().SetParent(TargetGrid.GetTransform());
        DragableObj.GetRectTransform().anchoredPosition = Vector2.zero;
        DragableObj.ChangeUIMask(true);
        DragableObj = null;
        SoundManager.Instance.Play(CommonTypes.SOUND_DRAG);
        HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
    }

    private void BreakDragable()
    {
        WeaponUIComponent TargetWeaponUI = GetWeapons()
            .FirstOrDefault(x => x.gameObject == TouchManager.Instance.GetTouch().GetPointerObject());
        if (TargetWeaponUI is not null && DragableObj is not null)
        {
            if (DragableObj.GetType() == TargetWeaponUI.GetType())
            {
                if (DragableObj.GetLevel() == TargetWeaponUI.GetLevel())
                {
                    if (DragableObj.GetLevel() == 9)
                    {
                        DragableObj.ChangeUIMask(true);
                        TargetWeaponUI.ChangeUIMask(true);
                        ReturnGrid();
                    }
                    else
                    {
                        if (LevelService.GetCachedLevel() == 1)
                        {
                            OnBoardingComponent.Instance.SwitchAnimation(CommonTypes.ONBOARDING_BACK);
                            GameManager.Instance.ChangeGameState(EGameState.STAND_BY);
                        }

                        DragableObj.ChangeUIMask(false);
                        DragableObj.Merge(false);
                        TargetWeaponUI.Merge(true);
                        TargetWeaponUI.ChangeUIMask(true);
                        DragableObj = null;
                        SoundManager.Instance.Play(CommonTypes.SOUND_MERGE);
                        HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                        return;
                    }
                }
                else
                {
                    ReturnGrid();
                }
            }
            else
            {
                ReturnGrid();
            }
        }
        else
        {
            ReturnGrid();
        }
    }

    private void BreakTrash()
    {
        if (DragableObj is not null)
        {
            int Counter = 0;
            foreach (var Weapon in GetWeapons())
            {
                if (Weapon.GetType() == DragableObj.GetType())
                {
                    Counter++;
                }
            }

            if (Counter <= 1)
            {
                ReturnGrid();
                return;
            }


            GetWeapons().Remove(DragableObj);
            Destroy(DragableObj.gameObject);
            DragableObj = null;
            SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
            HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
            InterfaceManager.Instance.GetTrashImage().color =
                GameManager.Instance.GetGameSettings().SelectedColor;
        }
    }

    private void ReturnGrid()
    {
        if (DragableObj is not null)
        {
            if (DragableObj.GetGrid() is null)
            {
                DragableObj.SetGrid(GetGrids()
                    .FirstOrDefault(x => x.GetIsEmpty() == false));
            }

            DragableObj.GetGrid().SetIsEmpty(true);
            DragableObj.GetGrid().SetWeaponUI(DragableObj);
            DragableObj.GetRectTransform().SetParent(DragableObj.GetGrid().GetTransform());
            DragableObj.ResetPos(Vector2.zero);
            DragableObj.ChangeUIMask(true);
            DragableObj = null;
        }
    }

    #endregion

    #region Button

    public void ShopButtonClick()
    {
        GameUtils.SwitchCanvasGroup(null, InterfaceManager.Instance.GetShopGroup());
        if (LevelService.GetCachedLevel() == 1)
        {
            if (OnBoardingComponent.Instance.GetAnimator().GetBool(CommonTypes.ONBOARDING_BACK) == true) return;
            OnBoardingComponent.Instance.SwitchAnimation(CommonTypes.ONBOARDING_ADD_BUTTON);
        }
    }

    public void CloseButtonClick()
    {
        if (GameManager.Instance.GetGameState() == EGameState.ONBOARDING) return;
        if (LevelService.GetCachedLevel() == 1)
        {
            OnBoardingComponent.Instance.NonSwitchAnimation(CommonTypes.ONBOARDING_ADD_BUTTON);
            OnBoardingComponent.Instance.NonSwitchAnimation(CommonTypes.ONBOARDING_MERGE);

            OnBoardingComponent.Instance.GetOnBoardingObj().SetActive(false);
            GameUtils.SwitchCanvasGroup(null, InterfaceManager.Instance.GetMenuCanvasGroup());
        }

        ResetSlots();
        LevelComponent.Instance.Save();
        GameManager.Instance.GetPlayerView().Generate();
        DOVirtual.DelayedCall(0.1f,
            () => { GameUtils.SwitchCanvasGroup(InterfaceManager.Instance.GetShopGroup(), null); });
        if (GameManager.Instance.GetCurreny() > 499)
        {
            SwitchExclamation(true);
        }
        else
        {
            SwitchExclamation(false);
        }
    }

    public void ResetSlots()
    {
        GridComponent weaponGrid =
            GetGrids().FirstOrDefault(x => x.GetIsMain() && x.GetWeaponType() == EWeaponType.WEAPON);
        int MaxWeaponLevel = 0;


        for (int i = 0; i < GetWeapons().Count; i++)
        {
            if (GetWeapons()[i].GetLevel() > MaxWeaponLevel)
            {
                if (GetWeapons()[i].GetType() == EWeaponType.WEAPON)
                {
                    MaxWeaponLevel = GetWeapons()[i].GetLevel();
                }
            }
        }

        int MaxHatLevel = 0;
        for (int i = 0; i < GetWeapons().Count; i++)
        {
            if (GetWeapons()[i].GetLevel() > MaxHatLevel)
            {
                if (GetWeapons()[i].GetType() == EWeaponType.HAT)
                {
                    MaxHatLevel = GetWeapons()[i].GetLevel();
                }
            }
        }

        GridComponent hatGrid = GetGrids().FirstOrDefault(x => x.GetIsMain() && x.GetWeaponType() == EWeaponType.HAT);

        if (weaponGrid.GetIsEmpty() is false)
        {
            WeaponUIComponent TargetWeapon = GetWeapons()
                .FirstOrDefault(x => x.GetType() == EWeaponType.WEAPON && x.GetLevel() == MaxWeaponLevel);
            TargetWeapon.GetGrid().SetIsEmpty(false);
            TargetWeapon.GetGrid().SetWeaponUI(null);
            TargetWeapon.SetGrid(null);

            weaponGrid.SetIsEmpty(true);
            weaponGrid.SetWeaponUI(TargetWeapon);

            TargetWeapon.SetGrid(weaponGrid);
            TargetWeapon.GetRectTransform().SetParent(weaponGrid.GetTransform());
            TargetWeapon.GetRectTransform().anchoredPosition = Vector2.zero;
            TargetWeapon.ChangeUIMask(true);
            DragableObj = null;
        }

        if (hatGrid.GetIsEmpty() is false)
        {
            WeaponUIComponent TargetWeapon = GetWeapons()
                .FirstOrDefault(x => x.GetType() == EWeaponType.HAT && x.GetLevel() == MaxHatLevel);
            TargetWeapon.GetGrid().SetIsEmpty(false);
            TargetWeapon.GetGrid().SetWeaponUI(null);
            TargetWeapon.SetGrid(null);

            hatGrid.SetIsEmpty(true);
            hatGrid.SetWeaponUI(TargetWeapon);

            TargetWeapon.SetGrid(hatGrid);
            TargetWeapon.GetRectTransform().SetParent(hatGrid.GetTransform());
            TargetWeapon.GetRectTransform().anchoredPosition = Vector2.zero;
            TargetWeapon.ChangeUIMask(true);
            DragableObj = null;
        }
    }

    public void AddButtonClick()
    {
        if (GameManager.Instance.GetCurreny() >= 500)
        {
            int randomValue = Random.Range(0, 2);
            WeaponUIComponent Weapon = null;
            if (LevelService.GetCachedLevel() == 1)
            {
                if (OnBoardingComponent.Instance.GetAnimator().GetBool(CommonTypes.ONBOARDING_BACK) == false)
                {
                    randomValue = 0;
                    OnBoardingComponent.Instance.SwitchAnimation(CommonTypes.ONBOARDING_MERGE);
                    OnBoardingComponent.Instance.NonSwitchAnimation(CommonTypes.ONBOARDING_ADD_BUTTON);
                }
            }

            if (randomValue == 0)
            {
                Weapon = Instantiate(GameManager.Instance.GetGameSettings().WeaponPrefab,
                    InterfaceManager.Instance.GetSWeaponUIRoot());
            }
            else
            {
                Weapon = Instantiate(GameManager.Instance.GetGameSettings().HelmetPrefab,
                    InterfaceManager.Instance.GetSWeaponUIRoot());
            }


            if (Weapon is not null)
            {
                GetWeapons().Add(Weapon);
                Weapon.SetLevel(GameManager.Instance.GetGameSettings().CurrentWeaponLevel);
                Weapon.Initialize();
                GridComponent TargetGridComp =
                    Grids.FirstOrDefault(x => x.GetIsEmpty() is false && x.GetIsMain() is false);
                if (TargetGridComp is null)
                {
                    GetWeapons().Remove(Weapon);
                    Destroy(Weapon.gameObject);
                    return;
                }

                Weapon.GetRectTransform().SetParent(TargetGridComp.GetTransform());
                Weapon.GetRectTransform().anchoredPosition = Vector2.zero;
                Weapon.SetGrid(TargetGridComp);
                TargetGridComp.SetWeaponUI(Weapon);
                TargetGridComp.SetIsEmpty(true);
                GameManager.Instance.SetCurrency(-500);
                InterfaceManager.Instance.OnPlayerCurrencyUpdated();
                SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
                HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
            }
        }
    }

    #endregion

    #region Getters

    public List<WeaponUIComponent> GetWeapons()
    {
        return Weapons;
    }

    public List<GridComponent> GetGrids()
    {
        return Grids;
    }

    #endregion
}