using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    const int numOfRegionInStage = 2;

    [SerializeField] private int stageNum;                                  //ステージ番号
    [SerializeField] Vector2[] Sizes = new Vector2[numOfRegionInStage];     //ステージサイズ
    [SerializeField] float cameraMovementRangeChengesPosX;                  //カメラの移動範囲が変わるx座標

    private CameraManager cameraManager;                                    //カメラ管理用
    bool usingBoxCollider;                                                  //ボックスコライダーを使っているか
    Vector2 firstEdgeColliderStageSize;                                     //エッジコライダー使用時のみ使用する変数
    Vector2 secondEdgeColliderStageSize;                                    //エッジコライダー使用時のみ使用する変数


    //ゲッター
    public int StageNum { get { return stageNum; } }
    public float CameraMovementRangeChengesPosX { get { return cameraMovementRangeChengesPosX; } }
    public bool UsingBoxCollider { get { return usingBoxCollider; } }
    public Vector2 FirstEdgeColliderStageSize { get { return firstEdgeColliderStageSize; } }
    public Vector2 SecondEdgeColliderStageSize { get { return secondEdgeColliderStageSize; } }

    private void Awake()
    {
        //ステージ領域がボックスコライダーを持つか判定
        if (GetComponent<BoxCollider2D>())
        { usingBoxCollider = true; }
        else { usingBoxCollider = false; }

        firstEdgeColliderStageSize = Sizes[0];
        secondEdgeColliderStageSize = Sizes[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
    }

    //ステージに当たった瞬間、当たったステージの場所にカメラを移動させる
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            cameraManager.NowStage = stageNum;
        }
    }
}
