using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OpeningTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    public PlayerController playerController;

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
        playerController.enabled = false;
        //playerController.rb.velocity = Vector2.zero;

        // Start the timeline
        timeline.Play();

        // Subscribe to the timeline's stopped event
        timeline.stopped += OnCutsceneEnded;

        // Disable the trigger collider to prevent re-triggering
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnCutsceneEnded(PlayableDirector director)
    {
        // Re-enable player control
        playerController.enabled = true;

        // Unsubscribe from the event
        timeline.stopped -= OnCutsceneEnded;
    }
}
