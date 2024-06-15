using System.Collections;
using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    #region Vari�veis Globais
    // Componentes:
    private PlayerStats _playerStats;
    #endregion

    #region Fun��es Unity
    private void Awake() => _playerStats = GetComponent<PlayerStats>();

    private void OnTriggerEnter(Collider col)
    {
        //GameObject colObjeto = col.gameObject;
        //Debug.Log(colObjeto.name);
        if (col.gameObject.layer == CollisionLayersManager.Instance.EnemyAttack.Index)
        {
            Debug.Log("Bateu");
            _playerStats.ChangeHealthPoints(-col.gameObject.GetComponent<EnemyStats>().Damage);
            
        }
        else if (col.gameObject.layer == CollisionLayersManager.Instance.HealthPack.Index)
        {
            _playerStats.ChangeHealthPoints(col.gameObject.GetComponent<HealthPack>().Points);
            Destroy(col.gameObject);
        }
    }
    #endregion
}
