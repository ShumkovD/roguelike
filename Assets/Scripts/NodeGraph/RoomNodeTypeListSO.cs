using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "部屋タイプリスト", menuName = "ScriptableObjects/ダンジョン/部屋タイプ-リスト")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region 部屋タイプリスト
    [Space(10)]
    [Header("部屋タイプリスト")]
    #endregion
    #region Tooltip
    [Tooltip("全てのRooｍNodeTypeSOをこちらにいれてください。ENUMの代わりに使っています")]
    #endregion
    public List<RoomNodeTypeSO> list;

    #region Validation
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
    #endregion

}
