using UnityEngine;

public class ClearSceneTransition : MonoBehaviour
{
    [SerializeField] GameObject FadeObject;

    private Fade fade;

    void Awake()
    {
        fade = FadeObject.GetComponent<Fade>();
        // Fadeにする
        fade.FadeRange = 1f;
    }

    void Start()
    {
        // 1秒でFadeOutする
        fade.FadeOut(1f);
    }

}
