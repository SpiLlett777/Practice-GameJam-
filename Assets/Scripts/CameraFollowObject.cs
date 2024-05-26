using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PlayerMovement _player;
    private bool _IsFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PlayerMovement>();

        if (_player.transform.rotation.y == 0)
        {
            _IsFacingRight = true;
        }
        else if (_player.transform.rotation.y == 180)
        {
            _IsFacingRight = false;
        }
    }
    private void Update()
    {
        transform.position = _player.transform.position;
    }
    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / _flipRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }
    private float DetermineEndRotation()
    {
        _IsFacingRight = !_IsFacingRight;
        if (_IsFacingRight)
        {
            return -180f;
        }
        else
        {
            return 0f;
        }
    }
}
