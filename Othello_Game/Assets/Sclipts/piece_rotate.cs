using UnityEngine;

//����߂��铮��

public class piece_rotate : MonoBehaviour
{
    //public float speed=0.1f;
    [SerializeField]
    private float height = 0.5f;
    [SerializeField]
    private float wtime = 0.8f; //��鎞��
    private bool r = false;//��]�J�n�̃g���K�[
    private int p = 0;//�\�Ɍ����Ă���� ��0 ��1
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

            //���Ԃ̔䗦���猻�݂̊p�x���v�Z
            this.transform.localEulerAngles = new Vector3(180*(-p+1)+(p*2-1)*180*time/wtime,0,0);
            //�߂���Ƃ��ɖ��܂�Ȃ��悤�Ɏ����グ��
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y,Mathf.Sin(Mathf.PI*time/wtime)/100);
            
            if (time>wtime)
            {
                //Debug.Log("�R�}��]�I���"+p);
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
        //Debug.Log("�R�}��]�͂���"+p);
        r = true;
    }
}
