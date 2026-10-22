using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDatabase : ScriptableObject
{
    public List<Character> characters;
    

    public Sprite GetCharacterPose(string characterName, string poseName = "default")
    {
        Character character = characters.Find(character => character.characterName == characterName);
        if (character == null) return null;
        switch (poseName)
        {
            case "default":
                return character.defaultPose;
            case "angry":
                return character.angry;
            case "sad":
                return character.sad;
            case "happy":
                return character.happy;
        }
        return null;
    }

    public Character GetCharacter(string characterName)
    {
        Character character = characters.Find(character => character.characterName == characterName);
        return character;
    }
}
