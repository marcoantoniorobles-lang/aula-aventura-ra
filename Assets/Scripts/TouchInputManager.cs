using UnityEngine;

public class TouchInputManager : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // En editor: clic del mouse. En Android: touch
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                AnimalTarget animal = hit.collider.GetComponent<AnimalTarget>();
                if (animal != null)
                    animal.HandleTouch();
            }
        }
    }
}
