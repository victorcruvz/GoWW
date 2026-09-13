using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProbadorCamara : MonoBehaviour
{
    public RawImage pantallaUI;
    private WebCamTexture camara;
    private Texture2D texturaModificada;

    void Start()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            string nombreCamara = WebCamTexture.devices[0].name;
            camara = new WebCamTexture(nombreCamara, 1280, 720, 30);

            // Asignamos la cámara desde el inicio para ver el video en vivo
            if (pantallaUI != null)
            {
                pantallaUI.texture = camara;
            }

            camara.Play();

            // Esperamos a procesar el fotograma
            StartCoroutine(AplicarFiltroRojo());
        }
        else
        {
            Debug.LogError("No se detectó ninguna cámara web.");
        }
    }

    IEnumerator AplicarFiltroRojo()
    {
        // Espera 1.5 segundos para dar tiempo a que la cámara entregue datos válidos
        yield return new WaitForSeconds(1.5f);

        if (camara != null && camara.isPlaying && pantallaUI != null)
        {
            // Extraemos los píxeles actuales
            Color32[] pixeles = camara.GetPixels32();

            // Forzamos el canal Rojo
            for (int i = 0; i < pixeles.Length; i++)
            {
                pixeles[i].r = 255;
            }

            // Creamos y aplicamos la nueva textura con los mismos datos
            texturaModificada = new Texture2D(camara.width, camara.height, TextureFormat.RGBA32, false);
            texturaModificada.SetPixels32(pixeles);
            texturaModificada.Apply();

            // Cambiamos la pantalla al resultado teñido
            pantallaUI.texture = texturaModificada;

            Debug.Log("¡Filtro rojo aplicado exitosamente!");
        }
    }

    void OnDisable()
    {
        if (camara != null && camara.isPlaying)
        {
            camara.Stop();
        }
    }
}