using UnityEngine;

public class DashDodge : MonoBehaviour
{
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.15f;
    public float dodgeCooldown = 0.5f;

    private CharacterController characterController;
    private Vector3 dodgeDirection;
    internal bool isDodging = false;
    private float dodgeTimer = 0f;
    private float cooldownTimer = 0f;

    internal bool dodgeInputDetected = false;
    private float dodgeInputTimer = 0.3f;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (cooldownTimer > 0)//Cooldowna girmisse cooldownu azalt
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isDodging && cooldownTimer <= 0)//su an dodge yapmiyosa ve cooldownda degilse
        {
            HandleDodgeInput();
        }

        if (isDodging)//Dodge iste, anlatmaya gerek yok
        {
            Dodge();
        }

        if (dodgeInputDetected)//A, S veya D ye bastigimiz zaman true olur
        {
            dodgeInputTimer -= Time.deltaTime;//Dodge detect suresini uygula

            if (dodgeInputTimer <= 0)//Dodge detect olur ama dodge atmazsa false yap
            {
                dodgeInputDetected = false;
                dodgeInputTimer = 0.5f;//Dodge suresini sifirla
            }
        }

    }

    private void HandleDodgeInput()
    {
        //Burada aslinda olmasi gereken, once a s d tuslarina basip sonra spacee basmak degil de
        //a s d ve space tuslarinin kombine halde calismasini saglamak, cunku once space basarsak dodge
        //calismiyor ve bu da olmasini istedigim bir sey degil

        if (Input.GetKeyDown(KeyCode.A))
        {
            dodgeDirection = -transform.right; // Sol
            dodgeInputDetected = true; // Dodge giriþini algýla
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            dodgeDirection = -transform.forward; // Geri
            dodgeInputDetected = true; // Dodge giriþini algýla
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            dodgeDirection = transform.right; // Saða
            dodgeInputDetected = true; // Dodge giriþini algýla
        }

        // Eðer dodge giriþ algýlandýysa, Space'e basmayý kontrol et
        if (dodgeInputDetected && Input.GetKeyDown(KeyCode.Space))
        {
            StartDodge();
        }
    }

    private void StartDodge()
    {
        isDodging = true;
        dodgeTimer = dodgeDuration;
        cooldownTimer = dodgeCooldown; // Dodge iþlemi bittiðinde cooldown süresini baþlat
        dodgeInputDetected = false; // Dodge giriþini sýfýrla
    }

    private void Dodge()
    {
        isDodging = true;
        // Dodge yönünde hareket et
        characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);

        dodgeTimer -= Time.deltaTime;
        if (dodgeTimer <= 0)
        {
            isDodging = false; // Dodge iþlemi bitti
        }
    }
}
