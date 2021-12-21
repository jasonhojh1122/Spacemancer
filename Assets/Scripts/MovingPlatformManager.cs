using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MovingPlatformManager : MonoBehaviour
{
    ///<summary>
    /// <c>List</c> of <c>MovingPlatforms' direction(MoveToEnd or not)</c>
    ///</summary>
    private List<bool> direction = new List<bool>();
    ///<summary>
    /// A tracking <c>List</c> of <c>MovingPlatform</c>
    ///</summary>
    [SerializeField] List<MovingPlatform> movingPlatforms;//Keep track of list of platforms

    ///<summary>
    /// Set MovingPlatform Direction of Specific ID
    ///</summary>
    /// <param name="id"> The MovingPlatform ID </param>
    /// <param name="val"> MoveToEnd  </param>
    public void SetDirection(int id,bool val){
        direction[id] = val;
    }
    ///<summary>
    /// Get MovingPlatform Direction of Specific ID
    ///</summary>
    /// <param name="id"> The MovingPlatform ID </param>
    ///<returns> Direction Value</returns>
    public bool getDirection(int id){
        return direction[id];
    }
    ///<summary>
    /// Register MovingPlatform to MovingPlatform Manager
    ///</summary>
    /// <param name="m"> MovingPlatform to track </param>
    /// <param name="id"> The MovingPlatform ID </param>
    public void Register(MovingPlatform m,int ID){
        movingPlatforms.Add(m);
        m.ID = movingPlatforms.Count - 1;
        direction.Add(m.moveToEnd);
        m.isInit = true;
    }
    
}
