using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button continueButton;
    [SerializeField] Button newGame;
    [SerializeField] Animator SceneFadeAnimator;

    SaveClass loadedSaveFile;

    EventSystem eventSystem;

    private void Awake()
    {
        eventSystem = EventSystem.current;
        eventSystem.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Check for save file
        SaveClass saveFile = SaveSystem.LoadGame();
        continueButton.gameObject.SetActive(saveFile != null);
        loadedSaveFile = saveFile;
        StartCoroutine(WaitForFade());
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        yield return new WaitForNextFrameUnit();
        eventSystem.enabled = true;
        if (continueButton.isActiveAndEnabled)
        {
            eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(newGame.gameObject);
        }
    }


    public void Continue()
    {
        //load the save file
        StartCoroutine(GoToSceneAfterFade(loadedSaveFile.locationName));
    }

    public void NewGame()
    {
        //Delete save
        SaveSystem.DeleteSave();
        StartCoroutine(GoToSceneAfterFade(1));
    }
    public void QuitGame()
    {
        Application.Quit();
    }


    IEnumerator GoToSceneAfterFade(string sceneName)
    {
        SceneFadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //Now go to scene
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator GoToSceneAfterFade(int buildIndex)
    {
        SceneFadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => SceneFadeAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        //Now go to scene
        SceneManager.LoadScene(buildIndex);
    }
}
