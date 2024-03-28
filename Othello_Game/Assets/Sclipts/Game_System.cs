using UnityEngine;
using System;

//���C���Q�[������

public class Game_System : MonoBehaviour

{   // �Ֆ�,box[��,�c]
    // ��:0 ��:1 ��:2
    private byte[,] box = new byte[8, 8];

    //�e�}�X�̒u�����ꍇ�ɗ��Ԃ��鐔[��0,��1][��,�c][����]
    private byte[][,][] B_sco = new byte[2][,][];

    public GameObject[] piece;//��̃��f��

    [SerializeField]
    private GameObject TurnText;//�^�[����\������e�L�X�g�I�u�W�F�N�g
    [SerializeField]
    private GameObject PassText;//�p�X��\������e�L�X�g�e�L�X�g�I�u�W�F�N�g
    [SerializeField]
    private GameObject PassButton;//�p�X�̓��͂��󂯎��{�^��
    [SerializeField]
    private GameObject GameEnd;//�Q�[���I���̏���������X�N���v�g�̂����I�u�W�F�N�g
    [SerializeField]
    private GameObject EndMenu;//�Q�[���I�����ɕ\�����郁�j���[
    [SerializeField]
    private float waitTime = 0.5f;//�O�̃v���C���[�̓��͂��玟�̃v���C���[�̓��͂Ɉڂ鎞��
    [SerializeField]
    private float npcWaitTime = 3f;//NPC���Y�ގ���
    private float time = 0f;//�Q�[����̎��Ԃ��Ǘ�����

    private bool playerTurn = false;    //���N�̃^�[���� ��(��)fallse ��(��)true
    public static int playMode = 3;//0:2�l�ΐ� 1:1�l�ΐ�(��j2:1�l�ΐ�i��j 3:NPC�ΐ�
    [SerializeField]
    private float pieceR = 1.1f;//��̔��a


    private Material[] pieceMaterials;//��̒u�����Ƃ��̃}�e���A��
    private Material[] pieceMaterialsImposs;//��̒u�����Ƃ��ł��Ȃ����̃}�e���A��
    private Material[] pieceMaterialsPoss;//��̒u�����Ƃ��ł���Ƃ��̃}�e���A��
    [SerializeField]
    private Material pieceBlackMaterial;
    [SerializeField]
    private Material pieceWhiteMaterial;
    [SerializeField]
    private Material pieceBlackMaterialImpossibleToPut;
    [SerializeField]
    private Material pieceWhiteMaterialImpossibleToPut;
    [SerializeField]
    private Material pieceBlackMaterialPossibleToPut;
    [SerializeField]
    private Material pieceWhiteMaterialPossibleToPut;

    private int consecutivePass = 0;//�A��pass��


    [SerializeField]
    private GameObject speedButton;//�X�s�[�h��ς���p�̃{�^��

    [SerializeField]
    private GameObject NPC_thinking;//NPC�̔Y�ރA�C�R��

    private float speed = 1;


    // Start is called before the first frame update
    void Start()
    {
        for (byte i = 0; i < B_sco.Length; ++i)
        {
            B_sco[i] = new byte[box.GetLength(0), box.GetLength(1)][];
            for (byte k = 0; k < B_sco[0].GetLength(0); k++)
            {
                for (byte j = 0; j < B_sco[0].GetLength(1); j++)
                {
                    B_sco[i][k, j] = new byte[8];
                }
            }
        }
        GameEnd.gameObject.SetActive(false);
        PassText.gameObject.SetActive(false);
        PassButton.gameObject.SetActive(false);
        EndMenu.gameObject.SetActive(false);
        NPC_thinking.GetComponent<NPC_img>().nomal();
        Debug.Log("�Q�[���J�n:�v���C���[�h" + playMode);
        pieceMaterials = new Material[] { pieceBlackMaterial, pieceWhiteMaterial };
        pieceMaterialsImposs = new Material[] { pieceBlackMaterialImpossibleToPut, pieceWhiteMaterialImpossibleToPut };
        pieceMaterialsPoss = new Material[] { pieceBlackMaterialPossibleToPut, pieceWhiteMaterialPossibleToPut };
        reset();
        speedUP(speedButton.GetComponent<speedUP_button>().speedButtoOn());//�X�s�[�h�A�b�v�{�^�����I�����ǂ����擾
    }

