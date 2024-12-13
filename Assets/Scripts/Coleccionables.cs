using System;
using System.Collections;
using System.Collections.Generic;
using mierdergames;
using UnityEngine;

[Serializable]
public class Coleccionables : MonoBehaviour
{
    [SerializeField] public List<SO_InteraccionBrick> efectosPala = new List<SO_InteraccionBrick>();
    private IdEfectoPala[] _miniPalaComps;
    private PalaController _palaComp;

    //OJO sustituir por "bola.cs" cuando lo hayamos integrado bien...
    private bola _bolaComp;

    private LevelController _levelComp;

    private void Awake()
    {
        _palaComp = FindObjectOfType<PalaController>();
        _levelComp = FindObjectOfType<LevelController>();
    }

    void Start()
    {
        _levelComp.LevelStarted += SetDefaultEfectoPala;
        _levelComp.LevelStarted += RefreshEfectosPala;    //IMPORTANTE, esto permite cargarlos la 1a vez...
        
        _palaComp.PalaChanged += RefreshEfectosPala;
    }


    //Re-suscribe a los eventos de cada trozo de pala
    private void RefreshEfectosPala()
    {
        //limpio antes de nada...
        if (_miniPalaComps != null && _miniPalaComps.Length > 0)
        {
            foreach (var compi in _miniPalaComps)
            {
                compi.SendEfectoPala -= SetCurrentEfectoPala;
            }
        }

        _miniPalaComps = FindObjectsOfType<IdEfectoPala>();
        foreach (var compo in _miniPalaComps)
        {
            compo.SendEfectoPala += SetCurrentEfectoPala;
        }
    }

    
    private void OnDestroy()
    {
        _palaComp.PalaChanged -= RefreshEfectosPala;
        _levelComp.LevelStarted -= SetDefaultEfectoPala;
        _levelComp.LevelStarted -= RefreshEfectosPala;
    }
    
    //Cambia por el efecto de la pala/personajito que toque, se lo aplica a la bola...
    private void SetCurrentEfectoPala(int idxListaEfectosPala, bola bComp)
    {
        //esa bola en concreto, le asignamos el efecto de la lista que toque...
        bComp.currentEffect = efectosPala[idxListaEfectosPala];
        
        print("Efecto Aplicado: " + efectosPala[idxListaEfectosPala].name);
    }

    
    //asigna un efecto de la lista a la bola en cuanto empieza la partida, by default.
    private void SetDefaultEfectoPala()
    {
        _bolaComp = FindObjectOfType<bola>();
        _bolaComp.Post_Start(efectosPala[0]);    //Le paso el soldado.
    }

}
