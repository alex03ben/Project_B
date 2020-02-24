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
    [Tooltip("Vitesse du ralentissement au sol, permet d'avoir une fluidité sur les mouvement et un meilleur feeling")]
    public float vitesseRalentissement = 100f;
    [Tooltip("Force de la gravité")]
    public float forceGravite = 20f;
    [Tooltip("Coefficient de reduction de la vitesse lorsque le joueur est accroupi ")]
    public float coefReducteur = 0.2f;
    [Tooltip("Ecrasement du personnage qaund tu es accroupi")]
    [Range(0, 1)]
    public float tailleAccroupi = 1f;


    [Header("Saut")]
    [Tooltip("Force du saut")]
    public float forceSaut = 9f;
    [Tooltip("Force pour l'air controle")]
    public float forceAirControle = 10f;
    [Tooltip("Vitesse du controle dans les air")]
    public float vitesseAirControle = 15f;
    [Tooltip("Layer sur la quel on va check si le joueur touche le sol ou pas wesh tmtc")]
    public LayerMask layerGroundCheck = -1;
    [Tooltip("La distance avant de check si le joueur touche le sol")]
    public float groundCheckDistance = 0.05f;



    [Header("Souris")]
    [Tooltip("Vitesse de rotation de la camera")]
    public float rotationSpeedCam = 200f;

    private Vector3 normalGround;
    private float saveVitesseMarche = 0f;
    private float saveHeightCharctere;
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
        //groundCheck();

        isGrounded = characterController.isGrounded;
        print("grounded: " + isGrounded);
        playerMouvement();
    }

    private void playerMouvement()
    {
        //=======================================================================CAMERA=========================================================
        // Rotation camera horizontale
        {
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

                if (playerInput.getInputCrouchDown())
                {
                    saveVitesseMarche = vitesseNormale;
                    saveHeightCharctere = characterController.height;

                    characterController.height = characterController.height - tailleAccroupi;

                    vitesseNormale = vitesseNormale * coefReducteur;
                    print("CROUCH !!!!");
                }else if (playerInput.getInputCrouchUp())
                {
                    characterController.height = saveHeightCharctere;
                    vitesseNormale = saveVitesseMarche;
                    print(" PLUS !! CROUCH !!!!");
                }

                Vector3 targetVelocity = playerPosition * vitesseNormale;


                characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, vitesseRalentissement * Time.deltaTime);


                if (isGrounded && playerInput.getInputJump())
                {
                    //characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
                    characterVelocity += Vector3.up * forceSaut;

                    //isGrounded = false;

                }
            }
            else
            {
                characterVelocity += playerPosition * vitesseAirControle * Time.deltaTime;

                float verticalVelocity = characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, vitesseAirControle);

                characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                characterVelocity += Vector3.down * forceGravite * Time.deltaTime;
            }
        }

        characterController.Move(characterVelocity * Time.deltaTime);


    }


    private void groundCheck()
    {
        float checkGroundDistance = isGrounded ? (characterController.skinWidth + groundCheckDistance) : 2f;

        isGrounded = false;
        normalGround = Vector3.up;
        print("----- D1 : "+checkGroundDistance);
        if (Time.time >= lastTimeJump + 0.02f)
        {
            print("----- D2: ");
            if (Physics.CapsuleCast(
                GetCapsuleBottomHemisphere(),
                GetCapsuleTopHemisphere(),
                characterController.radius,
                Vector3.down,
                out RaycastHit hit,
                groundCheckDistance,
                layerGroundCheck,
                QueryTriggerInteraction.Ignore
                ))
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


    private Vector3 GetCapsuleBottomHemisphere()
    {
        //print("test GetCapsuleBottomHemisphere = " + (transform.position + (transform.up * characterController.radius)));
        return transform.position + Vector3.up * (characterController.radius + Physics.defaultContactOffset);
    }

    private Vector3 GetCapsuleTopHemisphere()
    {
        //print("test GetCapsuleTopHemisphere = " + (transform.position + (transform.up * (atHeight - characterController.radius))));
        return transform.position + Vector3.up * (characterController.height - characterController.radius + Physics.defaultContactOffset);
    }

    private bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= characterController.slopeLimit;
    }
}