    // Update is called once per frame
    void Update()
    {
        byte n = 1;

        if (end_judgment() && consecutivePass < 2) //�Q�[�����I�����Ă��Ȃ����A�^�[���̕\��
        {
            TurnText.GetComponent<UnityEngine.UI.Text>().text = playerTurn ? "���̔�" : "���̔�";
        }

        if (wait(waitTime))//�O��̓��͂���K�莞�Ԃ����Ă�����
        {
            if (end_judgment() && consecutivePass < 2) //���Ԃ���Ƃ��낪����A�p�X���A�����Ă��Ȃ��ꍇ
            {
                if (!PassText.GetComponent<Pass_text>().get_display()) //�p�X���\������Ă��Ȃ����
                {
                    //�v���C���[���Z�b�g���v���C������
                    player_set();
                }
            }
            else
            {
                //���Ԃ���Ƃ��낪�Ȃ��A�܂��̓p�X���A�����Ă���Ƃ����΂炭�҂��ďI������
                if (wait(waitTime))
                {
                    //�C����ʂւ̈ڍs����
                    Destroy(PassText);
                    Destroy(PassButton);
                    Destroy(NPC_thinking);
                    Destroy(TurnText);
                    GameEnd.GetComponent<Game_End>().end();
                }
            }
        }



    }


    // �Ֆʂ����Z�b�g����(�����ʒu��)
    private void reset()
    {
        // ���ڂ����ׂăR�}�Ȃ��ɂ���
        for (byte y = 0; y < box.GetLength(1); y++)
        {
            for (byte x = 0; x < box.GetLength(0); x++)
            {
                put_piece(x, y, 0);
            }
        }
        // �I�Z���̏����ʒu�ɋ��u��
        byte n = 3;
        put_piece(n, n, 2);
        put_piece(n + 1, n, 1);
        put_piece(n, n + 1, 1);
        put_piece(n + 1, n + 1, 2);

        //�S�Ẵ}�X�̗��Ԃ��鐔���v�Z����
        put_Allscore();
    }

    // �Ֆʂ̔䗦��Ԃ�
    public int size()
    {
        return box.GetLength(0);
    }

    //�R�}�̒l��Ԃ�
    public int Get_piece(int x, int y)
    {
        return box[x, y];
    }

    //�z��x[]�̍��v��Ԃ�
    private int total(int[] x)
    {
        int ans = 0;
        for (int i = 0; i < x.Length; i++)
        {
            ans += x[i];
        }
        return ans;
    }
    private int total(byte[] x)
    {
        int ans = 0;
        for (int i = 0; i < x.Length; i++)
        {
            ans += x[i];
        }
        return ans;
    }

    // x,y�̃}�X��p�̋�(��1��2)��u�����ꍇ,��dx�cdy�����̋�͉������Ԃ��邩������
    private int count_point(int x, int y, int p, int dx, int dy)
    {
        int count = 0;
        // �J��Ԃ�(�����̋�o�Ă���܂ő�����)
        // (�\�����ʓ���������Ƃ��ɂ����ƌJ��Ԃ��𑱂������Ȃ����ߏ��(�Ֆʂ̔䗦)��݂���)
        for (byte i = 1; i < size() + 1; i++)
        {
            // �͈͊O�ɂȂ����ꍇ
            if (x + dx * i < 0 || x + dx * i > box.GetLength(0) - 1 || y + dy * i < 0 || y + dy * i > box.GetLength(1) - 1)
            {
                return 0;//�����̋�Ȃ������ꍇ�A0��Ԃ�
            }
            else
            {
                // ����̋�����ꍇ�J�E���g��1���₷
                if (box[x + dx * i, y + dy * i] == (p * -1 + 3))
                {
                    count++;
                }
                else if (box[x + dx * i, y + dy * i] == p)// �����̋�����ꍇ���܂ł̃J�E���g����Ԃ�
                {
                    return count;
                }
                else // �󂫃}�X��z��O�̐����̏ꍇ0��Ԃ�
                {
                    return 0;
                }
            }
        }
        return 0;
    }

