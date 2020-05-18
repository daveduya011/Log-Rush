using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public float speed;
    public int numOfSprites;
    private float cameraPositionY;
    private CameraController cameraController;
    private Parallax[] parallaxes;
    private Vector3 lastCameraPos;
    private float sizeY;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        parallaxes = new Parallax[numOfSprites];
        parallaxes[0] = GetComponentInChildren<Parallax>();

        Vector3 pos = parallaxes[0].transform.position;

        sizeY = parallaxes[0].GetComponent<SpriteRenderer>().bounds.size.y;
        
        for (int i = 0; i < numOfSprites - 1; i++) {
            float n = i % 2 == 0 ? i : -i;
            parallaxes[i+1] = Instantiate(parallaxes[0], new Vector3(pos.x, pos.y - sizeY * n, pos.z), Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Create parallax effect if graphics is not set to low
        float moveX = lastCameraPos.y - cameraController.transform.position.y;
        transform.position += Vector3.up * moveX * speed;

        cameraPositionY = cameraController.transform.position.y;

        lastCameraPos = cameraController.transform.position;
        if (numOfSprites <= 1)
            return;

        for (int i = 0; i < parallaxes.Length; i++) {
            float pos = parallaxes[i].transform.position.y;

            parallaxes[i].isCameraInPosition = cameraPositionY > pos - (sizeY / 2)
                && cameraPositionY < pos + (sizeY / 2);

            if (parallaxes[i].isCameraInPosition) {
                if (!parallaxes[i].isLastVisited) {
                    Vector3 currentpos = parallaxes[i].transform.position;
                    parallaxes[i].isLastVisited = true;

                    parallaxes[(i + 1) % numOfSprites].transform.position = currentpos + (Vector3.up * sizeY);
                }
            } else {
                parallaxes[i].isLastVisited = false;
            }
        }

    }

}
