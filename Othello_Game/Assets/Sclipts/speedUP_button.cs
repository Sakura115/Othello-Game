
using UnityEngine;
using UnityEngine.UI;

//�X�s�[�h�A�b�v�{�^��(�I���I�t�`��)

public class speedUP_button : MonoBehaviour
{
    [SerializeField] private Color nomal_color;
    [SerializeField] private Color speedUp_color;
    [SerializeField] private GameObject GameSystem;
    [SerializeField] private GameObject GameEnd;
    private Button button;
    private bool buttonOn = false;

    // Start is called before the first frame update
    void Start()
    {
        button= GetComponent<Button>();
        speedUp(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void speedUp(bool Up)
    {
        buttonOn = Up;
        ColorBlock buttonColor = button.colors;
        //�F�����ւ���
        if (Up)
        {
            buttonColor.normalColor = speedUp_color;
            buttonColor.highlightedColor = nomal_color;
            buttonColor.pressedColor = nomal_color;
            buttonColor.selectedColor = speedUp_color;
        }
        else
        {
            buttonColor.normalColor = nomal_color;
            buttonColor.highlightedColor = speedUp_color;
            buttonColor.pressedColor = speedUp_color;
            buttonColor.selectedColor = nomal_color;
        }
        button.colors = buttonColor;
        GameSystem.GetComponent<Game_System>().speedUP(Up);
        GameEnd.GetComponent<Game_End>().speedUP(Up);
    }

    public void speedUp()
    {
        speedUp(!buttonOn);
    }

    public bool speedButtoOn()
    {
        return buttonOn;
    }
}
