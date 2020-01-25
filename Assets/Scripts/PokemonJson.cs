using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PokemonJson
{
    public string name;
    public int id;
    public List<TypeSlot> types;
    public List<StatValue> stats;
    public List<MoveDescription> moves;
}

[Serializable]
public class TypeSlot
{
    public int slot;
    public Type type;
}

[Serializable]
public class Type
{
    public string name;
}

[Serializable]
public class StatValue
{
    public int base_stat;
    public Stat stat;
}

[Serializable]
public class Stat
{
    public string name;
}

[Serializable]
public class MoveDescription
{
    public Move move;
    public List<VersionGroupDetail> version_group_details;
}


[Serializable]
public class Move
{
    public string name;
    public string url;
}

[Serializable]
public class VersionGroupDetail
{
    public int level_learned_at;
    public VersionGroup version_group;
}

[Serializable]
public class VersionGroup
{
    public string name;
}

[Serializable]
public class MoveJson
{
    public int power;
    public Type type;
    public DamageClass damage_class;
    public string name;
}

[Serializable]
public class DamageClass
{
    public string name;
}
