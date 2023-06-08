using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class LevelComponent : Emir.Singleton<LevelComponent>
{
    #region Serializable Fields

    [SerializeField] private List<BulletComponent> Bullets = new List<BulletComponent>();

    [SerializeField] private List<EnemyComponent> Enemys = new List<EnemyComponent>();
    [SerializeField] private List<EnemyFireComponent> FireEnemys = new List<EnemyFireComponent>();
    [SerializeField] private Transform BulletPool;
    [SerializeField] private bool IsWeaponUpgradeLevel;
    [SerializeField] private float LevelCurrency;

    #endregion

    #region Private Fields

    private GameSettings _gameSettings => GameManager.Instance.GetGameSettings();
    private bool IsLevelComplete = false;

    #endregion

    private void Start()
    {
        CreateBullet();
        InventoryInitialize();
    }

    private void InventoryInitialize()
    {
        if (PlayerPrefs.GetFloat(CommonTypes.IS_START_DATA_KEY) == 1)
        {
            Load();
        }
    }

    public async void CreateBullet()
    {
        int Counter = 0;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.03f));
            BulletComponent Bullet = Instantiate(_gameSettings.BulletPrefab);
            Bullet.transform.SetParent(BulletPool);
            Bullet.transform.localPosition = Vector3.zero;
            GetBullets().Add(Bullet);
            Bullet.gameObject.SetActive(false);
            Counter++;
            if (Counter >= 100) break;
        }
    }

    public async UniTask EnemyCountControl(float delay = 0.25f)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        foreach (var enemy in GetEnemys())
        {
            if (enemy == null)
            {
                GetEnemys().Remove(enemy);
            }
        }

        foreach (var fireEnemy in GetFireEnemys())
        {
            if (fireEnemy == null)
            {
                GetFireEnemys().Remove(fireEnemy);
            }
        }

        if (GetEnemys().Count < 1 && GetFireEnemys().Count < 1)
        {
            if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
            if (IsLevelComplete == true) return;
            IsLevelComplete = true;
            Debug.Log("Oyunu Kazandın");
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            SliderComponent.Instance.UpdateSliderLevel();
            GameManager.Instance.ChangeGameState(EGameState.WIN);
            GameManager.Instance.OnGameStateChanged(EGameState.WIN);
            InterfaceManager.Instance.OnGameStateChanged(EGameState.WIN);
            GameManager.Instance.GetPlayerView().GetRigidbody().velocity = Vector3.zero;
            GameManager.Instance.GetPlayerView().GetAnimator().SetBool(CommonTypes.PLAYER_RUN_ANIM, false);
            if (IsWeaponUpgradeLevel)
            {
                GameManager.Instance.GetGameSettings().CurrentWeaponLevel++;
                PlayerPrefs.SetInt(CommonTypes.WEAPON_LEVEL_DATA,
                    PlayerPrefs.GetInt(CommonTypes.WEAPON_LEVEL_DATA) + 1);
            }

            InterfaceManager.Instance.OnPlayerCurrencyUpdated();
        }
    }

    #region Getters

    public float GetLevelCurrency()
    {
        return LevelCurrency;
    }

    public List<EnemyComponent> GetEnemys()
    {
        return Enemys;
    }

    public List<EnemyFireComponent> GetFireEnemys()
    {
        return FireEnemys;
    }


    public List<BulletComponent> GetBullets()
    {
        return Bullets;
    }

    public BulletComponent GetBullet()
    {
        BulletComponent Bullet = GetBullets().First();
        GetBullets().Remove(Bullet);
        Bullet.gameObject.SetActive(true);
        Bullet.SetIsActive(true);
        return Bullet;
    }

    #endregion

    #region SaveSystem

    public void Save()
    {
        SaveSlotDatas();
    }

    private void Load()
    {
        LoadSlotList();
        GameManager.Instance.GetGameSettings().CurrentWeaponLevel = PlayerPrefs.GetInt(CommonTypes.WEAPON_LEVEL_DATA);
    }

    private async void LoadSlotList()
    {
        Debug.Log($"Loading Slot List");
        var storedWeaponJson = PlayerPrefs.GetString(CommonTypes.WEAPON_DATA_KEY);
        var existingWeaponDatas = string.IsNullOrEmpty(storedWeaponJson)
            ? Array.Empty<ExistingWeaponData>()
            : JsonConvert.DeserializeObject<ExistingWeaponData[]>(storedWeaponJson);

        if (existingWeaponDatas.Length == 0) return;

        ClearWeapons();

        foreach (var existingWeaponData in existingWeaponDatas)
        {
            //ID : COUNTER

            WeaponUIComponent Weapon = null;
            if (existingWeaponData.WeaponType == EWeaponType.HAT)
            {
                //hat olacak
                Weapon = Instantiate(GameManager.Instance.GetGameSettings().HelmetPrefab,
                    InterfaceManager.Instance.GetSWeaponUIRoot());
            }
            else
            {
                //weapon olacak
                Weapon = Instantiate(GameManager.Instance.GetGameSettings().WeaponPrefab,
                    InterfaceManager.Instance.GetSWeaponUIRoot());
            }

            ShopComponent.Instance.GetWeapons().Add(Weapon);
            GridComponent TargetGridComp = ShopComponent.Instance.GetGrids()
                .FirstOrDefault(x => x.GetIsEmpty() == false && x.GetIsMain() == false);

            if (TargetGridComp is null)
            {
                ShopComponent.Instance.GetWeapons().Remove(Weapon);
                Destroy(Weapon.gameObject);
                return;
            }

            Weapon.GetRectTransform().SetParent(TargetGridComp.GetTransform());
            Weapon.GetRectTransform().anchoredPosition = Vector2.zero;
            Weapon.SetLevel(existingWeaponData.Id);
            Weapon.SetGrid(TargetGridComp);
            Weapon.DisableAllWeapons();
            Weapon.EnableTargetWeapon(existingWeaponData.Id);
            TargetGridComp.SetWeaponUI(Weapon);
            TargetGridComp.SetIsEmpty(true);
        }

        ShopComponent.Instance.ResetSlots();
    }

    private void ClearWeapons()
    {
        foreach (var weapon in ShopComponent.Instance.GetWeapons())
        {
            weapon.GetGrid().SetIsEmpty(false);
            weapon.SetGrid(null);
            Destroy(weapon.gameObject);
        }

        ShopComponent.Instance.GetWeapons().Clear();
    }

    private void SaveSlotDatas()
    {
        Debug.Log($"Saving Weapon List");
        var myList = CreateExistingWeaponData();
        var json = JsonConvert.SerializeObject(myList);
        PlayerPrefs.SetString(CommonTypes.WEAPON_DATA_KEY, json);
    }

    private ExistingWeaponData[] CreateExistingWeaponData()
    {
        Debug.Log($"Creating Existing Slot Data");

        var newStoredSlots = new List<ExistingWeaponData>();
        foreach (var Weapon in ShopComponent.Instance.GetWeapons())
        {
            newStoredSlots.Add(new ExistingWeaponData()
            {
                Id = Weapon.GetLevel(),
                WeaponType = Weapon.GetType()
            });
        }

        return newStoredSlots.ToArray();
    }


    // private void LoadSlotUIList()
    // {
    //     Debug.Log($"Loading Slot UI List");
    //     var storedSlotUIJson = PlayerPrefs.GetString(CommonTypes.SLOT_UI_DATA_KEY);
    //     var existingSlotUIDatas = string.IsNullOrEmpty(storedSlotUIJson)
    //         ? Array.Empty<ExistingSlotUIData>()
    //         : JsonConvert.DeserializeObject<ExistingSlotUIData[]>(storedSlotUIJson);
    //
    //     if (existingSlotUIDatas.Length == 0) return;
    //     foreach (var existingSlotUIData in existingSlotUIDatas)
    //     {
    //         //ID : COUNTER
    //         SlotUIComponent SlotUI = Instantiate(GameManager.Instance.GetGameSettings().SlotUIPrefab,
    //             InterfaceManager.Instance.GetSlotUIRoot());
    //         LevelComponent.Instance.GetSlotUIs().Add(SlotUI);
    //         GridComponent TargetGridComp =
    //             GridUIComponent.Instance.GetGrids().FirstOrDefault(x => x.GetIsEmpty() is false);
    //         if (TargetGridComp is null)
    //         {
    //             LevelComponent.Instance.GetSlotUIs().Remove(SlotUI);
    //             Destroy(SlotUI.gameObject);
    //             return;
    //         }
    //
    //         SlotUI.GetRectTransform().SetParent(TargetGridComp.GetTransform());
    //         SlotUI.GetRectTransform().anchoredPosition = Vector2.zero;
    //         SlotUI.SetGridComp(TargetGridComp);
    //         TargetGridComp.SetSlotUI(SlotUI);
    //         TargetGridComp.SetIsEmpty(true);
    //     }
    // }
    //
    // private void SaveSlotUIDatas()
    // {
    //     Debug.Log($"Saving Slot uı List");
    //     var myList = CreateExistingSlotUIData();
    //     var json = JsonConvert.SerializeObject(myList);
    //     PlayerPrefs.SetString(CommonTypes.SLOT_UI_DATA_KEY, json);
    // }
    //
    // private ExistingSlotUIData[] CreateExistingSlotUIData()
    // {
    //     Debug.Log($"Creating Existing Slot UI Data");
    //
    //     var newStoredSlotUI = new List<ExistingSlotUIData>();
    //     foreach (var SlotUI in GetSlotUIs())
    //     {
    //         newStoredSlotUI.Add(new ExistingSlotUIData()
    //         {
    //             Id = SlotUI.GetCounter()
    //         });
    //     }
    //
    //     return newStoredSlotUI.ToArray();
    // }

    #endregion

    public bool GetIsUpgradeLevel()
    {
        return IsWeaponUpgradeLevel;
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}