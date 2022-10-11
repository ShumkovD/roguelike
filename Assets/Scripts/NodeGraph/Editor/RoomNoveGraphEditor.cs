using Unity;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Xml.Serialization;

public class RoomNoveGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private  RoomNodeSO currentRoomNode = null;
    private  RoomNodeTypeListSO roomNodeTypeList;

    //���C�A�E�g�̕ϐ�
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;



    [MenuItem("�����̃O���t�G�f�B�^�[", menuItem = "Window/�_���W�����G�f�B�^�[/�����̃O���t�G�f�B�^�[")]
    private static void OpenWindow()
    {
        GetWindow<RoomNoveGraphEditor>("�����̃O���t�G�f�B�^�[");
    }

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //�����̃^�C�v�����[�h����
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// RoomNodeGraphSO���N���b�N����ƁA�G�f�B�^�[���J���܂�
    /// </summary>
    [OnOpenAsset(10)] //UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if(roomNodeGraph!= null)
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
        if(currentRoomNodeGraph != null)
        {
            //�C�x���g�̏���
            ProcessEvents(Event.current);

            //�m�[�h�̕`��
            DrawRoomNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null)
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
        for(int i = currentRoomNodeGraph.roomNodelist.Count - 1; i>=0; i--)
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
        switch(currentEvent.type)
        {
            //�}�E�X�{�^���������C�x���g
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
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
    }

    /// <summary>
    /// ���j���[�̕\��
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("�m�[�h�쐬"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
    }
    /// <summary>
    /// �}�E�X�ʒu�Ńm�[�h�̍쐬
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
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
    }

    /// <summary>
    /// �m�[�h�̕`��
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach(RoomNodeSO roomNode in currentRoomNodeGraph.roomNodelist)
        {
            roomNode.Draw(roomNodeStyle);
        }
        GUI.changed = true;
    }




}
