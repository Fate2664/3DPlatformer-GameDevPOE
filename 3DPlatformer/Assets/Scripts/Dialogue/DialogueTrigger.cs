using System.Collections.Generic;
using DG.Tweening;
using Platformer;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script manages player interaction with the dialogue trigger and what dialogues to show
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager DialogueManager;
    [SerializeField] private List<DialogueBase> dialogueList;

    [Header("Cat Spawn")] [SerializeField] private CatSpawnManager catSpawnManager;
    [SerializeField] private Transform catSpawnPoint;
    [SerializeField] private bool despawnCatAfterDialogue;
    [SerializeField] private int despawnCatAfterDialogueIndex = 2;

    [Header("Scene Loading")] 
    [SerializeField] private bool loadSceneOnDialogueEnd;
    [SerializeField] private string sceneToLoad;

    private Cat spawnedCat;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || DialogueManager.HasStartedDialogue) return;

        if (catSpawnPoint != null && spawnedCat == null)
        {
           spawnedCat = catSpawnManager.SpawnAt(catSpawnPoint);
        }
        DialogueManager.StartDialogueSequence(dialogueList, loadSceneOnDialogueEnd ? LoadSceneAfterDialogue : null, despawnCatAfterDialogue ? DespawnCatAfterDialogue : null);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
    }

    private void LoadSceneAfterDialogue()
    {
        if (string.IsNullOrWhiteSpace(sceneToLoad))
            return;

        SceneManager.LoadScene(sceneToLoad);
    }

    private void DespawnCatAfterDialogue(int completedIndex)
    {
        if (spawnedCat == null) return;
        
        spawnedCat.transform.DOScale(0f, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Destroy(spawnedCat.gameObject);
        });
       
    }
}