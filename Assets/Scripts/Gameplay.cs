using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Gameplay : MonoBehaviour
{
    public Camera view;

    public GameObject pos11, pos12, pos13, pos14, pos21, pos22, pos23, pos24, pos31, pos32, pos33, pos34,
                      prefCrane, prefKoi, prefDragon, prefEmpty;
    private GameObject[,] field = new GameObject[3, 4];
    private GameObject choosenCard, choosenPos;
    private List<GameObject> avail_cards = new List<GameObject>();
    private List<GameObject> enemy_cards = new List<GameObject>();
    private List<GameObject> types = new List<GameObject>();

    private bool step = true, pick_card = false, neko = true, onField = false, isDrag = false, isEnd = false;
    private short drag_start = 0, drag_finish = 0, cards_to_play = 2;
    private int count = 0, score = 100;

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

    void CardPlace(GameObject card, GameObject place)
    {
        place.GetComponent<Renderer>().sharedMaterial = card.GetComponent<Renderer>().sharedMaterial;
        place.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = card.gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text;
        place.gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().text = card.gameObject.transform.GetChild(1).GetComponent<TextMeshPro>().text;
        place.gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text = card.gameObject.transform.GetChild(2).GetComponent<TextMeshPro>().text;
    }

    void CardHold()
    {
        int mod = avail_cards.Count % 2;
        int div = avail_cards.Count / 2;
        List<float> x = new List<float>();
        Vector3 pos = new Vector3(0, 5.27f, -3.83f);
        if (mod == 1)
        {
            x.Add(0);
            for (int i = 1; i <= div; i++)
            {
                x.Add(0.2f * i);
                x.Add(-0.2f * i);
            }
        }
        else for (int i = 0; i < div; i++)
            {
                x.Add(0.1f + 0.2f * i);
                x.Add(-0.1f - 0.2f * i);
            }
        x.Sort((p1, p2) => p1.CompareTo(p2));
        for (int i = 0; i < avail_cards.Count; i++)
        {
            pos.x = x[i];
            GameObject clone = Instantiate(avail_cards[i], pos, Quaternion.Euler(45, 180, 0));
            clone.transform.localPosition += new Vector3(0, -0.01f*(avail_cards.Count - i - 1), 0);
            avail_cards[i] = clone;
        }
    }

    void EnemyStep()
    {
        bool isEmpty = true;
        short rand = (short)Random.Range(0, types.Count);
        enemy_cards.Add(types[rand]);
        for (int i = 0; i < 4; i++)
        {
            isEmpty = true;
            for (int j = 0; j < 3; j++) if (field[j, i].GetComponent<Renderer>().sharedMaterial != prefEmpty.GetComponent<Renderer>().sharedMaterial) isEmpty = false;
            if (isEmpty)
            {
                rand = (short)Random.Range(0, enemy_cards.Count);
                CardPlace(enemy_cards[rand], field[0, i]);
                enemy_cards.Remove(enemy_cards[rand]);
                break;
            }
        }
        if (!isEmpty) for (int i = 0; i < 4; i++)
                if (field[0, i].GetComponent<Renderer>().sharedMaterial == prefEmpty.GetComponent<Renderer>().sharedMaterial &&
                    field[1, i].GetComponent<Renderer>().sharedMaterial == prefEmpty.GetComponent<Renderer>().sharedMaterial)
                {
                    rand = (short)Random.Range(0, enemy_cards.Count);
                    CardPlace(enemy_cards[rand], field[0, i]);
                    enemy_cards.Remove(enemy_cards[rand]);
                    isEmpty = true;
                    break;
                }
        if (!isEmpty) for (int i = 0; i < 4; i++)
                if (field[0, i].GetComponent<Renderer>().sharedMaterial == prefEmpty.GetComponent<Renderer>().sharedMaterial)
                {
                    rand = (short)Random.Range(0, enemy_cards.Count);
                    CardPlace(enemy_cards[rand], field[0, i]);
                    enemy_cards.Remove(enemy_cards[rand]);
                    break;
                }
        step = true;
    }

    void CheckForEnd()
    {
        if (count >= 5)
        {
            score = score + count - 5;
            Debug.Log("Victory!");
            Debug.Log(score);
            isEnd = true;
        }
        if (count <= -5)
        {
            score = -50;
            Debug.Log("Defeat!");
            Debug.Log(score);
            isEnd = true;
        }
    }

    void Action()
    {
        for (int i = 0; i < 4; i++)
        {
            if (field[2, i].GetComponent<Renderer>().sharedMaterial != prefEmpty.GetComponent<Renderer>().sharedMaterial)
            {
                bool isOpened = true;
                int j;
                for (j = 1; j >= 0; j--) if (field[j, i].GetComponent<Renderer>().sharedMaterial != prefEmpty.GetComponent<Renderer>().sharedMaterial)
                    {
                        isOpened = false;
                        break;
                    }
                if (isOpened) count += int.Parse(field[2, i].transform.GetChild(0).GetComponent<TextMeshPro>().text);
                else if (j == 1)
                {
                    int diff = int.Parse(field[1, i].transform.GetChild(1).GetComponent<TextMeshPro>().text) - int.Parse(field[2, i].transform.GetChild(0).GetComponent<TextMeshPro>().text);
                    if (diff <= 0) CardPlace(prefEmpty, field[1, i]);
                    else field[1, i].transform.GetChild(1).GetComponent<TextMeshPro>().text = diff.ToString();
                }
            }
        }
        CheckForEnd();
        if (!isEnd)
        {
            for (int i = 1; i >= 0; i--)
            {
                if (i == 1) for (int j = 0; j < 4; j++) if (field[i, j].GetComponent<Renderer>().sharedMaterial != prefEmpty.GetComponent<Renderer>().sharedMaterial)
                            if (field[2, j].GetComponent<Renderer>().sharedMaterial == prefEmpty.GetComponent<Renderer>().sharedMaterial)
                                count -= int.Parse(field[i, j].transform.GetChild(0).GetComponent<TextMeshPro>().text);
                            else
                            {
                                int diff = int.Parse(field[2, j].transform.GetChild(1).GetComponent<TextMeshPro>().text) - int.Parse(field[i, j].transform.GetChild(0).GetComponent<TextMeshPro>().text);
                                if (diff <= 0) CardPlace(prefEmpty, field[2, j]);
                                else field[2, j].transform.GetChild(1).GetComponent<TextMeshPro>().text = diff.ToString();
                            }
                if (i == 0) for (int j = 0; j < 4; j++) if (field[i, j].GetComponent<Renderer>().sharedMaterial != prefEmpty.GetComponent<Renderer>().sharedMaterial &&
                            field[1, j].GetComponent<Renderer>().sharedMaterial == prefEmpty.GetComponent<Renderer>().sharedMaterial)
                        {
                            CardPlace(field[i, j], field[1, j]);
                            CardPlace(prefEmpty, field[i, j]);
                        }
            }
            CheckForEnd();
        }
    }

    void Start()
    {
        short rand;
        field = new GameObject[3, 4] {{pos11, pos12, pos13, pos14},
                                      {pos21, pos22, pos23, pos24},
                                      {pos31, pos32, pos33, pos34}};
        types.Add(prefCrane);
        types.Add(prefKoi);
        types.Add(prefDragon);
        for (int i = 0; i < cards_to_play; i++)
        {
            rand = (short)Random.Range(0, types.Count);
            avail_cards.Add(types[rand]);
        }
        for (int i = 0; i < 3; i++)
        {
            rand = (short)Random.Range(0, types.Count);
            enemy_cards.Add(types[rand]);
        }
        rand = (short)Random.Range(0, 4);
        for (int i = 0; i < 4; i++) if (i != rand) CardPlace(enemy_cards[Random.Range(0, enemy_cards.Count)], field[0, i]);
        CardHold();
    }

    void Update()
    {
        if (!isEnd)
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
                if (Input.GetMouseButtonUp(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 3) && onField)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (hit.transform.gameObject == field[2, i])
                            {
                                choosenPos = hit.transform.gameObject;
                                CardPlace(choosenCard, choosenPos);
                                for (int j = 0; j < avail_cards.Count; j++) Destroy(avail_cards[j]);
                                avail_cards.Remove(choosenCard);
                                CardHold();
                                FieldView();
                            }
                        }
                    }
                    if (Physics.Raycast(ray, out hit, 5) && avail_cards.Contains(hit.transform.gameObject) && !onField)
                    {
                        choosenCard = hit.transform.gameObject;
                        FieldView();
                    }
                }
                if (Input.GetKeyDown("left shift") && pick_card && !onField)
                {
                    score--;
                    for (int j = 0; j < avail_cards.Count; j++) Destroy(avail_cards[j]);
                    short rand = (short)Random.Range(0, 3f);
                    avail_cards.Add(types[rand]);
                    CardHold();
                    pick_card = false;
                }
                if (Input.GetKeyDown("space") && !onField)
                {
                    score--;
                    choosenCard = null;
                    choosenPos = null;
                    pick_card = true;
                    step = false;
                    Action();
                }
            }
            else EnemyStep();
        }
    }
}
