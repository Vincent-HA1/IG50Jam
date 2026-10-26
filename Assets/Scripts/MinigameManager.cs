using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MinigameManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] DialogueUIController dialogueManager;
    [SerializeField] GameObject minigameCanvas;
    [SerializeField] List<Minigame> minigames;

    public Action<string> ClearedMinigame;

    string nodeToPlay;
    string failNode;

    Minigame currentMinigame;

    [YarnCommand("play_minigame")]
    public void StartMinigame(string minigameName, string dialogueNodeToPlayOnCompletion, string dialogueFail)
    {
        nodeToPlay = dialogueNodeToPlayOnCompletion;
        failNode = dialogueFail;
        //enable canvas
        //Find minigame
        Minigame minigame = minigames.Find(m => m.name == minigameName);
        if (minigame)
        {
            minigameCanvas.SetActive(true);
            minigame.gameObject.SetActive(true);
            currentMinigame = minigame;
            currentMinigame.StartMinigame();
        }
    }

    public void EndMinigame()
    {
        //Run from UI perhaps
        minigameCanvas.SetActive(false);
        currentMinigame.gameObject.SetActive(false);
        dialogueManager.DialoguePlay(nodeToPlay);
    }

    void RetryMinigame()
    {
        dialogueManager.DialoguePlay(failNode);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Bind events
        foreach (Minigame minigame in minigames)
        {
            minigame.MinigameCleared += EndMinigame;
            minigame.MinigameFailed += RetryMinigame;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
