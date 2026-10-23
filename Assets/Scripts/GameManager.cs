using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Yarn.Unity;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] HUDManager hudManager;
    [SerializeField] DialogueUIController dialogueController;

    [Header("References")]
    [SerializeField] List<Evidence> allEvidence;
    [SerializeField] Animator SceneFadeAnimator;
    [SerializeField] GameObject startingRoom;

    List<Evidence> inventory = new List<Evidence>();

    List<Button> clickableElements;

    EventSystem eventSystem;
    GameObject currentRoom;
    public static bool cannotAct = false;
    public bool fadedOut = false;

    public void TransitionRoom(GameObject roomToGoTo)
    {
        FadeOut();
        StartCoroutine(GoToRoomAfterFade(roomToGoTo));
    }

    public void GoToLocation(string locationName)
    {
        //Save 
        SaveSystem.SaveGame(SceneManager.GetActiveScene().name, inventory);
        FadeOut();
        StartCoroutine(GoToSceneAfterFade(locationName));
    }

    void FadeOut()
    {
        cannotAct = true;
        SceneFadeAnimator.SetTrigger("FadeOut");
        EnableClickableElements(false);
        fadedOut = false;
    }

    void FadeIn()
    {
        SceneFadeAnimator.SetTrigger("FadeIn");
        StartCoroutine(WaitForFade());
        fadedOut = false;
    }

    public IEnumerator FadeOutAndIn()
    {
        FadeOut();
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        fadedOut = true;
        yield return new WaitForNextFrameUnit();
        FadeIn();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Load Save
        SaveClass loadedSave = SaveSystem.LoadGame();
        //Add all evidence back to the inventory
        if (loadedSave != null)
        {
            inventory = loadedSave.evidence;
            foreach (Evidence evidence in inventory)
            {
                Add_Evidence(evidence.evidenceName);
            }
        }
        hudManager.GoToTitle += () =>
        {
            FadeOut();
            StartCoroutine(GoToSceneAfterFade("TitleScreen"));
        };
        dialogueController.DialogueFinished += (bool allowClicks) => StartCoroutine(EnableClickableElementsCoroutine(allowClicks));
        //Retrieve clickable elements
        eventSystem = EventSystem.current;
        clickableElements = FindObjectsOfType<Button>(true).Where(x => x.CompareTag("Clickable")).ToList();
        EnableClickableElements(false);
        currentRoom = startingRoom;
        StartCoroutine(WaitForFade());
    }


    IEnumerator WaitForFade()
    {
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        yield return new WaitForNextFrameUnit();
        EnableClickableElements(true);
        cannotAct = false;
    }

    IEnumerator GoToRoomAfterFade(GameObject roomToGoTo)
    {
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //Now fade in after going to next room
        yield return null;
        currentRoom.SetActive(false);
        roomToGoTo.SetActive(true);
        currentRoom = roomToGoTo;
        FadeIn();
    }

    IEnumerator GoToSceneAfterFade(string sceneName)
    {
        eventSystem.enabled = false;
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //Now go to scene
        SceneManager.LoadScene(sceneName);
    }



    IEnumerator EnableClickableElementsCoroutine(bool isInteractable)
    {
        cannotAct = !isInteractable;
        yield return new WaitForSeconds(0.1f);
        EnableClickableElements(isInteractable);
    }

    void EnableClickableElements(bool isInteractable)
    {
        foreach (var clickableElement in clickableElements)
        {
            clickableElement.interactable = isInteractable;
        }
    }



    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("l"))
        //{
        //    SaveSystem.SaveGame(SceneManager.GetActiveScene().name, inventory);
        //}
    }

    [YarnCommand("obtain_evidence")]
    public void Add_Evidence(string evidenceName)
    {
        //Have a dictionary to map evidence name to evidence files
        print($"Added {evidenceName} to the inventory");
        Evidence evidenceToAdd = allEvidence.Find(evidence => evidence.evidenceName == evidenceName);
        if (evidenceToAdd != null)
        {
            //Add to inventory
            if(!inventory.Contains(evidenceToAdd)) inventory.Add(evidenceToAdd);
            //add to the HUD too
            hudManager.AddEvidence(evidenceToAdd);
        }
    }


}
