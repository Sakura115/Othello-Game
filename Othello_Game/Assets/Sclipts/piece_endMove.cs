using UnityEngine;

//ゲーム最後に駒を画面外に出して消す処理

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
            //処理が始まっていて、動作が終わっていたら消す
            this.gameObject.SetActive(false);
        }
    }

    public void moveStart()
    {
        //この関数が呼び出されたら処理を開始する
        GetComponent<move>().moveStart(new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + endPieceUpHeight));
        start = true;
    }
}
