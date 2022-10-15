using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomIDList = new List<string>();
    [HideInInspector] public List<string> childRoomIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

    #region Editor Code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;


    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "ノード";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        //部屋のタイプをロードする
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    public void Draw(GUIStyle nodeStyle)
    {
        //箱を描画する
        GUILayout.BeginArea(rect, nodeStyle);
        //変更を確認する
        EditorGUI.BeginChangeCheck();

        //親があったらかノードは入口だったら
        if (parentRoomIDList.Count > 0 || roomNodeType.isEntrance)
        {
            //変えられないタッグの表示
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            //ポップアップの表示
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            if (roomNodeTypeList.list[selected].isCorridor && !roomNodeTypeList.list[selection].isCorridor || 
                !roomNodeTypeList.list[selected].isCorridor && roomNodeTypeList.list[selection].isCorridor || 
                !roomNodeTypeList.list[selected].isBossRoom && roomNodeTypeList.list[selection].isBossRoom)
            {
                if (childRoomIDList.Count > 0)
                {
                    for (int i = childRoomIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomIDList[i]);

                        if (childRoomNode != null)
                        {
                            RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                            childRoomNode.RemoveParentRoomNodeIDFromRoomNode(id);
                        }
                    }
                }

            }
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);

        GUILayout.EndArea();

    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for(int i = 0;i<roomNodeTypeList.list.Count;i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }

    /// <summary>
    /// イベントの処理
    /// </summary>
    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //マウスボタンを押すイベント
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            //マウスボタンを離れるイベント
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            //マウスボタンを長押しのイベント
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// マウスイベントの処理
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if(currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if(currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }

    }
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        if(isSelected == true)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        if(isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }

    }


    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent);
        }
    }

    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    //ノードに子供を追加する
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childRoomIDList.Add(childID);
            return true;
        }
        return false;
    }

    //ノードに親を追加する
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomIDList.Add(parentID);
        return true;
    }

    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;

        foreach(RoomNodeSO roomNode in roomNodeGraph.roomNodelist)
        {
            if(roomNode.roomNodeType.isBossRoom && roomNode.parentRoomIDList.Count > 0)
                isConnectedBossNodeAlready = true;
        }

        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;

        if (childRoomIDList.Contains(childID))
            return false;

        if (id == childID)
            return false;

        if (parentRoomIDList.Contains(childID))
            return false;

        if (roomNodeGraph.GetRoomNode(childID).parentRoomIDList.Count > 0)
            return false;

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;

        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomIDList.Count >= Settings.maxChildCorridors)
            return false;

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false;

        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomIDList.Count > 0)
            return false;

        return true;
    }



    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if(childRoomIDList.Contains(childID))
        {
            childRoomIDList.Remove(childID);
            return true;
        }
        return false;
    }

    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (parentRoomIDList.Contains(parentID))
        {
            parentRoomIDList.Remove(parentID);
            return true;
        }
        return false;
    }

#endif

    #endregion Editor Code
}
