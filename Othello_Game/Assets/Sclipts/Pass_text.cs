using UnityEngine;

//パステキストの表示

public class Pass_text : MonoBehaviour
{
    private bool display = false;
    [SerializeField]
    private float WaitTime = 0.4f;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            if (wait(WaitTime))
            {
                display_Pass(false);
            }
        }
    }

    public void display_Pass(bool tmp)
    {
        gameObject.SetActive(tmp);
        display = tmp;
        time = 0;
    }
    public void display_Pass()
    {
        display_Pass(true);
    }

    public bool get_display()
    {
        return display;
    }

    private bool wait(float wtime)
    {
        time += Time.deltaTime;
        return time >= wtime;
    }
}
