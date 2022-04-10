using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

[System.Serializable]
public class PlacementData
{
    public GameObject _object;
    public Transform _placement;
}

public class MainController : MonoBehaviour
{
    public LayerMask interactableMask;

    //this will save the places and object place on them
    public PlacementData[] arrayOfPlaces = new PlacementData[5];

    [Space(10)]
    public Animator characterAnimator;

    [Header("UI")]
    public GameObject speechBubble, maxAmountPanel;
    public Transform itemContainerTransform;
    public GameObject itemUIDisplayPRefab;
    public TMP_Text totalPriceText;
    public List<GameObject> UiItemStorage;
    public new AudioSource audio;
    public ParticleSystem spark;

    public GameObject hoverPoint;

    //for private
    GameObject currentSelectedItem;
    Camera myCamera;
    RaycastHit hit;
    bool registerClicked;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
        registerClicked = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out hit, interactableMask))
        {
            if (Input.GetMouseButtonDown(0))
            {



                //for the interactions with register
                if (hit.collider.gameObject.CompareTag("Register") && registerClicked)
                {
                    registerClicked = !registerClicked;

                    speechBubble.SetActive(true);

                    float totalPrice = 0;

                    foreach (PlacementData pd in arrayOfPlaces)
                    {

                        if (pd._object != null)
                        {
                            GameObject temp = Instantiate(itemUIDisplayPRefab, itemContainerTransform);
                            ProductData product = pd._object.GetComponent<ProductData>();
                            temp.GetComponent<Image>().sprite = product.icon;
                            temp.GetComponentInChildren<Text>().text = "Price: " + product.price + "$";
                            totalPrice += product.price;
                            UiItemStorage.Add(temp);

                        }
                    }

                    if (totalPrice > 0)
                        totalPriceText.text = "Total Price: <br>" + totalPrice;
                    else
                        totalPriceText.text = "Nothing is selected to buy";
                }
                //removing items on click to register
                else if (!registerClicked)
                {
                    RemoveUIItems();
                }
                //for the interactions with items
                else
                {

                    if (!IsItemAtCounter(hit.collider.gameObject))
                    {
                        //for the character wave
                        if (hit.collider.gameObject.CompareTag("Player"))
                        {
                            characterAnimator.SetBool("Idle", false);
                            return;
                        }
                        //save the selected object
                        Transform availablePlace = GetAvailablePlaceAtCounter();
                        if (availablePlace != null)
                        {
                            currentSelectedItem = Instantiate(hit.collider.gameObject);
                            currentSelectedItem.name = hit.collider.gameObject.name;
                            currentSelectedItem.transform.position = hit.collider.gameObject.transform.position;
                            FillAtAvailablePlaceAtCounter(currentSelectedItem);
                            //for animation effect only
                            currentSelectedItem.transform.DOJump(availablePlace.position, 1f, 1, 0.5f, false).OnComplete(PlaySoundParticle);
                        }
                        else
                            StartCoroutine(ShowMaxAmount());
                    }
                }
            }
            /*
             * hover in item name
             */
            else
            {
                if (hit.collider.gameObject.CompareTag("Hover"))
                {
                    hoverPoint.SetActive(true);
                    hoverPoint.transform.position = hit.collider.gameObject.transform.position;

                }
                else
                {
                    hoverPoint.SetActive(false);
                }
            }
        }

        //additional work: by right clicking on the item on the counter 
        //the item will be deleted, so user can select more
        if (Input.GetMouseButtonDown(1) && !speechBubble.activeSelf)
        {
            if (Physics.Raycast(myCamera.ScreenPointToRay(Input.mousePosition), out hit, interactableMask))
            {

                if (IsItemAtCounter(hit.collider.gameObject))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
    void PlaySoundParticle()
    {
        audio.Play();
        spark.transform.position = currentSelectedItem.transform.position;
        spark.Play();
    }
    void RemoveUIItems()
    {
        registerClicked = !registerClicked;
        speechBubble.SetActive(false);
        for (int i = 0; i < UiItemStorage.Count; i++)
        {
            Destroy(UiItemStorage[i]);
        }
    }
    Transform GetAvailablePlaceAtCounter()
    {

        //if the object is not being placed then returning that slot
        for (int i = 0; i < arrayOfPlaces.Length; i++)
        {
            if (arrayOfPlaces[i]._object == null)
                return arrayOfPlaces[i]._placement;

        }
        //otherwise null - means no placement available
        return null;
    }

    IEnumerator ShowMaxAmount()
    {
        maxAmountPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        maxAmountPanel.SetActive(false);
    }

    void FillAtAvailablePlaceAtCounter(GameObject item)
    {
        for (int i = 0; i < arrayOfPlaces.Length; i++)
        {
            if (arrayOfPlaces[i]._object == null)
            {
                arrayOfPlaces[i]._object = item;
                break;
            }

        }
    }

    bool IsItemAtCounter(GameObject item)
    {

        for (int i = 0; i < arrayOfPlaces.Length; i++)
        {
            if (arrayOfPlaces[i]._object == item)
            {
                return true;
            }
        }

        return false;
    }
}