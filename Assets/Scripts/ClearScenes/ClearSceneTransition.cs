using UnityEngine;

public class ClearSceneTransition : MonoBehaviour
{
    [SerializeField] GameObject FadeObject;

    private Fade fade;

    void Start()
    {
        if (FadeObject == null)
        {
            Debug.LogError("[ClearSceneTransition] FadeObjectが設定されていません！", this);
            return;
        }

        fade = FadeObject.GetComponent<Fade>();

        if (fade != null)
        {
            // Fade側もStart()での準備が終わっているので、ここで呼べばエラーにならない
            fade.FadeRange = 1f;

            // 1秒でFadeOutする
            fade.FadeOut(1f);
        }
        else
        {
            Debug.LogError("[ClearSceneTransition] FadeObjectにFadeコンポーネントがついていません！", this);
        }
    }
}