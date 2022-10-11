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

    //レイアウトの変数
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;



    [MenuItem("部屋のグラフエディター", menuItem = "Window/ダンジョンエディター/部屋のグラフエディター")]
    private static void OpenWindow()
    {
        GetWindow<RoomNoveGraphEditor>("部屋のグラフエディター");
    }

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        //部屋のタイプをロードする
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// RoomNodeGraphSOをクリックすると、エディターが開きます
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
    ///エディターを描画する 
    /// </summary>
    private void OnGUI()
    {
        //ノードグラフはすでに選んでいる状態
        if(currentRoomNodeGraph != null)
        {
            //イベントの処理
            ProcessEvents(Event.current);

            //ノードの描画
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
    /// マウスはノードをハーバーする
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
    /// イベントの処理
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch(currentEvent.type)
        {
            //マウスボタンを押すイベント
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            default:
                break;
        }
    }


    /// <summary>
    /// マウスボタンを押すイベントの処理（ノードを除く）
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    /// <summary>
    /// メニューの表示
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("ノード作成"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
    }
    /// <summary>
    /// マウス位置でノードの作成
    /// </summary>
    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }
    /// <summary>
    /// マウス位置でノードの作成　ー　RoomNodeTypeSOを使うためにオーバーロード
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //ノードのアセットの作成
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        //ノードグラフにノードの追加
        currentRoomNodeGraph.roomNodelist.Add(roomNode);
        //ノードのプロパティの設定
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomType);

        //ノードグラフにノードの追加「アセット」
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// ノードの描画
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
