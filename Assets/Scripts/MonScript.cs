using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonScript : MonoBehaviour
{
    //public enum Stat {Hp, Attack, Defence, SpecialAttack, SpecialDefence, Speed, Evasion, Accuracy}
    float[] statMultiplier = { 0.25f, 0.286f, 0.333f, 0.4f, 0.5f, 0.667f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

    public bool isTemplate = true;
    public bool hasFainted;
    public bool isPlayerPokemon = false;

    public List<string> statuses;
    public int[] statChanges; //0 hp 1 attack 2 defence 3 sp. attack 4 sp. defence 5 speed 6 evasion 7 accuracy
    public List<string> immunities;

    public int pokedexID;

    float temp;   

    public List<AttackScript> moves = new List<AttackScript>();

    int numberOfMovesLearned = 0;
    

    JsonReader jsonReader;
    CombatManager combatManager;

    public int level;
    [SerializeField] float requiredExp;
    [SerializeField] float exp;

    int hpIV;
    int attackIV;
    int defenceIV;
    int specialAttackIV;
    int specialDefenceIV;
    int speedIV;

    public float attack;
    public float defence;
    public float specialAttack;
    public float specialDefence;
    public float speed;

    public float tempEvasion = 1;
    public float tempAccuracy = 1;

    int goodNature;
    int badNature;

    public float maxHp;
    public float currentHp;

    public List<CombatManager.Type> type;

    public int baseStatTotal;

    EncounterManager encounterManager;

    //ability

    // Start is called before the first frame update
    void Start()
    {
        encounterManager = FindObjectOfType<EncounterManager>();
        
        statChanges = new int[8];
        for (int i = 0; i < 8; i++)
        {
            statChanges[i] = 0;
        }

        jsonReader = FindObjectOfType<JsonReader>();
        combatManager = FindObjectOfType<CombatManager>();

        type.Add ((CombatManager.Type)System.Enum.Parse(typeof(CombatManager.Type), jsonReader.GetMon(pokedexID).type[0]));

        if (jsonReader.GetMon(pokedexID).type.Length > 1)
        {
            type.Add( (CombatManager.Type)System.Enum.Parse(typeof(CombatManager.Type), jsonReader.GetMon(pokedexID).type[1]));
        }
        else
        {
            type.Add(CombatManager.Type.None);
        }

        hpIV = Random.Range(0, 32);
        attackIV = Random.Range(0, 32);
        defenceIV = Random.Range(0, 32);
        specialAttackIV = Random.Range(0, 32);
        specialDefenceIV = Random.Range(0, 32);
        speedIV = Random.Range(0, 32);

        goodNature = Random.Range(1, 6);
        badNature = Random.Range(1, 6);
        while (badNature == goodNature)
        {
            badNature = Random.Range(1, 6);
        }

        SetStats(true);

        name = jsonReader.GetMon(pokedexID).name.english;
        isTemplate = false;

        baseStatTotal = jsonReader.GetMon(pokedexID).mon.HP + jsonReader.GetMon(pokedexID).mon.Attack + jsonReader.GetMon(pokedexID).mon.Defense + jsonReader.GetMon(pokedexID).mon.Sp_Attack + jsonReader.GetMon(pokedexID).mon.Sp_Defense + jsonReader.GetMon(pokedexID).mon.Speed;
        exp = Mathf.Pow(level, 3);
    }

    float NatureCheck(int stat)
    {
        if (goodNature == stat)
        {
            return 1.1f;
        }
        else if (badNature == stat)
        {
            return 0.9f;
        }
        else
        {
            return 1f;
        }
    }

    public void SetStats(bool fullHeal)
    {
        if(!fullHeal)
        {
            temp = currentHp / maxHp;
        }
        
        if (type.Contains(CombatManager.Type.Electric))
        {
            immunities.Add("paralysis");
        }

        maxHp = ((2 * jsonReader.GetMon(pokedexID).mon.HP + hpIV) * level) / 100 + level + 10;
        attack = (((2 * jsonReader.GetMon(pokedexID).mon.Attack + attackIV) * level) / 100 + 5) * NatureCheck(1);
        defence = (((2 * jsonReader.GetMon(pokedexID).mon.Defense + defenceIV) * level) / 100 + 5) * NatureCheck(2);
        specialAttack = (((2 * jsonReader.GetMon(pokedexID).mon.Sp_Attack + specialAttackIV) * level) / 100 + 5) * NatureCheck(3);
        specialDefence = (((2 * jsonReader.GetMon(pokedexID).mon.Sp_Defense + specialDefenceIV) * level) / 100 + 5) * NatureCheck(4);
        speed = (((2 * jsonReader.GetMon(pokedexID).mon.Speed + speedIV) * level) / 100 + 5) * NatureCheck(5);

        if (!fullHeal)
        {
            currentHp = maxHp * temp;
        }
        else
        {
            currentHp = maxHp;
        }

        Debug.Log(statChanges[1]);
        Debug.Log(statMultiplier[6]);

        maxHp *= statMultiplier[statChanges[0] + 6];
        attack *= statMultiplier[statChanges[1] + 6];
        defence *= statMultiplier[statChanges[2] + 6];
        specialAttack *= statMultiplier[statChanges[3] + 6];
        specialDefence *= statMultiplier[statChanges[4] + 6];
        speed *= statMultiplier[statChanges[5] + 6];

        
        if (statuses.Contains("paralysis"))
        {
            speed /= 2;
        }

        requiredExp = Mathf.Pow(level + 1, 3);
    }

    public float TakeDamage(float damage)
    {
        currentHp -= damage;
        if(currentHp <= 0)
        {
            hasFainted = true;
            //Destroy(gameObject);
        }
        return damage;
    }

    public void Faint()
    {
        
        //combatManager.OnPokeFaint(GetComponent<MonScript>());
        for (int i = 0; i < 4; i++)
        {
            moves[i].SelfDestruct();
        }
        //Destroy(gameObject);
    }

    public void removeFromBattle()
    {
        if (hasFainted)
        {
            combatManager = FindObjectOfType<CombatManager>();
            combatManager.replacePokemon(isPlayerPokemon);
            Destroy(gameObject);
        }

    }

    public void GetExp(float expGain)
    {
        exp += expGain;
        while (exp >= requiredExp)
        {
            Debug.Log(name + " leveled up!");
            level++;
            EncounterManager.BaseMons b = encounterManager.availableBasePokemon.Find(x => x.pokedexID == this.pokedexID);
            if (b.levelWherePokemonLearnsMove[numberOfMovesLearned] == level)
            {
                AttackScript newMove = encounterManager.CreateAttack(b.learnableMoves[numberOfMovesLearned], transform);
                moves[0].SelfDestruct();
                moves.Remove(moves[0]);
                moves.Add(newMove);
                Debug.Log(name + " learned " + newMove.name);
                numberOfMovesLearned++;
            }
            SetStats(false);
            
        }
    }

    public void SetMoves()
    {

    }

}
