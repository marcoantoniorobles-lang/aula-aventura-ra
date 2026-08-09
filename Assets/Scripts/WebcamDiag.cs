using UnityEngine;

/// <summary>
/// Script de diagnostico: detecta camaras disponibles y verifica acceso.
/// Adjuntar a cualquier GameObject temporal para diagnosticar.
/// ELIMINAR antes de build final.
/// </summary>
public class WebcamDiag : MonoBehaviour
{
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log($"[WebcamDiag] Dispositivos de camara encontrados: {devices.Length}");

        if (devices.Length == 0)
        {
            Debug.LogWarning("[WebcamDiag] NO se encontraron camaras. " +
                "Verificar: Configuracion > Privacidad > Camara en Windows.");
            return;
        }

        foreach (var d in devices)
            Debug.Log($"[WebcamDiag]   Camara: '{d.name}' (frontal: {d.isFrontFacing})");

        // Intentar abrir la primera camara
        WebCamTexture tex = new WebCamTexture(devices[0].name, 640, 480, 30);
        tex.Play();
        Debug.Log($"[WebcamDiag] Webcam iniciada: playing={tex.isPlaying}, " +
            $"size={tex.width}x{tex.height}, deviceName='{tex.deviceName}'");
    }
}
