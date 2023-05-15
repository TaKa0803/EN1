using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerPrefab; //レベルデザイン
    public GameObject boxPrefab;
    public GameObject goalfab;

    
    public GameObject clearText;
    int[,] map;
    GameObject[,] field;//ゲーム管理用


    void Start()
    {
        map = new int[,]
        {
            {0,0,0,0,0, },
            {0,3,1,3,0, },
            {0,0,2,0,0, },
            {0,2,3,2,0, },
            {0,0,0,0,0, }
        };
        field = new GameObject
            [
            map.GetLength(0),
            map.GetLength(1)
            ];


        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 1)
                {
                    field[y,x] = Instantiate(
                        playerPrefab,
                        new Vector3(x,map.GetLength(0)-y,0),
                        Quaternion.identity
                        );
                }

                if (map[y, x] == 2)
                {
                    field[y, x] = Instantiate(
                        boxPrefab,
                        new Vector3(x, map.GetLength(0)-y, 0),
                        Quaternion.identity
                        );
                }
                if (map[y, x] == 3)
                {
                    field[y, x] = Instantiate(
                        goalfab,
                        new Vector3(x, map.GetLength(0) - y, 0.01f),
                        Quaternion.identity
                        );
                }

            }
        }
    }
    

    bool IsCleard() {
        List<Vector2Int> goals = new List<Vector2Int> ();

        for(int y = 0; y < map.GetLength(0); y++)
        {
            for(int x = 0; x < map.GetLength(1); x++)
            {
                if (map[y, x] == 3)
                {
                    goals.Add(new Vector2Int(x, y));
                }
            }
        }

        for(int i = 0; i < goals.Count; i++)
        {
            GameObject f = field[goals[i].y, goals[i].x];
            if (f == null || f.tag != "Box")
            {
                //一つでもなければ地下送り
                return false;
            }
        }

        return true;


    }

    Vector2Int GetPlayerIndex()
    {
        for(int y = 0; y < field.GetLength(0); y++)
        {
            for (int x = 0; x < field.GetLength(1); x++)
            {
                if (field[y, x] == null) { continue; }
                if (field[y, x].tag == "Player") { return new Vector2Int(x, y); }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool MoveTag(string Tag,Vector2Int movefrom,Vector2Int moveto)
    {
        //配列外参照していないか
        if (moveto.y < 0 || moveto.y >= field.GetLength(0)) { return false; }
        if (moveto.x < 0 || moveto.x >= field.GetLength(1)) { return false; }

        //Boxタグを盛っていたら再起処理
        if (field[moveto.y, moveto.x] != null && field[moveto.y, moveto.x].tag == "Box")
        {
            Vector2Int velocity = moveto - movefrom;
            bool success = MoveTag(Tag, moveto, moveto + velocity);
            if (!success) { return false; }
        }

        //GameObjectでの座標を移動させてからインデックスの入れ替え
        field[movefrom.y, movefrom.x].transform.position =
            new Vector3(moveto.x, field.GetLength(0) - moveto.y, 0);
        field[moveto.y, moveto.x] = field[movefrom.y, movefrom.x];
        field[movefrom.y, movefrom.x] = null;
        return true;
    }


    // Update is called once per frame
    void Update()
    {
        if (IsCleard() == false)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector2Int playerIndex = GetPlayerIndex();
                MoveTag("Player", playerIndex, playerIndex + new Vector2Int(1, 0));
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector2Int playerIndex = GetPlayerIndex();
                MoveTag("Player", playerIndex, playerIndex + new Vector2Int(-1, 0));

            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector2Int playerIndex = GetPlayerIndex();
                MoveTag("Player", playerIndex, playerIndex + new Vector2Int(0, -1));
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector2Int playerIndex = GetPlayerIndex();
                MoveTag("Player", playerIndex, playerIndex + new Vector2Int(0, 1));

            }
        }else
        {
            clearText.SetActive(true);
            Debug.Log("Clear!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
