using UnityEngine;

//タイトル画面での処理

public class Title : MonoBehaviour
{

    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject othello;
    [SerializeField]
    private GameObject startButton;
    [SerializeField]
    private GameObject modeMenu;
    [SerializeField]
    private GameObject modeMenu2;
    private bool mode_input = false;
    private bool camera_set = false;

    private float time;
    private bool StartOn;
    //private float move, rotate;

    [SerializeField]
    private float waitTime = 0.5f;
    [SerializeField]
    private Vector3 camera_xyz = Vector3.zero;
    [SerializeField]
    private Vector3 camera_rotate = Vector3.zero;
    //public float speed = 0.1f;

    /*
    [SerializeField]
    private Vector3 board_xyz = Vector3.zero;
    [SerializeField]
    private Vector3 board_rotate = Vector3.zero;
    [SerializeField]
    private GameObject board;*/

    void Start()
    {
        modeMenu.gameObject.SetActive(false);
        modeMenu2.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        /*
        startButton.gameObject.SetActive(false);
        Debug.Log("up");
        othello.GetComponent<othello_ani>().up_start();
        */
        //Time.timeScale = 0;
        time = 0f;
        StartOn = false;
        mode_input = false;
        camera_set = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (StartOn)
        {
            if (!camera_set)
            {

                if (!mainCamera.GetComponent<move>().get_moving())
                {
                    Debug.Log("カメラ移動完了");
                    modeMenu.gameObject.SetActive(true);
                    camera_set = true;
                }
            }
            if (camera_set)
            {
                if (mode_input)
                {
                    toNext(1);
                }
            }
        }
    }

    public void Game_Start()
    {
        Debug.Log("再生");
        //Time.timeScale = 1;        
        StartOn = true;
        Destroy(startButton);
        othello.GetComponent<othello_ani>().down_start();
        mainCamera.GetComponent<move>().moveStart(camera_xyz, camera_rotate);
        //board.GetComponent<move>().moveStart(board_xyz, board_rotate);
    }


    //sシーンへ遷移する
    private void toNext(int s)
    {
        Debug.Log("遷移");
        Application.LoadLevel(s);
    }

    private bool wait(float wtime)
    {
        time += Time.deltaTime;
        return time >= wtime;
    }


    public void menu(int n)
    {
        if (n == -1)
        {
            Destroy(modeMenu);
            modeMenu2.gameObject.SetActive(true);
        }
        else
        {
            Game_System.playMode = n;
            Debug.Log("プレイモード" + n);
            mode_input = true;
        }
    }


}
