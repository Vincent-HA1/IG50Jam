using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvidenceScreen : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Image evidenceImage;
    [SerializeField] TMPro.TextMeshProUGUI evidenceDescriptionText;

    public void SetUI(Sprite evidenceSprite, string evidenceDescription)
    {
        evidenceImage.sprite = evidenceSprite;
        evidenceDescriptionText.text = evidenceDescription; 
    }
}
