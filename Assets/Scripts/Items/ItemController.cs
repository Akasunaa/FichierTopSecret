using Mono.Cecil.Rocks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 *  Component used by the Item category of objects, that have special properties
 *  Items are typically "stored" somewhere as in they are not instanciated but rather get created when a player meets certain requirements/interactions
 *  Once collected by the Player, the prefab hides itself as a player's child and will create the according itemname.txt file in the Player folder
 *  The collection is handled by another script : it will create the item's prefab, assign it as a player's child AND THEN call RecuperatingItem()
 */
public class ItemController : ModifiableController
{
    [Header("Item Variables")]
    [SerializeField] private string item_name;
    [Header("Item elements")]
    [SerializeField] private GameObject itemSprite;

    public void OnEnable()
    {
        itemSprite.SetActive(false);
    }

    /**
     *  Function called by external scripts when the Player interacts with objects containing items
     *  It will create the item's file in the corresponding folder
     */
    public int RecuperatingItem()
    {
        FileParser fileParser = GetComponent<FileParser>();
        FileInfo fileInfo = new FileInfo(fileParser.filePath);
        using (StreamWriter sw = new StreamWriter(fileInfo.FullName))
        {
            sw.Write(fileParser.targetModifiable.ToFileString());
        }
        return 0;
    }

    /**
     *  Function that will check if the player already possesses the associated item
     */
    public bool CheckPresenceItem()
    {
        FileParser fileParser = GetComponent<FileParser>();
        FileInfo fileInfo = new FileInfo(fileParser.filePath);
        return fileInfo.Exists;
        //Debug.Log("PLAYER ALREADY HAS ITEM");
        //Debug.Log("PLAYER DOESN'T HAVE ITEM");
    }

    public override void SetDefaultProperties()
    {
        
    }
}
