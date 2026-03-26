using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Mission
{
    public string description;
    public MissionCategory category;
    public MissionType type;
    public int target;
    public int rewardCoins;

    [HideInInspector] public int progress;
    [HideInInspector] public bool completed;
    [HideInInspector] public bool claimed;
}

[System.Serializable]
public class MissionChain
{
    public MissionType type;
    public List<Mission> stages;
    public int currentStage;
}

public enum MissionCategory
{
    Daily,
    Progression
}

public enum MissionType
{
    Coins,
    Distance,
    NPCCatch
}

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance;

    [Header("Mission Pools")]
    public List<Mission> dailyPool = new List<Mission>();

    [Header("Progression Chains")]
    public List<MissionChain> progressionChains = new List<MissionChain>();

    [HideInInspector] public List<Mission> activeDaily = new List<Mission>();
    [HideInInspector] public List<Mission> activeProgress = new List<Mission>();

    const string DAILY_RESET_KEY = "MISSION_LAST_RESET";
    const string DAILY_MISSIONS_KEY = "MISSION_DAILY_DATA";

    int runCoins = 0;
    int runDistance = 0;
    int runNPCCatch = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CheckDailyReset();
        SetupProgressMissions();
    }

    // updating ng progress
    public void AddRunProgress(MissionType type, int amount)
    {
        switch (type)
        {
            case MissionType.Coins: runCoins += amount; break;
            case MissionType.Distance: runDistance += amount; break;
            case MissionType.NPCCatch: runNPCCatch += amount; break;
        }
    }

    public void CommitRunProgress()
    {
        AddProgress(MissionType.Coins, runCoins);
        AddProgress(MissionType.Distance, runDistance);
        AddProgress(MissionType.NPCCatch, runNPCCatch);

        runCoins = 0;
        runDistance = 0;
        runNPCCatch = 0;

        SaveDailyMissions();
    }

    public void ResetRunProgress()
    {
        runCoins = 0;
        runDistance = 0;
        runNPCCatch = 0;
    }

    // dailies
    void CheckDailyReset()
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        string savedDate = PlayerPrefs.GetString(DAILY_RESET_KEY, "");

        if (savedDate != today)
        {
            GenerateDailyMissions();
            SaveDailyMissions();

            PlayerPrefs.SetString(DAILY_RESET_KEY, today);
            PlayerPrefs.Save();
        }
        else
        {
            if (activeDaily.Count == 0) GenerateDailyMissions();

            LoadDailyMissions();
        }
    }

    void GenerateDailyMissions()
    {
        activeDaily.Clear();

        List<Mission> pool = new List<Mission>(dailyPool);

        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            activeDaily.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    void SetupProgressMissions()
    {
        activeProgress.Clear();

        foreach (MissionChain chain in progressionChains)
        {
            if (chain.stages.Count > 0)
                activeProgress.Add(chain.stages[chain.currentStage]);
        }
    }

    public void AddProgress(MissionType type, int amount)
    {
        UpdateMissionList(activeDaily, type, amount);
        UpdateMissionList(activeProgress, type, amount);
    }

    void UpdateMissionList(List<Mission> missions, MissionType type, int amount)
    {
        foreach (Mission m in missions)
        {
            if (m.type != type) continue;
            if (m.completed) continue;

            m.progress += amount;

            if (m.progress >= m.target)
            {
                m.progress = m.target;
                m.completed = true;
            }
        }
    }

    // claim
    public void ClaimReward(Mission mission)
    {
        if (!mission.completed || mission.claimed)
            return;

        mission.claimed = true;

        CoinStorage.Instance.AddCoins(mission.rewardCoins);

        if (mission.category == MissionCategory.Progression)
        {
            AdvanceProgression(mission);
        }

        SaveDailyMissions();

        //  check if lahat is done
        if (mission.category == MissionCategory.Daily && AreAllDailyClaimed())
        {
            ShowDailyRewardBox();
        }
    }

    void AdvanceProgression(Mission mission)
    {
        foreach (MissionChain chain in progressionChains)
        {
            if (chain.stages.Contains(mission))
            {
                chain.currentStage++;

                if (chain.currentStage < chain.stages.Count)
                {
                    activeProgress.Remove(mission);
                    activeProgress.Add(chain.stages[chain.currentStage]);
                }

                break;
            }
        }
    }

    public bool AreAllDailyClaimed()
    {
        foreach (var m in activeDaily)
        {
            if (!m.claimed)
                return false;
        }
        return true;
    }

    public void ShowDailyRewardBox()
    {
        DailyRewardBox box = FindObjectOfType<DailyRewardBox>();
        if (box != null)
        {
            box.ShowRewardPanel();
        }
    }

    void SaveDailyMissions()
    {
        for (int i = 0; i < activeDaily.Count; i++)
        {
            PlayerPrefs.SetInt(DAILY_MISSIONS_KEY + "_PROG_" + i, activeDaily[i].progress);
            PlayerPrefs.SetInt(DAILY_MISSIONS_KEY + "_COMP_" + i, activeDaily[i].completed ? 1 : 0);
            PlayerPrefs.SetInt(DAILY_MISSIONS_KEY + "_CLAIM_" + i, activeDaily[i].claimed ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    void LoadDailyMissions()
    {
        for (int i = 0; i < activeDaily.Count; i++)
        {
            activeDaily[i].progress = PlayerPrefs.GetInt(DAILY_MISSIONS_KEY + "_PROG_" + i, 0);
            activeDaily[i].completed = PlayerPrefs.GetInt(DAILY_MISSIONS_KEY + "_COMP_" + i, 0) == 1;
            activeDaily[i].claimed = PlayerPrefs.GetInt(DAILY_MISSIONS_KEY + "_CLAIM_" + i, 0) == 1;
        }
    }
}