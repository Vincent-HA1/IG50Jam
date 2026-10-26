using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class LockpickManager : Minigame
{
    [Header("References")]
    [SerializeField] Image lockCircle;
    [SerializeField] Image pointerArrow;
    [SerializeField] TMPro.TextMeshProUGUI timerText;
    [SerializeField] Transform targetSectionsParent;
    [SerializeField] GameObject targetSectionPrefab;

    [SerializeField] float rotateSpeed = 20f;
    [SerializeField] int minTargetSections = 4;
    [SerializeField] int maxTargetSections = 6;
    [SerializeField] float minAngleDiff = 5;
    [SerializeField] float maxAngleDiff = 10;
    [SerializeField] int duration = 10;

    List<TargetSection> targetSections = new List<TargetSection>();
    int targetSectionsNo;
    float sectionAngleOffset;
    float timer = 0;

    int targetSectionsHit = 0;

    bool minigameActive = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        targetSectionsHit = 0;
        minigameActive = true;
        DeleteTargetSections();
        timer = duration;
        sectionAngleOffset = targetSectionPrefab.GetComponent<Image>().fillAmount * 360;
        targetSectionsNo = UnityEngine.Random.Range(minTargetSections, maxTargetSections + 1);
        float angle = 360 / (float)targetSectionsNo;
        //Create stuff
        for (int i = 0; i < targetSectionsNo; i++)
        {
            float angleTotal = angle * i + UnityEngine.Random.Range(minAngleDiff, maxAngleDiff);
            GameObject section = Instantiate(targetSectionPrefab, targetSectionsParent);
            section.transform.rotation = Quaternion.Euler(0, 0, -angleTotal);
            TargetSection newSection = new TargetSection(section, angleTotal);
            newSection.angleBoundMin = angleTotal;
            newSection.angleBoundMax = angleTotal + sectionAngleOffset;
            targetSections.Add(newSection);
        }
    }

    void DeleteTargetSections()
    {
        foreach(TargetSection targetSection in targetSections)
        {
            Destroy(targetSection.targetSection);
        }
        targetSections.Clear();
    }


    private void Update()
    {
        if (!minigameActive) return;
        if (Input.GetKeyDown(KeyCode.Mouse0)||Input.GetKeyDown(KeyCode.Space))
        {
            CompareToSections();
        }
        CheckTimer();
    }

    void CheckTimer()
    {
        timerText.text = String.Format("{0:0.00}", timer);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            MinigameFailed?.Invoke();
            minigameActive = false;
        }

    }

    void CompareToSections()
    {
        float angleCheck = Mathf.Repeat(
            (pointerArrow.transform.eulerAngles.z - 270) * -1,
            360f
        );
        foreach (TargetSection targetSection in targetSections)
        {
            if (targetSection.WithinBounds(angleCheck))
            {
                //Hit target
                HitTarget(targetSection.targetSection);
            }
        }
    }
    void HitTarget(GameObject targetSection)
    {
        targetSection.SetActive(false);
        targetSectionsHit += 1;
        if(targetSectionsHit >= targetSectionsNo)
        {
            //If Hit all targets
            //End Minigame
            MinigameCleared?.Invoke();
            minigameActive = false;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!minigameActive) return;
        pointerArrow.transform.RotateAround(lockCircle.transform.position, Vector3.forward, -rotateSpeed * Time.deltaTime);
    }
}

public class TargetSection
{
    public GameObject targetSection;
    public float angle;
    public float angleBoundMin;
    public float angleBoundMax;

    public TargetSection(GameObject targetSection, float angle)
    {
        this.targetSection = targetSection;
        this.angle = angle;
    }

    public bool WithinBounds(float angle)
    {
        return angle >= angleBoundMin && angle<=angleBoundMax;
    }

}

public class Minigame: MonoBehaviour
{
    public Action MinigameFailed;
    public Action MinigameCleared;

    public string minigameName;

    public virtual void StartMinigame()
    {

    }

    public virtual void RetryMinigame()
    {

    }

}