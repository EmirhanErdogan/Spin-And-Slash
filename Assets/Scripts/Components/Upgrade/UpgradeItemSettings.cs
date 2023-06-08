using Emir;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EUpgrade
{
    NONE,
    HOLESIZE,
    HOLESPEED,
    ADDHOLE,
    GETNUMBER,
    COINUPGRADE,
    DIAMONDDROP,
    EXPERİENCEUPGRADE,
    HOLEPULL,
}

[CreateAssetMenu(menuName = "Emir/Game/Upgrade", fileName = "Upgrade", order = 1)]
[System.Serializable]
public class UpgradeItemSettings : ScriptableObject
{
    public EUpgrade type;
    public int Level = 1;
}