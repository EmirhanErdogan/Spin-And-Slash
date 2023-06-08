using TMPro;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Emir
{
    public class InterfaceManager : Singleton<InterfaceManager>
    {
        #region Serializable Fields

        [Header("Transforms")] [SerializeField]
        private RectTransform m_canvas;

        [SerializeField] private RectTransform m_Touchcanvas;
        [SerializeField] private RectTransform m_currencySlot;
        [SerializeField] private DynamicJoystick Joystick;

        [Header("Panels")]
        // [SerializeField] private UIWinPanel m_winPanel;
        // [SerializeField] private UILosePanel m_losePanel;
        [Header("Texts")]
        [SerializeField]
        private TextMeshProUGUI m_currencyText;

        [SerializeField] private TextMeshProUGUI LoseEnemyCountText;
        [SerializeField] private TextMeshProUGUI LoseCurrencyCountText;
        [SerializeField] private TextMeshProUGUI WinEnemyCountText;
        [SerializeField] private TextMeshProUGUI WinCurrencyCountText;

        [SerializeField] private TextMeshProUGUI m_levelText;
        [SerializeField] private TextMeshProUGUI m_comboText;
        [SerializeField] private GameObject m_comboObj;

        [Header("Canvas Groups")] [SerializeField]
        private CanvasGroup m_menuCanvasGroup;

        [SerializeField] private CanvasGroup m_gameCanvasGroup;
        [SerializeField] private CanvasGroup m_commonCanvasGroup;
        [SerializeField] private CanvasGroup SettingsGroup;
        [SerializeField] private CanvasGroup MenuGroup;
        [SerializeField] private CanvasGroup WinGroup;
        [SerializeField] private CanvasGroup LoseGroup;
        [SerializeField] private CanvasGroup JoystickGroup;
        [SerializeField] private CanvasGroup ShopGroup;
        [SerializeField] private GameObject SettingsIcon;
        [SerializeField] private GameObject ShopIcon;
        [SerializeField] private Image TrashIcon;

        [Header("Prefabs")] [SerializeField] private RectTransform m_currencyPrefab;

        [SerializeField] private Transform WeaponUIRoot;
        [SerializeField] private Transform m_floatingSlot;

        #endregion

        #region Private Fields

        private int CurrentCurrency;
        private bool SettingsBool = true;
        private int EnemyCount = 0;

        #endregion

        /// <summary>
        /// Awake.
        /// </summary>
        private void Start()
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                CurrentCurrency = GameManager.Instance.GetCurreny();
                OnGameStateChanged(GameManager.Instance.GetGameState());
                OnPlayerCurrencyUpdated();
                EnemyCount = LevelComponent.Instance.GetEnemys().Count + LevelComponent.Instance.GetFireEnemys().Count;
            });
            EventService.AddListener<HitEnemyEvent>(OnEnemyHit);
        }

        private void OnEnemyHit(HitEnemyEvent data)
        {
            PrintComboText(GameManager.Instance.GetPlayerView().GetComboCount());
            if (m_comboObj.activeSelf is false)
            {
                EnableComboText(true);
            }
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="worldPosition"></param>
        public void FlyCurrencyFromWorld(Vector3 worldPosition)
        {
            Camera targetCamera = CameraManager.Instance.GetCamera();
            Vector3 screenPosition = GameUtils.WorldToCanvasPosition(m_canvas, targetCamera, worldPosition);
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.anchoredPosition = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.5F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function helper for fly currency animation to target currency icon.
        /// </summary>
        /// <param name="screenPosition"></param>
        public void FlyCurrencyFromScreen(Vector3 screenPosition)
        {
            Vector3 targetScreenPosition = m_canvas.InverseTransformPoint(m_currencySlot.position);

            RectTransform createdCurrency = Instantiate(m_currencyPrefab, m_canvas);
            createdCurrency.position = screenPosition;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdCurrency.transform.DOLocalMove(targetScreenPosition, 0.5F));

            sequence.OnComplete(() => { Destroy(createdCurrency.gameObject); });

            sequence.Play();
        }

        /// <summary>
        /// This function called when game state changed.
        /// </summary>
        /// <param name="e"></param>
        public void OnGameStateChanged(EGameState GameState)
        {
            switch (GameState)
            {
                case EGameState.STAND_BY:

                    SettingsIcon.SetActive(true);
                    ShopIcon.SetActive(true);
                    LevelTextUpdated();
                    if (LevelService.GetCachedLevel() == 0)
                    {
                        GetShopIcon().SetActive(false);
                    }

                    break;
                case EGameState.STARTED:

                    GameUtils.SwitchCanvasGroup(m_menuCanvasGroup, m_gameCanvasGroup);
                    GameUtils.SwitchCanvasGroup(null, JoystickGroup);
                    ShopIcon.SetActive(false);

                    break;
                case EGameState.WIN:

                    //m_Touchcanvas.gameObject.SetActive(false);
                    GameUtils.SwitchCanvasGroup(JoystickGroup, WinGroup);
                    WinCurrencyCountText.text = LevelComponent.Instance.GetLevelCurrency().ToString();
                    WinEnemyCountText.text = EnemyCount.ToString();
                    OnPlayerCurrencyUpdated();

                    break;
                case EGameState.LOSE:
                    GameUtils.SwitchCanvasGroup(JoystickGroup, LoseGroup);
                    int EnemyCounter = LevelComponent.Instance.GetEnemys().Count +
                                       LevelComponent.Instance.GetFireEnemys().Count;
                    LoseCurrencyCountText.text = (500).ToString();
                    LoseEnemyCountText.text = (EnemyCount - EnemyCounter).ToString();
                    OnPlayerCurrencyUpdated();

                    break;
            }
        }

        /// <summary>
        /// This function called when player currency updated.
        /// </summary>
        /// <param name="e"></param>
        public void OnPlayerCurrencyUpdated()
        {
            string currencyText = m_currencyText.text;

            currencyText = currencyText.Replace(".", String.Empty);
            currencyText = currencyText.Replace(",", String.Empty);
            currencyText = currencyText.Replace("$", String.Empty);
            currencyText = currencyText.Replace("K", String.Empty);
            currencyText = currencyText.Replace("M", String.Empty);
            currencyText = currencyText.Replace("#", String.Empty);

            int cachedCurrency = CurrentCurrency;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => cachedCurrency, x => cachedCurrency = x, GameManager.Instance.GetCurreny(),
                CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION));

            sequence.OnUpdate(() => { m_currencyText.text = cachedCurrency.ConvertMoney().ToString(); });
            sequence.OnComplete(() => { CurrentCurrency = GameManager.Instance.GetCurreny(); });
            sequence.SetId(m_currencyText.GetInstanceID());
            sequence.Play();
        }

        /// <summary>
        /// This function helper for change settings panel state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeSettingsPanelState()
        {
            // Debug.Log("dawdawd");
            // if (DOTween.IsTweening(GetSettingsGroup().GetInstanceID()))
            //     return;

            Sequence sequence = DOTween.Sequence();


            sequence.Join(SettingsComponent.Instance.GetMaskObj().DOLocalMoveY(SettingsBool ? 9.75f : 290f, 0.25f));
            sequence.OnStart(() =>
            {
                SettingsBool = !SettingsBool;
                if (SettingsBool)
                {
                    SoundManager.Instance.Play(CommonTypes.SOUND_CLICK);
                    HapticManager.Instance.PlayHaptic(HapticTypes.MediumImpact);
                }
            });
            // sequence.OnComplete(() => { GetSettingsGroup().blocksRaycasts = SettingsBool; });
            // sequence.SetId(GetSettingsGroup().GetInstanceID());
            sequence.Play();
        }

        public void LevelTextUpdated()
        {
            // m_levelText.text = string.Format("LEVEL {0}", LevelService.GetCachedLevel(1));
        }

        /// <summary>
        /// This function makes floating text appear.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="force"></param>
        public void PrintFloatingText(string message, Color color, bool force = false)
        {
            if (DOTween.IsTweening(CommonTypes.TWEEN_FLOATING))
            {
                if (!force)
                    return;

                DOTween.Kill(CommonTypes.TWEEN_FLOATING, true);
            }

            GameSettings gameSettings = GameManager.Instance.GetGameSettings();
            TMP_Text createdText = Instantiate(gameSettings.FloatingTextPrefab, m_floatingSlot);

            createdText.text = message;
            createdText.color = color;

            createdText.transform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(createdText.transform.DOScale(Vector3.one, 0.25F));
            sequence.Join(createdText.transform.DOLocalMoveY(1, 0.25F).SetDelay(0.5F));
            sequence.Join(createdText.DOFade(0, 0.25F));

            sequence.SetId(CommonTypes.TWEEN_FLOATING);
            sequence.Play();
        }


        public Transform GetSWeaponUIRoot()
        {
            return WeaponUIRoot;
        }

        public Image GetTrashImage()
        {
            return TrashIcon;
        }

        public CanvasGroup GetSettingsGroup()
        {
            return SettingsGroup;
        }

        public RectTransform GetGameCanvas()
        {
            return m_Touchcanvas;
        }

        public DynamicJoystick GetJoystick()
        {
            return Joystick;
        }

        public void DeactivateJoystick()
        {
            Joystick.OnPointerUp(null);
            Joystick.gameObject.SetActive(false);
        }

        public void ActivateJoystick()
        {
            Joystick.gameObject.SetActive(true);
        }

        public void EnableComboText(bool value)
        {
            m_comboObj.SetActive(value);
        }

        public void PrintComboText(int value)
        {
            DOTween.Kill("Combo");
            m_comboObj.transform.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.Join(m_comboObj.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 0, 0));
            sequence.OnStart(() => { m_comboText.text = string.Format("{0}", value); });
            sequence.Play();
            sequence.SetId("Combo");
        }

        public CanvasGroup GetJoystickGroup()
        {
            return JoystickGroup;
        }

        public CanvasGroup GetShopGroup()
        {
            return ShopGroup;
        }

        public GameObject GetShopIcon()
        {
            return ShopIcon;
        }

        public CanvasGroup GetMenuCanvasGroup()
        {
            return m_menuCanvasGroup;
        }

        protected override void OnDestroy()
        {
            EventService.RemoveListener<HitEnemyEvent>(OnEnemyHit);
        }
    }
}