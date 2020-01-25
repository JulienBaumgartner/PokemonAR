using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PokemonData
{

    public const int LEVEL = 50;

    public enum Types
    {
        steel,
        fighting,
        dragon,
        water,
        electric,
        fire,
        ice,
        bug,
        normal,
        grass,
        poison,
        psychic,
        rock,
        ground,
        ghost,
        dark,
        flying,
        fairy
    }

    public static float[,] typeArray =
        //steel,fighting,  dragon,  water, electric, fire,   ice,  bug,  normal,   grass,   poison,   psychic,  rock,    ground,  ghost,    dark,   flying,  fairy
        { {0.5f,   1.0f,    1.0f,   0.5f,     0.5f,  0.5f,  2.0f,  1.0f,  1.0f,    1.0f,     1.0f,      1.0f,    2.0f,    1.0f,    1.0f,    1.0f,    1.0f,    2.0f}, //steel,
          {2.0f,   1.0f,    1.0f,   1.0f,     1.0f,  1.0f,  2.0f,  0.5f,  2.0f,    1.0f,     0.5f,      0.5f,    1.0f,    1.0f,    0.0f,    1.0f,    0.5f,    0.5f,}, //fighting,
          {0.5f,   1.0f,    2.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  1.0f,    1.0f,     1.0f,      1.0f,    1.0f,    1.0f,    1.0f,    1.0f,    1.0f,    0.0f}, //dragon,
          {1.0f,   1.0f,    0.5f,   0.5f,     1.0f,  2.0f,  1.0f,  1.0f,  1.0f,    0.5f,     1.0f,      1.0f,    2.0f,    2.0f,    1.0f,    1.0f,    1.0f,    1.0f}, //water,
          {1.0f,   1.0f,    0.5f,   2.0f,     0.5f,  1.0f,  1.0f,  1.0f,  1.0f,    0.5f,     1.0f,      1.0f,    1.0f,    1.0f,    1.0f,    1.0f,    2.0f,    1.0f}, //electric,
          {2.0f,   1.0f,    0.5f,   0.5f,     1.0f,  0.5f,  2.0f,  2.0f,  1.0f,    2.0f,     1.0f,      1.0f,    0.5f,    1.0f,    1.0f,    1.0f,    1.0f,    1.0f}, //fire,
          {0.5f,   1.0f,    2.0f,   0.5f,     1.0f,  0.5f,  0.5f,  1.0f,  1.0f,    2.0f,     1.0f,      1.0f,    1.0f,    2.0f,    1.0f,    1.0f,    2.0f,    1.0f}, //ice,
          {0.5f,   0.5f,    1.0f,   1.0f,     1.0f,  0.5f,  1.0f,  1.0f,  1.0f,    2.0f,     0.5f,      2.0f,    1.0f,    1.0f,    0.5f,    2.0f,    0.5f,    0.5f}, //bug,
          {0.5f,   1.0f,    1.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  1.0f,    1.0f,     1.0f,      1.0f,    0.5f,    1.0f,    0.0f,    1.0f,    1.0f,    1.0f}, //normal,
          {0.5f,   1.0f,    0.5f,   2.0f,     1.0f,  0.5f,  1.0f,  0.5f,  1.0f,    0.5f,     0.5f,      1.0f,    2.0f,    2.0f,    1.0f,    1.0f,    0.5f,    1.0f}, //grass,
          {0.0f,   1.0f,    1.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  1.0f,    2.0f,     0.5f,      1.0f,    0.5f,    0.5f,    0.5f,    1.0f,    1.0f,    2.0f}, //poison,
          {0.5f,   2.0f,    1.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  1.0f,    1.0f,     2.0f,      0.5f,    1.0f,    1.0f,    1.0f,    0.0f,    1.0f,    1.0f}, //psychic,
          {0.5f,   0.5f,    1.0f,   1.0f,     1.0f,  2.0f,  2.0f,  2.0f,  1.0f,    1.0f,     1.0f,      1.0f,    1.0f,    0.5f,    1.0f,    1.0f,    2.0f,    1.0f}, //rock,
          {2.0f,   1.0f,    1.0f,   0.5f,     2.0f,  2.0f,  1.0f,  1.0f,  1.0f,    1.0f,     2.0f,      1.0f,    2.0f,    0.5f,    1.0f,    1.0f,    0.0f,    1.0f}, //ground,
          {1.0f,   1.0f,    1.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  0.0f,    1.0f,     1.0f,      2.0f,    1.0f,    1.0f,    2.0f,    0.5f,    1.0f,    1.0f}, //ghost,
          {1.0f,   1.0f,    1.0f,   1.0f,     1.0f,  1.0f,  1.0f,  1.0f,  1.0f,    1.0f,     1.0f,      2.0f,    1.0f,    1.0f,    2.0f,    0.5f,    1.0f,    0.5f}, //dark,
          {1.0f,   2.0f,    1.0f,   1.0f,     0.5f,  1.0f,  1.0f,  2.0f,  1.0f,    2.0f,     1.0f,      1.0f,    0.5f,    1.0f,    1.0f,    1.0f,    1.0f,    1.0f}, //flying,
          {0.5f,   2.0f,    2.0f,   1.0f,     1.0f,  0.5f,  1.0f,  1.0f,  1.0f,    1.0f,     0.5f,      1.0f,    1.0f,    1.0f,    1.0f,    2.0f,    1.0f,    1.0f}, //fairy
        };                                         

    public int id;
    public string pkmnName;
    public Types type;
    public int maxHP;
    public int crtHP;
    public int targetHP;
    public int attack;
    public int attackSpe;
    public int defense;
    public int defenseSpe;
    public int speed;
    public List<MoveDetails> moves = new List<MoveDetails>();
    public bool isLoadingEnded = false;
    
    public void PrintData()
    {
        if(isLoadingEnded)
        {
            Debug.Log("name: " + pkmnName);
            Debug.Log("type: " + type.ToString());
            Debug.Log("HP: " + crtHP + " / " +maxHP);
            Debug.Log("Speed: " + speed);
            Debug.Log("Attack: " + attack);
            Debug.Log("Attack Spe: " + attackSpe);
            Debug.Log("Defense: " + defense);
            Debug.Log("Defense Spe: " + defenseSpe);
            foreach(MoveDetails m in moves)
            {
                Debug.Log("Move " + m.name + ":");
                Debug.Log("\t Type: " + m.type.ToString());
                Debug.Log("\t Power: " + m.power);
                Debug.Log("\t Damage type: " + m.damageType.ToString());
            }
        }
        else
        {
            Debug.Log("Loading...");
        }
    }

    public IEnumerator RequestPokemonInfo(int id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://pokeapi.co/api/v2/pokemon/" + id))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                Debug.Log("Nope, tu veux un magicarpe?");
            }
            else
            {
                PokemonJson pokemonJson = JsonUtility.FromJson<PokemonJson>(webRequest.downloadHandler.text);
                pkmnName = char.ToUpper(pokemonJson.name[0]) + pokemonJson.name.Substring(1); ;
                this.id = pokemonJson.id;
                foreach(TypeSlot t in pokemonJson.types)
                {
                    if(t.slot == 1)
                    {
                        type = (Types)Enum.Parse(typeof(Types), t.type.name);
                        break;
                    }
                }
                foreach(StatValue s in pokemonJson.stats)
                {
                    switch(s.stat.name)
                    {
                        case "hp":
                            int iv = 31;
                            maxHP = (2 * s.base_stat + iv) * LEVEL / 100 + LEVEL + 10;
                            crtHP = maxHP;
                            targetHP = maxHP;
                            break;
                        case "speed":
                            iv = 31;
                            speed = (2 * s.base_stat + iv) * LEVEL / 100 + 5;
                            break;
                        case "special-defense":
                            iv = 31;
                            defenseSpe = (2 * s.base_stat + iv) * LEVEL / 100 + 5;
                            break;
                        case "special-attack":
                            iv = 31;
                            attackSpe = (2 * s.base_stat + iv) * LEVEL / 100 + 5;
                            break;
                        case "defense":
                            iv = 31;
                            defense = (2 * s.base_stat + iv) * LEVEL / 100 + 5;
                            break;
                        case "attack":
                            iv = 31;
                            attack = (2 * s.base_stat + iv) * LEVEL / 100 + 5;
                            break;
                    }
                }
                foreach(MoveDescription m in pokemonJson.moves)
                {
                    foreach (VersionGroupDetail v in m.version_group_details)
                    {
                        if(v.level_learned_at > 0 && v.level_learned_at <= LEVEL && v.version_group.name == "red-blue")
                        {
                            using (UnityWebRequest webRequestAttack = UnityWebRequest.Get(m.move.url))
                            {
                                yield return webRequestAttack.SendWebRequest();
                                if (webRequest.isNetworkError)
                                {
                                    Debug.Log("Attaque lutte!");
                                }
                                else
                                {
                                    MoveJson moveJson = JsonUtility.FromJson<MoveJson>(webRequestAttack.downloadHandler.text);
                                    if(moveJson.power > 0)
                                    {
                                        MoveDetails moveDetails = new MoveDetails();
                                        moveDetails.name = moveJson.name;
                                        moveDetails.power = moveJson.power;
                                        moveDetails.level = v.level_learned_at;
                                        moveDetails.type = (Types)Enum.Parse(typeof(Types), moveJson.type.name);
                                        moveDetails.damageType = (MoveDetails.DamageType)Enum.Parse(typeof(MoveDetails.DamageType), moveJson.damage_class.name);
                                        moves.Add(moveDetails);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        moves.Sort((a, b) => a.level.CompareTo(b.level));
        isLoadingEnded = true;
    }
}

public class MoveDetails
{
    public enum DamageType
    {
        physical,
        special,
        status
    }

    public string name;
    public int power;
    public int level;
    public PokemonData.Types type;
    public DamageType damageType;
}