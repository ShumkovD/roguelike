using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "部屋グラフ", menuName = "ScriptableObjects/ダンジョン/部屋グラフ")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodelist = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
}
