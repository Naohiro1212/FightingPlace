using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject fillObject;

    private PlayerStatus status;

    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        status =
            GetComponentInParent<PlayerStatus>();

        if (slider == null)
        {
            Debug.LogError(
                "[HPBar] Slider‚ª‚ ‚è‚Ü‚¹‚ñ"
            );

            return;
        }

        if (status == null)
        {
            Debug.LogError(
                "[HPBar] PlayerStatus‚ª‚ ‚è‚Ü‚¹‚ñ"
            );

            return;
        }

        slider.minValue = 0;
        slider.maxValue = status.MaxHealth;

        UpdateHPBar();
    }


    private void Update()
    {
        if (slider == null ||
            status == null)
        {
            return;
        }

        UpdateHPBar();
    }


    private void UpdateHPBar()
    {
        slider.value =
            status.CurrentHealth;

        if (fillObject != null)
        {
            fillObject.SetActive(
                status.CurrentHealth > 0
            );
        }
    }
}