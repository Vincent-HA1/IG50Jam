using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class DialogueInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineAdvancer lineAdvancer;
    [SerializeField] private InputActionReference nextLineAction;

    public bool canAdvanceText = true;
    private void OnEnable()
    {
        nextLineAction.action.performed += OnSubmit;
        nextLineAction.action.Enable();
    }

    private void OnDisable()
    {
        nextLineAction.action.performed -= OnSubmit;
        nextLineAction.action.Disable();
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (!canAdvanceText) return;
        lineAdvancer.OnInputHurryUpLines();
        lineAdvancer.OnInputHurryUpOptions();

    }
}
