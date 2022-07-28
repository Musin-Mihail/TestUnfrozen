using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
    private bool _boolPlayerChoice = false;
    [SerializeField] private Transform _playerChoice;
    [SerializeField] private Transform _selectTarget;
    private Transform _playerFight;
    private Transform _playerWait;
    private Generation _generation = new Generation();
    private Fight _fight;
    private void Start()
    {
        _playerFight = _playerChoice.GetChild(0).transform;
        _playerWait = _playerChoice.GetChild(1).transform;
        _fight = gameObject.AddComponent<Fight>();
        RestartGame();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _boolPlayerChoice == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (_fight.GetboolFight() == true)
                {
                    for (int i = 0; i < _generation.GetCountCharactersRight(); i++)
                    {
                        if (_generation.GetCharacterRight(i).GetBody() == hit.transform)
                        {
                            _fight.SetTargetCharacter(_generation.GetCharacterRight(i));
                            _fight.SetIndexTarget(i);
                            _boolPlayerChoice = false;
                            _selectTarget.gameObject.SetActive(false);
                            break;
                        }
                    }
                }
                else
                {
                    if (hit.transform == _playerFight)
                    {
                        _fight.SetBoolFight(true);
                        _playerChoice.gameObject.SetActive(false);
                        _selectTarget.gameObject.SetActive(true);
                    }
                }
                if (hit.transform == _playerWait)
                {
                    _fight.SetBoolWait(true);
                    _playerChoice.gameObject.SetActive(false);
                    _boolPlayerChoice = false;
                }
            }
        }
    }
    public void RestartGame()
    {
        List<Character.Character> turn = _generation.StartGeneration();
        StartCoroutine(_fight.StartFight(_generation, turn, this));
    }
    public void ChangeAutoFight(Button button)
    {
        Text text = button.GetComponentInChildren<Text>();
        Image image = button.GetComponent<Image>();
        if (_fight.GetBoolAutoFight() == false)
        {
            _fight.SetBoolAutoFight(true);
            text.text = "???????\n???????";
            image.color = Color.green;
        }
        else
        {
            _fight.SetBoolAutoFight(false);
            text.text = "???????\n????????";
            image.color = Color.red;
        }
    }
    public bool GetStatusPlayerChoice()
    {
        return _boolPlayerChoice;
    }
    public void SetStatusPlayerChoice(bool bool1)
    {
        _boolPlayerChoice = bool1;
    }
    public void EnablePlayerChoice()
    {
        _playerChoice.gameObject.SetActive(true);
    }
}