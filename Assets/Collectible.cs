using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Coin,
        Cash,
        Gem
    }

    public CollectibleType collectibleType;

    [Header("Audio")]
    public AudioClip pickupSound;

    [Header("Hover")]
    public float hoverAmplitude = 0.2f; // height
    public float hoverSpeed = 2f;       // speed

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (LevelManager.Instance == null)
            return;

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        switch (collectibleType)
        {
            case CollectibleType.Coin:
                LevelManager.Instance.CollectCoin();
                break;

            case CollectibleType.Cash:
                LevelManager.Instance.CollectCash();
                break;

            case CollectibleType.Gem:
                LevelManager.Instance.CollectGem();
                break;
        }

        Destroy(gameObject);
    }
}