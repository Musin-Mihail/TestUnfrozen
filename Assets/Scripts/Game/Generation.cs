using System.Collections.Generic;
using UnityEngine;
public class Generation
{
    private Vector3 centerLeft = new Vector3(-5, 0, 0);
    private Vector3 centerRight = new Vector3(5, 0, 0);
    private List<Vector3> characterLocationsLeft = new List<Vector3>();
    private List<Vector3> characterLocationsRight = new List<Vector3>();
    private Quaternion rotationLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion rotationRight = Quaternion.Euler(0, -180, 0);
    private List<Character> PoolCharacters = new List<Character>();
    private List<Character> characterLeft = new List<Character>();
    private List<Character> characterRight = new List<Character>();
    public Generation()
    {
        characterLocationsLeft.Add(new Vector3(-10, 0, 0));
        characterLocationsLeft.Add(new Vector3(-15, 0, 0));
        characterLocationsLeft.Add(new Vector3(-20, 0, 0));
        characterLocationsLeft.Add(new Vector3(-25, 0, 0));

        characterLocationsRight.Add(new Vector3(10, 0, 0));
        characterLocationsRight.Add(new Vector3(15, 0, 0));
        characterLocationsRight.Add(new Vector3(20, 0, 0));
        characterLocationsRight.Add(new Vector3(25, 0, 0));
    }
    public List<Character> StartGeneration()
    {
        List<Character> turn = new List<Character>();
        bool leftSide;
        foreach (var item in characterLocationsLeft)
        {
            leftSide = true;
            PoolAddCharacter(leftSide, item, centerLeft, characterLeft, rotationLeft, turn);
        }
        foreach (var item in characterLocationsRight)
        {
            leftSide = false;
            PoolAddCharacter(leftSide, item, centerRight, characterRight, rotationRight, turn);
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character tempCharacter = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = tempCharacter;
        }
        return turn;
    }
    private void PoolAddCharacter(bool leftSide, Vector3 position, Vector3 center, List<Character> characters, Quaternion rotation, List<Character> turn)
    {
        if (PoolCharacters.Count > 0)
        {
            Character tempCharacter = PoolCharacters[0];
            PoolCharacters.RemoveAt(0);
            tempCharacter.Reset(position, leftSide, center);
            turn.Add(tempCharacter);
            characters.Add(tempCharacter);
        }
        else
        {
            GameObject GO;
            GO = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Character"), position, rotation);
            Character character = new Character(GO, leftSide, center);
            turn.Add(character);
            characters.Add(character);
        }
    }
    public int GetCountCharactersLeft()
    {
        return characterLeft.Count;
    }
    public int GetCountCharactersRight()
    {
        return characterRight.Count;
    }
    public Character GetCharacterLeft(int index)
    {
        return characterLeft[index];
    }
    public Character GetCharacterRight(int index)
    {
        return characterRight[index];
    }
    public void AddCharacterPool(Character character)
    {
        PoolCharacters.Add(character);
    }
    public void AddCharacterLeft(Character character)
    {
        characterLeft.Add(character);
    }
    public void AddCharacterRight(Character character)
    {
        characterRight.Add(character);
    }
    public void ChangeCharacterLeft(int index, Character character)
    {
        characterLeft[index] = character;
    }
    public void ChangeCharacterRight(int index, Character character)
    {
        characterRight[index] = character;
    }
    public void RemoveCharacterLeft(int index)
    {
        characterLeft.RemoveAt(index);
    }
    public void RemoveCharacterRight(int index)
    {
        characterRight.RemoveAt(index);
    }
    public List<Character> GetListCharacterLeft()
    {
        return characterLeft;
    }
    public List<Character> GetListCharacterRight()
    {
        return characterRight;
    }
}