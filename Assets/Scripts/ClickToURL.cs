using UnityEngine;

public class ClickToURL : MonoBehaviour
{
    [SerializeField] string URL;

    private void OnMouseDown()
    {
        Application.OpenURL(URL);
    }
}
