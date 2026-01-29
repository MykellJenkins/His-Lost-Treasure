using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetUiElementToSelect : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private EventSystem eventsystem;
    [SerializeField] private Selectable uiElementToSelect;

    [Header("Visualizaiton")]
    [SerializeField] private bool visualizeInEditor;
    [SerializeField] private Color navigationColour = Color.green;

    private void Reset()
    {
        eventsystem = Object.FindAnyObjectByType<EventSystem>();

        if (eventsystem == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one.");
        }
    }

    public void JumpToElement()
    {
        if (eventsystem == null)
        {
            Debug.LogWarning("EventSystem reference is missing. Cannot set selected UI element.");

            if (uiElementToSelect == null)
            {
                Debug.LogWarning("UI Element to select reference is missing.");
            }

            eventsystem.SetSelectedGameObject(uiElementToSelect.gameObject);
        }
    }








}
