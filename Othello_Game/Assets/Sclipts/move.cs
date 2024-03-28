using UnityEngine;

//ï®ÇìÆÇ©Ç∑(ÉJÉÅÉâÇ‚ÉRÉ}Ç‚UIÇ»Ç«)

public class move : MonoBehaviour
{
    [SerializeField] private float speed = 0.1f;
    private bool moving = false;
    private bool noRotate = false;
    private Vector3 target_xyz, rotate_xyz;
    private float accS;//â¡ë¨speed (â¡éZ)
    private float accS_t;//â¡ë¨speed éûä‘Ç…ÇÊÇÈâ¡éZ(ÇæÇÒÇæÇÒëÅÇ≠Ç»ÇÈ)
    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        accS = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            if (obj_move(target_xyz, rotate_xyz))
            {
                transform.localPosition = target_xyz;
                if (!noRotate)
                {
                    transform.localEulerAngles = rotate_xyz;
                }
                accS_t = 0;
                moving = false;
            }
        }
    }
    private bool obj_move(Vector3 target_xyz, Vector3 rotate_xyz)
    {
        accS_t += 0.1f * Time.deltaTime;
        float position = Mathf.Abs((target_xyz - transform.localPosition).magnitude);
        if (!noRotate)
        {
            float rotate = Mathf.Abs((rotate_xyz - transform.localEulerAngles).magnitude);
            transform.localPosition = Vector3.Lerp(transform.localPosition, target_xyz, (speed + accS + accS_t) * position / rotate * Time.deltaTime * 10);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, rotate_xyz, (speed + accS + accS_t) * rotate / position * Time.deltaTime * 10);
            return position < 0.001 && rotate < 0.001;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target_xyz, (speed + accS + accS_t) * Time.deltaTime * 10);
            return position < 0.001;
        }
    }

    public void moveStart(Vector3 target_position, Vector3 taget_rotate)
    {
        moving = true;
        target_xyz = target_position;
        rotate_xyz = taget_rotate;
        noRotate = false;
        //moving = obj_move(target_xyz, rotate_xyz);
    }
    public void moveStart(Vector3 target_position)
    {
        moveStart(target_position, transform.localEulerAngles);
        noRotate = true;
    }

    public bool get_moving()
    {
        return moving;
    }

    public float get_speed()
    {
        return speed;
    }

    public void speedChange(float n)
    {
        speed = (float)n;
    }

    public void acceleration(float n)
    {
        accS = n;
        Debug.Log(this.gameObject.name+"â¡ë¨ "+n);
    }
}
