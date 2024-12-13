using System;
using System.Collections;
using System.Collections.Generic;
using mierdergames;
using UnityEngine;
using mierdergames.FeedbackEffects;

public class bola : MonoBehaviour
{
    //eventos de la bola
    public delegate void NotifyBolaCreated();  //la bola acaba de ser creada y ya está en el nivel.
    public delegate void NotifyBolaPerdida();  //lleva mucho tiempo sin rebotar contra la pala o un brick...
    public delegate void NotifyBolaMuere();    //cae por abajo...
    public delegate void NotifyBolaEfecto(int efecto);    //la bola cambia al efecto X...
    public delegate void NotifyBolaAburrida(); //lleva tiempo sin tocar un ladrillo...
    //
    public event NotifyBolaCreated BolaCreated;
    public event NotifyBolaPerdida BolaPerdida;
    public event NotifyBolaMuere BolaMuere;
    public event NotifyBolaEfecto BolaEfecto;
    public event NotifyBolaAburrida BolaAburrida;
    
        
    private Transform _camtransform;
    private FeedbackEffects _shakeComp;
    
    //data de la bola
    [Header("Bola Data")]
    [SerializeField] private Vector2 _balldir;
    [SerializeField] public float ballSpeed;
    [SerializeField] private float _defaultSpeed = 2f;
    [SerializeField] private float _defaultIncrementSpeed = 0.1f;
    
    //efecto de la bola con los bricks, el "preset" de daños...
    [Header("Preset de Daños")] [SerializeField]
    public SO_InteraccionBrick currentEffect;
    private SO_InteraccionBrick _defaultEffect;
    
    //metricas
    [Space]
    [SerializeField] private float _timerball; //tiempo que lleva la bola moviendose...
    [SerializeField] private float _limitperdida = 8f; //a partir de 8seg, la considero perdida
    [SerializeField] private bool _isPerdida = false;
    [SerializeField] private float _timerwithoutbreaking; //tiempo que lleva sin romper nada..
    [SerializeField] private float _limitaburrida = 15f; //a partir de 15seg, la considero aburrida
    
    void Awake()
    {
        if (!_camtransform) _camtransform = Camera.main.transform;
        _shakeComp = FindObjectOfType<FeedbackEffects>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _balldir = Vector2.one;
        ballSpeed = _defaultSpeed;
        
        //notifico que ya ha sido creada
        BolaCreated?.Invoke();
        print("Bola creada.");
    }
    
    /// <summary>
    /// Post-Setup inicial, le asigno un skill/efecto de pala by default y lo suscribo al evento del trocito de pala.
    /// </summary>
    /// <param name="effect"></param>
    public void Post_Start(SO_InteraccionBrick effect)
    {
        if (!currentEffect) currentEffect = effect;        //Recibo el efecto desde Coleccionables.    
                                                  
        _defaultEffect = currentEffect;                    //Interaccion del Soldado by default.
        currentEffect.ApplyEffect += ApplyInteraction;     //Evento de Interacciones
    }

    // Update is called once per frame
    void Update()
    {
        //movimiento
        transform.Translate(_balldir.x * ballSpeed * Time.deltaTime, _balldir.y * ballSpeed * Time.deltaTime, 0);
        
        //metricas de la bola
        if (_isPerdida) return; //y me ahorro todo lo siguiente...

        _timerball += Time.deltaTime;
        _timerwithoutbreaking += Time.deltaTime;
        
        //si pasa tiempo rebotando "raro" es que está perdida...
        if (_timerball > _limitperdida)
        {
            _isPerdida = true;
            BolaPerdida?.Invoke();
        }
        
        //si pasa tiempo sin romper ladrillos, es que está aburrida...
        if (_timerwithoutbreaking > _limitaburrida) BolaAburrida?.Invoke();
    }


