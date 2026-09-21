using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float speed = 40f;      // Sebesség
    public float turnSpeed = 60f;  // Kanyarodás

    [Header("Effects")]
    [Tooltip("Particle system to play when moving forward")]
    public ParticleSystem forwardFx;

    [Tooltip("Only play FX when vertical input exceeds this value (forward > 0)")]
    public float forwardThreshold = 0.1f;

    [Tooltip("If false, FX will be disabled entirely")]
    public bool useFx = true;

    [Header("Wind")]
    [Tooltip("Reference to WindManger in scene. If null, wind is ignored.")]
    public WindManger windManager;

    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRot;
    private float lastOffShoreTime = -3f;
    private float lastHomeReachedTime = -15f;
    internal bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        startPos = transform.position;
        startRot = transform.rotation;
        if (windManager != null) windManager.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical");      // W / S
        float turn = Input.GetAxis("Horizontal");    // A / D

        // FX kezelése
        if (useFx && forwardFx != null)
        {
            var emission = forwardFx.emission;
            bool shouldPlay = move > forwardThreshold;
            emission.enabled = shouldPlay;

            // Optionally play/stop to ensure correct particle behavior in case emission module doesn't start/stop immediate
            if (shouldPlay && !forwardFx.isPlaying) forwardFx.Play();
            if (!shouldPlay && forwardFx.isPlaying) forwardFx.Stop();
        }
        
        // Wind handling (affects player only)
        Vector3 windDelta = Vector3.zero;
        if (windManager != null)
        {
            windDelta = windManager.ApplyWind(rb, Time.fixedDeltaTime);
        }

        if(!canMove) return;
        // Fordulás
        if (Mathf.Abs(move) > 0.01f)
        {
            // When moving backwards, invert the rotation.
            float turnDirection = move < 0f ? -1f : 1f;
            Vector3 rotation = Vector3.up * turn * turnDirection * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }

        // Haladás
        float currentSpeed = speed;
        if (move < 0) currentSpeed *= 0.5f;
        Vector3 forwardMovement = rb.rotation * Vector3.forward * move * currentSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + forwardMovement + windDelta);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void HandleCollision(GameObject other)
    {
        if (other == null) return;

        if (other.CompareTag("Crash"))
        {
            HandleCrashed();
        }

        if (other.CompareTag("Hartya"))
        {
            HandleHartya(other);
        }

        if (other.CompareTag("EoM"))
        {
            HandleEoMReached();
        }

        if (other.CompareTag("Home"))
        {
            HandleHomeReached();
        }

        if (other.CompareTag("Enemy"))
        {
            HandleEnemyReached();
        }

        if (other.CompareTag("Border"))
        {
            HandleBorderCollision();
        }

        if (other.CompareTag("NWZ"))
        {
            HandleNoWindZoneEnter();
        }
        if (other.CompareTag("EndTrigger"))
        {
            HandleEndTrigger(other);
        }
        if (other.CompareTag("ChestTrigger"))
        {
            HandleChestTrigger(other);
        }
        if (other.CompareTag("Chest"))
        {
            HandleChest();
        }
    }

    private void HandleEoMReached()
    {
        Debug.Log("End of Map reached!");
        transform.Translate(Vector3.back * 0.3f);
        //Turn around with 2f speed
        transform.Rotate(0, 180, 0);
        MonologManager.Instance.EndOfMap();
    }

    private void HandleNoWindZoneEnter()
    {
        if (windManager != null)
        {
            windManager.enabled = false;
            Debug.Log("Entered No Wind Zone: Wind disabled.");
        }
    }
    private void HandleHartya(GameObject other)
    {
        Debug.Log("Hartya collected!");
        Destroy(other);
        if(SoundManager.Instance != null)
            SoundManager.Instance.HartyaSoundSource.Play();
        if(TopBarManager.Instance.Carried == 0 && TopBarManager.Instance.Stored == 0) 
            MonologManager.Instance.PickUp();
        SceneManager.Instance.SpawnedRemaining--;
        QuizManager.Instance.QuizPanelEnabled();
        if(QuizManager.Instance.showQuiz)
            StartCoroutine(WaitForQuizClose());
        if(TopBarManager.Instance.Carried+TopBarManager.Instance.Stored >= 4 && !DBManager.Instance.hasAscended)
            SceneManager.Instance.InitializeAscensionScene();
    }

    private System.Collections.IEnumerator WaitForQuizClose()
    {
        yield return new WaitUntil(() => !QuizManager.Instance.quizPanel.activeInHierarchy);
    }
    private static void HandleCrashed()
    {
        Debug.Log("Crashed!");
        MonologManager.Instance.Crashed(() =>
        {
            TopBarManager.Instance.ResetCarried();
            SceneManager.Instance.ResetPlayer();
        });
    }
    void HandleHomeReached()
    {
        //only run once every 15s
        if (Time.time - lastHomeReachedTime < 15f) return;
        lastHomeReachedTime = Time.time;

        if (TopBarManager.Instance != null)
        {
            // If the player has no cargo, do not store/advance the level and inform the player
            if (TopBarManager.Instance.Carried == 0 && SceneManager.Instance.SpawnedRemaining > 0)
            {
                Debug.Log("You can't return with an epty cargo");
                MonologManager.Instance.EmptyCargo();
                return;
            }
            
            if(TopBarManager.Instance.Stored == 0)
                MonologManager.Instance.FirstCollect();
            else
                MonologManager.Instance.Homecoming();

            TopBarManager.Instance.StoreCarried();
            if(OnlineManager.Instance.IsLoggedIn) OnlineManager.Instance.OnlineSave();
        }
        else
        {
            Debug.LogWarning("TopBarManager not present in scene; cannot store Carried.");
        }
        
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.OnPlayerReachedHome();
        }
        else
        {
            Debug.LogWarning("SceneManager not found; cannot progress level.");
        }

        Debug.Log("Home reached!");
    }
    void HandleEnemyReached()
    {
        Debug.Log("Enemy cached us!");
        MonologManager.Instance.EnemyCrash(() =>
        {
        TopBarManager.Instance.ResetCarried();
        SceneManager.Instance.ResetPlayer();
        });
    }
    void HandleBorderCollision()
    {
        Debug.Log("Border hit!");
        if (Time.time > lastOffShoreTime + 3f)
        {
            MonologManager.Instance.OffShore();
            lastOffShoreTime = Time.time;
        }
        transform.Translate(Vector3.back * 0.3f);
    }
    private void HandleEndTrigger(GameObject other)
    {
        Debug.Log("End Trigger hit!");
        MonologManager.Instance.EndGame();
        Destroy(other);
    }
    private void HandleChestTrigger(GameObject other)
    {
        Debug.Log("Chest Trigger hit!");
        MonologManager.Instance.NearChest();
        Destroy(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NWZ"))
        {
            HandleNoWindZoneExit();
        }
    }
    private void HandleNoWindZoneExit()
    {
        if (windManager != null)
        {
            if(DBManager.Instance != null && DBManager.Instance.Stored > 0)
            {
                windManager.enabled = true;
                windManager.SetRandomWind();
                Debug.Log("Exited No Wind Zone: Wind enabled.");
                if(!DBManager.Instance.firstWind) MonologManager.Instance.WindUp();
            }
        }
    }

    private void HandleChest()
    {
        Debug.Log("Chest found.");
        MonologManager.Instance.ChestCollected();
    }

    public void ResetToStart()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    
}
