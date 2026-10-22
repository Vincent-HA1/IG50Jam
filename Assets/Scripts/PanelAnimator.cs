using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelAnimator : MonoBehaviour
{
    public Action<bool> DrawerAnimationComplete;

    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Open()
    {
        anim.SetTrigger("Open");
    }

    public void Hide()
    {
        anim.SetTrigger("Close");
    }

    public void SetOpen()
    {
        DrawerAnimationComplete?.Invoke(true);
    }

    public void SetHidden()
    {
        DrawerAnimationComplete?.Invoke(false);
    }
}
