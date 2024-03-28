using UnityEngine;
using System;

//メインゲーム処理

public class Game_System : MonoBehaviour

{   // 盤面,box[横,縦]
    // 空:0 黒:1 白:2
    private byte[,] box = new byte[8, 8];

    //各マスの置いた場合に裏返せる数[黒0,白1][横,縦][方向]
    private byte[][,][] B_sco = new byte[2][,][];

    public GameObject[] piece;//駒のモデル

    [SerializeField]
    private GameObject TurnText;//ターンを表示するテキストオブジェクト
    [SerializeField]
    private GameObject PassText;//パスを表示するテキストテキストオブジェクト
    [SerializeField]
    private GameObject PassButton;//パスの入力を受け取るボタン
    [SerializeField]
    private GameObject GameEnd;//ゲーム終了の処理をするスクリプトのついたオブジェクト
    [SerializeField]
    private GameObject EndMenu;//ゲーム終了時に表示するメニュー
    [SerializeField]
    private float waitTime = 0.5f;//前のプレイヤーの入力から次のプレイヤーの入力に移る時間
    [SerializeField]
    private float npcWaitTime = 3f;//NPCが悩む時間
    private float time = 0f;//ゲーム上の時間を管理する

    private bool playerTurn = false;    //今誰のターンか 黒(先)fallse 白(後)true
    public static int playMode = 3;//0:2人対戦 1:1人対戦(先）2:1人対戦（後） 3:NPC対戦
    [SerializeField]
    private float pieceR = 1.1f;//駒の半径


    private Material[] pieceMaterials;//駒の置いたときのマテリアル
    private Material[] pieceMaterialsImposs;//駒の置くことができない時のマテリアル
    private Material[] pieceMaterialsPoss;//駒の置くことができるときのマテリアル
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

    private int consecutivePass = 0;//連続pass数


    [SerializeField]
    private GameObject speedButton;//スピードを変える用のボタン

