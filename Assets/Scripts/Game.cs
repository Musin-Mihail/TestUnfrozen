using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
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
    float delayRun = 2.5f;
    float delayShoot = 0.5f;
    bool PlayerChoice = false;
    [SerializeField] Transform playerChoice;
    [SerializeField] Transform selectTarget;
    [SerializeField] Transform playerFight;
    [SerializeField] Transform playerWait;
    bool boolFight = false;
    bool boolWait = false;
    bool boolAutoFight = false;
    Character targetCharacter;
    int indexTarget;
    void Start()
    {
        character = Resources.Load<GameObject>("Character");
        RestartGame();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerChoice == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.zero);
            if (hit.collider != null)
            {
                if (boolFight == true)
                {
                    for (int i = 0; i < characterRight.Count; i++)
                    {
                        if (characterRight[i].GetBody() == hit.transform)
                        {
                            targetCharacter = characterRight[i];
                            indexTarget = i;
                            PlayerChoice = false;
                            selectTarget.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    if (hit.transform == playerFight)
                    {
                        boolFight = true;
                        playerChoice.gameObject.SetActive(false);
                        selectTarget.gameObject.SetActive(true);
                    }
                }
                if (hit.transform == playerWait)
                {
                    boolWait = true;
                    playerChoice.gameObject.SetActive(false);
                    PlayerChoice = false;
                }
            }
        }
    }
    void Generation()
    {
        bool leftSide;
        turn.Clear();
        foreach (var item in characterLocationsLeft)
        {
            leftSide = true;
            if (DeceasedCharacters.Count > 0)
            {
                Character temp = DeceasedCharacters[0];
                DeceasedCharacters.RemoveAt(0);
                temp.Reset(item.position, leftSide, centerLeft);
                turn.Add(temp);
                characterLeft.Add(temp);
            }
            else
            {
                GameObject GO = Instantiate(this.character, item.position, Quaternion.identity);
                Character character = new Character(GO, leftSide, centerLeft);
                turn.Add(character);
                characterLeft.Add(character);
            }
        }
        foreach (var item in characterLocationsRight)
        {
            leftSide = false;
            if (DeceasedCharacters.Count > 0)
            {
                Character temp = DeceasedCharacters[0];
                DeceasedCharacters.RemoveAt(0);
                temp.Reset(item.position, leftSide, centerRight);
                turn.Add(temp);
                characterRight.Add(temp);
            }
            else
            {
                GameObject GO = Instantiate(this.character, item.position, Quaternion.Euler(0, 180, 0));
                Character character = new Character(GO, leftSide, centerRight);
                turn.Add(character);
                characterRight.Add(character);
            }
        }
        for (int i = 0; i < turn.Count; i++)
        {
            Character temp = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = temp;
        }
    }
    void RestartGame()
    {
        Generation();
        StartCoroutine(Fight());
    }
    IEnumerator WaitPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (PlayerChoice == false)
            {
                boolFight = false;
                yield break;
            }
        }
    }
    IEnumerator Fight()
    {
        while (true)
        {
            List<Character> newTurn = new List<Character>();
            for (int y = 0; y < turn.Count; y++)
            {
                if (characterRight.Count == 0 || characterLeft.Count == 0)
                {
                    int count = characterRight.Count;
                    for (int z = 0; z < count; z++)
                    {
                        DeceasedCharacters.Add(characterRight[0]);
                        characterRight.RemoveAt(0);
                    }
                    count = characterLeft.Count;
                    for (int x = 0; x < count; x++)
                    {
                        DeceasedCharacters.Add(characterLeft[0]);
                        characterLeft.RemoveAt(0);
                    }
                    yield return new WaitForSeconds(2.0f);
                    RestartGame();
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    //bool attack = true;
                    //StartCoroutine(turn[y].RunCenter(attack));
                    bool leftSide = turn[y].getSide();
                    if (leftSide == true)
                    {
                        if (boolAutoFight == false)
                        {
                            PlayerChoice = true;
                            playerChoice.gameObject.SetActive(true);
                            yield return StartCoroutine(WaitPlayer());
                            if (boolWait == false)
                            {
                                bool attack = true;
                                StartCoroutine(turn[y].RunCenter(attack));
                                yield return StartCoroutine(PlayerAttack(y));
                                StartCoroutine(turn[y].RunBack());
                                yield return new WaitForSeconds(delayRun);
                            }
                            else
                            {
                                boolWait = false;
                            }
                        }
                        else
                        {
                            bool attack = true;
                            StartCoroutine(turn[y].RunCenter(attack));
                            yield return StartCoroutine(Attack(characterRight, y));
                            StartCoroutine(turn[y].RunBack());
                            yield return new WaitForSeconds(delayRun);
                        }
                    }
                    else
                    {
                        bool attack = true;
                        StartCoroutine(turn[y].RunCenter(attack));
                        yield return StartCoroutine(Attack(characterLeft, y));
                        StartCoroutine(turn[y].RunBack());
                        yield return new WaitForSeconds(delayRun);
                    }
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
    IEnumerator PlayerAttack(int turnIndex)
    {
        bool attack = false;
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(targetCharacter.GetBoneHead(), cts.Token, targetCharacter.GetBody()));
        StartCoroutine(targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        targetCharacter.TakeAwayHealth();
        for (int i = 0; i < turn.Count; i++)
        {
            Transform temp = targetCharacter.GetBody();
            if (turn[i].GetBody() == temp)
            {
                turn[i] = targetCharacter;
                break;
            }
        }
        if (targetCharacter.GetDeath() == true)
        {
            targetCharacter.SetLayerBack();
            DeceasedCharacters.Add(targetCharacter);
            characterRight.RemoveAt(indexTarget);
        }
        else
        {
            StartCoroutine(targetCharacter.Hit());
            StartCoroutine(targetCharacter.RunBack());
            characterRight[indexTarget] = targetCharacter;
        }
    }
    IEnumerator Attack(List<Character> Characters, int turnIndex)
    {
        int index = Random.Range(0, Characters.Count);
        Character temp = Characters[index];
        bool attack = false;
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(temp.GetBoneHead(), cts.Token, temp.GetBody()));
        StartCoroutine(temp.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        temp.TakeAwayHealth();
        int index2 = turn.IndexOf(Characters[index]);
        turn[index2] = temp;
        if (temp.GetDeath() == true)
        {
            temp.SetLayerBack();
            DeceasedCharacters.Add(temp);
            Characters.RemoveAt(index);
        }
        else
        {
            StartCoroutine(temp.Hit());
            StartCoroutine(temp.RunBack());
            Characters[index] = temp;
        }
    }
    public void ChangeAutoFight(Button button)
    {
        Text text = button.GetComponentInChildren<Text>();
        Image image = button.GetComponent<Image>();
        if (boolAutoFight == false)
        {
            boolAutoFight = true;
            text.text = "Автобой\nВключён";
            image.color = Color.green;
        }
        else
        {
            boolAutoFight = false;
            text.text = "Автобой\nВыключен";
            image.color = Color.red;
        }
    }
}