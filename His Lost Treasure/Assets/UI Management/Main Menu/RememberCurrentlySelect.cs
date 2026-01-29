using UnityEngine;
using UnityEngine.EventSystems;

public class RememberCurrentlySelect : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject rememberedSelectedObject;

    private void Reset()
    {
        eventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in the scene. Please add one.");
        }

        rememberedSelectedObject = eventSystem.currentSelectedGameObject;
    }

    private void Update()
    {
        if (!eventSystem)
            return;

        if (eventSystem.currentSelectedGameObject && rememberedSelectedObject != eventSystem.currentSelectedGameObject)
        {
            rememberedSelectedObject = eventSystem.currentSelectedGameObject;
        }


        if (!eventSystem.currentSelectedGameObject && rememberedSelectedObject)
        {
            eventSystem.SetSelectedGameObject(rememberedSelectedObject);
        }
    }
 









}
