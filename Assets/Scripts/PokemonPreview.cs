#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonPreview : MonoBehaviour
{
    public void BtOk()
    {
        var pokemonObj = transform.Find("Pokemon").GetChild(0);
        pokemonObj.SetParent(GameObject.Find("OwnPkmn").transform);
        pokemonObj.localPosition = Vector3.zero;
        pokemonObj.localRotation = Quaternion.identity;
        GameObject.FindObjectOfType<GameManager>().PkmonChoosed(int.Parse(pokemonObj.name.Substring(0,3)));
    }
}
#endif