    [SerializeField]
    private GameObject NPC_thinking;//NPCの悩むアイコン

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
        Debug.Log("ゲーム開始:プレイモード" + playMode);
        pieceMaterials = new Material[] { pieceBlackMaterial, pieceWhiteMaterial };
        pieceMaterialsImposs = new Material[] { pieceBlackMaterialImpossibleToPut, pieceWhiteMaterialImpossibleToPut };
        pieceMaterialsPoss = new Material[] { pieceBlackMaterialPossibleToPut, pieceWhiteMaterialPossibleToPut };
        reset();
        speedUP(speedButton.GetComponent<speedUP_button>().speedButtoOn());//スピードアップボタンがオンかどうか取得
    }

    // Update is called once per frame
    void Update()
    {
        byte n = 1;

        if (end_judgment() && consecutivePass < 2) //ゲームが終了していない時、ターンの表示
        {
            TurnText.GetComponent<UnityEngine.UI.Text>().text = playerTurn ? "白の番" : "黒の番";
        }

        if (wait(waitTime))//前回の入力から規定時間たっていたら
        {
            if (end_judgment() && consecutivePass < 2) //裏返せるところがあり、パスが連続していない場合
            {
                if (!PassText.GetComponent<Pass_text>().get_display()) //パスが表示されていなければ
                {
                    //プレイヤーをセットしプレイさせる
                    player_set();
                }
            }
            else
            {
                //裏返せるところがない、またはパスが連続しているときしばらく待って終了する
                if (wait(waitTime))
                {
                    //修了画面への移行処理
                    Destroy(PassText);
                    Destroy(PassButton);
                    Destroy(NPC_thinking);
                    Destroy(TurnText);
                    GameEnd.GetComponent<Game_End>().end();
                }
            }
        }



    }


    // 盤面をリセットする(初期位置に)
    private void reset()
    {
        // 升目をすべてコマなしにする
        for (byte y = 0; y < box.GetLength(1); y++)
        {
            for (byte x = 0; x < box.GetLength(0); x++)
            {
                put_piece(x, y, 0);
            }
        }
        // オセロの初期位置に駒を置く
        byte n = 3;
        put_piece(n, n, 2);
        put_piece(n + 1, n, 1);
        put_piece(n, n + 1, 1);
        put_piece(n + 1, n + 1, 2);

        //全てのマスの裏返せる数を計算する
        put_Allscore();
    }

    // 盤面の比率を返す
    public int size()
    {
        return box.GetLength(0);
    }

    //コマの値を返す
    public int Get_piece(int x, int y)
    {
        return box[x, y];
    }

    //配列x[]の合計を返す
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

    // x,yのマスにpの駒(黒1白2)を置いた場合,横dx縦dy方向の駒は何枚裏返せるか数える
    private int count_point(int x, int y, int p, int dx, int dy)
    {
        int count = 0;
        // 繰り返し(自分の駒が出てくるまで続ける)
        // (予期せぬ動作をしたときにずっと繰り返しを続けさせないため上限(盤面の比率)を設けた)
        for (byte i = 1; i < size() + 1; i++)
        {
            // 範囲外になった場合
            if (x + dx * i < 0 || x + dx * i > box.GetLength(0) - 1 || y + dy * i < 0 || y + dy * i > box.GetLength(1) - 1)
            {
                return 0;//自分の駒がなかった場合、0を返す
            }
            else
            {
                // 相手の駒だった場合カウントを1増やす
                if (box[x + dx * i, y + dy * i] == (p * -1 + 3))
                {
                    count++;
                }
                else if (box[x + dx * i, y + dy * i] == p)// 自分の駒だった場合今までのカウント数を返す
                {
                    return count;
                }
                else // 空きマスや想定外の数字の場合0を返す
                {
                    return 0;
                }
            }
        }
        return 0;
    }

    // x,yのマスにpの駒(黒1白2)を置いた場合,相手の駒を全部で何枚裏返せるか数える
    private int count_Allpoint(int x, int y, int p)
    {
        if (box[x, y] == 0)
        {
            // 全方向の合計
            return (count_point(x, y, p, -1, 0) + count_point(x, y, p, -1, 1) + count_point(x, y, p, 0, 1)
                    + count_point(x, y, p, 1, 1) + count_point(x, y, p, 1, 0) + count_point(x, y, p, 1, -1)
                    + count_point(x, y, p, 0, -1) + count_point(x, y, p, -1, -1));
        }
        else
        {
            // 置こうとしたマスに駒がおいてあった場合、0を返す
            return 0;
        }
    }
    public int count_Allpoint(int x, int y, int p, int[] n)
    {// n[]に右から反時計回りの順で各方向の裏返せる数を返す
        if (box[x, y] == 0)
        {
            byte[] ans = new byte[8];
            // nの配列とansの配列に右から反時計回りの順で裏返せる数を入れる
            for (byte i = 0; i < 8; i++)
            {
                ans[i] = (byte)count_point(x, y, p, (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)), -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)));
                if (i < n.Length)
                {
                    n[i] = (int)ans[i];
                }
            }
            // 全方向の合計
            return total(ans);

        }
        else
        {
            // 置こうとしたマスに駒がおいてあった場合、0を返す
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
    {// n[]に右から反時計回りの順で各方向の裏返せる数を返す
        if (box[x, y] == 0)
        {
            byte[] ans = new byte[8];
            // nの配列とansの配列に右から反時計回りの順で裏返せる数を入れる
            for (byte i = 0; i < 8; i++)
            {
                ans[i] = (byte)count_point(x, y, p, (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)), -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)));
                if (i < n.Length)
                {
                    n[i] = ans[i];
                }
            }
            // 全方向の合計
            return total(ans);

        }
        else
        {
            // 置こうとしたマスに駒がおいてあった場合、0を返す
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


    //pの駒(黒1白2)の全てのマスの裏返せる数を入れる
    private void put_Allscore(int p)
    {
        p--;
        p %= 2;//想定外の数が入力されてエラーが起きないための保険
        for (byte y = 0; y < B_sco[p].GetLength(1); y++)
        {
            for (byte x = 0; x < B_sco[p].GetLength(0); x++)
            {
                count_Allpoint(x, y, p + 1, B_sco[p][x, y]);
            }

        }
    }
    private void put_Allscore()
    {//両方の駒の全てのマスの裏返せる数を入れる
        put_Allscore(1);
        put_Allscore(2);
    }

    // x,yのマスにpの駒(黒1白2)を置く。値が不正だった場合,また置けないマスに置こうとした場合falseを返す
    private bool put(int x, int y, int p)//x,y コマの位置　p　コマの色(0:空 1:黒 2:白)
    {

        // 置こうとしたマスの裏返せる数が0だった場合,値が不正だった場合
        if (!put_judgement(x, y, p))
        {
            Debug.Log("コマが置けません" + p);
            return false;
        }
        else
        {
            byte[] n = B_sco[p - 1][x, y];
            // 全方面の裏返せる駒の数の分裏返す
            for (byte i = 0; i < n.Length; i++)
            {
                for (byte k = 1; k < n[i] + 1; k++)
                {
                    // 駒を裏返す
                    int tx = x + (int)Mathf.Round(Mathf.Cos(Mathf.PI / 4 * i)) * k;
                    int ty = y + -1 * (int)Mathf.Round(Mathf.Sin(Mathf.PI / 4 * i)) * k;
                    box[tx, ty] = (byte)p;
                    //Debug.Log("コマ回転" + tx + ty );
                    piece[tx + ty * 8].GetComponent<piece_rotate>().rotate_start(p);//駒のオブジェクトに反映
                }
            }
            put_piece(x, y, p);//駒のオブジェクト、データ配列に反映する
            //全てのマスの裏返せる数を更新する
            put_Allscore();
            return true;
        }
    }


    private void put_piece(int x, int y, int p)//x,y コマの位置　p　コマの色(0:空 1:黒 2:白)
    {
        box[x, y] = (byte)p;
        if (p == 0)
        { piece[x + y * 8].gameObject.SetActive(false); }
        else
        {
            p--;
            p %= 2;//不正な値の除外

            //Debug.Log("コマを置きました" + x + y + p);
            piece[x + y * 8].GetComponent<MeshRenderer>().materials = pieceMaterials;//駒のマテリアルを通常のものにする
            piece[x + y * 8].transform.localEulerAngles = new Vector3(180 * p, 0, 0);//駒の面を指定された色の面に変える
            piece[x + y * 8].gameObject.SetActive(true);//駒を表示する
            /*float tmp = -Rset * 8 / 2-Rset/2;
            GameObject piece = Instantiate(piecePrefab, this.transform.position + new Vector3(tmp+Rset*x,0,tmp+Rset*y), Quaternion.Euler(Euler));
            piece.gameObject.transform.parent = this.gameObject.transform;
            piece.transform.eulerAngles = new Vector3(180*p - 90, 0, 0);
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);*/
        }
    }

    private bool put_judgement(int x, int y, int p)
    {
        // 裏返せる数が0だった場合,値が不正だった場合
        if ((p < 0 || p > 2) || (x < 0 || x >= box.GetLength(0)) || (y < 0 || y >= box.GetLength(1)) || total(B_sco[(p - 1) % 2][x, y]) == 0)
        {
            return false;
        }
        return true;
    }

    private void player_set()
    {
        int p = Convert.ToInt32(playerTurn) + 1;//ターン(黒1白2)
        //Debug.Log("プレイヤー" + p);


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
                //少し待ってからNPCを起動する
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

    private bool player_notNPC(int p)//pターン(黒1白2)のプレイヤーが人かNPCか(true:人 false:NPC)
    {
        const bool P = true;//人の時に返すもの
        const bool N = false;//NPCの時に返すもの

        //0:2人対戦 1:1人対戦(先）2:1人対戦（後） 3:NPC対戦
        switch (playMode)
        {
            case 0:return P;//人
            case 1: return !playerTurn ? P : N;//先(黒)の時、人　後(白)の時、NPC; 
            case 2: return !playerTurn ? N : P;//先(黒)の時、NPC　後(白)の時、人
            case 3: return N;//NPC

            default:return N;//例外の時、NPC
        }
    }

    public void pass()
    {
        Debug.Log("Pass" + (!playerTurn?"黒":"白") + "," + consecutivePass);

        playerTurn = !playerTurn;//ターンを変える
        PassText.GetComponent<Pass_text>().display_Pass();//パスメッセージの表示
        consecutivePass++;//連続パス数を増やす
        PassButton.gameObject.SetActive(false);//パスボタンを非表示にする
    }

    //もっともたくさん裏返せるところにpの駒(黒1白2)を置くNPC
    private bool NPC(int p)
    {
        p %= 3;
        //角のマスにおける場合、優先して置く
        for (byte y = 0; y < 2; y++)
        {
            for (byte x = 0; x < 2; x++)
            {
                if (put_judgement((box.GetLength(0) - 1) * x, (box.GetLength(1) - 1) * y, p))//左右上下の角マスに置くことができるか判断する
                {
                    return put((box.GetLength(0) - 1) * x, (box.GetLength(1) - 1) * y, p);//置くことができる場合、億
                }
            }
        }

        //0:一番大きい値,1:そのx座標,2:そのy座標
        byte[] max = new byte[3];

        //探し始める点(置く位置にランダム性を持たせる)
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

    private bool Player(int p)//true　入力完了
    {
        PassButton.gameObject.SetActive(true);//パスボタンの表示

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//マウス位置からレイを取得
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray, out hit))
        {
            for (byte i = 0; i < piece.Length; i++)//マスの数だけ繰り返す
            {
                //マス上に駒が置かれていない場合のみ処理をする
                if (box[i % 8, i / 8] == 0)//もし空きマスだったら
                {
                    //マウスがそのマスの上にあったら
                    if (Vector3.Distance(hit.point, piece[i].transform.position) < pieceR)//レイの当たった位置がそのマスだったら(そのマスに置かれる駒の中心点からの距離がコマの半径以下だったら)
                    {
                        //ルール上置けるかどうか判断する
                        if (put_judgement(i % 8, i / 8, p))//置けるとき
                        {
                            piece[i].GetComponent<MeshRenderer>().materials = pieceMaterialsPoss;//置ける時のマテリアルにする

                            if (Input.GetMouseButtonDown(0))//空きマスの上でマウスの左ボタンが押された時
                            {
                                //駒を置く
                                //Debug.Log("put");
                                put(i % 8, i / 8, p);
                                return true;
                            }
                        }
                        else
                        {
                            //置けない時、置けないときのマテリアルにする
                            piece[i].GetComponent<MeshRenderer>().materials = pieceMaterialsImposs;
                        }
                        piece[i].transform.localEulerAngles = new Vector3(180 * (p - 1), 0, 0);//駒をプレイヤーの色の向きにする
                        piece[i].gameObject.SetActive(true);//駒を表示する
                    }
                    else
                    {
                        //マウスがそのマスの上になければ駒を非表示にする
                        piece[i].gameObject.SetActive(false);
                    }
                }

            }
        }
        return false;
    }

    //裏返せるところがまだあるかどうかの判定 ない場合falseを返す
    private bool end_judgment(int p)
    { //pの駒の判定
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
    { //全部の判定
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

    /*//pの駒(黒1白2)の合計点
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


    public void speedUP(float speed) //0に近いほど早くなる
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
