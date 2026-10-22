using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Panel : MonoBehaviour
{

    [Header("References")]
    [SerializeField] PanelAnimator panelAnimator;
    public bool isOpen { get; private set; }

    bool animating = false;

    void Start()
    {
        panelAnimator.DrawerAnimationComplete += SetAnimatingStatus;
    }


    public void CheckCasePanel()
    {
        if (animating) return;
        animating = true;
        if (isOpen)
        {
            panelAnimator.Hide();
        }
        else
        {
            panelAnimator.gameObject.SetActive(true);
            panelAnimator.Open();
            isOpen = true;
        }
    }


    void SetAnimatingStatus(bool isOpen)
    {
        animating = false;
        this.isOpen = isOpen;
        if(!isOpen) panelAnimator.gameObject.SetActive(false);
    }


}
