using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    #region Vari�veis Globais
    // Inspector:
    [Header("Configura��es:")]

    [Header("Chave:")]
    [SerializeField] private DoorKeys.Key targetKey;

    [Header("Refer�ncias:")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Outline outlineEffect;
    [SerializeField] private Transform gfxTransform;

    [HideInInspector] public bool CanInteract = false;

    // Refer�ncias:
    private static Transform _playerTransform;

    private static bool[] _doorsWithKeyOpeneds = new bool[50];
    private static List<int> _doorsWithoutKeyOpeneds = new List<int>();
    #endregion

    #region Fun��es Unity
    private void Awake() => _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    private void Start() => VerifyAlreadyOpened();

    private void Update()
    {
        if (CanInteract)
        {
            //if (PlayerIsFacing())
            //{
                outlineEffect.eraseRenderer = false;
                if (targetKey != DoorKeys.Key.None)
                {
                    if (Input.GetKeyDown(KeyCode.E) && HasKey())
                    {
                        Open();
                        _doorsWithKeyOpeneds[(int)targetKey] = true;
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Open();
                        _doorsWithoutKeyOpeneds.Add(gameObject.GetInstanceID());
                    }
                }
            //}
        }
        else
        {
            outlineEffect.eraseRenderer = true;
        }
    }
    #endregion

    #region Fun��es Pr�prias
    private void Open()
    {
        outlineEffect.eraseRenderer = true;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("door open");
        doorAnimator.enabled = true;
        Destroy(gameObject);
    }

    private void VerifyAlreadyOpened()
    {
        if (targetKey != DoorKeys.Key.None) // Porta com Chave
        {
            for (int i = 0; i < _doorsWithKeyOpeneds.Length; i++)
            {
                if (i == (int)targetKey)
                {
                    if (_doorsWithKeyOpeneds[i] == true)
                    {
                        Open();
                        break;
                    }
                }
            }
        }
        else // Porta sem Chave
        {
            //testei com o debug.log, a linha debaixo nn � chamada
            for (int i = 0; i < _doorsWithoutKeyOpeneds.Count; i++)
            {
                if (_doorsWithoutKeyOpeneds[i] == gameObject.GetInstanceID())
                {
                    Open();
                    break;
                }
            }
        }
    }

    private bool HasKey()
    {
        for (int i = 0; i < PlayerProgress.KeysCollected.Count; i++)
        {
            if (PlayerProgress.KeysCollected[i] == targetKey)
                return true;
        }

        return false;
    }

    private bool PlayerIsFacing()
    {
        var triggerDirection = (gfxTransform.localPosition - _playerTransform.position).normalized;
        var playerDirection = _playerTransform.forward;

        float scalar = Vector3.Dot(triggerDirection, playerDirection);

        float angulo = Mathf.Acos(scalar) * Mathf.Rad2Deg;

        if (angulo <= 45f)
            return true;

        return false;
    }
    #endregion
}
