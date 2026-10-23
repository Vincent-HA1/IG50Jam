using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public Action GoToTitle;

    [Header("Pause Menu References")]
    [SerializeField] GameObject pauseMenu;
    
    [Header("Evidence References")]
    [SerializeField] Panel evidencePanel;
    [SerializeField] Transform evidenceParent;
    [SerializeField] GameObject evidencePrefab;
    [SerializeField] EvidenceScreen evidenceScreen;
    [SerializeField] GameObject blockClicks;

    List<EvidenceIcon> evidenceIcons = new List<EvidenceIcon>();

    bool gamePaused = false;
    bool casePanelOpen = false;
    void Start()
    {
        //test
    }

    // Update is called once per frame
    void Update()
    {
        if (evidencePanel.isOpen)
        {
            blockClicks.SetActive(true);
        }
        else
        {
            blockClicks.SetActive(false);
        }

        //Pause Menu
        if (!GameManager.cannotAct && !DialogueUIController.dialogueIsPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
            {
                ClosePauseMenu();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        pauseMenu.SetActive(true);
        gamePaused = true;
    }

    public void ClosePauseMenu()
    {
        //For now just deactivate gameobject
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void QuitToTitle()
    {
        GoToTitle?.Invoke();
    }

    public void AddEvidence(Evidence evidenceToAdd)
    {
        //Spawn an evidence icon in the inventory panel
        //hook up events
        GameObject newEvidenceIcon = Instantiate(evidencePrefab, evidenceParent);
        EvidenceIcon evidenceIcon = newEvidenceIcon.GetComponent<EvidenceIcon>();
        evidenceIcon.InitialiseEvidence(evidenceToAdd);
        evidenceIcon.EvidenceClicked += ShowEvidence;//.onClick.AddListener(ShowEvidence); //Should pass in evidence details here
        evidenceIcons.Add(evidenceIcon);    
    }

    void ShowEvidence(Evidence evidenceToShow)
    {
        //Using evidence passed in to populate evidence screen. For nwo just enable the gameohkect
        evidenceScreen.gameObject.SetActive(true);
        evidenceScreen.SetUI(evidenceToShow.evidenceFull, evidenceToShow.description);
        
    }
}
