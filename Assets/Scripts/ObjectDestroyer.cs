using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField]
    private Color blueColor = Color.blue;
    [SerializeField]
    private Color redColor = Color.red;

    [Header("References")]
    [SerializeField]
    private DrawingOnPanel drawingOnPanelScript;
    [SerializeField]
    private AudioSource backgroundMusicSource;

    private void Awake()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (drawingOnPanelScript == null)
        {
            drawingOnPanelScript = FindObjectOfType<DrawingOnPanel>();
            if (drawingOnPanelScript == null)
            {
                Debug.LogError("DrawingOnPanel script not found in the scene!");
            }
        }

        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = FindObjectOfType<AudioSource>();
            if (backgroundMusicSource == null)
            {
                Debug.LogError("Background music AudioSource not found in the scene!");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            HandleBlockCollision(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            HandleFinishCollision();
        }
        else if (collision.gameObject.CompareTag("AntiBlock"))
        {
            HandleAntiBlockCollision(collision);
        }
    }

    private void HandleBlockCollision(GameObject block)
    {
        if (drawingOnPanelScript != null)
        {
            drawingOnPanelScript.countCopy--;
            if (drawingOnPanelScript.countCopy == 0 && backgroundMusicSource != null)
            {
                backgroundMusicSource.Stop();
            }
        }

        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = blueColor;
        }

        AudioClip blockSound = Resources.Load<AudioClip>("Music/Block");
        if (blockSound != null)
        {
            AudioSource.PlayClipAtPoint(blockSound, transform.position);
        }
        else
        {
            Debug.LogError("Audio clip 'Block' not found in Resources/Music!");
        }

        Destroy(gameObject);
    }

    private void HandleFinishCollision()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Renderer blockRenderer = block.GetComponent<Renderer>();
            if (blockRenderer != null)
            {
                blockRenderer.material.color = redColor;
            }
        }
    }

    private void HandleAntiBlockCollision(Collision collision)
    {
        if (drawingOnPanelScript != null)
        {
            drawingOnPanelScript.countCopy++;
        }

        Destroy(collision.gameObject);
    }
}
