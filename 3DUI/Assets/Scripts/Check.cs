using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script checks over all thirteen wires in the wire puzzle to see if they have been rotated into their
 * correct positions - thus solving the puzzle. If so, it will inform the players they can communicate each
 * other and open up a portal for them to do so.
 *
 * Author: Alexander Li
 */
public class Check : MonoBehaviour
{
    // Wires in the circuit that can be rotated
    GameObject squareZero;
    GameObject squareOne;
    GameObject squareTwo;
    GameObject squareThree;
    GameObject squareFour;
    GameObject squareFive;
    GameObject squareSix;
    GameObject squareSeven;
    GameObject squareEight;
    GameObject squareNine;
    GameObject squareTen;
    GameObject squareEleven;
    GameObject squareTwelve;

    // Instructions for what the players should do to solve the puzzle
    GameObject instructions;

    // Message telling the player when they've completed the puzzle
    GameObject complete;

    // The two portals that open upon the circuit's completion
    public GameObject presentWirePortal;
    public GameObject futureWirePortal;
    
    // Start is called before the first frame update
    void Start()
    {
        squareZero = GameObject.Find("Cube (00)");
        squareOne = GameObject.Find("Cube (01)");
        squareTwo = GameObject.Find("Cube (02)");
        squareThree = GameObject.Find("Cube (03)");
        squareFour = GameObject.Find("Cube (04)");
        squareFive = GameObject.Find("Cube (05)");
        squareSix = GameObject.Find("Cube (06)");
        squareSeven = GameObject.Find("Cube (07)");
        squareEight = GameObject.Find("Cube (08)");
        squareNine = GameObject.Find("Cube (09)");
        squareTen = GameObject.Find("Cube (10)");
        squareEleven = GameObject.Find("Cube (11)");
        squareTwelve = GameObject.Find("Cube (12)");
        instructions = GameObject.Find("Instructions");
        complete = GameObject.Find("Complete");
        instructions.SetActive(true);
        complete.SetActive(false);
    }

    // Update is called once per frame and checks the rotation of all thirteen wires to see if they have
    // all been rotated the correct number of degrees. If so, the instructions for solving the puzzle
    // will be replaced by a message informing the player they can now communicate with the other player
    // via portal.
    void Update()
    {
        // The rotation of the wires
        float rZero = squareZero.transform.eulerAngles.z;
        float rOne = squareOne.transform.eulerAngles.z;
        float rTwo = squareTwo.transform.eulerAngles.z;
        float rThree = squareThree.transform.eulerAngles.z;
        float rFour = squareFour.transform.eulerAngles.z;
        float rFive = squareFive.transform.eulerAngles.z;
        float rSix = squareSix.transform.eulerAngles.z;
        float rSeven = squareSeven.transform.eulerAngles.z;
        float rEight = squareEight.transform.eulerAngles.z;
        float rNine = squareNine.transform.eulerAngles.z;
        float rTen = squareTen.transform.eulerAngles.z;
        float rEleven = squareEleven.transform.eulerAngles.z;
        float rTwelve = squareTwelve.transform.eulerAngles.z;

        if(is270(rZero) && is180(rOne) && is90(rTwo) && is180(rThree) && (is270(rFour) || is90(rFour))
            && is0(rFive) && is270(rSix) && (is0(rSeven) || is180(rSeven)) && is270(rEight)
            && is0(rNine) && is90(rTen) && is90(rEleven) && is90(rTwelve))
        {
            complete.SetActive(true);
            instructions.SetActive(false);
            presentWirePortal.gameObject.SetActive(true);
            futureWirePortal.gameObject.SetActive(true);
        }
    }

    // Helper functions for checking if the wires have been rotated the correct number of degrees.
    // Checks if the given rotation is 270 degrees, or is 270 degrees plus any number of 360 degree rotations in either direction.
    bool is270(float angle)
    {
        if ((angle - 270) % 360 == 0)
        {
            return true;
        }
        return false;
    }

    // Checks if the given rotation is 0 degrees, or is 0 degrees plus any number of 360 degree rotations in either direction.
    bool is0(float angle)
    {
        if ((angle > 0 && angle < 0.01) || (angle % 360 == 0))
        {
            return true;
        }
        return false;
    }

    // Checks if the given rotation is 180 degrees, or is 180 degrees plus any number of 360 degree rotations in either direction.
    bool is180(float angle)
    {
        if ((angle - 180) % 360 == 0)
        {
            return true;
        }
        return false;
    }

    // Checks if the given rotation is 90 degrees, or is 90 degrees plus any number of 360 degree rotations in either direction.
    bool is90(float angle)
    {
        if ((angle - 90) % 360 == 0)
        {
            return true;
        }
        return false;
    }
}
