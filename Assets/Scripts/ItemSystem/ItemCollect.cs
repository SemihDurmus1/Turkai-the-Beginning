using Cinemachine;
using System;
using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    public Item itemReference;

    //Bu prefabi tek bi yerden alinacak sekilde yapabilirim belki,
    //belki bi prefabs poolu falan filan uydurabilirim
    public GameObject collectCanvasPrefab; 

    private GameObject collectCanvasInstance; // Instantiate edilmiþ Canvas'ý tutmak için

    [SerializeField]private float pickupRange = 2f;

    [SerializeField]private Transform playerCameraRoot;

    private void Start()
    {
        //collectCanvas.SetActive(false);
        //playerCameraTransform = Camera.main.transform;

        playerCameraRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot").transform;

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //Burada item collect canvasinin acilmasi ve yakinina girince alma islemi calisacak.

    //    if (other.CompareTag("Player"))
    //    {
    //        //Burada itemi addleyemese bile item siliniyor,
    //        //InventorySystemde itemin eklenip eklenmedigine
    //        //dair bilgi return eden bir sistem yazmam lazim bunun icin
    //        InventorySystem.Instance.AddItem(itemReference, 1);
    //        InventoryUI.Instance.RefreshUI();
    //        Destroy(gameObject);
    //    }
    //}

    private void Update()
    {
        float distance = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

        if (distance <= pickupRange)
        {
            ShowCollectCanvas();
            LookAtPlayer();

            if (Input.GetKeyDown(KeyCode.F))
            {
                CollectNearestItem();
            }
        }
        else
        {
            if (collectCanvasInstance != null)
            {
                HideCollectCanvas();
            }
        }
    }

    private void CollectNearestItem()
    {
        Ray ray = new Ray(playerCameraRoot.position, playerCameraRoot.forward * pickupRange);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.transform.root == gameObject.transform.root)
            {
                CollectItem();
            }
        }
        else
        {
            Debug.Log("Ray hicbir seye carpmadi");
        }
    }

    private void ShowCollectCanvas()
    {
        if (collectCanvasInstance == null)
        {
            collectCanvasInstance = Instantiate(collectCanvasPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);

            collectCanvasInstance.SetActive(true);

            // Dünya ölçeðini normalize ediyoruz (item'ýn scale'ine göre ters oranlý ayarlýyoruz)
            Vector3 inverseScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
            collectCanvasInstance.transform.localScale = inverseScale * 0.3f;  // Sabit bir scale ayarý yapýyoruz
        }
        else
        {
            collectCanvasInstance.SetActive(true);
        }
    }

    private void HideCollectCanvas()
    {
        if (collectCanvasInstance != null)
        {
            collectCanvasInstance.SetActive(false);
        }
    }

    private void LookAtPlayer()
    {
        if (collectCanvasInstance != null)
        {
            collectCanvasInstance.transform.LookAt(playerCameraRoot);

            collectCanvasInstance.transform.rotation = 
                Quaternion.Euler(
                -playerCameraRoot.rotation.eulerAngles.x,
                collectCanvasInstance.transform.rotation.eulerAngles.y, 
                0);

        }
    }

    private void CollectItem()
    {
        InventorySystem.Instance.AddItem(itemReference, 1);
        InventoryUI.Instance.RefreshUI();

        //collectCanvasPrefab.SetActive(false);
        if (collectCanvasInstance != null)
        {
            Destroy(collectCanvasInstance);
        }
        Destroy(gameObject);
    }
}



//float distance = Vector3.Distance(PlayerManager.Instance.transform.position, transform.position);

//if (distance <= pickupRange)
//{
//    Debug.Log("Pick Range icinde");

//    if (collectCanvasInstance == null)
//    {
//        collectCanvasInstance = Instantiate(collectCanvasPrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity, transform);

//        collectCanvasInstance.SetActive(true);

//        // Dünya ölçeðini normalize ediyoruz (item'ýn scale'ine göre ters oranlý ayarlýyoruz)
//        Vector3 inverseScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f / transform.localScale.z);
//        collectCanvasInstance.transform.localScale = inverseScale * 0.3f;  // Sabit bir scale ayarý yapýyoruz
//    }

//    LookAtPlayer();

//    if (Input.GetKeyDown(KeyCode.F))
//    {
//        CollectItem();
//    }
//}
//else
//{
//    if (collectCanvasInstance != null)
//    {
//        Destroy(collectCanvasInstance);
//    }
//}