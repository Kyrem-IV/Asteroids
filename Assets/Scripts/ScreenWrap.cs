using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScreenWrap : MonoBehaviour
{
    private Rigidbody2D myRigidBody;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // get the screen position of object in pixels
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // get right side of the screen in world units
        float rightSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        float leftSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x;

        float topOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        float bottomOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;

        // if object is moving through left side of the screen, come back out at the right
        if (screenPos.x <= 0 && myRigidBody.linearVelocity.x < 0)
        {
            transform.position = new Vector2(rightSideOfScreenInWorld, transform.position.y);
        }
        // if object is moving through right side of the screen, come back out at the left
        else if (screenPos.x >= Screen.width && myRigidBody.linearVelocity.x > 0)
        {
            transform.position = new Vector2(leftSideOfScreenInWorld, transform.position.y);
        }
        
        // if object is moving through the top of screen, come back out at the bottom
        else if (screenPos.y >= Screen.height && myRigidBody.linearVelocity.y > 0)
        {
            transform.position = new Vector2(transform.position.x, bottomOfScreenInWorld);
        }
        // if object is moving through the bottom of screen, come back out at the top
        else if (screenPos.y <= 0 && myRigidBody.linearVelocity.y < 0)
        {
            transform.position = new Vector2(transform.position.x, topOfScreenInWorld);
        }
    }
}
