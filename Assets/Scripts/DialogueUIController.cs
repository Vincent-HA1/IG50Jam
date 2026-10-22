
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueUIController : MonoBehaviour
{

    public Action<bool> DialogueFinished;

    [Header("References")]
    [SerializeField] private DialogueRunner runner;
    [SerializeField] private GameObject darkBackground;
    [SerializeField] GameObject clickBlocker;
    [SerializeField] GameManager gameManager;
    [SerializeField] DialogueInput dialogueInput;

    [Header("Cutscenes")]
    [SerializeField] List<Cutscene> cutscenes;
    [SerializeField] GameObject cutsceneCanvas;
    [SerializeField] Image cutsceneImage;
    [SerializeField] Image nextCutsceneImage;
    [SerializeField] float cutsceneTransitionSpeed = 5;

    public static bool dialogueIsPlaying = false;
    bool cutscenePlaying = false;
    Cutscene currentCutscene;
    int currentCutsceneImageIndex;

    Color transparentColor = new Color(1, 1, 1, 0);
    float colorLerp = 0;

    [YarnCommand("show_cutscene")]
    public void PlayCutscene(int cutsceneIndex)
    {
        //show cutscene by enabling object and playing through the images after each text
        if (cutsceneIndex >= cutscenes.Count || cutscenePlaying) return;
        cutscenePlaying = true;
        currentCutscene = cutscenes[cutsceneIndex];
        if (currentCutscene != null)
        {
            //Enable cutscene
            StartCoroutine(StartCutsceneAfterFade());
        }
    }

    IEnumerator StartCutsceneAfterFade()
    {
        //Fade out and play cutscene
        StartCoroutine(gameManager.FadeOutAndIn());
        dialogueInput.canAdvanceText = false;
        yield return null;
        yield return new WaitUntil(() => gameManager.fadedOut);
        StartCutscene();
        yield return new WaitUntil(() => GameManager.cannotAct == false);
        //run dialogue
        DialoguePlay(currentCutscene.dialogueNodeTitle);
        yield return new WaitForSeconds(0.5f);
        dialogueInput.canAdvanceText = true;
    }

    [YarnCommand("next_cutscene_image")]
    public void NextCutsceneImage()
    {
        //show next image
        currentCutsceneImageIndex += 1;
        if (currentCutsceneImageIndex >= currentCutscene.cutsceneImages.Count) return;
        StartCoroutine(TransitionToNextImage());

    }


    [YarnCommand("light_off")]
    public void LightOff()
    {
        darkBackground.SetActive(true);
    }

    [YarnCommand("light_on")]
    public void LightOn()
    {
        darkBackground.SetActive(false);
    }

    public void SetDialoguePlaying(bool isPlaying)
    {
        dialogueIsPlaying = isPlaying;
        clickBlocker.SetActive(dialogueIsPlaying);
        //If dialogue has finished
        if(!dialogueIsPlaying)
        {
            if (cutscenePlaying)
            {
                //End cutscene
                StartCoroutine(EndCutsceneAfterFade());
            }
            else
            {
                DialogueFinished?.Invoke(true);
            }
        }
    }

    // Called when the player clicks your UI button
    public void DialoguePlay(string nodeTitle)
    {
        if (runner == null || runner.IsDialogueRunning || nodeTitle.Length <= 0)
            return;
        runner.StartDialogue(nodeTitle);
    }

    private void Start()
    {
        dialogueIsPlaying = false;
    }


    void StartCutscene()
    {
        //Need to scene fade out and then in
        currentCutsceneImageIndex = 0;
        cutsceneImage.sprite = currentCutscene.cutsceneImages[currentCutsceneImageIndex];
        if(currentCutscene.cutsceneImages.Count > 1) nextCutsceneImage.sprite = currentCutscene.cutsceneImages[currentCutsceneImageIndex + 1];
        nextCutsceneImage.color = transparentColor;
        cutsceneCanvas.SetActive(true);

    }

    void EndCutscene()
    {
        cutsceneCanvas.SetActive(false);
        cutscenePlaying = false;
        //DialogueFinished?.Invoke(true);
    }

    IEnumerator EndCutsceneAfterFade()
    {
        StartCoroutine(gameManager.FadeOutAndIn());
        yield return null;
        yield return new WaitUntil(() => gameManager.fadedOut);
        EndCutscene();
    }

    IEnumerator TransitionToNextImage()
    {
        dialogueInput.canAdvanceText = false;
        nextCutsceneImage.color = transparentColor;
        colorLerp = 0;
        //Lerp alpha of temp image to 1, and then set the initial cutscene image to the same image. then reset the temp image to 0 alpha once again, hiding it
        while (colorLerp < 1)
        {
            nextCutsceneImage.color = Color.Lerp(transparentColor, Color.white, colorLerp);
            colorLerp = Mathf.Clamp(colorLerp + Time.deltaTime * cutsceneTransitionSpeed, 0, 1) ;
            yield return null;
        }
        //The new image is visible now, so set the cutscene image to the new image
        cutsceneImage.sprite = nextCutsceneImage.sprite;
        yield return null;
        nextCutsceneImage.color = transparentColor;
        //If there is a next image after this, set it
        if (currentCutsceneImageIndex < currentCutscene.cutsceneImages.Count - 1)
        {
            nextCutsceneImage.sprite = currentCutscene.cutsceneImages[currentCutsceneImageIndex + 1];
        }
        dialogueInput.canAdvanceText = true;
    }


}
