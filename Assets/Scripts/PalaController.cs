using System;
using UnityEngine;

namespace mierdergames
{
    [RequireComponent(typeof(Personajitos))]
    public class PalaController : MonoBehaviour
    {
        //eventos
        public delegate void NotifyPalaChanged();
        public event NotifyPalaChanged PalaChanged;
        
        public Transform pala;
        public float palaspeed;
        private Vector3 palapos;
        private float Ypos;
        [SerializeField] public SO_Personajito[] currentCombo;
        private bool canMove;
            
        private Personajitos _persoComp;

        #if UNITY_ANDROID && !UNITY_EDITOR
            private Camera _cam;
        #endif
       
        
        void Awake()
        {
            _persoComp = GetComponent<Personajitos>();
            Ypos = pala.position.y;
            
            #if UNITY_ANDROID && !UNITY_EDITOR
                _cam = Camera.main;
            #endif
        }
        
        void Start()
        {
            _persoComp.PersonajitosCargados += SetComboPala;
        }
        
        void Update()
        {
            if (!canMove) return;
            
            #if UNITY_EDITOR  
                pala.transform.Translate(Input.GetAxis("Horizontal") * palaspeed * Time.deltaTime, 0, 0);
                if (Input.GetKeyDown(KeyCode.Escape)) UnityEditor.EditorApplication.isPlaying = false;
            #endif
            
            #if UNITY_ANDROID && !UNITY_EDITOR
                if (Input.touchCount >= 1)
                {
                    var newpos = _cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    //palapos = new Vector3(newpos.x, palapos.y,0);
                    palapos = new Vector3(newpos.x,Ypos,0);

                    pala.transform.position = palapos;
                }
            #endif
        }
        
        
        //ELEGIMOS UNOS PERSONAJITOS PARA LOS TROZO DE PALA. 
        private void SetComboPala()
        {
            //de momento, sólo desbloqueo el soldado y el magoFuego
            _persoComp.UnlockPersonajito(_persoComp.lista[0]);
            _persoComp.UnlockPersonajito(_persoComp.lista[2]);
        
            //vamos a forzarlo a los "mago|soldado|soldado".
            currentCombo = new SO_Personajito[3];
            currentCombo[0] = _persoComp.desbloqueados[1];    //el segundo de la lista, el magoFuego
            currentCombo[1] = _persoComp.desbloqueados[0];    //soldado
            currentCombo[2] = _persoComp.desbloqueados[0];    //soldado
            
            //aqui viene lo de instanciar el gameobject del personajito como hijo del transform de la Pala.
            //...
            //añado el miniscript de idEfecto a cada trozo de pala, con su correspondiente valor
            pala.GetChild(0).gameObject.AddComponent<IdEfectoPala>();    //izq
            pala.GetChild(1).gameObject.AddComponent<IdEfectoPala>();    //cent
            pala.GetChild(2).gameObject.AddComponent<IdEfectoPala>();    //der
            pala.GetChild(0).GetComponent<IdEfectoPala>().idEfecto = currentCombo[0].idInteraccion;
            pala.GetChild(1).GetComponent<IdEfectoPala>().idEfecto = currentCombo[1].idInteraccion;
            pala.GetChild(2).GetComponent<IdEfectoPala>().idEfecto = currentCombo[2].idInteraccion;
            
            PalaChanged?.Invoke();
            print("Combo de pala cambiado.");
        }

        
        
        /// <summary>
        /// habilita el control de la pala, para el ingame de la partida
        /// </summary>
        public void EnableControl()
        {
            canMove = true;
        }

        /// <summary>
        /// desactiva el control de la pala, ideal para cutscenes o menus, etc...
        /// </summary>
        public void DisableControl()
        {
            canMove = false;
        }

        /// <summary>
        /// reposiciona la pala a su lugar correcto, al inicio del nivel.
        /// </summary>
        public void InitPala()
        {
            palapos = pala.transform.position;  
        }

        void OnDestroy()
        {
            _persoComp.PersonajitosCargados -= SetComboPala;
        }

        
    }
}