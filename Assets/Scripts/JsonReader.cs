using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JsonReader : MonoBehaviour
{
    public TextAsset monFile;
    public TextAsset moveFile;
    


    [System.Serializable]
    public struct Mons
    {
        public int HP;
        public int Attack;
        public int Defense;
        public int Sp_Attack;
        public int Sp_Defense;
        public int Speed;
        public int baseStatTotal;
        
    }

    [System.Serializable]
    public struct Name
    {
        public string english;
        public string japanese;
        public string chinese;
        public string french;
    }

    [System.Serializable]
    public class Move
    {
        public string ename;
        public float accuracy;
        public int id;
        public bool isAutoHit;
        public int pp;
        public float power;
        public string moveType;
        public string category;
        public Effect effect;
        public int numberOfAdditionalAttacks;
    }

    [System.Serializable]
    public struct Effect
    {
        public float chance;
        public string status;
        public string stat;
        public int stages;
        public string affectedPokemon;
    }

    [System.Serializable]
    public class Mon
    {
        public int id;
        public Name name;
        public string[] type;
        public Mons mon;

    }
    [System.Serializable]
    public class MonList
    {
        public Mon[] pokemon;
    }
    [System.Serializable]
    public class MoveList
    {
        public Move[] tempMoves;
    }
    public MonList monList = new MonList();
    public MoveList moveList = new MoveList();

    // Start is called before the first frame update
    void Start()
    {
        monList = JsonUtility.FromJson<MonList>(monFile.text);
        moveList = JsonUtility.FromJson<MoveList>(moveFile.text);
        /*for (int i = 0; i < monList.pokemon.Length; i++)
        {
            monList.pokemon[i].SetLearnableMoves(GetMove(Random.Range(0, 4)));
        }*/
    }


    public Mon GetMon (int pokeID)
    {
        return monList.pokemon[pokeID - 1];
    }
    public Move GetMove (int moveID)
    {
        return moveList.tempMoves[moveID];
    }
}
