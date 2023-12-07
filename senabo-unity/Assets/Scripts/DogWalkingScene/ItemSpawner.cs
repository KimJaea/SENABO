using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIModalManager;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject dogObject;
    [SerializeField]
    private GameObject snackObject;
    [SerializeField]
    private GameObject poopObject;
    [SerializeField]
    private GameObject snackLocation;
    [SerializeField]
    private GameObject poopLocation;
    /*[SerializeField]
    private GameObject pingPaw;
    */
    public enum ItemType
    {
        Snack,
        Poop,
        Ping
    }


    public ItemType Snack => ItemType.Snack;
    public ItemType Poop => ItemType.Poop;
    public ItemType Ping => ItemType.Ping;

    public void HandleSpawnAction(ItemType item)
    {
        switch (item)
        {
            case ItemType.Snack:
                SpawnItem(snackObject, snackLocation);
                break;
            case ItemType.Poop:
                SpawnItem(poopObject, poopLocation);
                break;
        }
    }

    public void HandleRemoveAction(ItemType item)
    {
        switch (item)
        {
            case ItemType.Snack:
                RemoveItem(snackObject);
                break;
            case ItemType.Poop:
                RemoveItem(poopObject);
                break;
        }
    }

    public void SpawnItem(GameObject item, GameObject location)
    {
        if (dogObject.activeInHierarchy)
        {
            Vector3 position = location.GetComponent<Transform>().position;
            item.SetActive(true);
            item.transform.position = position;
        }
    }

    public void RemoveItem(GameObject item)
    {
        item.SetActive(false);
    }
}
