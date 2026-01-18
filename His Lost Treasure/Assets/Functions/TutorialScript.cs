using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    int tutorialStep = 0;

    string[] tutorialMessages =
    {

        "Use WASD to move,Press Enter to Continue.",
        "Press SPACE to jump,Press Enter to Continue",
        "Reach the goal to win! Press Enter to end this dialog",
    };

    void Start()
    {
        ShowTutorial();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))

            NextStep();
    }



public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = tutorialMessages[tutorialStep];
    }

    public void NextStep()
    {
        tutorialStep++;

        if (tutorialStep >= tutorialMessages.Length)
        {
            tutorialPanel.SetActive(false);
        }
        else
        {
            tutorialText.text = tutorialMessages[tutorialStep];
        }
    }
}

