using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }


    #region Header �_���W����
    [Space(10)]
    [Header("�_���W����")]
    #endregion
    #region �c�[���`�b�v
    [Tooltip("RoomNodeTypeListSO���g���Ă�������")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;

}