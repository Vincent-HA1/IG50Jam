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
    [SerializeField] List<Button> allTravelArrows;
    [SerializeField] Animator SceneFadeAnimator;
    [SerializeField] GameObject startingRoom;
    [SerializeField] GameObject itemsCanvas;
    [SerializeField] Image itemImage;

    [Header("Settings")]
    [SerializeField] bool playCutsceneOnStart = false;
    [SerializeField] int indexOfCutsceneToPlay;

    List<Evidence> inventory = new List<Evidence>();

    List<Button> clickableElements;

    EventSystem eventSystem;
    GameObject currentRoom;
    public static bool cannotAct = false;
    public bool fadedOut = false;


    [YarnCommand("obtain_evidence")]
    public void Add_Evidence(string evidenceName)
    {
        //Have a dictionary to map evidence name to evidence files
        print($"Added {evidenceName} to the inventory");
        Evidence evidenceToAdd = allEvidence.Find(evidence => evidence.evidenceName == evidenceName);
        if (evidenceToAdd != null)
        {
            //Add to inventory
            if (!inventory.Contains(evidenceToAdd)) inventory.Add(evidenceToAdd);
            //add to the HUD too
            hudManager.AddEvidence(evidenceToAdd);
        }
    }

    [YarnCommand("show_evidence")]
    public void Show_Evidence(string evidenceName)
    {
        //Have a dictionary to map evidence name to evidence files
        Evidence evidenceToShow = allEvidence.Find(evidence => evidence.evidenceName == evidenceName);
        if (evidenceToShow != null)
        {
            itemsCanvas.SetActive(true);
            itemImage.sprite = evidenceToShow.evidenceFull;
        }
    }


    [YarnCommand("hide_evidence")]
    public void Hide_Evidence()
    {
        itemsCanvas.SetActive(false);
    }


    [YarnCommand("block_travel")]
    public void Block_Travel()
    {
        //disable all arrows
        allTravelArrows.ForEach(button => button.gameObject.SetActive(false));
    }

    [YarnCommand("allow_travel")]
    public void Allow_Travel()
    {
        //enable all arrows
        allTravelArrows.ForEach(button => button.gameObject.SetActive(true));
    }

    [YarnCommand("goto_room")]
    public void GoToRoomFromDialogue(string roomName)
    {
        List<GameObject> objects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).ToList();

        TransitionRoom(objects.Find(obj => obj.name == roomName));
    }

    public void TransitionRoom(GameObject roomToGoTo)
    {
        FadeOut();
        StartCoroutine(GoToRoomAfterFade(roomToGoTo));
    }



    [YarnCommand("change_location")]
    public void GoToLocation(string locationName)
    {
        //Save 
        SaveSystem.SaveGame(SceneManager.GetActiveScene().name, inventory.Select(evidence => evidence.evidenceName).ToList());
        FadeOut();
        StartCoroutine(GoToSceneAfterFade(locationName));
    }

    [YarnCommand("enable_object")]
    public void EnableObject(string objectName)
    {
        List<GameObject> objects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).ToList();

        objects.Find(obj => obj.name == objectName).SetActive(true);
    }

    [YarnCommand("disable_object")]
    public void DisableObject(string objectName)
    {
        GameObject.Find(objectName).SetActive(false);
    }

    [YarnCommand("end_game")]
    public void EndGame()
    {
        //Save 
        SaveSystem.DeleteSave();
        FadeOut();
        StartCoroutine(GoToSceneAfterFade("TitleScreen"));
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
            List<string> evidenceNames = loadedSave.evidence;
            foreach (string evidenceName in evidenceNames)
            {
                Add_Evidence(evidenceName);
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
        if (playCutsceneOnStart)
        {
            fadedOut = true; //already faded out when scene starts
            dialogueController.PlayCutscene(indexOfCutsceneToPlay);
        }
        else
        {
            StartCoroutine(WaitForFade());
        }
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
    void LateUpdate()
    {
        //Disable selection for non dialogue
        if (eventSystem.enabled && !DialogueUIController.dialogueIsPlaying )
        {
            eventSystem.SetSelectedGameObject(null);
        }
        //if (Input.GetKeyDown("l"))
        //{
        //    SaveSystem.SaveGame(SceneManager.GetActiveScene().name, inventory);
        //}
    }




}
