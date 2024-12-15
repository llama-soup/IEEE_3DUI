using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
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
    GameObject instructions;
    GameObject complete;
    public GameObject presentWirePortal;
    public GameObject futureWirePortal;
    // bool change = true;
    // int index = 0;
    
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
        complete.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
            Debug.Log("Completed puzzle");
            // instructions.transform.Translate(0,0,0.1499986f * 0.05f);
            // complete.transform.Translate(0,0,-0.1499986f * 0.05f);
            complete.SetActive(true);
            instructions.SetActive(false);
            // change = false;
            presentWirePortal.gameObject.SetActive(true);
            futureWirePortal.gameObject.SetActive(true);
        }

        //Debugging code:
        if(is270(rZero) && is180(rOne))
        {
            Debug.Log("squareZero and rOne");
        }
    }

    // Helper functions for calculating angle
    bool is270(float angle)
    {
        if ((angle - 270) % 360 == 0)
        {
            return true;
        }
        return false;
    }

    bool is0(float angle)
    {
        if ((angle > 0 && angle < 0.01) || (angle % 360 == 0))
        {
            return true;
        }
        return false;
    }

    bool is180(float angle)
    {
        if ((angle - 180) % 360 == 0)
        {
            return true;
        }
        return false;
    }

    bool is90(float angle)
    {
        if ((angle - 90) % 360 == 0)
        {
            return true;
        }
        return false;
    }

}
