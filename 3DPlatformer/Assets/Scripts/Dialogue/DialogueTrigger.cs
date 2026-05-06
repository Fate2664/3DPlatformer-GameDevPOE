using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script manages player interaction with the dialogue trigger and what dialogues to show
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager DialogueManager;
    [SerializeField] private List<DialogueBase> dialogueList;
    [SerializeField] private bool loadSceneOnDialogueEnd;
    [SerializeField] private string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || DialogueManager.HasStartedDialogue) return;
        DialogueManager.StartDialogueSequence(dialogueList, loadSceneOnDialogueEnd ? LoadSceneAfterDialogue : null);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
    }

    private void LoadSceneAfterDialogue()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
