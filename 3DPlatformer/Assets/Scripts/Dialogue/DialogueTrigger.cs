using System.Collections.Generic;
using UnityEngine;

//This script manages player interaction with the dialogue trigger and what dialogues to show
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager DialogueManager;
    [SerializeField] private List<DialogueBase> dialogueList;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || DialogueManager.HasStartedDialogue) return;
        DialogueManager.StartDialogueSequence(dialogueList);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
    }
}
