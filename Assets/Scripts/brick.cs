using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brick : MonoBehaviour

{
    [Header("Datos del Brick")]
    [SerializeField] public int life;
    [SerializeField] private int idBrick;
    public int IdBrick => idBrick;

    //Acceso a cosas del juego
    private LevelController _levComp;

    //Animaciones
    private Animator _animComp;
    
    //pivots para particulas o efectos
    private List<Vector2> _fxSpawn;
    

    void Awake()
    {
        _levComp = FindObjectOfType<LevelController>();
    }

    
    /// <summary>
    /// Le quito vida (aka daño) al brick. Le paso cuanto daño hago
    /// </summary>
    /// <param name="dmg"></param>
    public void RestaVida(int dmg = 1)
    {
        if (life > 1)
        {
            life -= dmg;
        }
        else
        {
            //lo quito del Levelcontroller antes de destruirlo
            _levComp.RemoveBrick(this.transform);
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        //lanzo partículas o lo que sea
    }
    
}
