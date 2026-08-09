using UnityEngine;
using System.Collections;

// Aula Aventura RA - Modulo 2: Ordenar numeros
// Se coloca en cada pelota numerada (1-5).
// El jugador la arrastra con el dedo hasta el slot correcto.
public class NumberBall : MonoBehaviour
{
    [Header("Configuracion")]
    public int ballNumber = 1;              // Numero que representa esta pelota (1-5)
    public Color ballColor = Color.white;   // Color visual de la pelota (white = no tinta la textura)

    [Header("Estado")]
    public bool isPlaced = false;           // True cuando ya fue colocada correctamente

    private Vector3 originalLocalPosition;  // Posicion relativa al ImageTarget (estable ante el AR tracking)
    private bool isDragging = false;
    private Camera mainCam;
    private Renderer rend;
    private Vector3 dragOffset;
    private float dragHeight;               // Altura fija durante el arrastre, medida a lo largo
                                             // del eje "arriba" del propio marcador (ImageTarget),
                                             // no del mundo. Asi el arrastre sigue la inclinacion
                                             // real de la hoja impresa sin importar el angulo de la webcam.

    void Start()
    {
        originalLocalPosition = transform.localPosition;
        mainCam = Camera.main;
        rend = GetComponent<Renderer>();

        // Solo aplicar color si no tiene textura asignada (evita tapar texturas de Kenney)
        if (rend != null && rend.material.mainTexture == null)
            rend.material.color = ballColor;
    }

    void Update()
    {
        if (isPlaced) return;

        HandleDragInput();
    }

    void HandleDragInput()
    {
        // Funciona tanto con mouse (editor) como con touch (Android)
        if (Input.GetMouseButtonDown(0))
        {
            TryPickUp(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            DragTo(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Drop();
        }
    }

    void TryPickUp(Vector3 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isDragging = true;

                // Se mide la altura de la bola a lo largo del eje "arriba" del marcador
                // (no del eje Y del mundo), para que el arrastre respete la inclinacion
                // real del ImageTarget tal como lo ve la camara/webcam.
                Vector3 refUp = transform.parent != null ? transform.parent.up : Vector3.up;
                Vector3 refOrigin = transform.parent != null ? transform.parent.position : Vector3.zero;
                dragHeight = Vector3.Dot(transform.position - refOrigin, refUp) + 0.05f; // Levanta ligeramente al arrastrar

                StartCoroutine(ScalePulse(1.2f));
            }
        }
    }

    void DragTo(Vector3 screenPos)
    {
        // Proyecta el rayo de la camara en el plano del marcador (ImageTarget), no en el
        // plano horizontal del mundo. Esto evita que la bola se acerque o aleje de la
        // camara (y por lo tanto cambie de tamano visualmente) cuando el marcador esta
        // inclinado respecto a la webcam.
        Ray ray = mainCam.ScreenPointToRay(screenPos);

        Vector3 planeNormal = transform.parent != null ? transform.parent.up : Vector3.up;
        Vector3 refOrigin = transform.parent != null ? transform.parent.position : Vector3.zero;
        Vector3 planePoint = refOrigin + planeNormal * dragHeight;
        Plane dragPlane = new Plane(planeNormal, planePoint);

        if (dragPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            transform.position = worldPos;
        }
    }

    void Drop()
    {
        isDragging = false;

        // Busca el slot mas cercano
        SlotTarget nearest = FindNearestSlot();

        if (nearest != null && nearest.slotNumber == ballNumber && !nearest.isOccupied)
        {
            // Encaja correctamente
            PlaceInSlot(nearest);
        }
        else
        {
            // Regresa a origen
            StartCoroutine(ReturnToOrigin());
        }
    }

    void PlaceInSlot(SlotTarget slot)
    {
        isPlaced = true;
        slot.OccupySlot(this);

        // Se eleva ligeramente a lo largo del eje "arriba" del marcador (no del mundo),
        // para que la bola quede apoyada sobre el slot sin importar la inclinacion del ImageTarget.
        Vector3 placeUp = transform.parent != null ? transform.parent.up : Vector3.up;
        transform.position = slot.transform.position + placeUp * 0.02f;
        StartCoroutine(ScalePulse(1.3f));

        if (GameManager_M2.Instance != null)
            GameManager_M2.Instance.OnBallPlaced();
    }

    SlotTarget FindNearestSlot()
    {
        SlotTarget[] slots = FindObjectsByType<SlotTarget>(FindObjectsSortMode.None);
        SlotTarget nearest = null;
        float minDist = 0.03f; // Distancia maxima de "snap" en metros AR (menor que la
                                // separacion entre filas 0.04 para exigir arrastre real, pero suficiente para tolerar imprecision de camara/mouse)

        // Se comparan las posiciones convertidas al espacio LOCAL del marcador (ImageTarget),
        // no al espacio del mundo. La bola ahora se arrastra sobre el plano del marcador
        // (ver DragTo), asi que la distancia debe medirse en ese mismo plano; comparar en
        // coordenadas de mundo quedaba desincronizado si el marcador esta inclinado.
        Transform reference = transform.parent != null ? transform.parent : transform;
        Vector2 ballLocalXZ = LocalXZ(reference, transform.position);

        foreach (SlotTarget slot in slots)
        {
            Vector2 slotLocalXZ = LocalXZ(reference, slot.transform.position);
            float dist = Vector2.Distance(ballLocalXZ, slotLocalXZ);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = slot;
            }
        }
        return nearest;
    }

    // Convierte una posicion de mundo a coordenadas locales relativas a 'reference' y
    // devuelve solo el plano X/Z de ese espacio local (equivalente al plano del marcador).
    Vector2 LocalXZ(Transform reference, Vector3 worldPos)
    {
        Vector3 local = reference.InverseTransformPoint(worldPos);
        return new Vector2(local.x, local.z);
    }

    IEnumerator ReturnToOrigin()
    {
        float duration = 0.25f;
        float t = 0f;
        Vector3 start = transform.localPosition;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, originalLocalPosition, t / duration);
            yield return null;
        }
        transform.localPosition = originalLocalPosition;
    }

    IEnumerator ScalePulse(float targetScale)
    {
        Vector3 original = transform.localScale;
        Vector3 big = original * targetScale;
        float half = 0.1f;
        float t = 0f;

        while (t < half) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(original, big, t / half); yield return null; }
        t = 0f;
        while (t < half) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(big, original, t / half); yield return null; }

        transform.localScale = original;
    }
}
