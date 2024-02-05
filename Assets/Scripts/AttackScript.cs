using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public int id;
    public float accuracy;
    public float power;
    public int maxUses;
    public bool isPhysical;
    public bool isDamaging;
    public bool doesMakeContact;
    public bool isAutoHit;
    public float effectChance;
    public int stages;
    public string status;
    public string stat;
    public string affectedPokemon;
    public int numberOfAdditionalAttacks;
    public CombatManager.Type moveType;
    JsonReader jsonReader;
    


    public int uses;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetStats()
    {

        jsonReader = FindObjectOfType<JsonReader>();
        this.accuracy = jsonReader.GetMove(id).accuracy;
        this.power = jsonReader.GetMove(id).power;
        this.maxUses = jsonReader.GetMove(id).pp;
        this.isAutoHit = jsonReader.GetMove(id).isAutoHit;
        this.moveType = (CombatManager.Type)System.Enum.Parse(typeof(CombatManager.Type), jsonReader.GetMove(id).moveType);
        this.numberOfAdditionalAttacks = jsonReader.GetMove(id).numberOfAdditionalAttacks;
        if(jsonReader.GetMove(id).effect.status != null)
        {
            this.status = jsonReader.GetMove(id).effect.status;
            this.affectedPokemon = jsonReader.GetMove(id).effect.affectedPokemon;
        }
        if (jsonReader.GetMove(id).effect.stat != null)
        {
            this.stat = jsonReader.GetMove(id).effect.stat;
            this.stages = jsonReader.GetMove(id).effect.stages;
            this.affectedPokemon = jsonReader.GetMove(id).effect.affectedPokemon;
        }

        this.effectChance = jsonReader.GetMove(id).effect.chance;
        
        if (jsonReader.GetMove(id).category == "physical")
        {
            isPhysical = true;
        }
        else
        {
            isPhysical = false;
        }
        uses = maxUses;
        name = jsonReader.GetMove(id).ename;
    }
    public void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
