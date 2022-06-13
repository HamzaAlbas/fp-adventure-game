using UnityEngine;

public class HeadBob : MonoBehaviour
{
    #region VARIABLES
    
    public float walkingBobbingSpeed = 14f;
    public float bobbingAmount = 0.05f;
    public PlayerController controller;

    private float _defaultPosY;
    private float _timer;
    
    #endregion
    
    private void Start()
    {
        _defaultPosY = transform.localPosition.y;
    }

    private void Update()
    {
        if(Mathf.Abs(controller.moveDirection.x) > 0.1f || Mathf.Abs(controller.moveDirection.z) > 0.1f)
        {
            // Player is moving
            _timer += Time.deltaTime * walkingBobbingSpeed;
            var localPosition = transform.localPosition;
            localPosition = new Vector3(localPosition.x, _defaultPosY + Mathf.Sin(_timer) * bobbingAmount, localPosition.z);
            transform.localPosition = localPosition;
        }
        else
        {
            // Idle
            _timer = 0;
            var localPosition = transform.localPosition;
            localPosition = new Vector3(localPosition.x, Mathf.Lerp(localPosition.y, _defaultPosY, Time.deltaTime * walkingBobbingSpeed), localPosition.z);
            transform.localPosition = localPosition;
        }
    }
}
