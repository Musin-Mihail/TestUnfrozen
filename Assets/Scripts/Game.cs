using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] List<Transform> characterLocationsLeft;
    [SerializeField] List<Transform> characterLocationsRight;
    [SerializeField] Transform centerLeft;
    [SerializeField] Transform centerRight;
    List<Character> turn = new List<Character>();
    GameObject character;
    List<Character> characterLeft = new List<Character>();
    List<Character> characterRight = new List<Character>();
    List<Character> DeceasedCharacters = new List<Character>();
    void Start()
    {
        // Атака или пропустить ход. Выбор цели атаки. Движение к центру. При движении в центр другие персонажы сдвигаются вглубь. Перезапуск, пул персонажей.
        Generation();
        StartCoroutine(AutoFight());
    }
    void Generation()
    {
        character = Resources.Load<GameObject>("Character");
        bool leftSide;
        foreach (var item in characterLocationsLeft)
        {
            GameObject GO = Instantiate(this.character, item.position, Quaternion.identity);
            leftSide = true;
            Character character = new Character(GO, leftSide, centerLeft);
            turn.Add(character);
            characterLeft.Add(character);
        }
        foreach (var item in characterLocationsRight)
        {
            GameObject GO = Instantiate(this.character, item.position, Quaternion.Euler(0, 180, 0));
            leftSide = false;
            Character character = new Character(GO, leftSide, centerRight);
            turn.Add(character);
            characterRight.Add(character);
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character temp = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = temp;
        }
    }
    IEnumerator AutoFight()
    {
        for (int i = 0; i < 100; i++)
        {
            List<Character> newTurn = new List<Character>();
            for (int y = 0; y < turn.Count; y++)
            {
                if (characterRight.Count == 0 || characterLeft.Count == 0)
                {
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    StartCoroutine(turn[y].RunCenterAtack());
                    if (turn[y].getSide() == true)
                    {
                        int index = Random.Range(0, characterRight.Count);
                        Character temp = characterRight[index];
                        StartCoroutine(temp.RunCenter());
                        yield return new WaitForSeconds(3);
                        StartCoroutine(turn[y].Shoot(temp.GetPositionHead()));
                        yield return new WaitForSeconds(0.5f);
                        temp.TakeAwayHealth();
                        int index2 = turn.IndexOf(characterRight[index]);
                        turn[index2] = temp;
                        if (temp.GetDeath() == true)
                        {
                            temp.SetLayerBack();
                            DeceasedCharacters.Add(temp);
                            characterRight.RemoveAt(index);
                        }
                        else
                        {
                            StartCoroutine(temp.RunBack());
                            characterRight[index] = temp;
                        }
                    }
                    else
                    {
                        int index = Random.Range(0, characterLeft.Count);
                        Character temp = characterLeft[index];
                        StartCoroutine(temp.RunCenter());
                        yield return new WaitForSeconds(3);
                        StartCoroutine(turn[y].Shoot(temp.GetPositionHead()));
                        yield return new WaitForSeconds(0.5f);
                        temp.TakeAwayHealth();
                        int index2 = turn.IndexOf(characterLeft[index]);
                        turn[index2] = temp;

                        if (temp.GetDeath() == true)
                        {
                            temp.SetLayerBack();
                            DeceasedCharacters.Add(temp);
                            characterLeft.RemoveAt(index);
                        }
                        else
                        {
                            StartCoroutine(temp.RunBack());
                            characterLeft[index] = temp;
                        }
                    }
                    yield return StartCoroutine(turn[y].RunBack());
                }
            }
            foreach (var item in turn)
            {
                if (item.GetDeath() == false)
                {
                    newTurn.Add(item);
                }
            }
            turn = newTurn;
        }
    }
}