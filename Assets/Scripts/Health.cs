using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public const int m_maxHealth = 100;
    [SyncVar (hook = "OnChangeHealth")]public int m_currentHealth = m_maxHealth;
    public RectTransform m_healthBar;
    public bool DestroyOnDeath;
    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }

    public void TakeDamage(int Amount)
    {
        if (!isServer)
        {
            return;
        }
        m_currentHealth -= Amount;
        if (m_currentHealth <= 0)
        {
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                m_currentHealth = m_maxHealth;
                RpcRespawn();
            }
        }
        
    }
    void OnChangeHealth(int m_Health)
    {
        m_healthBar.sizeDelta = new Vector2(m_Health * 2, m_healthBar.sizeDelta.y);
    }
    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 m_spawnPoint = Vector3.zero;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                m_spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }
            transform.position = m_spawnPoint;
        }
    }
}
