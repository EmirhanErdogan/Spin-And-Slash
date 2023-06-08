using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Emir;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;


public class UIUpgradeItem : MonoBehaviour
{
    [SerializeField] private UpgradeItemSettings settings;
    [SerializeField] private TextMeshProUGUI _text;


    public void Click()
    {
      

        switch (settings.type)
        {
            // case EUpgrade.ADDHOLE:
            //     UpgradeAddHole();
            //     break;
            // case EUpgrade.HOLEPULL:
            //     UpgradeHolePull();
            //     break;
            // case EUpgrade.HOLESIZE:
            //     UpgradeHoleSize();
            //     break;
            // case EUpgrade.GETNUMBER:
            //     UpgradeGetNumber();
            //     break;
            // case EUpgrade.HOLESPEED:
            //     UpgradeHoleSpeed();
            //     break;
            // case EUpgrade.COINUPGRADE:
            //     UpgradeCoinUpgrade();
            //     break;
            // case EUpgrade.DIAMONDDROP:
            //     UpgradeDiamondDrop();
            //     break;
            // case EUpgrade.EXPERİENCEUPGRADE:
            //     UpgradeExperianceUpgrade();
            //     break;
        }

       
    }

  

    public void ResetText()
    {
        _text.text = String.Format("LV {0}", settings.Level);
    }

    public UpgradeItemSettings GetSettings()
    {
        return settings;
    }
    
}