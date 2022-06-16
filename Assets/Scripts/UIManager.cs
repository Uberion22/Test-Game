//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class UIManager : MonoBehaviour
//{
//    [SerializeField] private TextMeshProUGUI endGameText;
//    [SerializeField] private TextMeshProUGUI roundText;
//    [SerializeField] private TextMeshProUGUI combatQuery;
//    [SerializeField] private Button attackBtn;
//    [SerializeField] private Button skipBtn;
//    [SerializeField] private Button restartBtn;
//    [SerializeField] private Button startBtn;
//    [SerializeField] private AudioSource audioSource;
    
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    public void RestartBtnClick()
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    public void OnStartBtnClick()
//    {
//        attackBtn.gameObject.SetActive(true);
//        skipBtn.gameObject.SetActive(true);
//        ShowRoundMessage(roundNumber);
//        startBtn.gameObject.SetActive(false);
//        NextStep();
//    }

//    private void ShowRoundMessage(int round)
//    {
//        roundText.text = $"Round: {round}";
//        ShowMessage();
//        Invoke("HideMessage", 3);
//    }

//    private void ShowMessage()
//    {
//        roundText.gameObject.SetActive(true);
//    }

//    private void HideMessage()
//    {
//        roundText.gameObject.SetActive(false);
//    }

//    private void ShowEndGameText(string text)
//    {
//        endGameText.gameObject.SetActive(true);
//        endGameText.SetText(text);
//        restartBtn.gameObject.SetActive(true);
//    }

//    private void SetCombatQueryText(List<GameObject> combatQuery, int currentStep)
//    {
//        var text = "";
//        for (var i = 0; i < combatQuery.Count; i++)
//        {
//            var color = MessageColors.DefaultCharacter;
//            if (combatQuery[i].CompareTag(CharacterTags.Dead))
//            {
//                color = MessageColors.DeadCharacter;
//            }

//            if (i == currentStep)
//            {
//                color = MessageColors.CurrentCharacter;
//            }

//            text += $"<color={color}>{combatQuery[i].name}</color> " + "\n";
//        }

//        this.combatQuery.text = text;
//    }
//}
