using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
    [SerializeField] private List<Transform> characterLocationsLeft;
    [SerializeField] private List<Transform> characterLocationsRight;
    [SerializeField] private Transform centerLeft;
    [SerializeField] private Transform centerRight;
    private List<Character> turn = new List<Character>();
    private GameObject character;
    private List<Character> characterLeft = new List<Character>();
    private List<Character> characterRight = new List<Character>();
    private List<Character> PoolCharacters = new List<Character>();
    private float delayRun = 2.5f;
    private float delayShoot = 0.5f;
    private bool PlayerChoice = false;
    [SerializeField] private Transform playerChoice;
    [SerializeField] private Transform selectTarget;
    [SerializeField] private Transform playerFight;
    [SerializeField] private Transform playerWait;
    private bool boolFight = false;
    private bool boolWait = false;
    private bool boolAutoFight = false;
    private Character targetCharacter;
    private int indexTarget;
    private void Start()
    {
        character = Resources.Load<GameObject>("Character");
        RestartGame();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && PlayerChoice == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
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
    private void RestartGame()
    {
        Generation();
        StartCoroutine(Fight());
    }
    private void Generation()
    {
        bool leftSide;
        turn.Clear();
        foreach (var item in characterLocationsLeft)
        {
            leftSide = true;
            if (PoolCharacters.Count > 0)
            {
                Character tempCharacter = PoolCharacters[0];
                PoolCharacters.RemoveAt(0);
                tempCharacter.Reset(item.position, leftSide, centerLeft);
                turn.Add(tempCharacter);
                characterLeft.Add(tempCharacter);
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
            if (PoolCharacters.Count > 0)
            {
                Character tempCharacter = PoolCharacters[0];
                PoolCharacters.RemoveAt(0);
                tempCharacter.Reset(item.position, leftSide, centerRight);
                turn.Add(tempCharacter);
                characterRight.Add(tempCharacter);
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
            Character tempCharacter = turn[i];
            int randomIndex = Random.Range(i, turn.Count);
            turn[i] = turn[randomIndex];
            turn[randomIndex] = tempCharacter;
        }
    }
    //Зделать пул
    private IEnumerator Fight()
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
                        PoolCharacters.Add(characterRight[0]);
                        characterRight.RemoveAt(0);
                    }
                    count = characterLeft.Count;
                    for (int x = 0; x < count; x++)
                    {
                        PoolCharacters.Add(characterLeft[0]);
                        characterLeft.RemoveAt(0);
                    }
                    yield return new WaitForSeconds(2.0f);
                    RestartGame();
                    yield break;
                }
                if (turn[y].GetDeath() == false)
                {
                    bool leftSide = turn[y].getSide();
                    if (leftSide == true)
                    {
                        if (boolAutoFight == false)
                        {
                            PlayerChoice = true;
                            playerChoice.gameObject.SetActive(true);
                            yield return StartCoroutine(PlayerWaiting());
                            if (boolWait == false)
                            {
                                yield return StartCoroutine(PlayerAttack(y));
                            }
                            else
                            {
                                boolWait = false;
                            }
                        }
                        else
                        {
                            yield return StartCoroutine(AIAttack(characterRight, y));
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(AIAttack(characterLeft, y));
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
    IEnumerator PlayerWaiting()
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
    private IEnumerator PlayerAttack(int turnIndex)
    {
        bool attack = true;
        StartCoroutine(turn[turnIndex].RunCenter(attack));
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(targetCharacter.GetBoneHead(), cts.Token, targetCharacter.GetBody()));
        attack = false;
        StartCoroutine(targetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        targetCharacter.TakeAwayHealth();
        for (int i = 0; i < turn.Count; i++)
        {
            Transform tempTransform = targetCharacter.GetBody();
            if (turn[i].GetBody() == tempTransform)
            {
                turn[i] = targetCharacter;
                break;
            }
        }
        if (targetCharacter.GetDeath() == true)
        {
            targetCharacter.SetLayerBack();
            PoolCharacters.Add(targetCharacter);
            characterRight.RemoveAt(indexTarget);
        }
        else
        {
            StartCoroutine(targetCharacter.Hit());
            StartCoroutine(targetCharacter.RunBack());
            characterRight[indexTarget] = targetCharacter;
        }
        StartCoroutine(turn[turnIndex].RunBack());
        yield return new WaitForSeconds(delayRun);
    }
    private IEnumerator AIAttack(List<Character> Characters, int turnIndex)
    {
        bool attack = true;
        StartCoroutine(turn[turnIndex].RunCenter(attack));
        int index = Random.Range(0, Characters.Count);
        Character TargetCharacter = Characters[index];
        attack = false;
        CancellationTokenSource cts = new CancellationTokenSource();
        StartCoroutine(turn[turnIndex].Aim(TargetCharacter.GetBoneHead(), cts.Token, TargetCharacter.GetBody()));
        StartCoroutine(TargetCharacter.RunCenter(attack));
        yield return new WaitForSeconds(delayRun);
        StartCoroutine(turn[turnIndex].Shoot(cts));
        yield return new WaitForSeconds(delayShoot);
        TargetCharacter.TakeAwayHealth();
        int index2 = turn.IndexOf(Characters[index]);
        turn[index2] = TargetCharacter;
        if (TargetCharacter.GetDeath() == true)
        {
            TargetCharacter.SetLayerBack();
            PoolCharacters.Add(TargetCharacter);
            Characters.RemoveAt(index);
        }
        else
        {
            StartCoroutine(TargetCharacter.Hit());
            StartCoroutine(TargetCharacter.RunBack());
            Characters[index] = TargetCharacter;
        }
        StartCoroutine(turn[turnIndex].RunBack());
        yield return new WaitForSeconds(delayRun);
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