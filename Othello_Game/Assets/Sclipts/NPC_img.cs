
using UnityEngine;
using UnityEngine.UI;

//NPC�̍l���Ă���摜���o��

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
        //�\������Ă��Ȃ�������\������
        if (!gameObject.active)
        {
            gameObject.SetActive(true);
        }
        int t = turn % NPC_thinking.Length;
        //���ݕ\������Ă���摜�ƈႤ���̂�������摜��ς���
        if (t != nowSprite)
        {
            Debug.Log(turn);
            img.sprite = NPC_thinking[t];
            nowSprite = t;
        }
    }
}
