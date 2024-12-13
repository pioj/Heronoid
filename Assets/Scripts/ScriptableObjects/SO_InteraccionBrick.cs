using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace mierdergames
{

    [Serializable]
    public enum TypeInteraction
    {
        None = 0,        //no daña, no rompe
        Normal = 1,      //daño por defecto (más los buffos, etc)
        Destroy = 2,     //destruye de un solo golpe (fuego vs fuego.., daño máximo de la vida del brick...)
        Nullify = 3,     //NO daña y vuelve a aplicar el SO estándar de la bola, stats y relación de bricks...
    }

    [Serializable]
    public struct InteractionBrick
    {
        [SerializeField] public int idBrick;
        [SerializeField] public TypeInteraction interaction;

        public InteractionBrick(int id, TypeInteraction tipo=TypeInteraction.Normal)
        {
            idBrick = id;
            interaction = tipo;
        }
    }

    //SO que personaliza cada interacción de la bola con ese ID de ladrillo, y qué efecto causa.
    //Cada skill/trozo de pala aplica un SO propio a los bricks.
    [Serializable]
    public class SO_InteraccionBrick : ScriptableObject
    {
        //eventos
        public delegate void NotifyApplyEffect(brick bk, TypeInteraction tipo);
        public event NotifyApplyEffect ApplyEffect;

        public List<InteractionBrick> interacciones;

        
        //investiga si un item de la lista existe, y devuelve el index del elemento
        private int GetIndexItem(int id)
        {
           for (int i=0; i < interacciones.Count; i++)
           {
               if (interacciones[i].idBrick.Equals(id)) return i;
           }
           return -1;
        }

        
        /// <summary>
        /// Notifica la interacción de la bola con el tipo de ladrillo , y qué resultado tendrá.
        /// </summary>
        /// <param name="bk"></param>
        public void DoInteraction(brick bk)
        {
            var inter = GetIndexItem(bk.IdBrick);
            if (inter == -1) return;    //lo omite, que es lo mismo que darle un 0, NONE de interaction...
            
            ApplyEffect?.Invoke(bk, interacciones[inter].interaction);
        }
        
    }
    
}
