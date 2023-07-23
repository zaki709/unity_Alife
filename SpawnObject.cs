using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameObjectに対して本CSファイルをアペンドしてください

public class SpawnObject: MonoBehaviour{

    //プライベート変数
    private int numDir = 16; //キューブが横方向に広がる次元数
    private int height = 15; //キューブの縦方向の数
    private int radius = 7; //キューブの半径における個数
    private float scale = 0.25f;//キューブの大きさ
    private int indexOfCubes;


    //パブリック変数
    public GameObject cubeParent;
    public GameObject centerPoint;
    public GameObject circleStartPoint;
    public GameObject ParentPos;
    public GameObject Main;
    public GameObject Scren;

    public GameObject[] cube;
    public Vector3[] cubeVectors;//各キューブと、コアキューブとの位置関係をベクトルにして保存
    public Vector3 coreVector;//コアキューブのベクトル 常に(0,0,0)
    public Vector3 vec;//コアキューブ以外のキューブが平行移動するときのベクトルを保持するもの
    public Vector3 adjectiveCorePos;//コアキューブ core[0]の移動前のポジション
    public Vector3 currentCorePos;//コアキューブ core[0]の移動後のポジション

    public int sumOfCubes;//総キューブ数
    public int mid_numOfCubes = 0;//中間層のキューブの数
    public int upr_numOfCubes = 0;//上層のキューブの数
    public int lwr_numOfCubes = 0;//下層のキューブの数
    public float distance;//キューブ同士の距離

    public bool isBreath;//キューブ生命体が息をしているかどうか
    public float omega;//息をしているときの速度を操る各速度

    public float lifeTime;//生命体が生きている時間

    //平行移動辞書の作成
    Dictionary<string,bool> move = new Dictionary<string,bool>{
        {"Up",false},
        {"Down",false},
        {"Right",false},
        {"Left",false},
    };
    public float mvd = 0.1f;

