using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EyeTrackingTarget))]
[RequireComponent(typeof(Interactable))]
public class DwellButton : BaseFocusHandler
{
    [Tooltip("Configurable duration to trigger an event if a user has been looking at the target for more than this duration.")]
    [SerializeField]
    [Range(0, 20)]
    private float timeToTriggerFocusInSec = 0.35f;

    // TODO:
    //[Tooltip("Configurable duration to trigger an event if a user has been looking at the target for more than this duration.")]
    [SerializeField]
    [Range(0, 20)]
    private float timeToTriggerDwellInSec = 0.85f;

    // TODO:
    //[Tooltip("Configurable duration to trigger an event if a user has been looking at the target for more than this duration.")]
    [SerializeField]
    [Range(0, 20)]
    private float timeToTriggerSelectInSec = 1.25f;

    // list of profiles can match themes with gameObjects
    [SerializeField]
    [HideInInspector]
    private List<InteractableProfileItem> Profiles = new List<InteractableProfileItem>();
   
    // the state logic for comparing state
    public InteractableStates StateManager;

    // the list of running theme instances to receive state changes
    private List<InteractableThemeBase> runningThemes = new List<InteractableThemeBase>();
    
    // a collection of states and basic state logic
    public States EyeGazeStates;

    private Interactable interactable;

    private DateTime cursorEnterTime;
    private bool hadFocus = false;
    private bool wasSelected = false;

    private void Awake()
    {
        StateManager = EyeGazeStates.SetupLogic();

        SetupThemes();

        interactable = GetComponent<Interactable>();

        // Init variables
        cursorEnterTime = DateTime.MaxValue;
    }

    protected virtual void SetupThemes()
    {
        // TODO: Check Profiles[0].Themes[0].States? == EyeGazeStates

        // Flatten input profile & themes to our runningThemes
        for (int i = 0; i < Profiles.Count; i++)
        {
            for (int j = 0; j < Profiles[i].Themes.Count; j++)
            {
                Theme theme = Profiles[i].Themes[j];
                if (Profiles[i].Target != null && theme != null)
                    //&& theme.States.Equals(this.EyeGazeStates))
                {
                    for (int n = 0; n < theme.Settings.Count; n++)
                    {
                        InteractableThemePropertySettings settings = theme.Settings[n];
                        settings.Theme = InteractableProfileItem.GetTheme(settings, Profiles[i].Target);

                        runningThemes.Add(settings.Theme);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles highlighting targets when the cursor enters its hit box.
    /// </summary>
    protected virtual void Update()
    {
        // TODO: Don't do anything (or reset) if button disabled? Interactable => disable

        // If we had focus, but lost it, reset 
        if (!HasFocus && hadFocus)
        {
            // Turn off all states
            ResetStates();

            this.cursorEnterTime = DateTime.MaxValue;

            hadFocus = false;
            wasSelected = false;
        }
        else if (HasFocus)
        {
            if (!hadFocus)
            {
                this.cursorEnterTime = DateTime.UtcNow;

                hadFocus = true;
            }
            else if (!wasSelected)
            {
                double cursorOnTime = (DateTime.UtcNow - this.cursorEnterTime).TotalSeconds;
                bool isFocus = IsStateActive(InteractableStates.InteractableStateEnum.Focus);
                bool isTargeted = IsStateActive(InteractableStates.InteractableStateEnum.Targeted);

                if (cursorOnTime > timeToTriggerFocusInSec 
                    && !isFocus && !isTargeted)
                {
                    SetStateActive(InteractableStates.InteractableStateEnum.Focus, true);
                }

                if (cursorOnTime > timeToTriggerDwellInSec
                    && !isTargeted)
                {
                    SetStateActive(InteractableStates.InteractableStateEnum.Targeted, true);
                }

                if (cursorOnTime > timeToTriggerSelectInSec)
                {
                    // We were selected, reset states and wait till new focus
                    wasSelected = true;
                    ResetStates();

                    // Perform selection on the interactable
                    this.interactable.OnPointerClicked(null);
                }
            }
        }

        for (int i = 0; i < runningThemes.Count; i++)
        {
            if (runningThemes[i].Loaded)
            {
                var s = StateManager.CurrentState();
                int t = StateManager.CurrentState().ActiveIndex;

                runningThemes[i].OnUpdate(StateManager.CurrentState().ActiveIndex, this.interactable);
            }
        }
    }

    /// <inheritdoc />
    public override void OnBeforeFocusChange(FocusEventData eventData)
    {
        // If we're the new target object,
        // add the pointer to the list of focusers.
        if (eventData.NewFocusedObject == gameObject && eventData.Pointer.InputSourceParent.SourceType == InputSourceType.Eyes)
        {
            eventData.Pointer.FocusTarget = this;
            Focusers.Add(eventData.Pointer);
        }
        // If we're the old focused target object,
        // remove the pointer from our list.
        else if (eventData.OldFocusedObject == gameObject)
        {
            Focusers.Remove(eventData.Pointer);

            // If there is no new focused target
            // clear the FocusTarget field from the Pointer.
            if (eventData.NewFocusedObject == null)
            {
                eventData.Pointer.FocusTarget = null;
            }
        }
    }

    protected void ResetStates()
    {
        SetStateActive(InteractableStates.InteractableStateEnum.Focus, false);
        SetStateActive(InteractableStates.InteractableStateEnum.Targeted, false);
    }

    protected void SetStateActive(InteractableStates.InteractableStateEnum state, bool isActive)
    {
        StateManager.SetStateValue(state, isActive ? 1 : 0);

        StateManager.CompareStates();
    }

    protected bool IsStateActive(InteractableStates.InteractableStateEnum state)
    {
        int test = StateManager.GetStateValue((int)state);
        return test > 0;
    }
}
