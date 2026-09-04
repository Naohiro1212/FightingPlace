using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class WFX_LightFlicker : MonoBehaviour
{
    [SerializeField] private float lightTime = 0.08f;

    private Light muzzleLight;
    private Coroutine lightCoroutine;

    private void Awake()
    {
        muzzleLight = GetComponent<Light>();

        // ç≈èâÇÕè¡ÇµÇƒÇ®Ç≠
        muzzleLight.enabled = false;
    }

    public void Flash()
    {
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }

        lightCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        muzzleLight.enabled = true;

        yield return new WaitForSeconds(lightTime);

        muzzleLight.enabled = false;

        lightCoroutine = null;
    }
}