    void Start(){

        //fps設定
        Application.targetFrameRate = 30; // 30fpsに設定

        //count cube nums
        mid_numOfCubes = radius * numDir + 1;
        for(int i = 1;i < 8;i++){
            int tmp = (radius - i) * numDir + 1;
            upr_numOfCubes = upr_numOfCubes + tmp;
            lwr_numOfCubes = lwr_numOfCubes + tmp;
        }
        sumOfCubes = mid_numOfCubes + upr_numOfCubes + lwr_numOfCubes;

        //game object
        cube = new GameObject[sumOfCubes];
        Main = new GameObject("Main");
        Scren = new GameObject("Scren");
        ParentPos = new GameObject("ParentPos");
        cubeParent = new GameObject("cubeParent");
        centerPoint = new GameObject("centerPoint");
        circleStartPoint = new GameObject("circleStartPoint");

        //variant
        isBreath = true;
        distance = 0.5f;
        cubeVectors = new Vector3[sumOfCubes];
        coreVector = new Vector3(0f,0f,0f);
        omega = (float)(Math.PI / 180) * 3f;
        lifeTime = 0f;



        //Make cube life
        //キューブを出現させる
        for(int i= 0; i< sumOfCubes;i++){
            cube[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube[i].transform.localScale = new Vector3(scale, scale, scale);
            cube[i].transform.parent = cubeParent.transform;
            cube[i].GetComponent<Renderer>().material.color = Color.cyan;
        }

        //cubeParent.transform.position = new Vector3(0,distance,0);


        //Middle Layer deploy
        cube[0].transform.position = new Vector3(0,0,0);
        cubeVectors[0] = new Vector3(0f,0f,0f);

        for(int i = 0;i < mid_numOfCubes - 1;i++){
            indexOfCubes += 1;
            float angle = (22.5f * i) % 360;
            float rad = ((i / numDir) + 1) * distance;
            var vx = Math.Cos(angle * (Math.PI / 180));
            var vz = Math.Sin(angle * (Math.PI / 180));
            rad = rad / (Math.Abs((float)vx) + Math.Abs((float)vz));

            cube[i+1].transform.position = new Vector3(rad * (float)vx,0,rad * (float)vz);
            cubeVectors[i+1] = (cube[i+1].transform.position - coreVector).normalized;
        }

        //Upper Layer deploy
        for(int i = 0;i < 6;i++){
            int a_layer_cubes = (6 - i) * numDir;
            float dy = i + 1;
            for (int j = 0;j < a_layer_cubes;j++){
                indexOfCubes += 1;
                float angle = (22.5f * j) % 360;
                float rad = ((j / numDir) + 1) * distance;
                var vx = Math.Cos(angle * (Math.PI / 180));
                var vz = Math.Sin(angle * (Math.PI / 180));
                rad = rad / (Math.Abs((float)vx) + Math.Abs((float)vz));
                cube[indexOfCubes].transform.position = new Vector3(rad * (float)vx,distance * dy,rad * (float)vz);   
                cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;
            }
            indexOfCubes += 1;
            cube[indexOfCubes].transform.position = new Vector3(0,distance * dy,0);
            cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;
        }
        indexOfCubes += 1;
        cube[indexOfCubes].transform.position = new Vector3(0,distance * (height / 2),0);
        cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;

        //Lower Layer deploy
        for(int i = 0;i < 6;i++){
            int a_layer_cubes = (6 - i) * numDir;
            float dy = (i + 1) * (-1);
            for (int j = 0;j < a_layer_cubes;j++){
                indexOfCubes += 1;
                float angle = (22.5f * j) % 360;
                float rad = ((j / numDir) + 1) * distance;
                var vx = Math.Cos(angle * (Math.PI / 180));
                var vz = Math.Sin(angle * (Math.PI / 180));
                rad = rad / (Math.Abs((float)vx) + Math.Abs((float)vz));
                cube[indexOfCubes].transform.position = new Vector3(rad * (float)vx,distance * dy,rad * (float)vz);   
                cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;
            }
            indexOfCubes += 1;
            cube[indexOfCubes].transform.position = new Vector3(0,distance * dy,0);
            cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;
        }
        indexOfCubes += 1;
        cube[indexOfCubes].transform.position = new Vector3(0,distance * (height / 2) * (-1),0);
        cubeVectors[indexOfCubes] = (cube[indexOfCubes].transform.position - coreVector).normalized;


        
    }

    //
    void Update(){
        lifeTime += 1f;
        move["Up"] = Input.GetKey(KeyCode.UpArrow);
        move["Down"] = Input.GetKey(KeyCode.DownArrow);
        move["Right"] = Input.GetKey(KeyCode.RightArrow);
        move["Left"] = Input.GetKey(KeyCode.LeftArrow);
    }


    void FixedUpdate(){

        //息をする動作
        //円運動のSinの値をターゲットに、各キューブがコアキューブとの相対ベクトルの方向に振動するように動く
        float targetVol = (float)(Math.Sin(omega * lifeTime)) / 10f;
        for(int i = 1;i < sumOfCubes;i++){
            Vector3 cur = cube[i].transform.position;
            cube[i].transform.position = Vector3.MoveTowards(cur,cur + targetVol*cubeVectors[i],mvd);
        }
        

        //平行移動
        //コアキューブcube[0]がまず移動する。コアキューブの移動ベクトルを、残りのキューブすべてに対して加算する。
        if(move["Up"]){
            adjectiveCorePos = cube[0].transform.position;
            cube[0].transform.Translate(0,mvd,0);
            currentCorePos = cube[0].transform.position;
            vec = currentCorePos - adjectiveCorePos;
            for(int i = 1;i < sumOfCubes;i++){
                Vector3 cur = cube[i].transform.position;
                cube[i].transform.position = Vector3.MoveTowards(cur,cur + vec,mvd);
            }
        }else if(move["Down"]){
            adjectiveCorePos = cube[0].transform.position;
            cube[0].transform.Translate(0,-1f * mvd,0);
            currentCorePos = cube[0].transform.position;
            vec = currentCorePos - adjectiveCorePos;
            for(int i = 1;i < sumOfCubes;i++){
                Vector3 cur = cube[i].transform.position;
                cube[i].transform.position = Vector3.MoveTowards(cur,cur + vec,mvd);
            }
        }else if(move["Right"]){
            adjectiveCorePos = cube[0].transform.position;
            cube[0].transform.Translate(mvd,0,0);
            currentCorePos = cube[0].transform.position;
            vec = currentCorePos - adjectiveCorePos;
            for(int i = 1;i < sumOfCubes;i++){
                Vector3 cur = cube[i].transform.position;
                cube[i].transform.position = Vector3.MoveTowards(cur,cur + vec,mvd);
            }
        }else if(move["Left"]){
            adjectiveCorePos = cube[0].transform.position;
            cube[0].transform.Translate(-1f * mvd,0,0);
            currentCorePos = cube[0].transform.position;
            vec = currentCorePos - adjectiveCorePos;
            for(int i = 1;i < sumOfCubes;i++){
                Vector3 cur = cube[i].transform.position;
                cube[i].transform.position = Vector3.MoveTowards(cur,cur + vec,mvd);
            }
        }

    }

    

}