using UnityEngine;

//駒をめくる動き

public class piece_rotate : MonoBehaviour
{
    //public float speed=0.1f;
    [SerializeField]
    private float height = 0.5f;
    [SerializeField]
    private float wtime = 0.8f; //回る時間
    private bool r = false;//回転開始のトリガー
    private int p = 0;//表に見せている面 黒0 白1
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (r)
        {
            time += Time.deltaTime;

            //時間の比率から現在の角度を計算
            this.transform.localEulerAngles = new Vector3(180*(-p+1)+(p*2-1)*180*time/wtime,0,0);
            //めくるときに埋まらないように持ち上げる
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y,Mathf.Sin(Mathf.PI*time/wtime)/100);
            
            if (time>wtime)
            {
                //Debug.Log("コマ回転終わり"+p);
                this.transform.localEulerAngles = new Vector3(180*p, 0, 0);
                this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
                r = false;
            }
        }
        
    }
    
    public void rotate_start(int tp)
    {
        time =0;
        this.p = (tp-1)%2;
        //Debug.Log("コマ回転はじめ"+p);
        r = true;
    }
}
