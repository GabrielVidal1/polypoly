using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopScreen : MonoBehaviour
{
    [SerializeField] private Button goButton;
    [SerializeField] private Image loadingImage;
    [SerializeField] private float pauseDuration = 3f;
    
    private CanvasGroup _canvasGroup;
    
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void Open()
    {
        _canvasGroup.alpha = 1f;
        StartCoroutine(GoButtonDelay());
    }

    IEnumerator GoButtonDelay()
    {
        goButton.interactable = false;

        loadingImage.fillAmount = 0f;
        
        for (float i = 0f; i < pauseDuration; i += Time.deltaTime)
        {
            loadingImage.fillAmount = i / pauseDuration;
            yield return 0;
        }

        loadingImage.fillAmount = 1f;
        goButton.interactable = true;
    }

    public void Go()
    {
        GameManager.instance.localPlayer.Refuel();
        _canvasGroup.alpha = 0f;
    }
}
