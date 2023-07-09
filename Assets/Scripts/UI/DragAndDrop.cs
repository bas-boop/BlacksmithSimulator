using UnityEngine;
using UnityEngine.Events;

public sealed class DragAndDrop : MonoBehaviour
{
    [SerializeField] private GameObject currentItemToDrag;
    private ItemManager _itemManager;

    [Header("Settings")]
    [SerializeField, Tooltip("The offset where the item is going to be dragged.")] private float dragOffset = 1;
    [SerializeField] private Transform spawnPointForItems;

    private Vector3 _mousePos;
    private Collider _lastDraggedItemCollider;
    
    [Header("Events")]
    
    [SerializeField] private UnityEvent onSpawnItem = new UnityEvent();
    [SerializeField] private UnityEvent onStopDragging = new UnityEvent();

    private void Start()
    {
        _itemManager = FindObjectOfType<ItemManager>();
    }

    private void Update()
    {
        SelectNewItemToDrag();
        
        if (Input.GetKeyDown(KeyCode.R)) // R for Remove item
        {
            _itemManager.RemoveItems(_lastDraggedItemCollider.gameObject);
            Destroy(_lastDraggedItemCollider.gameObject);
        }
        
        if (Input.GetMouseButtonUp(1)) onStopDragging?.Invoke();
        
        if (!Input.GetMouseButton(1)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            if(!hit.collider.gameObject.CompareTag("DragArea")) return;
            _mousePos = hit.point;
        }

        if (!currentItemToDrag) return;

        currentItemToDrag.GetComponent<ObjectConections>().SetObjectConnection();
        
        var itemPos = new Vector3(_mousePos.x, _mousePos.y, dragOffset);
        currentItemToDrag.transform.position = itemPos;
        if (_lastDraggedItemCollider) _lastDraggedItemCollider.enabled = false;
    }

    private void SelectNewItemToDrag()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        
        var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray1, out var hit1))
        {
            if (!hit1.collider.gameObject.CompareTag("DraggableItem")) return;
            if (_lastDraggedItemCollider) _lastDraggedItemCollider.enabled = true;
            
            currentItemToDrag = hit1.collider.gameObject;
            _lastDraggedItemCollider = currentItemToDrag.GetComponent<Collider>();

            currentItemToDrag.GetComponent<ObjectConections>().RemoveConnectedItems(hit1.collider.gameObject);
        }
    }

    public void SetItemToDrag(GameObject targetItem)
    {
        currentItemToDrag = Instantiate(targetItem, spawnPointForItems.position, targetItem.transform.rotation);
        _itemManager.AddItems(currentItemToDrag);
        onSpawnItem?.Invoke();
    }
}