    // x,y�̃}�X��p�̋�(��1��2)��u�����ꍇ,����̋��S���ŉ������Ԃ��邩������
    private int count_Allpoint(int x, int y, int p)
    {
        if (box[x, y] == 0)
        {
            // �S�����̍��v
            return (count_point(x, y, p, -1, 0) + count_point(x, y, p, -1, 1) + count_point(x, y, p, 0, 1)
                    + count_point(x, y, p, 1, 1) + count_point(x, y, p, 1, 0) + count_point(x, y, p, 1, -1)
                    + count_point(x, y, p, 0, -1) + count_point(x, y, p, -1, -1));
        }
        else
        {
            // �u�����Ƃ����}�X�ɋ�����Ă������ꍇ�A0��Ԃ�
            return 0;
        }
    }
    public int count_Allpoint(int x, int y, int p, int[] n)
    {// n[]�ɉE���甽���v���̏��Ŋe�����̗��Ԃ��鐔��Ԃ�
        if (box[x, y] == 0)
        {
            byte[] ans = new byte[8];
            // n�̔z���ans�̔z��ɉE���甽���v���̏��ŗ��Ԃ��鐔������
            for (byte i = 0; i < 8; i++)
            {
                ans[i] = (byte)count_point(x, y, p, (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)), -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)));
                if (i < n.Length)
                {
                    n[i] = (int)ans[i];
                }
            }
            // �S�����̍��v
            return total(ans);

        }
        else
        {
            // �u�����Ƃ����}�X�ɋ�����Ă������ꍇ�A0��Ԃ�
            for (byte i = 0; i < 8; i++)
            {
                if (i < n.Length)
                {
                    n[i] = 0;
                }
            }
            return 0;
        }
    }
    private int count_Allpoint(int x, int y, int p, byte[] n)
    {// n[]�ɉE���甽���v���̏��Ŋe�����̗��Ԃ��鐔��Ԃ�
        if (box[x, y] == 0)
        {
            byte[] ans = new byte[8];
            // n�̔z���ans�̔z��ɉE���甽���v���̏��ŗ��Ԃ��鐔������
            for (byte i = 0; i < 8; i++)
            {
                ans[i] = (byte)count_point(x, y, p, (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)), -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)));
                if (i < n.Length)
                {
                    n[i] = ans[i];
                }
            }
            // �S�����̍��v
            return total(ans);

        }
        else
        {
            // �u�����Ƃ����}�X�ɋ�����Ă������ꍇ�A0��Ԃ�
            for (byte i = 0; i < 8; i++)
            {
                if (i < n.Length)
                {
                    n[i] = 0;
                }
            }
            return 0;
        }
    }


    //p�̋�(��1��2)�̑S�Ẵ}�X�̗��Ԃ��鐔������
    private void put_Allscore(int p)
    {
        p--;
        p %= 2;//�z��O�̐������͂���ăG���[���N���Ȃ����߂̕ی�
        for (byte y = 0; y < B_sco[p].GetLength(1); y++)
        {
            for (byte x = 0; x < B_sco[p].GetLength(0); x++)
            {
                count_Allpoint(x, y, p + 1, B_sco[p][x, y]);
            }

        }
    }
    private void put_Allscore()
    {//�����̋�̑S�Ẵ}�X�̗��Ԃ��鐔������
        put_Allscore(1);
        put_Allscore(2);
    }

    // x,y�̃}�X��p�̋�(��1��2)��u���B�l���s���������ꍇ,�܂��u���Ȃ��}�X�ɒu�����Ƃ����ꍇfalse��Ԃ�
    private bool put(int x, int y, int p)//x,y �R�}�̈ʒu�@p�@�R�}�̐F(0:�� 1:�� 2:��)
    {

        // �u�����Ƃ����}�X�̗��Ԃ��鐔��0�������ꍇ,�l���s���������ꍇ
        if (!put_judgement(x, y, p))
        {
            Debug.Log("�R�}���u���܂���" + p);
            return false;
        }
        else
        {
            byte[] n = B_sco[p - 1][x, y];
            // �S���ʂ̗��Ԃ����̐��̕����Ԃ�
            for (byte i = 0; i < n.Length; i++)
            {
                for (byte k = 1; k < n[i] + 1; k++)
                {
                    // ��𗠕Ԃ�
                    int tx = x + (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)) * k;
                    int ty = y + -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)) * k;
                    box[tx, ty] = (byte)p;
                    //Debug.Log("�R�}��]" + tx + ty );
                    piece[tx + ty * 8].GetComponent<piece_rotate>().rotate_start(p);//��̃I�u�W�F�N�g�ɔ��f
                }
            }
            put_piece(x, y, p);//��̃I�u�W�F�N�g�A�f�[�^�z��ɔ��f����
            //�S�Ẵ}�X�̗��Ԃ��鐔���X�V����
            put_Allscore();
            return true;
        }
    }


    private void put_piece(int x, int y, int p)//x,y �R�}�̈ʒu�@p�@�R�}�̐F(0:�� 1:�� 2:��)
    {
        box[x, y] = (byte)p;
        if (p == 0)
        { piece[x + y * 8].gameObject.SetActive(false); }
        else
        {
            p--;
            p %= 2;//�s���Ȓl�̏��O

            //Debug.Log("�R�}��u���܂���" + x + y + p);
            piece[x + y * 8].GetComponent<MeshRenderer>().materials = pieceMaterials;//��̃}�e���A����ʏ�̂��̂ɂ���
            piece[x + y * 8].transform.localEulerAngles = new Vector3(180 * p, 0, 0);//��̖ʂ��w�肳�ꂽ�F�̖ʂɕς���
            piece[x + y * 8].gameObject.SetActive(true);//���\������
            /*float tmp = -Rset * 8 / 2-Rset/2;
            GameObject piece = Instantiate(piecePrefab, this.transform.position + new Vector3(tmp+Rset*x,0,tmp+Rset*y), Quaternion.Euler(Euler));
            piece.gameObject.transform.parent = this.gameObject.transform;
            piece.transform.eulerAngles = new Vector3(180*p - 90, 0, 0);
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);*/
        }
    }

    private bool put_judgement(int x, int y, int p)
    {
        // ���Ԃ��鐔��0�������ꍇ,�l���s���������ꍇ
        if ((p < 0 || p > 2) || (x < 0 || x >= box.GetLength(0)) || (y < 0 || y >= box.GetLength(1)) || total(B_sco[(p - 1) % 2][x, y]) == 0)
        {
            return false;
        }
        return true;
    }

    private void player_set()
    {
        int p = Convert.ToInt32(playerTurn) + 1;//�^�[��(��1��2)
        //Debug.Log("�v���C���[" + p);


        if (end_judgment(p))
        {
            if (player_notNPC(p))
            {
                if (Player(p))
                {
                    playerTurn = !playerTurn;
                    time = 0;
                    consecutivePass = 0;
                    PassButton.gameObject.SetActive(false);
                }
            }
            else
            {

                NPC_thinking.GetComponent<NPC_img>().thinking(p - 1);
                //�����҂��Ă���NPC���N������
                if (wait(npcWaitTime * speed))
                {
                    NPC(p);
                    NPC_thinking.GetComponent<NPC_img>().nomal();
                    playerTurn = !playerTurn;
                    time = 0;
                    consecutivePass = 0;
                }
            }
        }
        else
        {
            pass();
        }

        // return end_judgment();

    }

    private bool player_notNPC(int p)//p�^�[��(��1��2)�̃v���C���[���l��NPC��(true:�l false:NPC)
    {
        const bool P = true;//�l�̎��ɕԂ�����
        const bool N = false;//NPC�̎��ɕԂ�����

        //0:2�l�ΐ� 1:1�l�ΐ�(��j2:1�l�ΐ�i��j 3:NPC�ΐ�
        switch (playMode)
        {
            case 0:return P;//�l
            case 1: return !playerTurn ? P : N;//��(��)�̎��A�l�@��(��)�̎��ANPC; 
            case 2: return !playerTurn ? N : P;//��(��)�̎��ANPC�@��(��)�̎��A�l
            case 3: return N;//NPC

            default:return N;//��O�̎��ANPC
        }
    }

    public void pass()
    {
        Debug.Log("Pass" + (!playerTurn?"��":"��") + "," + consecutivePass);

        playerTurn = !playerTurn;//�^�[����ς���
        PassText.GetComponent<Pass_text>().display_Pass();//�p�X���b�Z�[�W�̕\��
        consecutivePass++;//�A���p�X���𑝂₷
        PassButton.gameObject.SetActive(false);//�p�X�{�^�����\���ɂ���
    }

    //�����Ƃ��������񗠕Ԃ���Ƃ����p�̋�(��1��2)��u��NPC
    private bool NPC(int p)
    {
        p %= 3;
        //�p�̃}�X�ɂ�����ꍇ�A�D�悵�Ēu��
        for (byte y = 0; y < 2; y++)
        {
            for (byte x = 0; x < 2; x++)
            {
                if (put_judgement((box.GetLength(0) - 1) * x, (box.GetLength(1) - 1) * y, p))//���E�㉺�̊p�}�X�ɒu�����Ƃ��ł��邩���f����
                {
                    return put((box.GetLength(0) - 1) * x, (box.GetLength(1) - 1) * y, p);//�u�����Ƃ��ł���ꍇ�A��
                }
            }
        }

        //0:��ԑ傫���l,1:����x���W,2:����y���W
        byte[] max = new byte[3];

        //�T���n�߂�_(�u���ʒu�Ƀ����_��������������)
        int nx = UnityEngine.Random.Range(0, B_sco[p - 1].GetLength(0));
        int ny = UnityEngine.Random.Range(0, B_sco[p - 1].GetLength(1));

        for (byte y = 0; y < B_sco[p - 1].GetLength(1); y++)
        {
            for (byte x = 0; x < B_sco[p - 1].GetLength(0); x++)
            {
                int tx = (nx + x) % B_sco[p - 1].GetLength(0);
                int ty = (ny + y) % B_sco[p - 1].GetLength(1);
                if (max[0] < total(B_sco[p - 1][tx, ty]))
                {
                    max[0] = (byte)total(B_sco[p - 1][tx, ty]);
                    max[1] = (byte)tx;
                    max[2] = (byte)ty;
                }
            }
        }
        return put(max[1], max[2], p);
    }

    private bool Player(int p)//true�@���͊���
    {
        PassButton.gameObject.SetActive(true);//�p�X�{�^���̕\��

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//�}�E�X�ʒu���烌�C���擾
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            for (byte i = 0; i < piece.Length; i++)//�}�X�̐������J��Ԃ�
            {
                //�}�X��ɋ�u����Ă��Ȃ��ꍇ�̂ݏ���������
                if (box[i % 8, i / 8] == 0)//�����󂫃}�X��������
                {
                    //�}�E�X�����̃}�X�̏�ɂ�������
                    if (Vector3.Distance(hit.point, piece[i].transform.position) < pieceR)//���C�̓��������ʒu�����̃}�X��������(���̃}�X�ɒu������̒��S�_����̋������R�}�̔��a�ȉ���������)
                    {
                        //���[����u���邩�ǂ������f����
                        if (put_judgement(i % 8, i / 8, p))//�u����Ƃ�
                        {
                            piece[i].GetComponent<MeshRenderer>().materials = pieceMaterialsPoss;//�u���鎞�̃}�e���A���ɂ���

                            if (Input.GetMouseButtonDown(0))//�󂫃}�X�̏�Ń}�E�X�̍��{�^���������ꂽ��
                            {
                                //���u��
                                //Debug.Log("put");
                                put(i % 8, i / 8, p);
                                return true;
                            }
                        }
                        else
                        {
                            //�u���Ȃ����A�u���Ȃ��Ƃ��̃}�e���A���ɂ���
                            piece[i].GetComponent<MeshRenderer>().materials = pieceMaterialsImposs;
                        }
                        piece[i].transform.localEulerAngles = new Vector3(180 * (p - 1), 0, 0);//����v���C���[�̐F�̌����ɂ���
                        piece[i].gameObject.SetActive(true);//���\������
                    }
                    else
                    {
                        //�}�E�X�����̃}�X�̏�ɂȂ���΋���\���ɂ���
                        piece[i].gameObject.SetActive(false);
                    }
                }

            }
        }
        return false;
    }

    //���Ԃ���Ƃ��낪�܂����邩�ǂ����̔��� �Ȃ��ꍇfalse��Ԃ�
    private bool end_judgment(int p)
    { //p�̋�̔���
        byte[,][] n = B_sco[(p - 1) % 2];
        int ans = 0;
        for (byte y = 0; y < n.GetLength(1); y++)
        {
            for (byte x = 0; x < n.GetLength(0); x++)
            {
                ans += total(n[x, y]);
            }
        }
        if (ans == 0) return false;
        else return true;
    }

    private bool end_judgment()
    { //�S���̔���
        bool tmp = false;
        for (byte y = 0; y < box.GetLength(1); y++)
        {
            for (byte x = 0; x < box.GetLength(0); x++)
            {
                if (Get_piece(x, y) == 0)
                {
                    tmp = true;
                }
            }
        }
        return tmp && (end_judgment(1) || end_judgment(2));
    }

    /*//p�̋�(��1��2)�̍��v�_
    private int Allpoints_total(int p)
    {
        byte ans = 0;
        for (byte y = 0; y < box.GetLength(1); y++)
        {
            for (byte x = 0; x < box.GetLength(0); x++)
            {
                if (box[x, y] == p)
                {
                    ans++;
                }
            }
        }
        return ans;
    }
    */

    private bool wait(float wtime)
    {
        wait();
        return time >= wtime;
    }
    private void wait()
    {
        time += Time.deltaTime;
    }


    public void speedUP(float speed) //0�ɋ߂��قǑ����Ȃ�
    {
        this.speed = (float)speed;
    }
    public void speedUP(bool Up = true)
    {
        if (Up)
        {
            speedUP(0.1f);
        }
        else
        {
            speedUP(1);
        }
    }
}
