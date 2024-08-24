using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpeningTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    public PlayerController playerController;

    public float timeToShrink = 4.3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCutscene();
        }
    }

    private void StartCutscene()
    {
        // Disable player control
        playerController.SetMovement(false);
        //playerController.enabled = false;
        //playerController.rb.velocity = Vector2.zero;

        // Start the timeline
        timeline.Play();

        // Start player size change over time
        StartCoroutine(ChangeSizeDuringTimeline());

        // Subscribe to the timeline's stopped event
        timeline.stopped += OnCutsceneEnded;

        // Disable the trigger collider to prevent re-triggering
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnCutsceneEnded(PlayableDirector director)
    {
        // Re-enable player control
        //playerController.enabled = true;
        playerController.SetMovement(true);

        // Unsubscribe from the event
        timeline.stopped -= OnCutsceneEnded;
    }

    // change player size over time
    private IEnumerator ChangeSizeDuringTimeline()
    {
        // After a while shrink character's size
        yield return new WaitForSeconds(timeToShrink);
        playerController.changeSize(false);

        yield return new WaitForSeconds(timeToShrink);
        playerController.changeSize(false);
    }
}
