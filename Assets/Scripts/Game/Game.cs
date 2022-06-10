using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
    private bool boolPlayerChoice = false;
    [SerializeField] private Transform playerChoice;
    [SerializeField] private Transform selectTarget;
    private Transform playerFight;
    private Transform playerWait;
    private Generation generation = new Generation();
    private Fight fight;
    private void Start()
    {
        playerFight = playerChoice.GetChild(0).transform;
        playerWait = playerChoice.GetChild(1).transform;
        fight = gameObject.AddComponent<Fight>();
        RestartGame();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && boolPlayerChoice == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (fight.GetboolFight() == true)
                {
                    for (int i = 0; i < generation.GetCountCharactersRight(); i++)
                    {
                        if (generation.GetCharacterRight(i).GetBody() == hit.transform)
                        {
                            fight.SetTargetCharacter(generation.GetCharacterRight(i));
                            fight.SetIndexTarget(i);
                            boolPlayerChoice = false;
                            selectTarget.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    if (hit.transform == playerFight)
                    {
                        fight.SetBoolFight(true);
                        playerChoice.gameObject.SetActive(false);
                        selectTarget.gameObject.SetActive(true);
                    }
                }
                if (hit.transform == playerWait)
                {
                    fight.SetBoolWait(true);
                    playerChoice.gameObject.SetActive(false);
                    boolPlayerChoice = false;
                }
            }
        }
    }
    public void RestartGame()
    {
        List<Character> turn = generation.StartGeneration();
        StartCoroutine(fight.StartFight(generation, turn, this));
    }
    public void ChangeAutoFight(Button button)
    {
        Text text = button.GetComponentInChildren<Text>();
        Image image = button.GetComponent<Image>();
        if (fight.GetBoolAutoFight() == false)
        {
            fight.SetBoolAutoFight(true);
            text.text = "Автобой\nВключён";
            image.color = Color.green;
        }
        else
        {
            fight.SetBoolAutoFight(false);
            text.text = "Автобой\nВыключен";
            image.color = Color.red;
        }
    }
    public bool GetStatusPlayerChoice()
    {
        return boolPlayerChoice;
    }
    public void SetStatusPlayerChoice(bool bool1)
    {
        boolPlayerChoice = bool1;
    }
    public void EnablePlayerChoice()
    {
        playerChoice.gameObject.SetActive(true);
    }
}