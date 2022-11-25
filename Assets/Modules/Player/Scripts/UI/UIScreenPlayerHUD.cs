using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using Modules.UIScreen;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIScreenPlayerHUD : UIScreen
{
    [SerializeField] private Button m_ammoButtomRef;

    [SerializeField] private RectTransform m_ammoButtonsContainer;

    private Dictionary<DiscardTypes, Button> m_ammoButtons = new();

    // Public methods -----------------------------------------------------------------------------------------------

    public override Tween Show(bool freeze = false, float duration = 0.5F)
    {
        return base.Show(freeze, duration);
    }

    public override Tween Hide(bool freeze = false, float duration = 0.5F)
    {
        return base.Hide(freeze, duration);
    }

    // Function to add ammo buttons to the HUD
    public void AddAmmoButton(DiscardTypes type, UnityAction OnClick)
    {
        // Clone the ammo button prefab
        Button ammoButtonClone = Instantiate(m_ammoButtomRef, m_ammoButtonsContainer);

        // Add the OnClick event to the button
        ammoButtonClone.onClick.AddListener(OnClick);

        // Add the button to the dictionary
        m_ammoButtons.Add(type, ammoButtonClone);

        ammoButtonClone.gameObject.SetActive(true);
    }

    public void UpdateAmmoButton(DiscardTypes type, int ammoCount)
    {
        // Get the button from the dictionary
        Button ammoButton = m_ammoButtons[type];

        // Update the ammo count text
        ammoButton.GetComponentInChildren<TMP_Text>().text = ammoCount.ToString();
    }

    // Unity methods ------------------------------------------------------------------------------------------------

    private void Awake()
    {
        m_ammoButtomRef.gameObject.SetActive(false);
    }

    // Private methods ----------------------------------------------------------------------------------------------
}