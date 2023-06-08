using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Emir;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerView : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Slider _slider;
    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private ParticleSystem DeadParticle;
    [SerializeField] private HelmetController _helmetController;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private float speed;
    [SerializeField] private Transform SliderRoot;
    [SerializeField] private float Health;
    [SerializeField] private float CurrentHealth;

    #endregion

    #region Private Fields

    private bool ISDeath;
    private bool WeaponIsActive;
    private int ComboCount = 0;
    private float ComboTimer = 0;

    #endregion

    private void Start()
    {
        SliderLookCamera();
        SliderInitialize();
        CurrentHealth = Health;
        ComboTimer = Time.time + 2.5f;
        EventService.AddListener<HitEnemyEvent>(OnEnemyHit);
        DOVirtual.DelayedCall(0.25f, () => { Generate(); });
    }

    private void OnEnemyHit(HitEnemyEvent data)
    {
        IncreaseComboCount();
        ComboControl();
    }

    private void FixedUpdate()
    {
        Movement();
        ResetComboCount();
    }

    /// <summary>
    /// This function helps for movement.
    /// </summary>
    private void Movement()
    {
        if (GameManager.Instance.GetGameState() != EGameState.STARTED) return;
        if (ISDeath is true) return;
        GameSettings gameSettings = GameManager.Instance.GetGameSettings();

        m_rigidbody.velocity = new Vector3(InterfaceManager.Instance.GetJoystick().Horizontal * speed, 0,
            InterfaceManager.Instance.GetJoystick().Vertical * speed);


        if (InterfaceManager.Instance.GetJoystick().Horizontal != 0 ||
            InterfaceManager.Instance.GetJoystick().Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(m_rigidbody.velocity);
            SliderLookCamera();
            if (m_rigidbody.velocity.magnitude > gameSettings.PlayerVelocityMinSpeed)
            {
                m_animator.SetBool(CommonTypes.PLAYER_RUN_ANIM, true);
                //vurabilir olucak
                WeaponIsActive = true;
            }
            else
            {
                //vuramaz olucak
                WeaponIsActive = false;
            }
        }
        else
        {
            m_animator.SetBool(CommonTypes.PLAYER_RUN_ANIM, false);
            WeaponIsActive = false;
        }

        if (m_rigidbody.velocity.magnitude > gameSettings.PlayerVelocityMinSpeed)
        {
            WeaponIsActive = true;
        }
        else
        {
            WeaponIsActive = false;
        }
    }

    public void Generate()
    {
        WeaponGenerate();
        HelmetGenerate();
    }

    private void HelmetGenerate()
    {
        GridComponent CurrentGrid = ShopComponent.Instance.GetGrids()
            .FirstOrDefault(x => x.GetIsMain() is true && x.GetWeaponType() == EWeaponType.HAT);
        _helmetController.DisableMeshes();
        _helmetController.EnableTargetMeshes(CurrentGrid.GetWeaponUI().GetLevel());
        Health = _helmetController.GetHelmets()
            .FirstOrDefault(x => x.GetLevel() == CurrentGrid.GetWeaponUI().GetLevel()).GetHealth();
        CurrentHealth = Health;
        SliderInitialize();

        Debug.Log("helmet" + CurrentGrid.GetWeaponUI().GetLevel());
    }

    private void WeaponGenerate()
    {
        GridComponent CurrentGrid = ShopComponent.Instance.GetGrids()
            .FirstOrDefault(x => x.GetIsMain() is true && x.GetWeaponType() == EWeaponType.WEAPON);
        _weaponController.DisableMeshes();
        _weaponController.EnableTargetMeshes(CurrentGrid.GetWeaponUI().GetLevel());
        Debug.Log("weapon" + CurrentGrid.GetWeaponUI().GetLevel());
    }

    public async void Damage(float DamageAmount)
    {
        if (ISDeath is true) return;
        SliderRoot.gameObject.SetActive(true);
        CurrentHealth -= DamageAmount;
        if (CurrentHealth <= 0)
        {
            m_rigidbody.velocity = Vector3.zero;
            CurrentHealth = 0;
            ISDeath = true;
            m_animator.SetTrigger("Dead");
            mesh.material.DOColor(Color.gray, 0.25f).SetDelay(0.15f);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            GameManager.Instance.ChangeGameState(EGameState.LOSE);
            GameManager.Instance.OnGameStateChanged(EGameState.LOSE);
            InterfaceManager.Instance.OnGameStateChanged(EGameState.LOSE);
            m_rigidbody.velocity = Vector3.zero;
        }

        DOVirtual.DelayedCall(1.5f, () => { SliderRoot.gameObject.SetActive(false); });
        SliderUpdated();
    }

    public void DamageEffect()
    {
    }


    private void SliderLookCamera()
    {
        SliderRoot.LookAt(CameraManager.Instance.GetVirtualCamera().transform);
    }

    private void SliderInitialize()
    {
        _slider.minValue = 0;
        _slider.maxValue = Health;
        _slider.value = Health;
    }

    private void SliderUpdated()
    {
        _slider.DOValue(CurrentHealth, 0.5f);
    }

    public bool GetIsDeath()
    {
        return ISDeath;
    }

    public Animator GetAnimator()
    {
        return m_animator;
    }

    public Rigidbody GetRigidbody()
    {
        return m_rigidbody;
    }

    public bool GetWeaponIsActive()
    {
        return WeaponIsActive;
    }

    public int GetComboCount()
    {
        return ComboCount;
    }

    public void IncreaseComboCount()
    {
        ComboCount++;
    }

    public void ResetComboCount()
    {
        if (ComboCount < 1) return;
        if (Time.time > ComboTimer)
        {
            ComboCount = 0;
            InterfaceManager.Instance.EnableComboText(false);
        }
    }

    private void ComboControl()
    {
        ComboTimer = Time.time + 2.5f;
    }

    private void OnDestroy()
    {
        EventService.RemoveListener<HitEnemyEvent>(OnEnemyHit);
    }
}