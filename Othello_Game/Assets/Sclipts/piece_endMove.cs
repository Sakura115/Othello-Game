using UnityEngine;

//�Q�[���Ō�ɋ����ʊO�ɏo���ď�������

public class piece_endMove : MonoBehaviour
{
    [SerializeField] private float endPieceUpHeight = 0.15f;
    private bool start = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(start&& !GetComponent<move>().get_moving())
        {
            //�������n�܂��Ă��āA���삪�I����Ă��������
            this.gameObject.SetActive(false);
        }
    }

    public void moveStart()
    {
        //���̊֐����Ăяo���ꂽ�珈�����J�n����
        GetComponent<move>().moveStart(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + endPieceUpHeight));
        start = true;
    }
}
