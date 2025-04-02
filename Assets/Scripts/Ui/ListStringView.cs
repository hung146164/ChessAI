using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ContentString : MonoBehaviour
{
    private string data;
    [SerializeField] private TMP_Text textField;

    public void SetData(string item)
    {
        data = item;
        textField.text = item.ToString(); 
    }

    public string GetData()
    {
        return data;
    }
}
public class ListContentString : MonoBehaviour
{
    private List<string>items = new List<string>();

    [SerializeField] private ContentString contentPrefab; 

    private void CreateContent(string itemData)
    {
        ContentString contentInstance = Instantiate(contentPrefab, transform);
        contentInstance.SetData(itemData);
        contentInstance.gameObject.name = itemData.ToString();
    }

    public void UpdateListContent(List<string> newItems)
    {
        ClearListContent();
        items = newItems;
        foreach (string item in newItems)
        {
            CreateContent(item);
        }
    }

    public void ClearListContent()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        items.Clear();
    }

    public void AddItem(string item)
    {
        items.Add(item);
        CreateContent(item);
    }

    public void RemoveItem(string item)
    {
        if (items.Remove(item))
        {
            foreach (Transform child in transform)
            {
                if(child.name==item.ToString())
                {
                    Destroy(child.gameObject);
                    return;
                }
            }
        }
    }
}


