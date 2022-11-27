using System.Collections.Generic;
using DG.Tweening;
using Enums;
using Modules.Factory;
using Modules.UIScreen;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIScreenPlayerHUD : UIScreen
{
    [SerializeField] private Button m_ammoButtomRef;
    [SerializeField] private RectTransform m_ammoButtonsContainer;

    [Space] [SerializeField] private TMP_Text m_cannonHealthText;
    [SerializeField] private TMP_Text m_collectorHealthText;

    [SerializeField] private SOTypeFactory m_trashTypes;

    private Dictionary<DiscardTypes, Button> m_ammoButtons = new();

    // Public methods -----------------------------------------------------------------------------------------------

    public override Tween Hide(bool freeze = false, float duration = 0.5F)
    {
        return base.Hide(freeze, duration);
    }

    public void SetButtonColorByType(DiscardTypes type)
    {
        if (!m_ammoButtons.TryGetValue(type, out var button)) return;

        var color = m_trashTypes.GetByType(type).Color;
        button.image.color = color;
    }

    public void SetButtonActionByType(DiscardTypes type, UnityAction action)
    {
        if (!m_ammoButtons.TryGetValue(type, out var button)) return;

        button.onClick.AddListener(action);
    }

    // Function to add ammo buttons to the HUD
    public void AddAmmoButton(DiscardTypes type)
    {
        // Clone the ammo button prefab
        Button ammoButtonClone = Instantiate(m_ammoButtomRef, m_ammoButtonsContainer);

        // Add the button to the dictionary
        m_ammoButtons.Add(type, ammoButtonClone);

        ammoButtonClone.gameObject.SetActive(true);
    }

    public void UpdateAmmoButton(DiscardTypes type, int ammoCount)
    {
        // Get the button from the dictionary
        Button ammoButton = m_ammoButtons[type];

        // Update the ammo count text
        ammoButton.GetComponentInChildren<TMP_Text>().text = $"{type.ToString()}\n{ammoCount}";
    }

    public void UpdateCannonHealth(int curHealth, int maxHealth)
    {
        m_cannonHealthText.text = $"Cannon Health: {curHealth}/{maxHealth}";
    }

    public void UpdateCollectorHealth(int curHealth, int maxHealth)
    {
        m_collectorHealthText.text = $"Collector Health: {curHealth}/{maxHealth}";
    }

    // Unity methods ------------------------------------------------------------------------------------------------

    private void Awake()
    {
        m_ammoButtomRef.gameObject.SetActive(false);
    }

    // Private methods ----------------------------------------------------------------------------------------------
}