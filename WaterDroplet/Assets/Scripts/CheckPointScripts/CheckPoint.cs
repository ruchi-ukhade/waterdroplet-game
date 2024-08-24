using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject respawnPoint;
    public PlayerController playerController;
    private bool saved = false;


    public GameObject progressSavedPanal;
    private CanvasGroup canvasGroup;
    public float fadeInDuration = 0.5f; // Duration of the fade-in effect
    public float fadeOutDuration = 0.5f; // Duration of the fade-out effect


    private void Start()
    {
        canvasGroup = progressSavedPanal.GetComponent<CanvasGroup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !saved)
        {
            saved = true;
            StartCoroutine(FadeIn());
            LevelManager.Instance.SetCheckpoint(respawnPoint.transform.position);
            LevelManager.Instance.SetPlayerSize(playerController.playerSize);
        }
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;

        //canvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;

    }
}
