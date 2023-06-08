using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelmetController : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField] private List<HelmetComponent> Helmets = new List<HelmetComponent>();

    #endregion

    #region Privatee Fields

    #endregion


    #region Setters

    #endregion

    public void EnableTargetMeshes(int TargetLevel)
    {
        HelmetComponent helmet = GetHelmets().FirstOrDefault(x => x.GetLevel() == TargetLevel);
        helmet.gameObject.SetActive(true);
    }

    public void DisableMeshes()
    {
        foreach (var Helmet in Helmets)
        {
            Helmet.gameObject.SetActive(false);
        }
    }

    #region Getters

    public List<HelmetComponent> GetHelmets()
    {
        return Helmets;
    }

    #endregion
}