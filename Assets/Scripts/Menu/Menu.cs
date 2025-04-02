using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    public bool isopen = false;

    public void Open()
    {
        gameObject.SetActive(true);
        isopen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        isopen = false;
    }
}