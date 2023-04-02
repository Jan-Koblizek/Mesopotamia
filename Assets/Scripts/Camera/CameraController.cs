using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private float movementX = 0.0f;
    private float movementY = 0.0f;
    [Tooltip("Speed of the movement when moving using arrow keys or ASWD")]
    public float speed = 1.0f;
    [Tooltip("Min X position the camera can move to.")]
    public int boundsXmin;
    [Tooltip("Max X position the camera can move to.")]
    public int boundsXmax;
    [Tooltip("Min Y position the camera can move to.")]
    public int boundsYmin;
    [Tooltip("Max Y position the camera can move to.")]
    public int boundsYmax;

    private float startXPos;
    private float startZPos;
    private float dragPointDistance;
    private bool isDragging = false;

    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponent<Camera>();
    }

    private void mouseDown()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hitData))
        {
            startXPos = hitData.point.x;
            startZPos = hitData.point.z;
            dragPointDistance = transform.position.y - hitData.point.y;

            isDragging = true;
        }
    }

    private void mouseUp()
    {
        isDragging = false;
    }

    public void DragObject()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos);
        float changeX = (ray.direction.x * (dragPointDistance / (-ray.direction.y)) + transform.position.x) - startXPos;
        float changeZ = (ray.direction.z * (dragPointDistance / (-ray.direction.y)) + transform.position.z) - startZPos;
        transform.position = new Vector3(transform.position.x - changeX, transform.position.y, transform.position.z - changeZ);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            DragObject();
        }
        if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            mouseDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseUp();
        }
        /*
        Vector2 mousePos = (Input.mousePosition / new Vector2(Screen.width, Screen.height));
        //Debug.Log(mousePos.y);
        if ((movementX >= -10f * speed) && (mousePos.x > 0.98f))
        {
            movementX += 50 * Time.deltaTime * speed + movementX * Time.deltaTime;
        }
        else if ((movementX <= 10f * speed) && (mousePos.x < 0.02f))
        {
            movementX -= 50 * Time.deltaTime * speed - movementX * Time.deltaTime;
        }
        else
        {
            movementX = movementX * 2 * (0.5f - Time.deltaTime);
            if (movementX < 0.0)
            {
                movementX = Mathf.Clamp(movementX + 5.0f, -50.0f * speed, 0.0f);
            }
            else
            {
                movementX = Mathf.Clamp(movementX - 5.0f, 0.0f, 50.0f * speed);
            }
        }

        if ((movementY >= -10f * speed) && (mousePos.y > 0.98f))
        {
            movementY += 50 * Time.deltaTime * speed + movementY * Time.deltaTime;
        }
        else if ((movementY <= 10f * speed) && (mousePos.y < 0.02f))
        {
            movementY -= 50 * Time.deltaTime * speed - movementY * Time.deltaTime;
        }
        else
        {
            movementY = movementY * 2 * (0.5f - Time.deltaTime);
            if (movementY < 0.0)
            {
                movementY = Mathf.Clamp(movementY + 5.0f, -50.0f * speed, 0.0f);
            }
            else
            {
                movementY = Mathf.Clamp(movementY - 5.0f, 0.0f, 50.0f * speed);
            }
        }
        movementX = Mathf.Clamp(movementX, -50.0f * speed, 50.0f * speed);
        movementY = Mathf.Clamp(movementY, -50.0f * speed, 50.0f * speed);
        */
        movementX = movementX * 2 * (0.5f - Time.deltaTime);
        if (movementX < 0.0)
        {
            movementX = Mathf.Clamp(movementX + 2.0f, -50.0f * speed, 0.0f);
        }
        else
        {
            movementX = Mathf.Clamp(movementX - 2.0f, 0.0f, 50.0f * speed);
        }
        movementY = movementY * 2 * (0.5f - Time.deltaTime);
        if (movementY < 0.0)
        {
            movementY = Mathf.Clamp(movementY + 2.0f, -50.0f * speed, 0.0f);
        }
        else
        {
            movementY = Mathf.Clamp(movementY - 2.0f, 0.0f, 50.0f * speed);
        }
        movementX = Mathf.Clamp(movementX, -50.0f * speed, 50.0f * speed);
        movementY = Mathf.Clamp(movementY, -50.0f * speed, 50.0f * speed);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movementX = -30.0f * speed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movementX = 30.0f * speed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movementY = 30.0f * speed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movementY = -30.0f * speed;
        }
        transform.position = transform.position + (new Vector3(0.75f * movementX, 0, 0.75f * movementY)) * Time.deltaTime;
        if (transform.position.x > boundsXmax)
        {
            transform.position = new Vector3(boundsXmax, transform.position.y, transform.position.z);
            movementX = 0;
        }
        if (transform.position.x < boundsXmin)
        {
            transform.position = new Vector3(boundsXmin, transform.position.y, transform.position.z);
            movementX = 0;
        }

        if (transform.position.z > boundsYmax)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, boundsYmax);
            movementY = 0;
        }
        if (transform.position.z < boundsYmin)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, boundsYmin);
            movementY = 0;
        }
    }
}
