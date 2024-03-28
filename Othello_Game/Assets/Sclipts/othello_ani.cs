using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]

//タイトル画面、アニメーション用

public class othello_ani : MonoBehaviour
{
    [SerializeField] private TimelineAsset up;
    [SerializeField] private TimelineAsset down;
    private PlayableDirector director;

    // Start is called before the first frame update
    void Start()
    {
        director = this.GetComponent<PlayableDirector>();
        up_start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void up_start()
    {
        director.Play(up);
    }

    public void down_start()
    {
        director.Play(down);
    }

}
