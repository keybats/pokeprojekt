using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [System.Serializable]
    public struct BaseMons 
    {
        public List<int> learnableMoves;
        public List<int> levelWherePokemonLearnsMove;
        public List<int> startingMoves;
        public int pokedexID;
    }


    [SerializeField] int pokePoolSize;
    public GameObject pokemonTemplate;
    public GameObject moveTemplate;
    [SerializeField] GameObject baseMonTemplate;
    public List<MonScript> playerPokemon;
    public List<BaseMons> availableBasePokemon;
    BaseMons baseMons;
    JsonReader jsonReader;
    int tempInt;
    

    void Start()
    {
        
        
        jsonReader = FindObjectOfType<JsonReader>();


        for (int i = 0; i < pokePoolSize; i++)
        {
            baseMons = new BaseMons();
            baseMons.pokedexID = Random.Range(1, 810);
            baseMons.learnableMoves = new List<int>();
            for (int k = 0; k < Random.Range(11, 32); k++)
            {
                
                baseMons.learnableMoves.Add(Random.Range(0, 24));
            }
            baseMons.levelWherePokemonLearnsMove = new List<int>();
            for (int k = 0; k < baseMons.learnableMoves.Count; k++)
            {
                tempInt = Random.Range(6, 101);
                if (!baseMons.levelWherePokemonLearnsMove.Contains(tempInt))
                {
                    baseMons.levelWherePokemonLearnsMove.Add(tempInt);
                }
                else
                {
                    k--;
                }
            }
            baseMons.levelWherePokemonLearnsMove.Sort();
            baseMons.startingMoves = new List<int>();
            for (int k = 0; k < 4; k++)
            {
                baseMons.startingMoves.Add(Random.Range(0, 24));
            }

            availableBasePokemon.Add(baseMons);
            
            Debug.Log(jsonReader.GetMon(availableBasePokemon[i].pokedexID).name.english + " " + i);

        }
        playerPokemon.Add(CreatePokemon(5, true));
    }

    public MonScript CreatePokemon(int level, bool isPlayerPokemon)
    {
        GameObject p = Instantiate(pokemonTemplate, new Vector3(100f, 100f, 100f), transform.rotation, gameObject.transform);
        
        MonScript instantiatedMon = p.GetComponent<MonScript>();
        BaseMons instantiatedBaseMon = availableBasePokemon[Random.Range(0, availableBasePokemon.Count)];
        instantiatedMon.pokedexID = instantiatedBaseMon.pokedexID;
        instantiatedMon.level = level;
        instantiatedMon.isPlayerPokemon = isPlayerPokemon;

        for (int i = 0; i < 4; i++)
        {                   
            instantiatedMon.moves.Add(CreateAttack(instantiatedBaseMon.learnableMoves[i], instantiatedMon.transform));
        }

        instantiatedMon.SetMoveNames();
        return p.GetComponent<MonScript>();
    }
    public AttackScript CreateAttack(int moveId, Transform parent) 
    {
        
        GameObject templateMove = Instantiate(moveTemplate, parent);
        AttackScript a = templateMove.GetComponent<AttackScript>();
        a.id = moveId;
        a.SetStats();
        return a;
    }
}
