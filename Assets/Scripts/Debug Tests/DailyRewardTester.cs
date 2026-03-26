using UnityEngine;

public class DailyRewardTester : MonoBehaviour
{
    public void DebugClaimAllDailies()
    {
        foreach (var m in MissionSystem.Instance.activeDaily)
        {
            // mark as complete
            m.progress = m.target;
            m.completed = true;

            // claim the reward properly
            MissionSystem.Instance.ClaimReward(m);
        }

        Debug.Log("All daily missions claimed!");
    }
}