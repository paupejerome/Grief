using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ChoiceHandler : MonoBehaviour
{
    [SerializeField] Text choiceText;
    [SerializeField] Image arrow;
    float arrowY= 50;
    float lineSpacing = 30;
    Vector3 arrowDefaultPosition;
    Image choiceBox;
    int choiceQuantity;
    int selectionIndex;
    bool isSelected = false;
    bool[] availableChoices;

    private void Start()
    {
        choiceBox = this.GetComponent<Image>();
        arrowDefaultPosition = arrow.rectTransform.localPosition;
    }

    public void OnConfirmSelection(InputAction.CallbackContext context)
    {
        if (context.performed && choiceBox.enabled)
        {
            isSelected = true;
        }
    }

    public void OnNavigationInput(InputAction.CallbackContext context)
    {
        if (context.performed && choiceBox.enabled)
        {
            selectionIndex = (int)Mathf.Repeat((selectionIndex - (int)context.ReadValue<Vector2>().y), choiceQuantity);
            arrow.rectTransform.localPosition = new Vector3(arrow.rectTransform.localPosition.x, arrowY - (lineSpacing * selectionIndex), arrow.rectTransform.localPosition.z);        
        }
    }

    void ShowChoiceBox(bool setter)
    {
        choiceText.enabled = setter;
        arrow.enabled = setter;
        choiceBox.enabled = setter;
    }

    public IEnumerator DoChoice(Choice[] choices, System.Action<int> selected)
    {
        selectionIndex = 0;
        arrow.rectTransform.localPosition = arrowDefaultPosition;
        availableChoices = new bool[choices.Length];

        ShowChoiceBox(true);

        int i = 0;
        choiceQuantity = 0;
        foreach (Choice choice in choices)
		{
            if(choice.minMood <= GameState.mood && choice.maxMood >= GameState.mood)
            {
                availableChoices[i] = true;
                choiceQuantity++;
                choiceText.text += choice.choiceText;
                choiceText.text += "\n";
            }
            i++;
        }

        while (!isSelected)
            yield return null;

        isSelected = false;
        choiceText.text = "";
        ShowChoiceBox(false);
        for (int j = 0; j < availableChoices.Length; j++)
        {  
            if (availableChoices[j])
            {
                selectionIndex--;
            }
            if(selectionIndex == -1)
            {
                selected(j);
                break;
            }
        }
    }
}
