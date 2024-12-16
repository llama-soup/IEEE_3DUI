using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
 // Rigidbody of the player.
 private Rigidbody rb; 
 private int count;


 // Movement along X and Y axes.
 private float movementX;
 private float movementY;
 private bool timerStopped = true;

 // Speed at which the player moves.
 public float speed = 0; 


 public Button restartButton;
 public TextMeshProUGUI countText;
 public TextMeshProUGUI envFacts;

[SerializeField] TextMeshProUGUI countdownText;
[SerializeField] float remainingTime;

 public string[] EnvironmentalFacts = {
        "Recycling one ton of paper saves 17 trees.",
        "Plastic takes up to 1000 years to decompose.",
        "Oceans produce at least 50% of the Earth’s oxygen.",
        "Deforestation contributes about 12% to 20% of global greenhouse gas emissions.",
        "Around 1 million species are at risk of extinction due to climate change.",
        "Another fact here",
        "Yet Another",
        "Anotha one",
        "Yes",
        "Climate Change is bad",
        "Another"
    };

 // Start is called before the first frame update.
 void Start()
    {
 // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();
        count =0;
        SetCountText();
        restartButton.gameObject.SetActive(false);
        countText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        timerStopped = true;
        remainingTime = 120;
    }     
 
 // This function is called when a move input is detected.
 void OnMove(InputValue movementValue)
    {
 // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();

 // Store the X and Y components of the movement.
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

     public void OnDoorTrigger(XRBaseInteractor interactor)
    {
        Debug.Log("Door interacted with! Timer started.");
        timerStopped = false; // Start the timer
        countText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);
    }

    void SetCountText() 
   {
       countText.text =  "Newspaper Count: " + count.ToString() + "/11";
   }

   void setEnvironmentalFact()
   {
      envFacts.text = EnvironmentalFacts[count-1];
   }

 // FixedUpdate is called once per fixed frame-rate frame.
 private void FixedUpdate() 
    {
 // Create a 3D movement vector using the X and Y inputs.
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);

 // Apply force to the Rigidbody to move the player.
        rb.AddForce(movement * speed); 

      if(!timerStopped)
      {
            if(remainingTime > 0)
         {
            remainingTime -= Time.deltaTime;
         }
         else if(remainingTime<0)
         {
            remainingTime = 0;
            envFacts.text = "You failed! Click to restart";
            restartButton.gameObject.SetActive(true);
            timerStopped = true;
         }
         int minutes = Mathf.FloorToInt(remainingTime/60);
         int seconds = Mathf.FloorToInt(remainingTime%60);
         countdownText.text = string.Format("{0:00}:{1:00}",minutes,seconds); 
      }

      if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("DoorTrigger")) // Check if the clicked object has the "DoorTrigger" tag
                {
                    Debug.Log("Door clicked! Timer started.");
                    timerStopped = false; // Start the timer
                    countText.gameObject.SetActive(true);
                     countdownText.gameObject.SetActive(true);
                }
            }
        }
   }
      
    

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("PickUp"))
      {
         other.gameObject.SetActive(false);
         count = count + 1;
         SetCountText();
         setEnvironmentalFact();

         if (count == 11)
         {
               timerStopped = true; // Stop the timer
               StartCoroutine(ShowCongratsMessageWithDelay());
         }
      }

   }

   public void RestartLevel()
{
    timerStopped = true; // Ensure the timer is stopped before restarting
    remainingTime = 120; // Reset the timer to its initial value
    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
}


   private IEnumerator ShowCongratsMessageWithDelay()
   {
      yield return new WaitForSeconds(5); // Wait for 5 seconds
      envFacts.text = "Congratulations! You've collected all the environmental stories!";
   }
   
}