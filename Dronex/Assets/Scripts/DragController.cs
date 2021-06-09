using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour
{
    public Transform arrowPrefab;
    public float arrowSensitivity;
    public float x2Distance;
    public string arrowRendererPath = "Arrow";
    public string arrowLineRendererPath = "PlatoonGhost";
    public Texture2D x1Texture;
    public Texture2D x2Texture;
    public string playerTag = "Player";
    public Camera playerCamera;
    private MoveController moveController;
    private MoveController.MovementMode movementMode;
    private Transform arrow;
    private Renderer arrowRenderer;
    private Color arrowColor;
    private Renderer arrowLineRenderer;
    private Color arrowLineColor;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 dragPoint;
    private bool onDragStarted = false;

    public void Start()
    {
        arrow = Instantiate(arrowPrefab, Vector3.zero, Quaternion.Euler(-90, 0, 0));
        arrow.gameObject.SetActive(false);

        arrowRenderer = arrow.Find(arrowRendererPath).GetComponent<Renderer>();
        arrowColor = arrowRenderer.material.color;

        arrowLineRenderer = arrow.Find(arrowLineRendererPath).GetComponent<Renderer>();
        arrowLineColor = arrowLineRenderer.material.color;

        moveController = GameObject.FindGameObjectWithTag(playerTag).GetComponent<MoveController>();
        movementMode = MoveController.MovementMode.Walk;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            startPoint = getMouseHitPoint();
            if (startPoint != Vector3.zero)
            {
                onDragStarted = true;
                movementMode = MoveController.MovementMode.Walk;
                arrow.gameObject.SetActive(false);
            }
        }

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            endPoint = getMouseHitPoint();
            onDragStarted = false;
        }

        Debug.DrawLine(
            new Vector3(startPoint.x, 5.0f, startPoint.z),
            startPoint,
            Color.blue);

        if (onDragStarted)
        {
            dragPoint = getMouseHitPoint();
            Debug.DrawLine(
                new Vector3(dragPoint.x, 5.0f, dragPoint.z),
                dragPoint,
                Color.black);

            Vector3 heading = dragPoint - startPoint;

            if (heading.magnitude > arrowSensitivity)
            {
                if (heading.magnitude > x2Distance)
                {
                    arrowRenderer.material.mainTexture = x2Texture;
                    movementMode = MoveController.MovementMode.Run;
                }
                else
                {
                    arrowRenderer.material.mainTexture = x1Texture;
                    movementMode = MoveController.MovementMode.Walk;
                }

                arrowRenderer.material.color = arrowColor;
                arrowLineRenderer.material.color = arrowLineColor;

                arrow.position = new Vector3(startPoint.x, startPoint.y + 1.0f, startPoint.z);
                arrow.gameObject.SetActive(true);
                arrow.LookAt(new Vector3(dragPoint.x, dragPoint.y + 1.0f, dragPoint.z));

                moveController.setLookPoint(dragPoint);
                moveController.setMovementMode(movementMode);
            }
            else
            {
                moveController.removeLookPoint();
                moveController.setMovementMode(movementMode);
            }
        }
        else
        {
            Debug.DrawLine(
                new Vector3(endPoint.x, 5.0f, endPoint.z),
                endPoint,
                Color.black);

            materialToGhost(arrowRenderer.material);
            materialToGhost(arrowLineRenderer.material);

            if (moveController.agentIsArrive())
            {
                arrow.gameObject.SetActive(false);
            }
        }
    }

    private void materialToGhost(Material material)
    {
        Color c = Color.black;
        c.a = 0.3f;
        material.color = c;
    }

    private Vector3 getMouseHitPoint()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
