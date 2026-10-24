using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.XR;
using Yarn.Unity;


public class CharacterManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] GameObject charactersParent;
    [SerializeField] Image character1Image;
    [SerializeField] Image character2Image;
    [SerializeField] CharacterDatabase characterDatabase;

    Character character1;
    Character character2;

    [YarnCommand("start_character_conv")]
    public void StartCharacterConversation(string char1, string char2 = "")
    {
        //enable the portraits
        character1Image.sprite = characterDatabase.GetCharacterPose(char1);
        character1 = characterDatabase.GetCharacter(char1);
        if (char2.Length > 0)
        {
            character2Image.sprite = characterDatabase.GetCharacterPose(char2);
            character2 = characterDatabase.GetCharacter(char2);
        }
        character2Image.enabled = char2.Length > 0;
        charactersParent.SetActive(true);

    }

    [YarnCommand("show_character_expression")]
    public void ShowCharacterExpression(string characterName, string characterExpression)
    {
        Sprite characterSprite = characterDatabase.GetCharacterPose(characterName, characterExpression);
        //show character epxression
        if (characterName == character1.characterName)
        {
            character1Image.sprite = characterSprite;
        }
        else if (characterName == character2.characterName)
        {
            character2Image.sprite = characterSprite;
        }
    }

    [YarnCommand("end_character_conv")]
    public void EndCharacterConversation()
    {
        //disable the portraits
        charactersParent.SetActive(false);

    }

}
