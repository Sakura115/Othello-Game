using System.Collections.Generic;
using UnityEngine;

public class Game_End : MonoBehaviour
{
    bool gameEnd = false;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject end_menu;
    [SerializeField] private GameObject BlackPointT;
    [SerializeField] private GameObject WhitePointT;
    [SerializeField] private GameObject GameSystem;
    [SerializeField] private GameObject WinnerText;
    [SerializeField] private GameObject HomeButton;
    [SerializeField] private Vector3 camera_xyz;
    [SerializeField] private Vector3 camera_rotate;
    [SerializeField] private Vector3 finish_xyzbefor;
    [SerializeField] private Vector3 finish_xyz_rect;
    [SerializeField] private Vector2 pieceBlack_xy;
    [SerializeField] private Vector2 pieceWhite_xy;
    [SerializeField] private byte pieceNumLimit = 20;
    [SerializeField] private float pieceHeight = 0.004f;
    [SerializeField] private float pieceWidthBlack = -0.022f;
    [SerializeField] private float pieceWidthWhite = 0.022f;
    [SerializeField] private float waitTime = 0.5f; //�X�s�[�h1�̎��̏W�v�I�����猋�ʕ\���܂ł̎���
    [SerializeField] private float pieceWaitTime = 0.2f;//�X�s�[�h1�̎��̋�̓���Ԃ̎���
    [SerializeField] private float FloorHeight = -1.6f;

    [SerializeField] private GameObject speedButton;

    private float time = 0f;
    private bool Move = false;
    private GameObject[] piece;
    private List<GameObject> moveP = new List<GameObject>();
    private byte black_point = 0;
    private byte white_point = 0;
    private byte movePieceN = 0;
    private bool endPieceMove = false;

    private float speed = 1;//����Ԃ̑���1�`0

