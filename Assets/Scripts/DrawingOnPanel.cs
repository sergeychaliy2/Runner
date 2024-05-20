using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using Dreamteck.Splines;

public class DrawingOnPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Settings")]
    public RectTransform panelRectTransform;
    public GameObject brush;
    public GameObject sphere;
    public GameObject replacementModel;
    public GameObject warningObject;
    public TextMeshProUGUI countCopyText; 
    public GameObject primeGamer; 

    private RectTransform panelRect;
    private bool isDrawing = false;
    private List<GameObject> drawnObjects = new List<GameObject>();
    private List<GameObject> copiedObjects = new List<GameObject>();

    private float scaleRatio = 0.1f;
    private Vector3 reducedScale = new Vector3(0.8f, 0.8f, 0.8f);
    private float drawDistanceThreshold = 80f;
    private Vector2 lastDrawPosition;

    [Space]
    [SerializeField]
    private int _countCopy = 20;

    public int countCopy
    {
        get { return _countCopy; }
        set
        {
            _countCopy = value;
            UpdateCountCopyText();

            if (_countCopy <= 0)
            {
                StopAllAnimations();
                if (warningObject != null)
                {
                    warningObject.SetActive(true);
                }
            }
            else
            {
                if (warningObject != null)
                {
                    warningObject.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        panelRect = panelRectTransform;
        UpdateCountCopyText();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_countCopy <= 0)
        {
            return;
        }

        ClearDrawnObjects();
        ClearCopiedObjects();
        if (IsPointerInsidePanel(eventData))
        {
            isDrawing = true;
            lastDrawPosition = eventData.position;
            Draw(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_countCopy <= 0)
        {
            return;
        }

        if (isDrawing)
        {
            if (IsPointerInsidePanel(eventData))
            {
                float distance = Vector2.Distance(eventData.position, lastDrawPosition);
                if (distance >= drawDistanceThreshold)
                {
                    lastDrawPosition = eventData.position;
                    Draw(eventData);
                }
            }
            else
            {
                isDrawing = false;
                ClearDrawnObjects();
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_countCopy <= 0)
        {
            return;
        }

        isDrawing = false;
        CreateAndMoveCopiesInFrontOfSphere();
        ClearDrawnObjects();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_countCopy <= 0)
        {
            return;
        }

        if (isDrawing)
        {
            isDrawing = false;
            ClearDrawnObjects();
        }
    }

    private bool IsPointerInsidePanel(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, eventData.position, eventData.pressEventCamera, out localPoint);
        return panelRect.rect.Contains(localPoint);
    }

    private void Draw(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, eventData.position, eventData.pressEventCamera, out localPoint);

        GameObject newBrush = Instantiate(brush, panelRect);
        newBrush.transform.localPosition = localPoint;

        newBrush.transform.localScale = new Vector3(10, 10, 10);

        drawnObjects.Add(newBrush);
    }

    private void CreateAndMoveCopiesInFrontOfSphere()
    {
        if (drawnObjects.Count == 0)
            return;

        Vector3 spherePosition = sphere.transform.position;
        float offset = 2.0f;
        Vector3 forwardDirection = sphere.transform.forward;
        Vector3 rightDirection = sphere.transform.right;

        for (int i = 0; i < drawnObjects.Count && i / 2 < countCopy; i += 2)
        {
            GameObject brushObject = drawnObjects[i];
            GameObject brushCopy = Instantiate(replacementModel);
            Vector3 localPosition = brushObject.transform.localPosition;

            Vector3 scaledPosition = new Vector3(localPosition.x * scaleRatio, 0, localPosition.y * scaleRatio);

            Vector3 adjustedPosition = rightDirection * scaledPosition.x + forwardDirection * scaledPosition.z;

            brushCopy.transform.position = spherePosition + forwardDirection * offset + adjustedPosition;

            brushCopy.transform.localScale = reducedScale;

            brushCopy.transform.SetParent(sphere.transform);

            copiedObjects.Add(brushCopy);
        }
    }

    private void UpdateCountCopyText()
    {
        if (countCopyText != null)
        {
            countCopyText.text = countCopy.ToString();
        }
    }

    private void ClearDrawnObjects()
    {
        foreach (GameObject obj in drawnObjects)
        {
            Destroy(obj);
        }
        drawnObjects.Clear();
    }

    private void ClearCopiedObjects()
    {
        foreach (GameObject obj in copiedObjects)
        {
            Destroy(obj);
        }
        copiedObjects.Clear();
    }

    private void StopAllAnimations()
    {
        if (primeGamer != null)
        {
            Animator animator = primeGamer.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }

            SplineFollower splineFollower = primeGamer.GetComponent<SplineFollower>();
            if (splineFollower != null)
            {
                splineFollower.enabled = false;
            }
        }
    }
}
