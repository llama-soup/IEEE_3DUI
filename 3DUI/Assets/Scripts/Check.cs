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
    // GameObject instructions;
    // GameObject congratulations;
    
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
        // instructions = GameObject.Find("Instructions");
        // congratulations = GameObject.Find("Congratulations");
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

        if(rZero == 270 && rOne == 180 && rTwo == 90 && rThree == 180 && (rFour == 0 || rFour == 180) && rFive == 270 && rSix == 0 && rSeven == 90 && rEight == 90)
        {
            // while(congratulations.transform.position.x <= 1000)
            // {
            //     instructions.transform.Translate(Vector3.right*Time.deltaTime*5000);
            //     congratulations.transform.Translate(Vector3.right*Time.deltaTime*5000);
            // }
        }
    }
}