    // Start is called before the first frame update
    void Start()
    {
        gameEnd = false;
        Move = false;
        endPieceMove = false;
        gameObject.SetActive(false);
        this.GetComponent<RectTransform>().anchoredPosition = finish_xyz_rect;
        finish_xyz_rect = this.transform.localPosition;
        this.transform.localPosition = finish_xyzbefor;
        end_menu.gameObject.SetActive(false);
        WinnerText.gameObject.SetActive(false);
        HomeButton.gameObject.SetActive(false);
        piece = GameSystem.GetComponent<Game_System>().piece;
        movePieceN = 0;
        FloorHeight = FloorHeight / piece[0].transform.parent.gameObject.transform.lossyScale.z;

        speedUP(speedButton.GetComponent<speedUP_button>().speedButtoOn());

        Debug.Log("end");
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���I�����[�h�ɂȂ�����
        if (gameEnd)
        {
            //���΂炭�҂��ăJ������finish���b�Z�[�W�𓮂���
            if (!Move && wait(waitTime * speed))
            {
                mainCamera.GetComponent<move>().moveStart(camera_xyz, camera_rotate);

                GetComponent<move>().moveStart(finish_xyz_rect);

                Move = true;
                time = 0;
            }
            //�J�����ƃ��b�Z�[�W�𓮂����n�߂Ă��Ē�~���Ă�����
            if (Move && !mainCamera.GetComponent<move>().get_moving() && !GetComponent<move>().get_moving())
            {
                //���U���g���j���[���o��
                end_menu.gameObject.SetActive(true);
                //��̐����A�_�����W�v����
                if (movePieceN < piece.Length)
                {
                    if (wait(pieceWaitTime*speed) && !piece[movePieceN].GetComponent<move>().get_moving())
                    {
                        byte box = (byte)GameSystem.GetComponent<Game_System>().Get_piece(movePieceN % 8, movePieceN / 8);
                        if (box != 0)
                        {
                            //Debug.Log(FloorHeight);
                            if (box == 1)
                            {
                                piece[movePieceN].GetComponent<piece_Move>().start_move(new Vector3(pieceBlack_xy.x + (pieceWidthBlack * (int)(black_point / pieceNumLimit)), pieceBlack_xy.y, (float)(FloorHeight + pieceHeight * (black_point % pieceNumLimit))));
                                black_point++;
                                BlackPointT.GetComponent<UnityEngine.UI.Text>().text = "" + black_point;
                            }
                            else if (box == 2)
                            {
                                piece[movePieceN].GetComponent<piece_Move>().start_move(new Vector3(pieceWhite_xy.x + (pieceWidthWhite * (int)(white_point / pieceNumLimit)), pieceWhite_xy.y, (float)(FloorHeight + pieceHeight * (white_point % pieceNumLimit))));
                                white_point++;
                                WhitePointT.GetComponent<UnityEngine.UI.Text>().text = "" + white_point;
                            }
                            time = 0;
                        }
                        movePieceN++;
                    }
                }
                else
                {
                    //���΂炭�҂��ď��s��\������
                    if (wait(waitTime * speed))
                    {
                        if (black_point > white_point)
                        {
                            WinnerText.GetComponent<UnityEngine.UI.Text>().text = "Black Win!!";
                        }
                        else if (black_point < white_point)
                        {
                            WinnerText.GetComponent<UnityEngine.UI.Text>().text = "White Win!!";
                        }
                        else
                        {
                            WinnerText.GetComponent<UnityEngine.UI.Text>().text = "Draw";
                        }
                        WinnerText.gameObject.SetActive(true);
                        HomeButton.gameObject.SetActive(true);
                    }
                }
            }
        }
        if (endPieceMove)//�z�[���{�^���������ꂽ
        {
            //���ʊO�ɏ����鉉�o
            if (wait(pieceWaitTime * speed))
            {
                float maxZ = FloorHeight - 1;
                bool end = true;
                foreach (GameObject p in piece)
                {
                    if (p.active && !moveP.Contains(p))
                    {
                        if (maxZ <= p.transform.localPosition.z)
                        {
                            maxZ = p.transform.localPosition.z;
                        }
                        end = false;
                    }
                }
                if (!end)
                {
                    foreach (GameObject p in piece)
                    {
                        if (p.active)
                        {
                            if (maxZ - pieceHeight / 2 < p.transform.localPosition.z && maxZ + pieceHeight / 2 > p.transform.localPosition.z)
                            {
                                p.GetComponent<piece_endMove>().moveStart();
                                moveP.Add(p);
                            }
                        }
                    }
                }
                else
                {
                    //���o���I�������J�ڂ���
                    Debug.Log("�J��");
                    Application.LoadLevel(0);
                }
                time = 0;
            }

        }
    }


    public void end()
    {
        if (!gameEnd)
        {
            Debug.Log("endStart");
            gameEnd = true;
            gameObject.SetActive(true);
            time = 0;
        }
    }
    private bool wait(float wtime)
    {
        wait();
        return time >= wtime;
    }
    private void wait()
    {
        time += Time.deltaTime;
    }

    public void title() //�^�C�g���J�ڊJ�n
    {
        endPieceMove = true;
        time = 0;
    }

    public void speedUP(float speed) //0�ɋ߂��قǑ����Ȃ�
    {
        this.speed = (float)speed;
        float tmp = mainCamera.GetComponent<move>().get_speed() * (1 - speed);
        mainCamera.GetComponent<move>().acceleration(tmp);
        tmp = GetComponent<move>().get_speed() * (1 - speed);
        GetComponent<move>().acceleration(tmp);
        tmp = piece[0].GetComponent<move>().get_speed() * (1 - speed);
        for (byte i = 0; i < piece.Length; i++)
        {
            piece[i].GetComponent<move>().acceleration(tmp*2);
        }
    }
    public void speedUP(bool Up = true)
    {
        if (Up)
        {
            speedUP(0.5f);
        }
        else
        {
            speedUP(1);
        }
    }
}
