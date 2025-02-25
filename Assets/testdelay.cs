using System.Collections;
using UnityEngine;
using System.Threading.Tasks; 

public class testdelay : MonoBehaviour
{
    public GameObject cubo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DestruirCubo();
        StartCoroutine(DestruirCuboCorrutina());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task DestruirCubo()
    {
        await Task.Delay(2000);
        cubo.gameObject.SetActive(false);
        await Task.Delay(2000);
        cubo.gameObject.SetActive(true);

        
    }

    IEnumerator DestruirCuboCorrutina()
    {
        yield return new WaitForSeconds(3);
        cubo.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        cubo.gameObject.SetActive(true);
    }
}
