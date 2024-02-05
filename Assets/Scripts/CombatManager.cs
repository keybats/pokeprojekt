using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public AttackScript testMove;
    public enum Type {None, Normal, Fighting, Flying, Poison, Ground, Rock, Bug, Ghost, Steel, Fire, Water, Grass, Electric, Psychic, Ice, Dragon, Dark, Fairy }
    float sameTypeAttackBonus = 1f;
    float critical = 1f;
    EncounterManager encounter;
    //List<MonScript> battlingPlayerTeamPokemon = new List<MonScript>();
    //List<MonScript> battlingEnemyTeamPokemon = new List<MonScript>();
    //Dictionary<float, MonScript> battlingPokemon = new Dictionary<float, MonScript>(6);
    List<MonScript> battlingPokemon = new List<MonScript>();

    float confusionAttackPower = 40f;

    float burnDamageMultiplier;
    readonly float ExpMultiplier = 0.21f;
    int numberOfPokemonPerTeam;

    MonScript playerTarget;

    //float criticalChanceMultiplier;
    //weather

    //attacker, defender
    float[,] TypeResistances = { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, 
                                 { 1, 1, 1, 1, 1, 1, 0.5f, 1, 0, 0.5f, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                                 { 1, 2, 1, 0.5f, 0.5f, 1, 2, 0.5f, 0, 2, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f },
                                 { 1, 1, 2, 1, 1, 1, 0.5f, 2, 1, 0.5f, 1, 1, 2, 0.5f, 1, 1, 1, 1, 1 },
                                 { 1, 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2 },
                                 { 1, 1, 1, 0, 2, 1, 2, 0.5f, 1, 2, 2, 1, 0.5f, 2, 1, 1, 1, 1, 1 },
                                 { 1, 1, 0.5f, 2, 1, 0.5f, 1, 2, 1, 0.5f, 2, 1, 1, 1, 1, 2, 1, 1, 1 },
                                 { 1, 1, 0.5f, 0.5f, 0.5f, 1, 1, 1, 0.5f, 0.5f, 0.5f, 1, 2, 1, 2, 1, 1, 2, 0.5f },
                                 { 1, 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 1 },
                                 { 1, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f, 0.5f, 1, 0.5f, 1, 2, 1, 1, 2 },
                                 { 1, 1, 1, 1, 1, 1, 0.5f, 2, 1, 2, 0.5f, 0.5f, 2, 1, 1, 2, 0.5f, 1, 1 },
                                 { 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
                                 { 1, 1, 1, 0.5f, 0.5f, 2, 2, 0.5f, 1, 0.5f, 0.5f, 2, 0.5f, 1, 1, 1, 0.5f, 1, 1 },
                                 { 1, 1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, 0.5f, 0.5f, 1, 1, 0.5f, 1, 1 },
                                 { 1, 1, 2, 1, 2, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 0.5f, 1, 1, 0, 1 },
                                 { 1, 1, 1, 2, 1, 2, 1, 1, 1, 0.5f, 0.5f, 0.5f, 2, 1, 1, 0.5f, 2, 1, 1 },
                                 { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1, 1, 1, 2, 1, 0, },
                                 { 1, 1, 0.5f, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5f, 0.5f },
                                 { 1, 1, 2, 1, 0.5f, 1, 1, 1, 1, 0.5f, 0.5f, 1, 1, 1, 1, 1, 2, 2, 1 } };

    // Start is called before the first frame update
    void Start()
    {
        encounter = GetComponent<EncounterManager>();
        
        
    }
    public void StartGame()
    {
        CombatStart(1, 8);
    }

    public void CombatStart(int numberOfOpponents, int opponentLevels)
    {
        
        for (int i = 0; i < numberOfOpponents; i++)
        {
            battlingPokemon.Add(encounter.playerPokemon[i]);
            MonScript newBattlingPokemon = encounter.CreatePokemon(opponentLevels, false);
            battlingPokemon.Add(newBattlingPokemon);
        }
        numberOfPokemonPerTeam = numberOfOpponents;
    }

    public void Attack(MonScript target, MonScript attacker, AttackScript move)
    {
        Debug.Log(attacker.name + " used " + move.name);
        float resistance = TypeResistances[(int)move.moveType, (int)target.type[0]] * TypeResistances[(int)move.moveType, (int)target.type[1]];
        
        if (attacker.statuses.Contains("freeze") && Random.Range(0, 4) != 0)
        {
            Debug.Log(attacker.name + " is frozen and cannot attack");
            return;
        }
        else if (attacker.statuses.Contains("freeze"))
        {
            attacker.statuses.Remove("freeze");
            Debug.Log(attacker.name + " is no longer frozen");
        }

        if (attacker.statuses.Contains("flinch"))
        {
            Debug.Log(attacker.name + " flinched");
            attacker.statuses.Remove("flinch");
            return;
        }

        if (attacker.statuses.Contains("confusion") && Random.Range(0, 100) < 33)
        {
            Debug.Log(attacker.name + " hit itself in its confusion for " + attacker.TakeDamage(((( 2 * attacker.level / 5) + 2) * confusionAttackPower * (attacker.attack / target.defence) / 50) + 2) *
                    (Random.Range(85f, 101f) / 100) + " damage");
            return;
        }
        else if (attacker.statuses.Contains("confusion") && Random.Range(0, 4) == 0)
        {
            Debug.Log(attacker.name + " snapped out of its confusion");
            attacker.statuses.Remove("confusion");
        } 

        if (attacker.statuses.Contains("paralysis") && Random.Range(0, 4) == 0)
        {
            Debug.Log(attacker.name + " couldn't move due to being paralized");
            return;
        }

        if (attacker.statuses.Contains("burn"))
        {
            burnDamageMultiplier = 0.5f;
        }
        else
        {
            burnDamageMultiplier = 1f;
        }

        if (attacker.type.Contains(move.moveType) && move.moveType != Type.None)
        {
            sameTypeAttackBonus = 1.5f;
        }
        else
        {
            sameTypeAttackBonus = 1f;
        }

        for (int i = 0; i < move.numberOfAdditionalAttacks + 1; i++)
        {
            if (target.tempEvasion > attacker.tempAccuracy * (move.accuracy) * (Random.Range(100.0f, 201.0f) / 100) && !move.isAutoHit)
            {
                Debug.Log("miss");
            }
            else
            {
                if (Random.Range(1.00f, 100f) < 4.18f)
                {
                    critical = 1.5f;
                    Debug.Log("Critical hit!");
                }
                else
                {
                    critical = 1f;
                }
                if (move.power != 0)
                {
                    if (move.isPhysical)
                    {
                        Debug.Log(target.TakeDamage(((((2 * attacker.level / 5) + 2) * move.power * (attacker.attack / target.defence) / 50) + 2) * critical *
                        (Random.Range(85f, 101f) / 100) * sameTypeAttackBonus * resistance * burnDamageMultiplier));
                    }
                    else
                    {
                        Debug.Log(target.TakeDamage(((((2 * attacker.level / 5) + 2) * move.power * (attacker.specialAttack / target.specialDefence) / 50) + 2) * critical *
                        (Random.Range(85f, 101f) / 100) * sameTypeAttackBonus * resistance));
                    }
                    if (resistance == 2)
                    {
                        Debug.Log("it's super effective");
                    }
                    else if (resistance == 0)
                    {
                        Debug.Log("it had no effect");
                        return;
                    }
                    else if (resistance < 1)
                    {
                        Debug.Log("it wasn't very effective");
                    }
                    if (move.moveType == Type.Fire && target.statuses.Contains("freeze"))
                    {
                        target.statuses.Remove("freeze");
                        Debug.Log(target.name + " was thawed by " + move.name);
                    }
                }
                if (move.status != "")
                {
                    if (move.effectChance >= Random.Range(0f, 1.01f) && !(move.status == "paralysis" && target.type.Contains(Type.Ground) && move.moveType == Type.Electric) && !target.immunities.Contains(move.status))
                    {
                        target.statuses.Add(move.status);
                        Debug.Log(target.name + " was affected by " + move.status);
                        target.SetStats(false);
                    }
                }
                if (move.stat != "")
                {
                    if (move.effectChance >= Random.Range(0f, 1.01f) || move.effectChance == 0)
                    {
                        MonScript affectedMon = target;
                        Debug.Log(move.affectedPokemon);
                        if (move.affectedPokemon == "user")
                        {

                            affectedMon = attacker;
                            //Debug.Log(affectedMon.name);
                        }
                        if (move.stat == "hp")
                        {
                            affectedMon.statChanges[0] = Mathf.Clamp(affectedMon.statChanges[0] + move.stages, -6, 6);
                        }
                        if (move.stat == "attack")
                        {
                            affectedMon.statChanges[1] = Mathf.Clamp(affectedMon.statChanges[1] + move.stages, -6, 6);
                        }
                        if (move.stat == "defence")
                        {
                            affectedMon.statChanges[2] = Mathf.Clamp(affectedMon.statChanges[2] + move.stages, -6, 6);
                        }
                        if (move.stat == "specialAttack")
                        {
                            affectedMon.statChanges[3] = Mathf.Clamp(affectedMon.statChanges[3] + move.stages, -6, 6);
                        }
                        if (move.stat == "specialDefence")
                        {
                            affectedMon.statChanges[4] = Mathf.Clamp(affectedMon.statChanges[4] + move.stages, -6, 6);
                        }
                        if (move.stat == "speed")
                        {
                            affectedMon.statChanges[5] = Mathf.Clamp(affectedMon.statChanges[5] + move.stages, -6, 6);
                        }
                        if (move.stat == "evasion")
                        {
                            affectedMon.statChanges[6] = Mathf.Clamp(affectedMon.statChanges[6] + move.stages, -6, 6);
                        }
                        if (move.stat == "accuracy")
                        {
                            affectedMon.statChanges[7] = Mathf.Clamp(affectedMon.statChanges[7] + move.stages, -6, 6);
                        }
                        if (move.stages > 0)
                        {
                            Debug.Log(affectedMon.name + "'s " + move.stat + " rises");
                        }
                        
                        else
                        {
                            Debug.Log(affectedMon.name + "'s " + move.stat + " decreases");
                        }
                        affectedMon.SetStats(false);
                    }
                }
            }


        }
    }

    int SortBySpeed(MonScript mon1, MonScript mon2)
    {
        return mon1.speed.CompareTo(mon2.speed);
    }

    public void CombatRound(int moveNumber)
    {
        battlingPokemon.Sort(SortBySpeed);
        
        foreach (MonScript pokemon in battlingPokemon)
        {
            if (!pokemon.hasFainted)
            {
                if (!pokemon.isPlayerPokemon)
                {
                    MonScript target = null;
                    for (int i = 0; i < battlingPokemon.Count; i++)
                    {
                        if (battlingPokemon[i].isPlayerPokemon)
                        {
                            target = battlingPokemon[i];
                        }
                    }

                    Attack(target, pokemon, pokemon.moves[Random.Range(0, 4)]);
                }
                else
                {
                    Attack(playerTarget, pokemon, pokemon.moves[moveNumber]);
                }
            }
            else
            {
                pokemon.Faint();
            }
        }
        
        /*
        if (battlingPokemon[0].speed < battlingPokemon[1].speed)
        {
            Attack(battlingPokemon[0], battlingPokemon[1], battlingPokemon[1].moves[Random.Range(0, 4)]);
            if (!battlingPokemon[0].hasFainted)
            {
                Attack(battlingPokemon[1], battlingPokemon[0], battlingPokemon[0].moves[moveNumber]);
            }
            else
            {
                battlingPokemon[0].Faint();
            }

        }
        else
        {
            Attack(battlingPokemon[1], battlingPokemon[0], battlingPokemon[0].moves[moveNumber]);
            if (!battlingPokemon[1].hasFainted)
            {
                Attack(battlingPokemon[0], battlingPokemon[1], battlingPokemon[1].moves[Random.Range(0, 4)]);
            }
            else
            {
                battlingPokemon[1].Faint();
            }
        }
        */

        foreach (MonScript pokemon in battlingPokemon)
        {
            pokemon.statuses.Remove("flinch");
            if (pokemon.statuses.Contains("poison"))
            {
                Debug.Log(pokemon.name + " took " + pokemon.TakeDamage(pokemon.maxHp / 8) + " from poison");
            }
            if (pokemon.statuses.Contains("burn"))
            {
                Debug.Log(pokemon.name + " took " + pokemon.TakeDamage(pokemon.maxHp / 16) + " from burning");
            }
            if (pokemon.statuses.Contains("confusion") && Random.Range(0, 4) == 0)
            {
                pokemon.statuses.Remove("confusion");
            }
            if (pokemon.hasFainted)
            {
                pokemon.Faint();
            }
        }
    }
    public void OnPokeFaint(MonScript faintedPokemon)
    {
        Debug.Log(faintedPokemon.name + " has fainted");
        if (faintedPokemon.isPlayerPokemon)
        {
            Debug.Log(battlingPokemon[1].name + " gained " + (((faintedPokemon.level * faintedPokemon.baseStatTotal * ExpMultiplier / 5) * Mathf.Pow(((2 * faintedPokemon.level + 10) / (faintedPokemon.level + battlingPokemon[0].level + 10)), 2.5f) + 1) * 1.4f) + "Exp");
            battlingPokemon[1].GetExp(((faintedPokemon.level * faintedPokemon.baseStatTotal * ExpMultiplier / 5) * Mathf.Pow(((2 * faintedPokemon.level + 10) / (faintedPokemon.level + battlingPokemon[0].level + 10)), 2.5f) + 1)* 1.4f);
            battlingPokemon[0] = encounter.CreatePokemon(5, true);
        }
        else
        {
            Debug.Log(battlingPokemon[0].name + " gained " +(((faintedPokemon.level * faintedPokemon.baseStatTotal * ExpMultiplier / 5) * Mathf.Pow(((2 * faintedPokemon.level + 10) / (faintedPokemon.level + battlingPokemon[0].level + 10)), 2.5f) + 1) * 1.4f) + "Exp");
            battlingPokemon[0].GetExp(((faintedPokemon.level * faintedPokemon.baseStatTotal * ExpMultiplier / 5) * Mathf.Pow(((2 * faintedPokemon.level + 10) / (faintedPokemon.level + battlingPokemon[0].level + 10)), 2.5f) + 1) * 1.4f);
            battlingPokemon[1] = encounter.CreatePokemon(8, true);
        }
    }
    public void DevGiveExp()
    {
        battlingPokemon[0].GetExp(1000);
    }
}
