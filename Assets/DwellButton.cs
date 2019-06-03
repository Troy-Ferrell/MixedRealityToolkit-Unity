using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EyeTrackingTarget))]
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
    private List<InteractableProfileItem> Profiles = new List<InteractableProfileItem>();
   
    // the state logic for comparing state
    public InteractableStates StateManager;

    // the list of running theme instances to receive state changes
    private List<InteractableThemeBase> runningThemes = new List<InteractableThemeBase>();
    
    // a collection of states and basic state logic
    private States EyeGazeStates;

    private DateTime cursorEnterTime;
    private bool hadFocus = false;
    private bool wasSelected = false;

    private void Awake()
    {
        EyeGazeStates = States.GetDefaultInteractableStates();

        StateManager = EyeGazeStates.SetupLogic();

        SetupThemes();

        // Init variables
        cursorEnterTime = DateTime.MaxValue;
    }

    protected virtual void SetupThemes()
    {
        // TODO: Check Profiles[0].Themes[0].States?

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

                if (cursorOnTime > timeToTriggerFocusInSec 
                    && !IsStateActive(InteractableStates.InteractableStateEnum.Focus))
                {
                    SetStateActive(InteractableStates.InteractableStateEnum.Focus, true);
                }

                if (cursorOnTime > timeToTriggerDwellInSec
                    && !IsStateActive(InteractableStates.InteractableStateEnum.Targeted))
                {
                    SetStateActive(InteractableStates.InteractableStateEnum.Targeted, true);

                }

                if (cursorOnTime > timeToTriggerSelectInSec)
                {
                    // kill everything and don't reset till
                    wasSelected = true;

                    // perform selection on interactable?

                    // TODO: Will this ease values back?
                    //ResetStates();
                }
            }
        }

        for (int i = 0; i < runningThemes.Count; i++)
        {
            if (runningThemes[i].Loaded)
            {
                var s = StateManager.CurrentState();
                int t = StateManager.CurrentState().ActiveIndex;

                runningThemes[i].OnUpdate(StateManager.CurrentState().ActiveIndex, false);
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
