using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    const int numOfRegionInStage = 2;

    [SerializeField] private int stageNum;                                  //�X�e�[�W�ԍ�
    [SerializeField] Vector2[] Sizes = new Vector2[numOfRegionInStage];     //�X�e�[�W�T�C�Y
    [SerializeField] float cameraMovementRangeChengesPosX;                  //�J�����̈ړ��͈͂��ς��x���W

    private CameraManager cameraManager;                                    //�J�����Ǘ��p
    bool usingBoxCollider;                                                  //�{�b�N�X�R���C�_�[���g���Ă��邩
    Vector2 firstEdgeColliderStageSize;                                     //�G�b�W�R���C�_�[�g�p���̂ݎg�p����ϐ�
    Vector2 secondEdgeColliderStageSize;                                    //�G�b�W�R���C�_�[�g�p���̂ݎg�p����ϐ�


    //�Q�b�^�[
    public int StageNum { get { return stageNum; } }
    public float CameraMovementRangeChengesPosX { get { return cameraMovementRangeChengesPosX; } }
    public bool UsingBoxCollider { get { return usingBoxCollider; } }
    public Vector2 FirstEdgeColliderStageSize { get { return firstEdgeColliderStageSize; } }
    public Vector2 SecondEdgeColliderStageSize { get { return secondEdgeColliderStageSize; } }

    private void Awake()
    {
        //�X�e�[�W�̈悪�{�b�N�X�R���C�_�[����������
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

    //�X�e�[�W�ɓ��������u�ԁA���������X�e�[�W�̏ꏊ�ɃJ�������ړ�������
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            cameraManager.NowStage = stageNum;
        }
    }
}
