using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Define a class to match the JSON structure
[System.Serializable]
public class GPIOResponse
{
    public int forward;
    public int backward;
}

public class PlayerController : MonoBehaviour
{
    public GameObject targetObject; // Reference to the target object
    public float speed = 10.0f; // Speed for movement
    private int forwardValue = 1; // Initial value to ensure no movement
    private int backwardValue = 1; // Initial value to ensure no movement

    void Start()
    {
        StartCoroutine(CheckGPIOContinuously()); // Start the CheckGPIOContinuously coroutine
    }

    void Update()
    {
        MovePlayer(forwardValue, backwardValue); // Call the MovePlayer method every frame to move the object
    }

    IEnumerator CheckGPIOContinuously()
    {
        while (true) // Run this coroutine continuously
        {
            yield return StartCoroutine(CheckGPIO()); // Start the CheckGPIO coroutine and wait for it to complete
            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before checking again
        }
    }

    IEnumerator CheckGPIO()
    {
        string url = "http://192.168.137.60:5000/gpio"; // Replace with your Flask server URL
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest(); // Send the HTTP GET request and wait for the response
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Request error: {request.error}"); // Log any request errors
                yield break; // Exit the coroutine if there's an error
            }

            // Parse the response
            string responseBody = request.downloadHandler.text;
            Debug.Log($"Response: {responseBody}");

            // Assuming the response is in JSON format
            GPIOResponse json = JsonUtility.FromJson<GPIOResponse>(responseBody);
            forwardValue = json.forward;
            backwardValue = json.backward;

            // Debug the values received
            Debug.Log($"Forward Value: {forwardValue}, Backward Value: {backwardValue}");
        }
    }

    void MovePlayer(int forwardValue, int backwardValue)
    {
        if (targetObject != null) // Check if a target object is assigned
        {
            if (forwardValue == 0 && backwardValue == 1)
            {
                targetObject.transform.Translate(Vector3.forward * speed * Time.deltaTime); // Move the object forward
            }
            else if (forwardValue == 1 && backwardValue == 0)
            {
                targetObject.transform.Translate(Vector3.back * speed * Time.deltaTime); // Move the object backward
            }
        }
        else
        {
            Debug.LogError("Target object is not assigned."); // Log an error if no target object is assigned
        }
    }
}