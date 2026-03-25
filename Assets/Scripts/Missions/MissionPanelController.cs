using UnityEngine;
using System.Collections.Generic;

public class MissionPanelController : MonoBehaviour
{
    public List<MissionUI> missionSlots;

    public MissionCategory category;

    int currentPage = 0;
    int missionsPerPage = 3;

    void Start()
    {
        RefreshPage();
    }

    public void SetCategory(MissionCategory newCategory)
    {
        category = newCategory;
        currentPage = 0;
        RefreshPage();
    }

    public void NextPage()
    {
        int totalPages = Mathf.CeilToInt(
            (float)GetMissionList().Count / missionsPerPage
        );

        currentPage++;

        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        RefreshPage();
    }

    public void PreviousPage()
    {
        int totalPages = Mathf.CeilToInt(
            (float)GetMissionList().Count / missionsPerPage
        );

        currentPage--;

        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        RefreshPage();
    }

    public void RefreshPage()
    {
        List<Mission> missions = GetMissionList();

        for (int i = 0; i < missionSlots.Count; i++)
        {
            int missionIndex = currentPage * missionsPerPage + i;

            if (missionIndex < missions.Count)
            {
                missionSlots[i].gameObject.SetActive(true);

                missionSlots[i].SetMission(
                    missions[missionIndex]
                );
            }
            else
            {
                missionSlots[i].gameObject.SetActive(false);
            }
        }
    }

    List<Mission> GetMissionList()
    {
        if (category == MissionCategory.Daily)
            return MissionSystem.Instance.activeDaily;

        return MissionSystem.Instance.activeProgress;
    }
}