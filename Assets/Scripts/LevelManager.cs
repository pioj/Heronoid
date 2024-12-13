using System.Collections.Generic;
using mierdergames;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    //eventos
    public delegate void NotifyLevelChanged(int idx);
    public delegate void NotifyAllLevelsCleared();
    public delegate void NotifyLevelManagerReady();

    public event NotifyAllLevelsCleared AllLevelsCleared;
    public event NotifyLevelChanged LevelChanged;
    public event NotifyLevelManagerReady LevelManagerReady;
    
    [SerializeField] public SO_LevelPack CurrentLevelPack;
    [SerializeField] private List<BaseLevel> _niveles;
    [SerializeField] public BaseLevel CurrentLevel;
    [SerializeField] private int levelIndex;
    
    private GameObject _currentLevelTheme;
    private SO_BossLevel _currentBossLevel;
    
    [SerializeField] private bool isLastLevel => CurrentLevel.Equals(_niveles[_niveles.Count-1]) && _niveles!= null && _niveles.Count > 0;


    private void SetCurrentLevel(int indx)
    {
        CurrentLevel = _niveles[indx];
        SetLevelTheme();    //vamos cambiando el theme de fondo
        
        LevelChanged?.Invoke(indx);
    }

    private void LevelPackCleared()
    {
        if (isLastLevel) AllLevelsCleared?.Invoke();
    }

    
    /// <summary>
    /// Pone todos los niveles del pack disponibles en la lista, le añade un BossLevel a final, y establece el 1er nivel. 
    /// </summary>
    public void Init()
    {
        if (CurrentLevelPack == null && CurrentLevelPack.niveles == null && CurrentLevelPack.niveles.Count < 1) return;
        
        _niveles = new List<BaseLevel>();
        foreach (var lev in CurrentLevelPack.niveles)
        {
            _niveles.Add(lev.level);
        }
            
        levelIndex = 0;
        SetCurrentLevel(levelIndex);    //1st level
        
        LevelManagerReady?.Invoke();
    }
    

    //Pone uno de los BossLevels disponibles como el nivel final en la lista jugable
    private void SetBossLevel()
    {
        if (CurrentLevelPack.RandomBossLevels == null && CurrentLevelPack.RandomBossLevels.Count < 1) return;

        var lb = Random.Range(0, CurrentLevelPack.RandomBossLevels.Count - 1);
        _currentBossLevel = CurrentLevelPack.RandomBossLevels[lb];
        
        _niveles.Add(_currentBossLevel.level);
    }

    //pone uno de los Themes disponibles como el actual, para el nivel siguiente
    private void SetLevelTheme()
    {
        if (CurrentLevelPack.themePrefabs == null && CurrentLevelPack.themePrefabs.Count < 1) return;

        var th = Random.Range(0, CurrentLevelPack.themePrefabs.Count - 1);
        _currentLevelTheme = CurrentLevelPack.themePrefabs[th];
    }

}
