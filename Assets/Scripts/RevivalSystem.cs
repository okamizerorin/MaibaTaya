using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RevivalSystem : MonoBehaviour
{
    public static RevivalSystem Instance;

    [Header("NPC Chase State")]
    [SerializeField] float chaseDuration = 40f;
    float chaseTimer;
    bool chaseActive;
    Coroutine chaseTimerRoutine;
    bool npcCaught = false;

    [Header("First Revive")]
    public bool firstFailUsed = false;
    public float autoReviveCountdown = 5f;
    public TextMeshProUGUI countdownText;
    public GameObject countdownFailUI;

    [Header("Second Revive")]
    public int maxRevives = 3;
    public int currentRevives = 3;
    public float secondReviveDuration = 5f;
    public GameObject secondRevivePanel;
    public TextMeshProUGUI secondCountdownText;

    [Header("Final Game Over")]
    public GameObject finalGameOverPanel;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI iceTubigOwnedText;

    public TMP_Text reviveCounterText;

    bool secondReviveActive;
    Coroutine secondReviveCoroutine;

    [Header("Stagger")]
    public float staggerDuration = 0.6f;
    public float staggerSlowdown = 0.5f;
    private bool isStaggered = false;

    [SerializeField] float failAnimDelay = 0.8f;

    [Header("References")]
    public PlayerMovement playerMovement;
    public ObstaclesCollision collisionHandler;
    public GameObject gameUI; 
    public GamePowerups powerups;
    public GameObject starterpwUI;

    [Header("NPC Chase")]
    public GameObject[] npcPrefabs;
    public float npcSpawnDistance = 3f;
    public LayerMask platformLayer;   
    public LayerMask obstacleLayer;
    GameObject activeNPC;
    public Slider chaseTimerSlider;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (chaseTimerSlider != null)
            chaseTimerSlider.gameObject.SetActive(false);
    }

    //ice tubig ui thingy
    void OnEnable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnIceTubigChanged += UpdateOwnedIceTubigUI;
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnIceTubigChanged -= UpdateOwnedIceTubigUI;
    }

    void UpdateOwnedIceTubigUI(int count)
    {
        if (iceTubigOwnedText != null)
            iceTubigOwnedText.text = count.ToString();
    }


    // fail
    public void OnPlayerFailed()
    {
        StartCoroutine(FailFlowRoutine());
    }

    IEnumerator FailFlowRoutine()
    {
        chaseActive = false;
        if (powerups != null) powerups.ResetAllPowerups();

        PlayFailAnimation();
        playerMovement.Freeze();

        yield return new WaitForSeconds(failAnimDelay);

        if (!firstFailUsed)
        {
            firstFailUsed = true;
            StartCoroutine(AutoReviveRoutine());
            yield break;
        }

        if (currentRevives > 0)
        {
            ShowSecondRevivePanel();
            yield break;
        }

        ShowFinalGameOver();
    }

    // phase 1 auto
    IEnumerator AutoReviveRoutine()
    {
        Time.timeScale = 0f;
        gameUI.SetActive(false);
        countdownFailUI.SetActive(true);
        starterpwUI.SetActive(false);

        float timer = autoReviveCountdown;
        while (timer > 0)
        {
            countdownText.text = Mathf.Ceil(timer).ToString();
            yield return new WaitForSecondsRealtime(1f);
            timer--;
        }

        countdownFailUI.SetActive(false);
        gameUI.SetActive(true);
        countdownText.text = "";
        
        RevivePlayerFirstPhase();
    }

    void RevivePlayerFirstPhase()
    {
        Time.timeScale = 1f;

        playerMovement.ResetAfterRevive();
        playerMovement.Unfreeze();
        ReturnToRunState();

        if (powerups != null)
        {
            powerups.ActivateInvincibility(3f);
        }

        collisionHandler.ResetFailState();

        if (activeNPC == null)
            SpawnChaseNPC();

        if (activeNPC != null)
        {
            NPCController npc = activeNPC.GetComponent<NPCController>();
            if (npc != null) npc.enabled = true;
        }

        StartChaseTimer();
    }

    // phase 2 second revive
    void ShowSecondRevivePanel()
    {
        Time.timeScale = 0f;
        gameUI.SetActive(false);
        secondRevivePanel.SetActive(true);
        secondReviveActive = true;

        if (secondReviveCoroutine != null)
            StopCoroutine(secondReviveCoroutine);

        // always start countdown regardless of currentRevives
        secondReviveCoroutine = StartCoroutine(SecondReviveCountdown());

        if (activeNPC != null)
        {
            NPCController npc = activeNPC.GetComponent<NPCController>();
            if (npc != null) npc.enabled = false;
        }

        UpdateReviveCounterUI();
    }

    IEnumerator SecondReviveCountdown()
    {
        float timeLeft = secondReviveDuration;

        while (timeLeft > 0f && secondReviveActive)
        {
            secondCountdownText.text = Mathf.Ceil(timeLeft).ToString();
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }

        secondReviveActive = false;
        secondRevivePanel.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1f;

        ShowFinalGameOver(); // after countdown, always show scoring
    }

    public void UseReviveItem()
    {
        if (!secondReviveActive) return;

        if (PlayerInventory.Instance.IceTubigCount > 0)
        {
            PlayerInventory.Instance.UseRevive();
            currentRevives--;
            UpdateReviveCounterUI();
        }
        else
        {
            // implement later, medj complicated for the time (buy in game)
            /*
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.ShowPanel(ShopManager.ShopItem.IceTubig);
            }
            */

            return;
        }

        secondReviveActive = false;

        if (secondReviveCoroutine != null)
            StopCoroutine(secondReviveCoroutine);

        secondRevivePanel.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1f;

        playerMovement.Unfreeze();
        playerMovement.ResetAfterRevive();
        ReturnToRunState();

        if (powerups != null)
            powerups.ActivateInvincibility(3f);

        collisionHandler.ResetFailState();

        if (!npcCaught)
        {
            ResumeChaseTimer();
        }

        if (activeNPC != null)
        {
            NPCController npc = activeNPC.GetComponent<NPCController>();
            if (npc != null) npc.enabled = true;
        }
    }

    void UpdateReviveCounterUI()
    {
        if (reviveCounterText != null)
            reviveCounterText.text = currentRevives + "/" + maxRevives;
    }


    // game over
    void ShowFinalGameOver()
    {
        Time.timeScale = 0f;
        gameUI.SetActive(false);
        finalGameOverPanel.SetActive(true);

        int runCoins = CoinCollect.Instance.coins;
        int distance = playerMovement.DistanceInMeters();
        int iceTubigCount = PlayerInventory.Instance.IceTubigCount;

        CoinCollect.Instance.SaveToTotal();
        CoinCollect.Instance.ResetCoins();

        HighScoreStorage.Instance.TrySetNewScore(distance);

        coinsText.text = runCoins.ToString(); 
        distanceText.text = distance.ToString();
        highScoreText.text = HighScoreStorage.Instance.HighScore.ToString();
        iceTubigOwnedText.text = PlayerInventory.Instance.IceTubigCount.ToString();

        // mission and unlock part
        if (MissionSystem.Instance != null)
        {
            MissionSystem.Instance.CommitRunProgress();
        }

        if (CharacterUnlockSystem.Instance != null)
        {
            CharacterUnlockSystem.Instance.CheckForUnlocks();
        }
    }

    // chase timer
    void StartChaseTimer()
    {
        chaseTimer = chaseDuration;
        chaseActive = true;
        ShowChaseTimerUI();

        if (chaseTimerRoutine != null) StopCoroutine(chaseTimerRoutine);
        chaseTimerRoutine = StartCoroutine(ChaseTimerRoutine());
    }

    void ResumeChaseTimer()
    {
        chaseTimer = chaseDuration;
        chaseActive = true;
        ShowChaseTimerUI();

        if (chaseTimerRoutine != null) StopCoroutine(chaseTimerRoutine);
        chaseTimerRoutine = StartCoroutine(ChaseTimerRoutine());
    }

    IEnumerator ChaseTimerRoutine()
    {
        while (chaseTimer > 0f && chaseActive)
        {
            chaseTimer -= Time.deltaTime;

            if (chaseTimerSlider != null)
            {
                chaseTimerSlider.value = chaseTimer / chaseDuration;
                if (chaseTimerSlider.fillRect != null)
                {
                    Image fill = chaseTimerSlider.fillRect.GetComponent<Image>();
                    if (fill != null)
                        fill.color = Color.Lerp(Color.red, Color.green, chaseTimer / chaseDuration);
                }
            }
            yield return null;
        }

        if (chaseActive)
        {
            chaseActive = false;
            HideChaseTimerUI();
            OnPlayerFailed();
        }
    }

    public void OnNPCCaught()
    {
        if (!chaseActive) return;

        chaseActive = false;

        if (chaseTimerRoutine != null)
        {
            StopCoroutine(chaseTimerRoutine);
            chaseTimerRoutine = null;
        }

        if (activeNPC != null)
        {
            // vfx later?
            Destroy(activeNPC);
        }

        HideChaseTimerUI();

        Debug.Log("CHASE WON");
        npcCaught = true;
        ReturnToNormalRun();
    }

    void ReturnToNormalRun()
    {
        if (playerMovement != null)
        {
            Debug.Log("Returning to standard endless run mode.");
        }
    }

    void ShowChaseTimerUI()
    {
        if (chaseTimerSlider != null) 
            chaseTimerSlider.gameObject.SetActive(true);
    }

    void HideChaseTimerUI()
    {
        if (chaseTimerSlider != null) 
            chaseTimerSlider.gameObject.SetActive(false);
    }

    public void SpawnChaseNPC()
    {
        if (playerMovement == null) return;

        Transform platform = GetPlayerCurrentPlatform();
        if (platform == null) return;

        Transform laneMiddle = platform.Find("LaneMiddle");
        if (laneMiddle == null) return;

        float roadCenterX = laneMiddle.position.x;
        float laneWidth = 3f;

        float roadY = -17.51f;

        int selectedLane = Random.Range(0, 3);

        float targetX = roadCenterX + (selectedLane - 1) * laneWidth;
        float spawnZ = playerMovement.transform.position.z + npcSpawnDistance;

        Vector3 spawnPos = new Vector3(targetX, roadY + 0.05f, spawnZ);
    
        int maxAttempts = 10;
        bool spotFound = false;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 checkArea = new Vector3(targetX, roadY + 1f, spawnZ + (i * 5f));

            if (!Physics.CheckBox(checkArea, new Vector3(1.2f, 1f, 4f), Quaternion.identity, obstacleLayer))
            {
                spawnPos.z = spawnZ + (i * 5f);
                spotFound = true;
                break;
            }
            selectedLane = (selectedLane + 1) % 3;
            targetX = roadCenterX + (selectedLane - 1) * laneWidth;
        }

        if (!spotFound) spawnPos.z += 10f;

        int playerIndex = CharacterSystem.Instance != null ? CharacterSystem.Instance.selectedCharacterIndex : 0;

        List<GameObject> possibleNPCs = new List<GameObject>();

        for (int i = 0; i < npcPrefabs.Length; i++)
        {
            
            if (i != playerIndex)
            {
                possibleNPCs.Add(npcPrefabs[i]);
            }
        }

        GameObject prefabToSpawn = npcPrefabs[0];

        if (possibleNPCs.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleNPCs.Count);
            prefabToSpawn = possibleNPCs[randomIndex];
        }

        activeNPC = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        CharacterController cc = activeNPC.GetComponent<CharacterController>();
        NPCController npc = activeNPC.GetComponent<NPCController>();

        if (cc != null) cc.enabled = false;

        activeNPC.transform.position = spawnPos;

        if (npc != null)
        {
            npc.playerRef = playerMovement;
            npc.roadCenterX = roadCenterX;
            npc.currentLane = selectedLane;
            npc.laneDistance = laneWidth;
        }

        StartCoroutine(ReEnableController(cc));
    }

    IEnumerator ReEnableController(CharacterController cc)
    {
        yield return null; 
        if (cc != null) cc.enabled = true;
    }

    Transform GetPlayerCurrentPlatform()
    {
        RaycastHit hit;
        Vector3 rayOrigin = playerMovement.transform.position + Vector3.up * 2f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 20f, platformLayer))
        {
            return hit.collider.transform.root;
        }
        return null;
    }

    // additional thingies
    public void ReturnToMenuSafe()
    {
        Time.timeScale = 1f;
        MusicBGManager.Instance.PlayMenuMusic();
        SceneManager.LoadScene("Main Menu");
    }

    public void StaggerPlayer()
    {
        if (isStaggered) return;
        StartCoroutine(StaggerRoutine());
    }

    IEnumerator StaggerRoutine()
    {
        isStaggered = true;
        Animator anim = playerMovement.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Stagger");

        float originalSpeed = playerMovement.currentSpeed;
        playerMovement.OnStagger(staggerSlowdown);
        yield return new WaitForSeconds(staggerDuration);
        playerMovement.RecoverFromStagger(originalSpeed);
        isStaggered = false;
    }

    public void PlayFailAnimation()
    {
        Animator anim = playerMovement.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Fail");
    }

    void ReturnToRunState()
    {
        Animator anim = playerMovement.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Fail");
            anim.ResetTrigger("Stagger");
            anim.Play("Movement", 0, 0f);
        }
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        CoinCollect.Instance.ResetCoins();
        playerMovement.ResetDistance();
        playerMovement.ResetSpeed();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}