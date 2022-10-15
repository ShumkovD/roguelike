using Unity;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Xml.Serialization;

public class RoomNoveGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    //���C�A�E�g�̕ϐ�
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    //���̌���
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    [MenuItem("�����̃O���t�G�f�B�^�[", menuItem = "Window/�_���W�����G�f�B�^�[/�����̃O���t�G�f�B�^�[")]
    private static void OpenWindow()
    {
        GetWindow<RoomNoveGraphEditor>("�����̃O���t�G�f�B�^�[");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        //���ʃX�^�C��
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        //�I��ł����Ԃ̃X�^�C��
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //�����̃^�C�v�����[�h����
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// RoomNodeGraphSO���N���b�N����ƁA�G�f�B�^�[���J���܂�
    /// </summary>
    [OnOpenAsset(10)] //UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }
        return false;
    }


    /// <summary>
    ///�G�f�B�^�[��`�悷�� 
    /// </summary>
    private void OnGUI()
    {
        
        //�m�[�h�O���t�͂��łɑI��ł�����
        if (currentRoomNodeGraph != null)
        {
            //�h���b�O���鎞�ɐ���`�悷��
            DrawDraggedLine();
            //�C�x���g�̏���
            ProcessEvents(Event.current);

            DrawRoomConnections();
            

            //�m�[�h�̕`��
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void DrawDraggedLine()
    {
        if(currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }



    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    /// <summary>
    /// �}�E�X�̓m�[�h���n�[�o�[����
    /// </summary>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = currentRoomNodeGraph.roomNodelist.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraph.roomNodelist[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodelist[i];
            }
        }
        return null;
    }


    /// <summary>
    /// �C�x���g�̏���
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //�}�E�X�{�^���������C�x���g
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }


    /// <summary>
    /// �}�E�X�{�^���������C�x���g�̏����i�m�[�h�������j
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }

        if(currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }

    }
    /// <summary>
    /// �}�E�X�{�^���𗣂��C�x���g�̏����i�m�[�h�������j
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if(roomNode != null)
            {
                if(currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    /// <summary>
    /// �}�E�X�{�^���𒷉����C�x���g�̏����i�m�[�h�������j
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }
    /// <summary>
    /// �}�E�X�̉E�{�^���̃C�x���g
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    public void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// ���j���[�̕\��
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("�m�[�h�쐬"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("�S�Ẵm�[�h��I��"), false, SelectAllRoomNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// �����폜����
    /// </summary>
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// �m�[�h�̑I�񂾏�Ԃ̍폜
    /// </summary>
    private void ClearAllSelectedRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodelist)
        {
            if(roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    public void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodelist)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }

    private void DrawRoomConnections()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodelist)
        {
            if(roomNode.childRoomIDList.Count > 0)
            {
                foreach(string childRoomNodeID in roomNode.childRoomIDList)
                {
                    if(currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }


    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        Vector2 midPosition = (endPosition + startPosition) * 0.5f;

        Vector2 direction = endPosition - startPosition;

        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);


        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);
        GUI.changed = true;
    }

    /// <summary>
    /// �}�E�X�ʒu�Ńm�[�h�̍쐬
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        if(currentRoomNodeGraph.roomNodelist.Count == 0)
        {
            CreateRoomNode(new Vector2(200f,200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// �}�E�X�ʒu�Ńm�[�h�̍쐬�@�[�@RoomNodeTypeSO���g�����߂ɃI�[�o�[���[�h
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //�m�[�h�̃A�Z�b�g�̍쐬
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        //�m�[�h�O���t�Ƀm�[�h�̒ǉ�
        currentRoomNodeGraph.roomNodelist.Add(roomNode);
        //�m�[�h�̃v���p�e�B�̐ݒ�
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomType);

        //�m�[�h�O���t�Ƀm�[�h�̒ǉ��u�A�Z�b�g�v
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        //���X�g���A�b�v�f�[�g����
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// �m�[�h�̕`��
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodelist)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    /// <summary>
    /// �C���X�y�N�^�[�őI�������I�u�W�F�N�g�����ς����
    /// </summary>
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if(roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }

}
