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
    GameObject instructions;
    GameObject congratulations;
    
    // Start is called before the first frame update
    void Start()
    {
        squareZero = GameObject.Find("Cube");
        squareOne = GameObject.Find("Cube (1)");
        squareTwo = GameObject.Find("Cube (2)");
        squareThree = GameObject.Find("Cube (3)");
        squareFour = GameObject.Find("Cube (4)");
        squareFive = GameObject.Find("Cube (5)");
        squareSix = GameObject.Find("Cube (6)");
        squareSeven = GameObject.Find("Cube (7)");
        squareEight = GameObject.Find("Cube (8)");
        instructions = GameObject.Find("Instructions");
        congratulations = GameObject.Find("Congratulations");
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

        if((rZero == 0 || rZero == 180) && rOne == 180 && rTwo == 0 && rThree == 270 && (rFour == 0 || rFour == 180) && rFive == 270 && rSix == 0 && rSeven == 90 && rEight == 90)
        {
            while(congratulations.transform.position.x <= 1000)
            {
                instructions.transform.Translate(Vector3.right*Time.deltaTime*5000);
                congratulations.transform.Translate(Vector3.right*Time.deltaTime*5000);
            }
        }
    }
}
