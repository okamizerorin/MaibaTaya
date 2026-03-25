using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionUI : MonoBehaviour
{
    public TMP_Text descriptionText;
    public TMP_Text progressText;
    public TMP_Text rewardText;

    public Button claimButton;
    public GameObject checkIcon;
    public GameObject completedOverlay;

    Mission mission;
    bool completionAnimationPlayed = false;

    public void SetMission(Mission m)
    {
        mission = m;

        completionAnimationPlayed = false;

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(ClaimReward);
    }

    void Update()
    {
        if (mission == null) return;

        descriptionText.text = mission.description;
        rewardText.text = mission.rewardCoins.ToString();

        // starting
        if (!mission.completed)
        {
            progressText.text = mission.progress + " / " + mission.target;

            progressText.gameObject.SetActive(true);
            claimButton.gameObject.SetActive(false);
            checkIcon.SetActive(false);
            completedOverlay.SetActive(false);
        }
        // complete pero not claimed yet
        else if (!mission.claimed)
        {
            progressText.gameObject.SetActive(false);
            claimButton.gameObject.SetActive(true);
            checkIcon.SetActive(true);
            completedOverlay.SetActive(false);
        }
        // claimed
        else
        {
            claimButton.gameObject.SetActive(false);
            progressText.gameObject.SetActive(false);
            checkIcon.SetActive(true);
            completedOverlay.SetActive(true);
        }
    }

    IEnumerator ShowClaimAfterDelay()
    {
        checkIcon.SetActive(true);

        yield return new WaitForSeconds(1f);

        progressText.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(true);
    }

    void ClaimReward()
    {
        MissionSystem.Instance.ClaimReward(mission);
    }
}