using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//pequeño script que se añade al gameobject de las palas para identificarlo con su efecto deseado ...
[Serializable]
public class IdEfectoPala : MonoBehaviour
{
    //eventos
    public delegate void NotifySendEfectoPala(int id, bola comp);
    public event NotifySendEfectoPala SendEfectoPala;
    
    [SerializeField] public int idEfecto;
    
    /// <summary>
    /// Notifica al manager de efectos/skills que ese trozo de pala aplica ese id/efecto/skill a esa bola concreta.
    /// </summary>
    /// <param name="comp"></param>
    public void SetPalaEffect(bola comp)
    {
        SendEfectoPala?.Invoke(idEfecto, comp);
    }
    
}
