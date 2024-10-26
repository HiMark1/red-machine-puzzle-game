using UnityEngine;
using Player.ActionHandlers;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float swipeSpeed = 0.01f;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private bool _isPanning = true;
    private Vector3 touchStart;
    private UnityEngine.Camera cam;
    private Vector2 touchStartPos;
    private bool isDragging = false;
    
    void MoveCamera(Vector3 direction)
    {
        if (!isDragging) return;
        Vector3 currentPos = cam.ScreenToWorldPoint(direction);
        Vector3 _direction = touchStart - currentPos;
        cam.transform.position += direction * swipeSpeed;

        //Ограничиваем перемещение по границам уровня
        cam.transform.position = new Vector3(
           Mathf.Clamp(cam.transform.position.x, minBounds.x, maxBounds.x),
           Mathf.Clamp(cam.transform.position.y, minBounds.y, maxBounds.y),
           cam.transform.position.z);
    }

    void IsPointerOverGameObject()
    {
        // Проверка нажатия по UI (например, Canvas)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            _isPanning = false;
            return;
        }
        touchStartPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Проверка, попал ли свайп на игровой объект с коллайдером
        Collider2D hitCollider = Physics2D.OverlapPoint(touchStartPos);
        if (hitCollider != null)
        {
            _isPanning = false;
            return;
        }

    }

    private void OnPointerDown(Vector3 vector)
    {
        IsPointerOverGameObject();
        touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }
    private void OnPointerUp(Vector3 vector)
    {
        _isPanning = true;
        isDragging = false;
    }

    private void OnDrag(Vector3 vector)
    {

        if (_isPanning)
        {
            Vector3 direction = touchStart - cam.ScreenToWorldPoint(Input.mousePosition);
            MoveCamera(direction);
        }

    }
    private void OnEnable()
    {
        cam = UnityEngine.Camera.main;
        ClickHandler.Instance.PointerDownEvent += OnPointerDown;
        ClickHandler.Instance.PointerUpEvent += OnPointerUp;
        ClickHandler.Instance.DragEvent += OnDrag;
    }

}
