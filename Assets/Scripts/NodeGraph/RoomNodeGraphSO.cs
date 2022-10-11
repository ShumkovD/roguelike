using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "�����O���t", menuName = "ScriptableObjects/�_���W����/�����O���t")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> roomNodelist = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
}
