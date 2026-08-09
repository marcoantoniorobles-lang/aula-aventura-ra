using UnityEngine;

// Aula Aventura RA - Modulo 2: Ordenar numeros
// Se coloca en cada slot vacio (casilla donde debe caer la pelota).
// Solo acepta la pelota con el numero correcto.
public class SlotTarget : MonoBehaviour
{
    [Header("Configuracion")]
    public int slotNumber = 1; // Numero esperado en este slot (1-5)
    public Color emptyColor = new Color(0.9f, 0.9f, 0.2f, 1f); // amarillo opaco - visible en AR
    public Color correctColor = new Color(0.1f, 0.9f, 0.1f, 1f); // verde opaco al colocar

    [HideInInspector] public bool isOccupied = false;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = emptyColor;
    }

    // Llamado por NumberBall cuando cae en este slot correctamente
    public void OccupySlot(NumberBall ball)
    {
        isOccupied = true;

        if (rend != null)
            rend.material.color = correctColor;

        Debug.Log("[SlotTarget] Slot " + slotNumber + " ocupado correctamente.");
    }

    public void ResetSlot()
    {
        isOccupied = false;
        if (rend != null)
            rend.material.color = emptyColor;
    }
}
