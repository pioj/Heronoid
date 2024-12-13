using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    
    //eventos
    public delegate void NotifyInitLevel();
    public delegate void NotifyLevelStarted();
    public delegate void NotifyLevelCleared();

    public event NotifyInitLevel InitLevel;
    public event NotifyLevelStarted LevelStarted;
    public event NotifyLevelCleared LevelCleared;
    
    public Transform rootLevel;
    public List<Transform> bricks;

    [Header("la Bola")] public GameObject bolaPrefab;


    void Start()
    {
        //WIP, más adelante todo esto lo gestiona el LevelManager o el GameManager...
        
        Init();
    }

    //Prepara los bricks del Level actual...
    void Init()
    {
        if (rootLevel == null) return;
        var amount = rootLevel.childCount;

        if (amount < 1) return;
        for (int i = 0; i < amount; i++)
        {
            bricks.Add(rootLevel.GetChild(i));
        }
        InitLevel?.Invoke();
        
        //por ejemplo, que empiece la partida despues de 2 secs, una vez se ha mostrado el cutscene, etc...
        StartCoroutine(StartCurrentLevelAfterSecs(2));
    }
    

    //puede ser llamado desde el script del brick, el LevelManager, o el GameManager...
    //NOTA: CUANDO SOPORTEMOS BOSSES, HAY QUE MODIFICAR ESTA FUNCION
    public void RemoveBrick(Transform briki)
    {
        if (briki == null) return;
        bricks.RemoveAt(bricks.IndexOf(briki));

        if (bricks.Count == 0)
        {
            LevelCleared?.Invoke();
            //aqui falta recoger todos los puntos y stats del nivel conseguidos, acumula al Player.
        }
    }

    //coRutina para preparar y comenzar un nivel
    private IEnumerator StartCurrentLevelAfterSecs(int secs)
    {
        yield return new WaitForSeconds(secs);
        
        //instancio la bola en el nivel y suscribo al evento de la Bola para esperar a que esté en el nivel
        var mainBola = Instantiate(bolaPrefab);
        var bolaComp = mainBola.GetComponent<bola>();
        
        //en cuanto exista el gameobject de la bola en el nivel, la partida puede comenzar.
        bolaComp.BolaCreated += AlertLevelStarted;
    }

    
    //Notifica que el nivel ha empezado
    private void AlertLevelStarted()
    {
        LevelStarted?.Invoke();
    }
}
