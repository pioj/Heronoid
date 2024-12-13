using System;
using UnityEngine;


namespace mierdergames
{
    #region BaseLevel
    
    [Serializable]
    public class BaseLevel
    {
       [SerializeField] public string levelname;
       [SerializeField] public int[] _leveldata;  //order is left->right,bottom->up
       
       public BaseLevel(int[] data)
       {
           levelname = "NewLevel_" + System.DateTime.Today.ToString();
           _leveldata = data;
       }

       public BaseLevel()
       {
           levelname = "NewLevel_" + System.DateTime.Today.ToString(); 
       }

       /// <summary>
       /// Sets the brick data array for this level. Remember the order: Left->Right,Bottom->Up
       /// </summary>
       /// <param name="data"></param>
       public void SetLeveldata(int[] data)
       {
           _leveldata = data;
       }

    }
    #endregion

    
    #region UserLevel

    [Serializable]
    public class UserLevel : BaseLevel
    {
        [SerializeField] public string author;

        public UserLevel(string name, string auth, int[] data)
        {
            levelname = name;
            author = auth;
            _leveldata = data;
        }

        public UserLevel(string auth, int[] data)
        {
            levelname = "NewLevel_" + System.DateTime.Today.ToString();
            _leveldata = data;
            author = auth;
        }

        public UserLevel(string auth)
        {
            levelname = "NewLevel_" + System.DateTime.Today.ToString();
            author = auth;
        }
    }
    #endregion

    #region BossLevel 
    
    [Serializable]
    public class BossLevel : BaseLevel
    {
        //eventos
        public delegate void NotifyBossLevelUnlocked(BossLevel bl);
        public event NotifyBossLevelUnlocked BossLevelUnlocked;
        
        [SerializeField] public bool isLocked;
        [SerializeField] public int keysNeeded;
        //[SerializeField] public SO_Boss Boss;
        
        public BossLevel(string name, int[] data, bool locked = true, int keys = 1)
        {
            levelname = name;
            _leveldata = data;
            isLocked = locked;
            keysNeeded = keys;
        }

        /// <summary>
        /// WIP Desbloquea el nivel del Boss, si se ha gastado las llaves, y notifica el desbloqueo
        /// </summary>
        public void UnlockBossLevel()
        {
            isLocked = false;
            BossLevelUnlocked?.Invoke(this);
        }
        
    }
    #endregion
}