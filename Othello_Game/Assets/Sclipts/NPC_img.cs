
using UnityEngine;
using UnityEngine.UI;

//NPCの考えている画像を出す

public class NPC_img : MonoBehaviour
{
    [SerializeField]
    private Sprite[] NPC_thinking;
    private Image img;
    private int nowSprite=-1;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void nomal()
    {
        gameObject.SetActive(false);
    }

    public void thinking(int turn)
    {
        //表示されていなかったら表示する
        if (!gameObject.active)
        {
            gameObject.SetActive(true);
        }
        int t = turn % NPC_thinking.Length;
        //現在表示されている画像と違うものだったら画像を変える
        if (t != nowSprite)
        {
            Debug.Log(turn);
            img.sprite = NPC_thinking[t];
            nowSprite = t;
        }
    }
}
