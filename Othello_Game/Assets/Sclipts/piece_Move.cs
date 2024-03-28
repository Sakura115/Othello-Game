using UnityEngine;

//�W�v���̋�̓���

public class piece_Move : MonoBehaviour
{

    private bool pieceUP;
    private bool pieceMove;
    private bool pieceDown;
    private bool start;
    private Vector3 target_xyz;
    public float height=0.1f; //�����鍂��
    // Start is called before the first frame update
    void Start()
    {
        start = false;
        pieceUP = false;
        pieceMove = false;
        pieceDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (!pieceUP)
            {
                GetComponent<move>().moveStart(new Vector3(transform.localPosition.x, transform.localPosition.y, height));
                pieceUP = true;
            }
            else if (!pieceMove && !GetComponent<move>().get_moving())
            {
                GetComponent<move>().moveStart(new Vector3(target_xyz.x, target_xyz.y, height));
                pieceMove = true;
            }
            else if (pieceMove && !GetComponent<move>().get_moving()&& !get_Setting())
            {
                GetComponent<move>().moveStart(target_xyz);
                pieceDown = true;
            }
        }
    }

    public void start_move(Vector3 target)
    {
        start = true;
        //Debug.Log("�R�}�ړ�");
        target_xyz = target;
    }
        public bool get_Setting()
    {
        return pieceDown&& !GetComponent<move>().get_moving();
    }
}
