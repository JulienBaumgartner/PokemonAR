using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameManager : MonoBehaviour
{
    #if UNITY_ANDROID
    public MyImageManager myImageManager;
    public ARTrackedImageManager arTrackedImageManager;
    public ARTapToPlaceObject arTapToPlaceObject;
    #endif
    public GameObject canvas;
    private PokemonData ownDatas = new PokemonData();
    private PokemonData enemyDatas = new PokemonData();
    private Animator ownAnimator;
    private Animator enemyAnimator;
    private Object[] prefabs;
    private Transform ownPanel, enemyPanel, panelAtk, panelInfo, panelEnd;
    private Button[] btsAtk;
    private bool hasAttacked = false;
    private bool lerpDamage = false;
    
    enum State {
        PlacePlane,
        ChoosePkmn,
        FightWait,
        FightOwn,
        FightEnemy,
        End
    };

    State currentState = State.PlacePlane;

    // Start is called before the first frame update
    void Start()
    {
        ownPanel = canvas.transform.Find("PanelOwn");
        enemyPanel = canvas.transform.Find("PanelEnemy");
        panelAtk = canvas.transform.Find("PanelAtk");
        panelInfo = canvas.transform.Find("PanelInfo");
        panelEnd = canvas.transform.Find("PanelEnd");
        btsAtk = panelAtk.GetComponentsInChildren<Button>();
        panelAtk.gameObject.SetActive(false);
        panelEnd.gameObject.SetActive(false);
        canvas.SetActive(false);
        prefabs = Resources.LoadAll("Prefabs", typeof(GameObject));
#if UNITY_ANDROID
        myImageManager.enabled = false;
        arTrackedImageManager.enabled = false;
#else
        PrepareGame();
#endif
    }
#if UNITY_ANDROID
    public void PlanePlaced(GameObject plane)
    {
        //called from arTapToPlaceObject
        GameObject prefab = (GameObject)prefabs[Random.Range(0, prefabs.Length)];
        int pokemonId = int.Parse(prefab.name);

        //TODO: Set enemy animator
        enemyAnimator = Instantiate(prefab, plane.transform.Find("EnemyPkmn")).GetComponent<Animator>();
        StartCoroutine(enemyDatas.RequestPokemonInfo(pokemonId));
        
        arTrackedImageManager.enabled = true;
        myImageManager.enabled = true;
        currentState = State.ChoosePkmn;
    }

    public void PkmonChoosed(int pokemonId)
    {
        //called from PokemonPreview
        foreach (var track in arTrackedImageManager.trackables)
        {
            Destroy(track.gameObject);
        }
    //TODO: Set own animator
        StartCoroutine(ownDatas.RequestPokemonInfo(pokemonId));
        ownAnimator = GameObject.Find("Gamepiece").transform.Find("OwnPkmn").GetChild(0).GetComponent<Animator>();
        myImageManager.enabled = false;
        arTrackedImageManager.enabled = false;

        currentState = State.FightWait;
    }

#else
    public void PrepareGame()
    {
        GameObject prefabOwn = (GameObject)prefabs[Random.Range(0, prefabs.Length)];
        GameObject prefabEnemy = (GameObject)prefabs[Random.Range(0, prefabs.Length)];
        int pokemonIdOwn = int.Parse(prefabOwn.name);
        int pokemonIdEnemy = int.Parse(prefabEnemy.name);
        ownAnimator = Instantiate(prefabOwn, GameObject.Find("Gamepiece").transform.Find("OwnPkmn")).GetComponent<Animator>();
        enemyAnimator = Instantiate(prefabEnemy, GameObject.Find("Gamepiece").transform.Find("EnemyPkmn")).GetComponent<Animator>();
        StartCoroutine(ownDatas.RequestPokemonInfo(pokemonIdOwn));
        StartCoroutine(enemyDatas.RequestPokemonInfo(pokemonIdEnemy));

        currentState = State.FightWait;
    }
    
#endif

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(currentState == State.FightWait && ownDatas.isLoadingEnded && enemyDatas.isLoadingEnded)
        {
            currentState = State.FightOwn;
            canvas.SetActive(true);

            UpdatePanel(ownPanel, ownDatas);
            UpdatePanel(enemyPanel, enemyDatas);
        }
        else if (currentState == State.FightOwn || currentState == State.FightEnemy)
        {
            UpdatePanel(ownPanel, ownDatas);
            UpdatePanel(enemyPanel, enemyDatas);
        }

        // SHOW ATTACK BUTTON
        if (currentState == State.FightOwn && !hasAttacked)
        {
            panelAtk.gameObject.SetActive(true);
            if(ownDatas.moves.Count < 4)
            {
                for (int i = 3; i >= ownDatas.moves.Count; i--)
                {
                    btsAtk[i].gameObject.SetActive(false);
                }
            }
            for(int i = 0; i < ownDatas.moves.Count && i < 4; i++)
            {
                btsAtk[i].gameObject.SetActive(true);
                MoveDetails move = ownDatas.moves[ownDatas.moves.Count - Mathf.Min(ownDatas.moves.Count, 4) + i];
                btsAtk[i].GetComponentInChildren<Text>().text = move.name + "\n(" + move.power + ", " + move.damageType.ToString() + ", "+ move.type.ToString() + ")";
            }
        }

        if(currentState == State.FightOwn && hasAttacked && lerpDamage)
        {
            enemyDatas.crtHP = (int)Mathf.Lerp(enemyDatas.crtHP, enemyDatas.targetHP, 0.25f);
            if (enemyDatas.crtHP <= enemyDatas.targetHP)
            {
                enemyDatas.crtHP = enemyDatas.targetHP;
                currentState = State.FightEnemy;
                hasAttacked = false;
                lerpDamage = false;
            }
        }

        if (enemyDatas.crtHP <= 0 && enemyDatas.isLoadingEnded)
        {
            enemyDatas.crtHP = 0;
            UpdatePanel(enemyPanel, enemyDatas);
            enemyAnimator.SetTrigger("ko");
            panelEnd.gameObject.SetActive(true);
            panelEnd.GetComponentInChildren<Text>().text = "Victory";
            currentState = State.End;
        }

        if (currentState == State.FightEnemy && !hasAttacked)
        {
            EnemyAttack();
        }

        if (currentState == State.FightEnemy && hasAttacked && lerpDamage)
        {
            ownDatas.crtHP = (int)Mathf.Lerp(ownDatas.crtHP, ownDatas.targetHP, 0.25f);
            if (ownDatas.crtHP <= ownDatas.targetHP)
            {
                ownDatas.crtHP = ownDatas.targetHP;
                currentState = State.FightOwn;
                hasAttacked = false;
                lerpDamage = false;
            }
        }

        if(ownDatas.crtHP <= 0 && ownDatas.isLoadingEnded)
        {
            ownDatas.crtHP = 0;
            UpdatePanel(ownPanel, ownDatas);
            ownAnimator.SetTrigger("ko");
            panelEnd.gameObject.SetActive(true);
            panelEnd.GetComponentInChildren<Text>().text = "Defeat";
            currentState = State.End;
        }

        

    }

    private void UpdatePanel(Transform panel, PokemonData data)
    {
        Text hpText = panel.Find("TextHP").GetComponent<Text>();
        Text nameText = panel.Find("Text").GetComponent<Text>();
        RectTransform hpBar = panel.Find("ImageHPEmpty").Find("ImageHPFull").GetComponent<RectTransform>();
        Image hpBarImage = panel.Find("ImageHPEmpty").Find("ImageHPFull").GetComponent<Image>();

        nameText.text = data.pkmnName;
        hpText.text = data.crtHP + " / " + data.maxHP;
        hpBar.localScale = new Vector3(data.crtHP / (float)data.maxHP, 1, 1);
        hpBarImage.color = hpBar.localScale.x > 0.5f ? Color.green : hpBar.localScale.x > 0.2f ? Color.yellow : Color.red;
    }

    public void OwnAttackButton(int i)
    {
        MoveDetails move = ownDatas.moves[ownDatas.moves.Count - Mathf.Min(ownDatas.moves.Count, 4) + i];

        if(move.damageType == MoveDetails.DamageType.physical)
        {
            ownAnimator.SetTrigger("attack1");
        }
        else if (move.damageType == MoveDetails.DamageType.special)
        {
            ownAnimator.SetTrigger("attack2");
        }
        else
        {
            Debug.LogError("Error: attack is not physical or special");
        }

        StartCoroutine(waitAndAnime(false, 1.5f, "hit", computeDamage(move, ownDatas, enemyDatas)));
        hasAttacked = true;
        panelAtk.gameObject.SetActive(false);
        panelInfo.GetComponentInChildren<Text>().text = ownDatas.pkmnName + " used " + move.name;
        float efficacity = attackEfficacity(move.type, enemyDatas.type);
        if (efficacity > 1)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's super effective!";
        }
        else if (efficacity < 1)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's not very effective...";
        }
        else if (efficacity == 0)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's doesn't affect enemy " + enemyDatas.pkmnName;
        }

    }

    private void EnemyAttack()
    {
        MoveDetails move = enemyDatas.moves[Random.Range(0, enemyDatas.moves.Count)];

        if (move.damageType == MoveDetails.DamageType.physical)
        {
            enemyAnimator.SetTrigger("attack1");
        }
        else if (move.damageType == MoveDetails.DamageType.special)
        {
            enemyAnimator.SetTrigger("attack2");
        }
        else
        {
            Debug.LogError("Error: attack is not physical or special (enemy)");
        }

        StartCoroutine(waitAndAnime(true, 1.5f, "hit", computeDamage(move, enemyDatas, ownDatas)));
        hasAttacked = true;
        panelInfo.GetComponentInChildren<Text>().text = "Enemy " + enemyDatas.pkmnName + " used " + move.name;
        float efficacity = attackEfficacity(move.type, ownDatas.type);
        if(efficacity > 1)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's super effective!";
        }
        else if(efficacity < 1)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's not very effective...";
        }
        else if(efficacity == 0)
        {
            panelInfo.GetComponentInChildren<Text>().text += "\nIt's doesn't affect " + ownDatas.pkmnName;
        }

    }

    private int computeDamage(MoveDetails move, PokemonData attacker, PokemonData target)
    {
        int damages = 0;
        float modifiers = attackEfficacity(move.type, target.type);
        
        if(move.damageType == MoveDetails.DamageType.physical)
        {
            damages = (int)(((2 * PokemonData.LEVEL / 5 + 2) * move.power * attacker.attack/target.defense / 50 + 2) * modifiers);
        }
        else if (move.damageType == MoveDetails.DamageType.special)
        {
            damages = (int)(((2 * PokemonData.LEVEL / 5 + 2) * move.power * attacker.attackSpe / target.defenseSpe / 50 + 2) * modifiers);
        }
        return damages;
    }

    private float attackEfficacity(PokemonData.Types atkType, PokemonData.Types targetType)
    {
        return PokemonData.typeArray[(int)atkType, (int)targetType];
    }

    public void Restart()
    {
        ownDatas.isLoadingEnded = false;
        ownDatas.moves.Clear();
        enemyDatas.isLoadingEnded = false;
        enemyDatas.moves.Clear();
        Destroy(GameObject.Find("Gamepiece").transform.Find("OwnPkmn").GetChild(0).gameObject);
        Destroy(GameObject.Find("Gamepiece").transform.Find("EnemyPkmn").GetChild(0).gameObject);

        panelAtk.gameObject.SetActive(false);
        panelEnd.gameObject.SetActive(false);
        canvas.SetActive(false);
#if UNITY_ANDROID
        myImageManager.enabled = false;
        arTrackedImageManager.enabled = false;
#else
        PrepareGame();
#endif
    }

    private IEnumerator waitAndAnime(bool own, float timeToWait, string trigger, int damage)
    {
        yield return new WaitForSeconds(timeToWait);
        if(own)
        {
            ownAnimator.SetTrigger(trigger);
            ownDatas.targetHP -= damage;
            ownDatas.targetHP = Mathf.Max(ownDatas.targetHP, 0);
        }
        else
        {
            enemyAnimator.SetTrigger(trigger);
            enemyDatas.targetHP -= damage;
            enemyDatas.targetHP = Mathf.Max(enemyDatas.targetHP, 0);
        }
        lerpDamage = true;
    }
}
