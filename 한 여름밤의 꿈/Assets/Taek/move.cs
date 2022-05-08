using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
    public Vector3[] waypoints; // 이동포인트 배열
    private Vector3 currPosition; //현재위치
    private int waypointIndex = 0; //이도 포인트 인덱스
    private float speed =4f; //속도

    // Use this for initialization
    void Start () {
        waypoints = new Vector3[3];
        waypoints[0] = new Vector3(22 , 14, 0);

        //이동포인트 배열에 값 할당
        waypoints.SetValue(new Vector3(22, 14, 0), 0);
        waypoints.SetValue(new Vector3(22, 30, 0), 1);
        waypoints.SetValue(new Vector3(22  , 14, 0), 2);
    }

    // Update is called once per frame
    void Update () {
        currPosition = transform.position;
        if (waypointIndex< waypoints.Length)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currPosition, waypoints[waypointIndex], step);

            //현 위치가 이동지점의 위지라면 배열 인덱스+1하여 다음 포인트로 이동하도록.??
            if (Vector3.Distance(waypoints[waypointIndex], currPosition) == 0f)
                waypointIndex++;
            if (waypointIndex == 2)
            { waypointIndex = 0; }


        }
	}
}
