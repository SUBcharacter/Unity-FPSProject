using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource UIAudio;
    [SerializeField] AudioClip placeClip;
    [SerializeField] AudioClip clickClip;
    [SerializeField] Button[] buttons;

    private void Awake()
    {
        buttons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (var button in buttons)
        {
            button.onClick.AddListener(OnClick);

            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();
            if(eventTrigger == null)
            {
                eventTrigger = button.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((_) => OnSelect());
            eventTrigger.triggers.Add(entry);
        }
    }

    public void OnSelect()
    {
        UIAudio.clip = placeClip;
        UIAudio.Play();
    }

    public void OnClick()
    {
        UIAudio.clip = clickClip;
        UIAudio.Play();
    }
}
