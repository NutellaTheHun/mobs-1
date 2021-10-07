using UnityEngine;
using System.Collections;

public struct Shelf {  //shelves that hold the objects
    public Vector3 v1,v2,v3,v4;
};

public  class ShelfComponent :MonoBehaviour {
    

    public  Shelf[] Shelves;

    void  Start() {
        Shelves = new Shelf[6]; //not actually shelves but the area in the middle of two shelves
        Transform sh = GameObject.Find("Shelf0").transform;
        Shelves[0].v1 = new Vector3(sh.position.x - 1, 0, sh.position.z - 5.5f);
        Shelves[0].v2 = new Vector3(sh.position.x, 0, sh.position.z - 5.5f);
        Shelves[0].v3 = new Vector3(sh.position.x, 0, sh.position.z + 5.5f);
        Shelves[0].v4 = new Vector3(sh.position.x - 1, 0, sh.position.z + 5.5f);

        for (int i = 1; i <= 4; i++) {
            Transform sh1 = GameObject.Find("Shelf" + (i - 1)).transform;
            Transform sh2 = GameObject.Find("Shelf" + (i)).transform;
            Shelves[i].v1 = new Vector3(sh1.position.x + 1, 0, sh1.position.z - 5.5f);
            Shelves[i].v2 = new Vector3(sh2.position.x - 1, 0, sh1.position.z - 5.5f);
            Shelves[i].v3 = new Vector3(sh2.position.x - 1, 0, sh1.position.z + 5.5f);
            Shelves[i].v4 = new Vector3(sh1.position.x + 1, 0, sh1.position.z + 5.5f);
        }

        sh = GameObject.Find("Shelf4").transform;
        Shelves[5].v1 = new Vector3(sh.position.x, 0, sh.position.z - 5.5f);
        Shelves[5].v2 = new Vector3(sh.position.x + 1, 0, sh.position.z - 5.5f);
        Shelves[5].v3 = new Vector3(sh.position.x + 1, 0, sh.position.z + 5.5f);
        Shelves[5].v4 = new Vector3(sh.position.x, 0, sh.position.z + 5.5f);
        
    }

    //Find the point on s which is closest to p
    public Vector3 FindClosestShelfPos(Vector3 p, int shelfInd) {
        Vector3 cp;
        Shelf s = Shelves[shelfInd];
        Vector3 dist1 = new Vector3(Mathf.Abs(s.v1.x - p.x), Mathf.Abs(s.v1.y - p.y), Mathf.Abs(s.v1.z - p.z));
        Vector3 dist2 = new Vector3(Mathf.Abs(s.v2.x - p.x), Mathf.Abs(s.v2.y - p.y), Mathf.Abs(s.v2.z - p.z));
        Vector3 dist3 = new Vector3(Mathf.Abs(s.v3.x - p.x), Mathf.Abs(s.v3.y - p.y), Mathf.Abs(s.v3.z - p.z));
        Vector3 dist4 = new Vector3(Mathf.Abs(s.v4.x - p.x), Mathf.Abs(s.v4.y - p.y), Mathf.Abs(s.v4.z - p.z));

        //X
        float minX = Mathf.Min(dist1.x, dist2.x, dist3.x, dist4.x);
        if (minX == dist1.x)
            cp.x = s.v1.x;
        else if (minX == dist2.x)
            cp.x = s.v2.x;
        else if (minX == dist3.x)
            cp.x = s.v3.x;
        else
            cp.x = s.v4.x;

        //Y
        float minY = Mathf.Min(dist1.y, dist2.y, dist3.y, dist4.y);
        if (minY == dist1.y)
            cp.y = s.v1.y;
        else if (minY == dist2.y)
            cp.y = s.v2.y;
        else if (minY == dist3.y)
            cp.y = s.v3.y;
        else
            cp.y = s.v4.y;

        //Z
        float minZ = Mathf.Min(dist1.z, dist2.z, dist3.z, dist4.z);
        if (minZ == dist1.z)
            cp.z = s.v1.z;
        else if (minZ == dist2.z)
            cp.z = s.v2.z;
        else if (minZ == dist3.z)
            cp.z = s.v3.z;
        else
            cp.z = s.v4.z;


        return cp;

    }
      
}
