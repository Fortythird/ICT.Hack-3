using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Gameplay : MonoBehaviour
{
    public Camera view;

    public GameObject pos11, pos12, pos13, pos14, pos21, pos22, pos23, pos24, pos31, pos32, pos33, pos34;
    private GameObject[,] field = new GameObject[4, 3];

    private bool step = false, pick_card = false, neko = true, onField = false, isDrag = false;
    private sbyte count = 0;
    private short drag_start = 0, drag_finish = 0;

    void FieldView()
    {
        if (!onField)
        {
            view.transform.position = new Vector3(0, 5.1f, 0);
            view.transform.rotation = Quaternion.Euler(90, 0, 0);
            onField = true;
        }
        else 
        {
            onField = false;
            view.transform.position = new Vector3(0, 7.5f, -5f);
            view.transform.rotation = Quaternion.Euler(27, 0, 0);
        }
    }

    void Start()
    {
        field = new GameObject[4, 3] {{pos11, pos21, pos31},
                                      {pos21, pos22, pos23},
                                      {pos31, pos32, pos33},
                                      {pos14, pos24, pos34}}; 
    }

    
    void Update()
    {
        if (step)
        {
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.y > 960f && !isDrag && !onField ||
                Input.GetMouseButtonDown(0) && !isDrag && onField)
            {
                drag_start = (short)Input.mousePosition.y;
                isDrag = true;
            }
            if (Input.GetMouseButtonUp(0) && isDrag)
            {
                drag_finish = (short)Input.mousePosition.y;
                isDrag = false;
                if (drag_start - drag_finish > 300 && !onField || drag_start - drag_finish < -300 && onField) FieldView();
            }
        }
        //else bot_step();
    }
}
