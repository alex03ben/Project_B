using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEngineController : MonoBehaviour
{
    [Header("Général")]
    [Tooltip("Référence vers la camera du joueur")]
    public Camera cameraPlayer;
    private float angleCamera = 0f;

    [Header("Déplacement")]
    [Tooltip("Vitesse maximum lorsque le joueur est sur le sol et qu'il ne sprint pas")]
    public float vitesseNormale = 10f;
    [Tooltip("Vitesse maximum du sprint")]
    public float vitesseSprint = 5f;
    [Tooltip("Vitesse du ralentissement au sol, permet d'avoir une fluidité sur les mouvement et un meilleur feeling")]
    public float vitesseRalentissement = 100f;
    [Tooltip("Force de la gravité")]
    public float forceGravite = 20f;


    [Header("Saut")]
    [Tooltip("Force du saut")]
    public float forceSaut = 9f;
    [Tooltip("Layer sur la quel on va check si le joueur touche le sol ou pas wesh tmtc")]
    public LayerMask layerGroundCheck = -1;
    [Tooltip("La distance avant de check si le joueur touche le sol")]
    public float groundCheckDistance = 0.05f;


    [Header("Souris")]
    [Tooltip("Vitesse de rotation de la camera")]
    public float rotationSpeedCam = 200f;

    private Vector3 normalGround;
    private PlayerInput playerInput;
    private CharacterController characterController;

    private float lastTimeJump = 0f;


    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public bool isCrouching { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        groundCheck();

        playerMouvement();
    }

    private void playerMouvement()
    {
        //=======================================================================CAMERA=========================================================
        // Rotation camera horizontale
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (playerInput.getLookInputHorizontal() * rotationSpeedCam), 0f), Space.Self);
        }

        // Rotation camera Verticale
        {

            angleCamera += playerInput.getLookInputVertical() * rotationSpeedCam;
            angleCamera = Mathf.Clamp(angleCamera, -89f, 89f);
            cameraPlayer.transform.localEulerAngles = new Vector3(angleCamera, 0, 0);
        }


        //============================================================================DEPLACEMENT================================================
        {
            Vector3 playerPosition = transform.TransformVector(playerInput.getMoveInput());

            if (isGrounded)
            {
                
                Vector3 targetVelocity = playerPosition * vitesseNormale;


                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, vitesseRalentissement * Time.deltaTime);

                
                if (isGrounded && playerInput.getInputJump())
                {
                    characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                    characterVelocity += Vector3.up * forceSaut;

                    //m_LastTimeJumped = Time.time;
                    //hasJumpedThisFrame = true;

                    isGrounded = false;
                    //m_GroundNormal = Vector3.up;

                }
            }
            else
            {
                characterVelocity += Vector3.down * forceGravite * Time.deltaTime;
            }
        }

        characterController.Move(characterVelocity * Time.deltaTime);


    }


    private void groundCheck()
    {
        float checkGroundDistance = isGrounded ? (characterController.skinWidth + groundCheckDistance) : 1f;

        isGrounded = false;
        normalGround = Vector3.up;
        print("----- D1");
        if (Time.time >= lastTimeJump + 0.02f)
        {
            print("----- D2: ");
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(characterController.height), characterController.radius, Vector3.down, out RaycastHit hit, checkGroundDistance, layerGroundCheck, QueryTriggerInteraction.Ignore))
            {
                print("----- D3");
                normalGround = hit.normal;

                if (Vector3.Dot(hit.normal, transform.up) > 0 && IsNormalUnderSlopeLimit(normalGround))
                {
                    isGrounded = true; 
                    print("----- D4");

                    if (hit.distance > characterController.skinWidth)
                    {
                        print("----- D5");
                        characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    Vector3 GetCapsuleBottomHemisphere()
    {
        //print("test GetCapsuleBottomHemisphere = " + (transform.position + (transform.up * characterController.radius)));
        return transform.position + (transform.up * characterController.radius);
    }

    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        //print("test GetCapsuleTopHemisphere = " + (transform.position + (transform.up * (atHeight - characterController.radius))));
        return transform.position + (transform.up * (atHeight - characterController.radius));
    }

    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= characterController.slopeLimit;
    }
}
