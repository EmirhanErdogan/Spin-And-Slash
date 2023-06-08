using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Emir;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderComponent : Singleton<SliderComponent>
{
    #region Serializable Fields

    [SerializeField] private Slider _slider;
    [SerializeField] private float SliderPercantage;
    [SerializeField] private TextMeshProUGUI PercantageText;
    [SerializeField] private TextMeshProUGUI Level1Text;
    [SerializeField] private TextMeshProUGUI Level2Text;
    [SerializeField] private TextMeshProUGUI Level3Text;
    [SerializeField] private TextMeshProUGUI Level4Text;

    #endregion


    private void Start()
    {
        _ = Initialize();
        LevelIconsInit();
    }

    public async UniTask Initialize()
    {
        await UniTask.Delay(2);
        Debug.Log(LevelService.GetCachedLevel());
        _slider.value = LevelService.GetCachedLevel();
        _slider.DOValue(LevelService.GetCachedLevel(), 0.25f);
        float CurrentLevel = (float)LevelService.GetCachedLevel(1) / 5;
        float targetMultiply = Mathf.Ceil(CurrentLevel);
        float asd = 5 * (targetMultiply - 1);
        _slider.minValue = asd;
        _slider.maxValue = asd + 5;
    }

    public void UpdateSlider()
    {
        // float value = _slider.maxValue -
        //               (LevelComponent.Instance.GetEnemys().Count + LevelComponent.Instance.GetFireEnemys().Count);
        // _slider.DOValue(value, CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION);
    }


    public void UpdateSliderLevel()
    {
        float value = LevelService.GetCachedLevel(1);
        _slider.DOValue(value, CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION);
    }

    public void SliderPercantageControl()
    {
        int CurrentPercantage = (int)SliderPercantage;
        float RefValue = _slider.value - _slider.minValue;
        float RefMaksValue = _slider.maxValue - _slider.minValue;
        SliderPercantage = (RefValue / RefMaksValue) * 100;

        string currencyText = SliderPercantage.ToString();
        Sequence sequence = DOTween.Sequence();

        sequence.Join(DOTween.To(() => CurrentPercantage, x => CurrentPercantage = x, (int)SliderPercantage,
            CommonTypes.UI_DEFAULT_FLY_CURRENCY_DURATION));

        sequence.OnUpdate(() => { PercantageText.text = string.Format("%{0}", (int)CurrentPercantage); });
        sequence.SetId(PercantageText.GetInstanceID());
        sequence.Play();
    }

    public void LevelIconsInit()
    {
        float CurrentLevel = (float)LevelService.GetCachedLevel(1) / 5;
        float targetMultiply = Mathf.Ceil(CurrentLevel);
        float asd = 5 * (targetMultiply - 1);
        Level1Text.text = ((int)asd + 1).ToString();
        Level2Text.text = ((int)asd + 2).ToString();
        Level3Text.text = ((int)asd + 3).ToString();
        Level4Text.text = ((int)asd + 4).ToString();
    }
}