using System;
using System.Collections;
using System.Collections.Generic;
using Platformer;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueVisuals dialogueVisuals;
    [SerializeField] private InputReader input;

    private  QueueBase<DialogueBase> dialogueQueue = new();
    private  QueueBase<string> sentences = new();
    private bool hasStartedDialogue = false;

    private void Update()
    {
        if (input.NextPressed)
        {
            DisplayNextDialogueText();
        }
    }

    public bool HasStartedDialogue
    {
        get => hasStartedDialogue;
        set => hasStartedDialogue = value;
    }

    public void StartDialogue(DialogueBase dialogue)
    {
        dialogueQueue.Clear();
        dialogueQueue.Enqueue(dialogue);
        BeginQueuedDialogues();
    }

    public void StartDialogueSequence(List<DialogueBase> dialogues)
    {
        dialogueQueue.Clear();
        foreach (DialogueBase dialogue in dialogues)
        {
            dialogueQueue.Enqueue(dialogue);
        }
        BeginQueuedDialogues();
    }

    private void BeginQueuedDialogues()
    {
        StopAllCoroutines();
        sentences.Clear();

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        hasStartedDialogue = true;
        dialogueVisuals.Show();
        StartNextDialogueBase();
    }

    private void StartNextDialogueBase()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        sentences.Clear();

        DialogueBase dialogue = dialogueQueue.Dequeue();
        dialogueVisuals.NameText.Text = dialogue.DialogueName;

        foreach (string textBlock in dialogue.DialogueText)
        {
            sentences.Enqueue(textBlock);
        }

        DisplayNextDialogueText();
    }

    private void DisplayNextDialogueText()
    {
        StopAllCoroutines();
        if (sentences.Count == 0)
        {
            StartNextDialogueBase();
            return;
        }

        string textToDisplay = (string)sentences.Dequeue();
        dialogueVisuals.DialogueText.Text = textToDisplay;
        StartCoroutine(ShowDialogueText(textToDisplay));
    }

    private IEnumerator ShowDialogueText(string textToDisplay)
    {
        dialogueVisuals.DialogueText.Text = "";
        foreach (char letter in textToDisplay.ToCharArray())
        {
            dialogueVisuals.DialogueText.Text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3f);

        DisplayNextDialogueText();
    }

    private void EndDialogue()
    {
        hasStartedDialogue = false;
        dialogueVisuals.Hide();
    }
}
