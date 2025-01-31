using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using UnityEngine.EventSystems;

public class NewMenuController : MonoBehaviour, ThirdPersonInputs.IUIActions
{
    #region Variables
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    private int selectedIndex = 0;
    private Button[] buttons;
    private ThirdPersonInputs inputs;

    private bool isUsingController = false; // Tracks input method
    private float lastInputTime = 0f;
    private float inputCooldown = 0.2f; // Prevents rapid selection
    private int lastDirection = 0; // -1 = Up, 1 = Down, 0 = Neutral
    #endregion

    #region Initialization
    void Awake()
    {
        inputs = new ThirdPersonInputs();
        inputs.Enable();
    }

    private void OnEnable()
    {
        inputs.Enable();
        inputs.UI.SetCallbacks(this);
    }

    private void OnDisable()
    {
        inputs.Disable();
        inputs.UI.RemoveCallbacks(this);
    }

    void Start()
    {
        buttons = new Button[] { startButton, settingsButton, quitButton };
        HandleSelectButton(0);
    }
    #endregion

    #region Navigation Handling
    void HandleNavigate(Vector2 direction)
    {
        if (!isUsingController)
        {
            isUsingController = true; // Controller is now active
            EventSystem.current.SetSelectedGameObject(null); // Prevents auto-selection from mouse
        }

        if (Time.time - lastInputTime < inputCooldown) return;

        int previousIndex = selectedIndex;

        if (direction.y > 0 && lastDirection != -1)
        {
            selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, buttons.Length - 1);
            lastDirection = -1;
        }
        else if (direction.y < 0 && lastDirection != 1)
        {
            selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, buttons.Length - 1);
            lastDirection = 1;
        }

        if (direction.y == 0)
        {
            lastDirection = 0;
        }

        if (previousIndex != selectedIndex)
        {
            lastInputTime = Time.time;
            HandleSelectButton(selectedIndex);
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        HandleNavigate(context.ReadValue<Vector2>());
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        HandleSelect();
    }

    public void OnResume(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Selection Handling
    void HandleSelect()
    {
        buttons[selectedIndex].onClick.Invoke();
    }

    void HandleSelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject); // Forces selection
        buttons[index].Select();
    }
    #endregion

    #region UI Interactions
    public void OnMousePos(InputAction.CallbackContext context)
    {
        HandleMousePosition(context.ReadValue<Vector2>());
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        HandleMouseClick();
    }

    private void OnMouseHover(Button hoveredButton)
    {
        if (isUsingController) return; // Ignore hover if controller is in use

        int hoveredIndex = System.Array.IndexOf(buttons, hoveredButton);
        if (hoveredIndex != selectedIndex)
        {
            selectedIndex = hoveredIndex;
            HandleSelectButton(selectedIndex);
        }
    }

    private void HandleMousePosition(Vector2 mousePosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var result in raycastResults)
        {
            Button hoveredButton = result.gameObject.GetComponent<Button>();
            if (hoveredButton != null)
            {
                OnMouseHover(hoveredButton);
                break;
            }
        }
    }

    private void HandleMouseClick()
    {
        isUsingController = false; // Mouse is now active

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (var result in raycastResults)
        {
            Button clickedButton = result.gameObject.GetComponent<Button>();
            if (clickedButton != null)
            {
                OnMouseClick(clickedButton);
                break;
            }
        }
    }

    private void OnMouseClick(Button clickedButton)
    {
        isUsingController = false; // Mouse is now active

        clickedButton.onClick.Invoke();
    }
    #endregion

    #region Selected Functions
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        Debug.Log("Settings Opened!");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion
}