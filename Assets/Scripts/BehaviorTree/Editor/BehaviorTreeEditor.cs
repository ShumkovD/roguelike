using Unity;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Xml.Serialization;
public class BehaviorTreeEditor : EditorWindow
{

    private GUIStyle nodeStyle;
    private static BehaviorTreeSO currentTree;

    #region ノードレイアウト変数
    [Header("ノードレイアウト")]
    [SerializeField] float nodeWidth;
    [SerializeField] float nodeHeight;
    [SerializeField] int nodePadding;
    [SerializeField] int nodeBorder;
    #endregion

    [MenuItem("BehaviorTreeEditor", menuItem = "Window/BehaviorTreeEditor")]

    private static void OpenEditorWindow()
    {
        GetWindow<BehaviorTreeEditor>("BehaviorTreeEditor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        nodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
    }
}