    private void OnCollisionEnter2D(Collision2D paco)
    {
        if (paco.transform.CompareTag("pala")) //LA PALA
        {
            ResetTimerBall(); //WIP , habrá que tener en cuenta la bola perdida vertical...
            
            var choque = paco.contacts[0].normal;
            
            //TROZO DE APLICAR EL EFECTO A LA BOLA
            var palaComp = paco.transform.GetComponent<IdEfectoPala>();
            palaComp.SetPalaEffect(this);
            
            BolaEfecto?.Invoke(palaComp.idEfecto);
            
            ClampBallDir();

            //rebota o refleja la dirección en la normal opuesta a la dada
            _balldir = Vector2.Reflect(_balldir, choque);
        }
        
        if (paco.transform.CompareTag("muros")) //MUROS EXTERIORES
        {
            var choque = paco.contacts[0].normal;

            //si la bola está perdida, le damos algo de chaos a la direccion...
            if (_isPerdida)
            {
                //determino el eje con menor valor y le doy chaos, incremento 0.3f la direccion más leve...
                var eje = Mathf.Min(Mathf.Abs(_balldir.x), Mathf.Abs(_balldir.y));
                _balldir = (eje.Equals(Mathf.Abs(_balldir.x))) //es el eje x?
                    ? new Vector2(_balldir.x + (0.3f * Mathf.Sign(_balldir.x)), _balldir.y)
                    : new Vector2(_balldir.x, _balldir.y + (0.3f * Mathf.Sign(_balldir.y)) ); 
                
                ClampBallDir();
                _balldir = Vector2.Reflect(_balldir, choque);
                _isPerdida = false;
                ResetTimerBall();
            }
            else
            {
                ClampBallDir();
                
                //rebota o refleja la dirección en la normal opuesta a la dada
                _balldir = Vector2.Reflect(_balldir, choque);
            }
        }

        else if (paco.transform.CompareTag("bricks")) //LOS BRICKS
        {
            ResetTimerBall();
            ResetTimerAburrida();
            
            //WIP Usamos el nuevo ScriptableObject para asignar a un evento común...
            var brickComp = paco.transform.GetComponent<brick>();
            currentEffect.DoInteraction(brickComp);
            

            //rebota o refleja la dirección en la normal opuesta a la dada
            var choque = paco.contacts[0].normal;
            _balldir = Vector2.Reflect(_balldir, choque);

            FastBall();
            ClampBallDir();
            
            //prueba de shaking
            StartCoroutine(_shakeComp.Shake3(_camtransform,  new Vector2(0.1f,0.075f) ) );
        }
    }

    private void OnTriggerEnter2D(Collider2D paco)
    {
        if (paco.transform.CompareTag("death"))
        {
            BolaMuere?.Invoke();
            ResetTimerBall();
            ResetTimerAburrida();

            //reinicio el efecto normal de la bola (soldado por defecto).
            currentEffect = _defaultEffect;
            BolaEfecto?.Invoke(0); //creo que el 0 es el efecto normal...
            
            ResetBall();
        }
    }


    /// <summary>
    /// vuelve a poner los valores por defecto a la bola, la recoloca, etc.
    /// </summary>
    public void ResetBall()
    {
        transform.position = Vector3.zero;
        ballSpeed = _defaultSpeed;
        _balldir = Vector2.one;

        _isPerdida = false;
    }


    /// <summary>
    /// aumentamos la velocidad de la bola (gameplay por defecto al romper un brick)
    /// </summary>
    public void FastBall()
    {
        ballSpeed += _defaultIncrementSpeed;
    }

    
    //reseteo el contador de pelota perdida
    private void ResetTimerBall()
    {
        _timerball = 0f;
    }

    //reseteo el contador de pelota aburrida
    private void ResetTimerAburrida()
    {
        _timerwithoutbreaking = 0f;
    }

    
    //se asegura de que _balldir siempre sea vector de dirección, no velocidad.
    private void ClampBallDir()
    {
        _balldir.x = Mathf.Clamp(_balldir.x, -1f, 1f);
        _balldir.y = Mathf.Clamp(_balldir.y, -1f, 1f);
    }


    //evaluamos y cambiamos la interaccion de la bola con los bricks en funcion del skill/efecto aplicado
    private void ApplyInteraction(brick bk, TypeInteraction tipo)
    {
        if (tipo.Equals(TypeInteraction.Normal)) bk.RestaVida();
        else if (tipo.Equals(TypeInteraction.Destroy)) bk.RestaVida(bk.life);
        else if (tipo.Equals(TypeInteraction.None)) bk.RestaVida(0);
        //si es de tipo TypeInteraction.Nullify cambiará su currentEffect y su SO_data...
        else if (tipo.Equals(TypeInteraction.Nullify))
        {
            currentEffect = _defaultEffect;
            BolaEfecto?.Invoke(0); //creo que el 0 es el efecto normal...
        }
        //
    }

    private void OnDestroy()
    {
        currentEffect.ApplyEffect -= ApplyInteraction;
    }
}
