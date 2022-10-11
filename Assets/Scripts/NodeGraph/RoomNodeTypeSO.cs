using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "部屋タイプ", menuName = "ScriptableObjects/ダンジョン/部屋タイプ")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("グラフエディターで見えるかどうか")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("経路ですか")]
    #endregion
    public bool isCorridor;
    #region Header
    [Header("上下経路ですか")]
    #endregion
    public bool isCorridorNS;
    #region Header
    [Header("左右経路ですか")]
    #endregion
    public bool isCorridorEW;
    #region Header
    [Header("入口ですか")]
    #endregion
    public bool isEntrance;
    #region Header
    [Header("ボスの部屋ですか")]
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("タイプは「なし」にする必要があるですか")]
    #endregion
    public bool isNone;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}
