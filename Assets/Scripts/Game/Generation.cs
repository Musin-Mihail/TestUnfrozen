using System.Collections.Generic;
using UnityEngine;
public class Generation
{
    private Vector3 _centerLeft = new Vector3(-5, 0, 0);
    private Vector3 _centerRight = new Vector3(5, 0, 0);
    private List<Vector3> _characterLocationsLeft = new List<Vector3>();
    private List<Vector3> _characterLocationsRight = new List<Vector3>();
    private Quaternion _rotationLeft = Quaternion.Euler(0, 0, 0);
    private Quaternion _rotationRight = Quaternion.Euler(0, -180, 0);
    private List<Character.Character> _poolCharacters = new List<Character.Character>();
    private List<Character.Character> _characterLeft = new List<Character.Character>();
    private List<Character.Character> _characterRight = new List<Character.Character>();
    public Generation()
    {
        _characterLocationsLeft.Add(new Vector3(-10, 0, 0));
        _characterLocationsLeft.Add(new Vector3(-15, 0, 0));
        _characterLocationsLeft.Add(new Vector3(-20, 0, 0));
        _characterLocationsLeft.Add(new Vector3(-25, 0, 0));

        _characterLocationsRight.Add(new Vector3(10, 0, 0));
        _characterLocationsRight.Add(new Vector3(15, 0, 0));
        _characterLocationsRight.Add(new Vector3(20, 0, 0));
        _characterLocationsRight.Add(new Vector3(25, 0, 0));
    }
    public List<Character.Character> StartGeneration()
    {
        List<Character.Character> turn = new List<Character.Character>();
        bool leftSide;
        foreach (var item in _characterLocationsLeft)
        {
            leftSide = true;
            PoolAddCharacter(leftSide, item, _centerLeft, _characterLeft, _rotationLeft, turn);
        }
        foreach (var item in _characterLocationsRight)
        {
            leftSide = false;
            PoolAddCharacter(leftSide, item, _centerRight, _characterRight, _rotationRight, turn);
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character.Character tempCharacter = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = tempCharacter;
        }
        return turn;
    }
    private void PoolAddCharacter(bool leftSide, Vector3 position, Vector3 center, List<Character.Character> characters, Quaternion rotation, List<Character.Character> turn)
    {
        if (_poolCharacters.Count > 0)
        {
            Character.Character tempCharacter = _poolCharacters[0];
            _poolCharacters.RemoveAt(0);
            tempCharacter.Reset(position, leftSide, center);
            turn.Add(tempCharacter);
            characters.Add(tempCharacter);
        }
        else
        {
            GameObject GO;
            GO = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Character"), position, rotation);
            Character.Character character = new Character.Character(GO, leftSide, center);
            turn.Add(character);
            characters.Add(character);
        }
    }
    public int GetCountCharactersLeft()
    {
        return _characterLeft.Count;
    }
    public int GetCountCharactersRight()
    {
        return _characterRight.Count;
    }
    public Character.Character GetCharacterLeft(int index)
    {
        return _characterLeft[index];
    }
    public Character.Character GetCharacterRight(int index)
    {
        return _characterRight[index];
    }
    public void AddCharacterPool(Character.Character character)
    {
        _poolCharacters.Add(character);
    }
    public void AddCharacterLeft(Character.Character character)
    {
        _characterLeft.Add(character);
    }
    public void AddCharacterRight(Character.Character character)
    {
        _characterRight.Add(character);
    }
    public void ChangeCharacterLeft(int index, Character.Character character)
    {
        _characterLeft[index] = character;
    }
    public void ChangeCharacterRight(int index, Character.Character character)
    {
        _characterRight[index] = character;
    }
    public void RemoveCharacterLeft(int index)
    {
        _characterLeft.RemoveAt(index);
    }
    public void RemoveCharacterRight(int index)
    {
        _characterRight.RemoveAt(index);
    }
    public List<Character.Character> GetListCharacterLeft()
    {
        return _characterLeft;
    }
    public List<Character.Character> GetListCharacterRight()
    {
        return _characterRight;
    }
}