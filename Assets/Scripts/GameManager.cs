using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemys;

    public TextMeshProUGUI endGameText;
    public Button AttackBtn;
    public Button SkipBtn;
    public Button restartBtn;
    private List<GameObject> allPlayers;

    private bool playerLastAttacking = false;

    public List<int> steps = new List<int>(5);

    private int currentStep = 0;
    public bool isPlayerStep;
    public string enemyName;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSteps();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GenerateSteps()
    {
        allPlayers = enemys.Where(e => !e.CompareTag("Dead")).ToList();
        var count = allPlayers.Count();
        var r = new System.Random();
        for (int i = count - 1; i >= 1; i--)
        {
            int j = r.Next(i + 1);
            // обменять значения data[j] и data[i]
            var temp = allPlayers[j];
            allPlayers[j] = allPlayers[i];
            allPlayers[i] = temp;
        }
    }

    IEnumerator WaitBeforeNextStep()
    {
        yield return new WaitForSeconds(6);
        NextStep();
    }

    public void StartNextStep()
    {
        //StartCoroutine(WaitBeforeNextStep());
        if (!allPlayers.Any(e => e.CompareTag("Player")))
        {
            endGameText.gameObject.SetActive(true);
            endGameText.SetText("Game Over!");
            restartBtn.gameObject.SetActive(true);
        }
        else if (!allPlayers.Any(e => e.CompareTag("Enemy")))
        {
            endGameText.gameObject.SetActive(true);
            endGameText.SetText("You Win!");
            restartBtn.gameObject.SetActive(true);
        }
        else
        {
            Invoke("NextStep", 5);
        }
    }

    public void NextStep()
    {
        allPlayers = enemys.Where(e => !e.CompareTag("Dead")).ToList();
        if (currentStep >= allPlayers.Count)
        {
            currentStep = 0; 
            GenerateSteps();
        }

        if (allPlayers[currentStep].CompareTag("Enemy"))
        {
            SetButtonsStatus(false);
            var players = allPlayers.Where(e => e.CompareTag("Player")).ToList();
            var playerIndex = Random.Range(0, players.Count - 1);
            allPlayers[currentStep].SendMessage($"StartAttack", players[playerIndex].name);
            currentStep++;
        }
        else
        {
            SetButtonsStatus(true);
            Debug.Log("Player step!" + currentStep);
        }
        
        Debug.Log("Current Step is: " + currentStep);
        
    }

    public void PlayerAttack(string enemyName)
    {
        this.enemyName = enemyName;
        //allPlayers[currentStep].SendMessage($"StartAttack", enemyName);
    }

    private void SetButtonsStatus(bool status)
    {
        isPlayerStep = status;
        SkipBtn.interactable = status;
        AttackBtn.interactable = status;
    }

    public void OnSkipDown()
    {
        enemyName = String.Empty;
        currentStep++;
        NextStep();
    }

    public void OnAttackDown()
    {
        if (enemyName != String.Empty)
        {
            allPlayers[currentStep].SendMessage($"StartAttack", enemyName);
            currentStep++;
            SetButtonsStatus(false);
        }
    }
    public void RestartBtnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
