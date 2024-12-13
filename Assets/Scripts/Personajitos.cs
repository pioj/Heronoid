using System;
using System.Collections;
using System.Collections.Generic;
using mierdergames;
using UnityEngine;


namespace mierdergames 
{
    //sera renombrado como HeroManager , o algo así...
    public class Personajitos : MonoBehaviour
    {
        //eventos
        public delegate void NotifyPersonajitosCargados();
        public delegate void NotifyPersonajitoDesbloqueado(SO_Personajito pj);
        public event NotifyPersonajitosCargados PersonajitosCargados;
        public event NotifyPersonajitoDesbloqueado PersonajitoDesbloqueado;
        
        public List<SO_Personajito> lista = new List<SO_Personajito>();
        public List<SO_Personajito> desbloqueados = new List<SO_Personajito>();
        [SerializeField] private List<SO_Personajito> bloqueados;

        void Start()
        {
            //WIP , de momento no leemos la sesion del player
            bloqueados = new List<SO_Personajito>(lista);
            PersonajitosCargados?.Invoke();
            print("Lista de Personajitos cargada.");
        }

        /// <summary>
        /// Desbloquea un Personajito de la lista disponible, ya tenemos otro más para jugar
        /// </summary>
        /// <param name="pj"></param>
        public void UnlockPersonajito(SO_Personajito pj)
        {
            if (pj == null) return;
            
            desbloqueados.Add(pj);
            bloqueados.Remove(pj);
            PersonajitoDesbloqueado?.Invoke(pj);
            print("Personajito " + pj.name + " desbloqueado.");
        }
        
    }
}
