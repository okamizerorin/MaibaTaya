using UnityEngine;

public class MissionUISwitcher : MonoBehaviour
{
    public MissionPanelController panel;

    public void ShowDaily()
    {
        panel.SetCategory(MissionCategory.Daily);
    }

    public void ShowProgression()
    {
        panel.SetCategory(MissionCategory.Progression);
    }
}