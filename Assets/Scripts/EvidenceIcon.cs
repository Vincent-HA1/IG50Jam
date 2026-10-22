using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceIcon : MonoBehaviour
{
    public Action<Evidence> EvidenceClicked;
    [Header("References")]
    [SerializeField] Image evidenceImage;
    [SerializeField] Button button;

    Evidence evidence;

    public void InitialiseEvidence(Evidence evidence)
    {
        this.evidence = evidence;
        evidenceImage.sprite = evidence.evidenceIcon;
        button.onClick.AddListener(() => EvidenceClicked?.Invoke(evidence));
        //Do more later

    }
}
