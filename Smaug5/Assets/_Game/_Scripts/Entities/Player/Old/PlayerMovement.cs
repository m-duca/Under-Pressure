using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Vari�veis Globais
    [Header("Modelo:")] 
    public GameObject playerBody1;
    public GameObject playerBody2;

    [Header("Movimenta��o")]
    public bool isSprinting = false;
    public float sprintSpeed = 30f;
    public float normalSpeed = 15f;
    public float moveSpeed;
    public float gravity = -9.81f;
    public CharacterController characterController;

    [Header("Pulo")]
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float crouchGroundCheckY = -0.32f;
    private float defaultGroundCheckY;
    private bool isGrounded;

    private Vector3 velocity;

    [Header("Agachar")]
    public bool isCrouching = false;
    public float crouchSpeed = 7.5f;
    float playerColliderHeight = 3.66f;
    float crouchColliderHeight = 1.83f;

    [Header("Escalar")]
    public float climbingDistance = 0.4f;
    public Transform stairsCheck;
    public LayerMask stairsMask;
    public bool isClimbing = false;

    [Header("Anima��o")]
    public RuntimeAnimatorController defaultController;
    public RuntimeAnimatorController withGunController;
    public RuntimeAnimatorController crouchController;
    public RuntimeAnimatorController climbController;

    [Header("Refer�ncias:")] 
    public CameraHeadBob cameraHeadBobScript;

    private Animator playerAnimator;
    private Rigidbody rb;

    private enum PlayerModel
    {
        DEFAULT,
        WITH_GUN
    }
    #endregion

    #region Fun��es Unity
    private void Awake() => ChangeModel(PlayerModel.DEFAULT);

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultGroundCheckY = groundCheck.transform.localPosition.y;
    }

    void DesativarFilhos(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(false);
            DesativarFilhos(child.gameObject);
        }
    }

    void AtivarFilhos(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(true);
            AtivarFilhos(child.gameObject);
        }
    }


    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //Checa se o jogador est� no ch�o
        isClimbing = Physics.CheckSphere(stairsCheck.position, climbingDistance, stairsMask); //Checa se o jogador est� escalando


        if (!isGrounded)
        {
            if (!isClimbing)
            {
                cameraHeadBobScript.Stop();
            }

            isCrouching = false;
        }
        else
        {
            if (!cameraHeadBobScript.IsActive())
            {
                cameraHeadBobScript.Enable();
            }
        }

        if (isGrounded && velocity.y < -20)
        {
            velocity.y = -2f; //Podia ser 0, mas o checksphere ativa antes, ent � mais seguro deixar menor
        }

        #region Andar
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        #endregion

        #region Animando
        if (isCrouching) // Agachado
        {
            if (playerAnimator.runtimeAnimatorController != crouchController)
            {
                ChangeModel(PlayerModel.DEFAULT);

                playerAnimator.runtimeAnimatorController = crouchController;
                playerAnimator.speed = 1f;
            }

            // Checa Anima��o de Andar Agachado
            if (move != Vector3.zero && !isSprinting)
            {
                playerAnimator.SetBool("isWalking", true);
            }
            else
            {
                playerAnimator.SetBool("isWalking", false);
            }
        }
        else if (isClimbing) // Escalando
        {
            if (playerAnimator.runtimeAnimatorController != climbController)
            {
                ChangeModel(PlayerModel.DEFAULT);

                playerAnimator.runtimeAnimatorController = climbController;
            }
        }
        else // Default & Com Arma
        {
            if (PlayerStats.HasGun)
            {
                if (playerAnimator.runtimeAnimatorController != withGunController)
                {
                    ChangeModel(PlayerModel.WITH_GUN);

                    playerAnimator.runtimeAnimatorController = withGunController;
                    playerAnimator.speed = 1f;
                }
            }
            else
            {
                if (playerAnimator.runtimeAnimatorController != defaultController)
                {
                    ChangeModel(PlayerModel.DEFAULT);

                    playerAnimator.runtimeAnimatorController = defaultController;
                    playerAnimator.speed = 1f;
                }
            }
            
            // Checa Anima��o de Andar
            if (move != Vector3.zero && velocity.y <= -1.9f)
            {
                if (isSprinting) // Caso estiver Correndo
                {
                    playerAnimator.SetBool("isWalking", false);
                    playerAnimator.SetBool("isSprinting", true);
                }
                else // Caso s� estiver Caminhando
                {
                    playerAnimator.SetBool("isWalking", true);
                    playerAnimator.SetBool("isSprinting", false);
                }
            }
            else
            {
                playerAnimator.SetBool("isWalking", false);
                playerAnimator.SetBool("isSprinting", false);
            }

            // Desativa Anima��o de Pulo
            playerAnimator.SetFloat("speedY", velocity.y);
        }
        #endregion

        if (!isClimbing) //Movimenta��o normal, enquanto n�o est� escalando
        {
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }

        #region Escalar
        if (isClimbing)
        {
            gravity = 0f;
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 climbDirection = Vector3.up * verticalInput * moveSpeed;

            if (verticalInput == 0)
            {
                velocity.y = 0f;
            }

            characterController.Move(climbDirection * Time.deltaTime);
        }
        else
        {
            gravity = -9.81f;
        }
        #endregion

        #region Pular
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching) //Se est� no ch�o e n�o est� agachado, pode pular
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            playerAnimator.SetTrigger("Jump");
        }
        #endregion

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        #region Correr
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching) //Se est� no ch�o, n�o est� agachado, e pressionar shift, pode correr
        {
            isSprinting = true; //Muda a velocidade
            moveSpeed = sprintSpeed;
        }
        else if (!isCrouching)
        {
            isSprinting = false; //Muda a velocidade
            moveSpeed = normalSpeed;
        }
        #endregion

        //ARRUMAR, ELE T� CAINDO NO CH�O
        #region Agachar
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            isCrouching = true;
            characterController.height = crouchColliderHeight;
            //characterController.center = new Vector3(characterController.center.x, 0.0f, characterController.center.z); // Ajusta o centro
            moveSpeed = crouchSpeed;

            groundCheck.transform.localPosition = new Vector3(groundCheck.transform.localPosition.x, crouchGroundCheckY,
                groundCheck.transform.localPosition.z);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            characterController.height = playerColliderHeight;
            //characterController.center = new Vector3(characterController.center.x, 0.5f, characterController.center.z); // Ajusta o centro
            //O correto � ficar no 0, mas ele afunda se for 0. Ver como arrumar.
            transform.position += Vector3.up * 0.75f;

            groundCheck.transform.localPosition = new Vector3(groundCheck.transform.localPosition.x, defaultGroundCheckY,
                groundCheck.localPosition.z);
            moveSpeed = normalSpeed;
        }
        #endregion
    }

    // Ativa o modelo do Player desejado para as anima��es
    private void ChangeModel(PlayerModel type)
    {
        if (type == PlayerModel.DEFAULT)
        {
            playerBody1.SetActive(true);
            playerBody2.SetActive(false);

            playerAnimator = playerBody1.GetComponent<Animator>();
        }
        else // WITH_GUN
        {
            playerBody1.SetActive(false);
            playerBody2.SetActive(true);

            playerAnimator = playerBody2.GetComponent<Animator>();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    #endregion
}
