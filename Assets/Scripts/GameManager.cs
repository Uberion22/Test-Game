using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemys;

    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI combatQuery;
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button startBtn;
    [SerializeField] private AudioSource audioSource;

    private List<GameObject> allPlayers;
    private int currentStep;
    private MyBandit currentEnemy;
    private int roundNumber;

    // Start is called before the first frame update
    void Start()
    {
        MyBandit.enemySelected += EnemySelected;
        MyBandit.startNextStep += StartNextStep;
        audioSource.Play();
        GenerateSteps();
    }

    public void GenerateSteps()
    {
        roundNumber++;
        allPlayers = enemys.Where(e => !e.CompareTag(CharacterTags.Dead)).ToList();
        var count = allPlayers.Count;
        var r = new System.Random();
        for (int i = count - 1; i >= 1; i--)
        {
            int j = r.Next(i + 1);
            var temp = allPlayers[j];
            allPlayers[j] = allPlayers[i];
            allPlayers[i] = temp;
        }
        SetCombatQueryText(allPlayers);
    }

    private IEnumerator WaitBeforeNextStep()
    {
        yield return new WaitForSeconds(6);
        NextStep();
    }

    public void StartNextStep()
    {
        if (!allPlayers.Any(e => e.CompareTag(CharacterTags.Player)))
        {
            ShowEndGameText(EndGameText.LooseText);
        }
        else if (!allPlayers.Any(e => e.CompareTag(CharacterTags.Enemy)))
        {
            ShowEndGameText(EndGameText.WinText);
        }
        else
        {
            Invoke("NextStep", 5);
        }
    }

    public void NextStep()
    {
        currentEnemy = null;
        SetCombatQueryText(allPlayers);
        if (currentStep >= allPlayers.Count)
        {
            currentStep = 0; 
            GenerateSteps();
            ShowRoundMessage(roundNumber);
        }

        if (allPlayers[currentStep].CompareTag(CharacterTags.Enemy))
        {
            MyBandit.isPlayerStep = false;
            SetActionButtonsStatus(false);
            var players = allPlayers.Where(e => e.CompareTag(CharacterTags.Player)).ToList();
            var playerIndex = Random.Range(0, players.Count - 1);
            var currentTarget = players[playerIndex].GetComponent<MyBandit>();
            allPlayers[currentStep].SendMessage($"StartAttack", currentTarget);
            currentStep++;
        }
        else if(allPlayers[currentStep].CompareTag(CharacterTags.Player))
        {
            allPlayers[currentStep].SendMessage($"SetBlueColor");
            SetActionButtonsStatus(true);
            MyBandit.isPlayerStep = true;
            Debug.Log("Player step! " + currentStep);
        }
        else
        {
            currentStep++;
            NextStep();
        }
        Debug.Log("Current Step is: " + currentStep);
    }

    private void EnemySelected(MyBandit selectedEnemy)
    {
        currentEnemy = selectedEnemy;
    }

    private void SetActionButtonsStatus(bool status)
    {
        skipBtn.interactable = status;
        attackBtn.interactable = status;
    }

    public void OnSkipDown()
    {
        currentEnemy = null;
        currentStep++;
        NextStep();
    }

    public void OnAttackDown()
    {
        if (currentEnemy != null)
        {
            allPlayers[currentStep].SendMessage($"StartAttack", currentEnemy);
            currentStep++;
            SetActionButtonsStatus(false);
        }
    }

    public void RestartBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnStartBtnClick()
    {
        attackBtn.gameObject.SetActive(true);
        skipBtn.gameObject.SetActive(true);
        ShowRoundMessage(roundNumber);
        startBtn.gameObject.SetActive(false);
        NextStep();
    }

    private void ShowRoundMessage(int round)
    {
        roundText.text = $"Round: {round}";
        ShowMessage();
        Invoke("HideMessage",3);
    }

    private void ShowMessage()
    {
        roundText.gameObject.SetActive(true);
    }

    private void HideMessage()
    {
        roundText.gameObject.SetActive(false);
    }

    private void ShowEndGameText(string text)
    {
        endGameText.gameObject.SetActive(true);
        endGameText.SetText(text);
        restartBtn.gameObject.SetActive(true);
    }

    private void SetCombatQueryText(List<GameObject> combatQuery)
    {
        var text = "";
        for(var i = 0; i < combatQuery.Count; i++)
        {
            var color = MessageColors.DefaultCharacter;
            if (combatQuery[i].CompareTag(CharacterTags.Dead))
            {
                color = MessageColors.DeadCharacter;
            }

            if (i == currentStep)
            {
                color = MessageColors.CurrentCharacter;
            }

            text += $"<color={color}>{combatQuery[i].name}</color> " + "\n";
        }

        this.combatQuery.text = text;
    }
}
