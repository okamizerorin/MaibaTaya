using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DailyRewardBox : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Animator bagAnimator;
    public Button tapButton;

    public GameObject idleBag;    
    public GameObject resultPanel;

    [Header("Result UI")]
    public TMP_Text coinsText;
    public TMP_Text iceTubigText;
    public TMP_Text starterText;

    private bool opened = false;
    private bool canClose = false;

    void Awake()
    {
        panel.SetActive(false);
        resultPanel.SetActive(false);

        tapButton.onClick.AddListener(OnTap);
    }

    public void ShowRewardPanel()
    {
        panel.SetActive(true);

        opened = false;
        canClose = false;

        idleBag.SetActive(true);
        resultPanel.SetActive(false);

        // pop in
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
    }

    void OnTap()
    {
        // pers tap
        if (!opened)
        {
            opened = true;

            idleBag.SetActive(false);

            bagAnimator.gameObject.SetActive(true);
            bagAnimator.Play("RewardBox");

            float animLength = bagAnimator.GetCurrentAnimatorStateInfo(0).length;
            Invoke(nameof(ShowRewards), animLength);
        }
        // 2nd tap
        else if (canClose)
        {
            ClosePanel();
        }
    }

    void ShowRewards()
    {
        // randomize rewardss
        int coins = Random.Range(200, 1001);
        int iceTubig = Random.Range(1, 4);
        int starterType = Random.Range(0, 3);

        string starterName = "";

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddRevive(iceTubig);

            if (starterType == 0)
            {
                PlayerInventory.Instance.AddShieldStarter(1);
                starterName = "Superhero! x1";
            }
            else if (starterType == 1)
            {
                PlayerInventory.Instance.AddSpeedStarter(1);
                starterName = "Ninja Plus! x1";
            }
            else
            {
                PlayerInventory.Instance.AddDoubleCoinsStarter(1);
                starterName = "Double Barya! x1";
            }
        }

        CoinStorage.Instance.AddCoins(coins);

        // ui
        resultPanel.SetActive(true);

        coinsText.text = "+" + coins;
        iceTubigText.text = "+" + iceTubig;
        starterText.text = starterName;

        resultPanel.transform.localScale = Vector3.zero;
        resultPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        canClose = true;
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }
}