using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    //public Animator endingAnimator;
    public Canvas scoreboardCanvas;
    public CanvasGroup scoreboardCanvasGroup;
    public float fadeDuration = 1f;


    // Move player to right position
    public Vector2 playerEndPosition;
    public float playerEndXPosition;

    public float moveSpeed = 5f;
    public GameObject player;
    private Rigidbody2D playerRb;
    private PlayerController playerController;


    // Player stats
    //private int retries = 0;
    //private int jumps = 0;
    //private int sizeChanges = 0;
    public Text retriesText;
    public Text jumpsText;
    public Text sizeChangesText;
    public float timeForOneIncrement = 0.1f;


    // Timeline
    public PlayableDirector timeline;
    public float timelineLength;

    // Stars
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public float timeBetweenStars;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        playerController = player.GetComponent<PlayerController>();
    }

    // When player reaches the end
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(EndGameSequence());
        }
    }


    // Main IEnumerator to manage the entire sequence
    private IEnumerator EndGameSequence()
    {
        playerController.SetMovement(false);
        StartCoroutine(MovePlayerToPosition());


        // Play ending animation
        timeline.Play();
        // Wait for animation to finish
        yield return new WaitForSeconds(timelineLength);


        scoreboardCanvasGroup.alpha = 0f;
        scoreboardCanvas.gameObject.SetActive(true);

        // Fade in scoreboard
        StartCoroutine(FadeInUI());
        yield return new WaitForSeconds(1f);
        // Start stats counting
        StartCoroutine(IncrementStats(retriesText, 0, LevelManager.Instance.getRetries()));
        StartCoroutine(IncrementStats(jumpsText, 0, LevelManager.Instance.getJumps()));
        StartCoroutine(IncrementStats(sizeChangesText, 0, LevelManager.Instance.getSizeChanges()));

        StartCoroutine(ShowStars(playerController.playerSize, timeBetweenStars));

    }


    private IEnumerator MovePlayerToPosition()
    {
        while (playerRb.position.x != playerEndXPosition)
        {
            // Calculate new X position
            float newX = Mathf.MoveTowards(playerRb.position.x, playerEndXPosition, moveSpeed * Time.deltaTime);

            // Create new position vector, keeping Y the same
            Vector2 newPosition = new Vector2(newX, playerRb.position.y);

            // Apply new position
            playerRb.position = newPosition;

            yield return null;
        }
    }


    private IEnumerator FadeInUI()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            scoreboardCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }


    // Count up to the stats
    private IEnumerator IncrementStats(Text textTarget, int currentStats, int targetStats)
    {
        while(currentStats < targetStats)
        {
            currentStats++;
            string currentStatsString = currentStats.ToString();
            textTarget.text = currentStatsString;
            yield return new WaitForSeconds(timeForOneIncrement);
        }
    }

    // Show Stars
    private IEnumerator ShowStars(int starNumber, float timeBetweenStars)
    {
        if(starNumber >= 1)
        {
            star1.SetActive(true);
            yield return new WaitForSeconds(timeBetweenStars);
        }
        if (starNumber >= 2)
        {
            star2.SetActive(true);
            yield return new WaitForSeconds(timeBetweenStars);
        }
        if (starNumber == 3)
        {
            star3.SetActive(true);

        }
    }


}
