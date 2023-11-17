using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] 
    private float _speed = 5f;
    private CharacterController _characterController;

    [SerializeField] 
    private Transform _handTransform;

    private GameObject _itemInHand;
    private Vector3 _itemInHandPosition = Vector3.zero;
    private Quaternion _itemInHandRotate = new Quaternion(0,0,0,0);
    private Ray _ray;
    private GameObject _previousObjectInCameraFront;
    private float _forceValue = 15f;
    private float _interactionDistance = 2f;
    
    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Получаем пользовательский ввод.
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
    
        // Вычисляем вектор направления.
        var inputDirection = new Vector3(horizontalInput, 0, verticalInput);
        // Конвертируем локальное направление персонажа вперед в мировое.
        var direction = transform.TransformDirection(inputDirection);

        // Перемещаем персонажа.
        _characterController.SimpleMove(direction * _speed);

        _ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        HighlitingObject();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetMouseButtonDown(0) && _itemInHand != null)
        {
            ThrowAwayItem();
        }
    }

    private void ResetHighlitingObject()
    {
        if (_previousObjectInCameraFront != null)
        {
            _previousObjectInCameraFront.GetComponent<InteractableItem>().RemoveFocus();
            _previousObjectInCameraFront = null;
        }
    }

    private void HighlitingObject()
    {
        if (Physics.Raycast(_ray, out var hitInfo, _interactionDistance))
        {
            if (hitInfo.collider.gameObject != _previousObjectInCameraFront)
            {
                ResetHighlitingObject();
                if (hitInfo.collider.GetComponent<InteractableItem>() != null)
                {
                    hitInfo.collider.GetComponent<InteractableItem>().SetFocus();
                    _previousObjectInCameraFront = hitInfo.collider.gameObject;
                }
            }
        }
        else
        {
            ResetHighlitingObject();
        }
    }

    private void Interact()
    {
        if (Physics.Raycast(_ray, out var hitInfo, _interactionDistance))
        {
            if (hitInfo.collider.GetComponent<Door>() != null)
            {
                var door = hitInfo.collider.GetComponent<Door>();
                door.SwitchDoorState();
            }
            else if (hitInfo.collider.GetComponent<InteractableItem>() != null)
            {
                var item = hitInfo.collider.GetComponent<InteractableItem>();
                if (_itemInHand == null)
                {
                    TakeItem(item);
                }
                else
                {
                    DropItem();
                    _itemInHand = null;
                    TakeItem(item);
                }
            }
        }
    }

    private void TakeItem(InteractableItem item)
    {
        item.transform.SetParent(_handTransform);
        item.transform.localPosition = _itemInHandPosition;
        item.transform.rotation = _itemInHandRotate;
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().isKinematic = true;
        _itemInHand = item.gameObject;
    }

    private void DropItem()
    {
        _itemInHand.GetComponent<Rigidbody>().useGravity = true;
        _itemInHand.GetComponent<Rigidbody>().isKinematic = false;
        _itemInHand.transform.SetParent(null, true);
    }

    private void ThrowAwayItem()
    {
        DropItem();
        _itemInHand.GetComponent<Rigidbody>().AddForce(transform.forward * _forceValue, ForceMode.Impulse);
        _itemInHand = null;
    }
}