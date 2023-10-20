using UnityEngine;


/*public struct VR_Shelf {  //shelves that hold the objects
    public Vector3 v1,v2,v3,v4;

};*/

public struct aisle
{
    public Vector3 wp1, wp2;
}


public class VR_ShelfComponent:MonoBehaviour {
    
    private const int IsleNum = 11; //for the 11 isles in ShopperWorld
    /*public  VR_Shelf[] Shelves;
    float _halfShelfWidth = 0.5f;//0.55f; //previous val: 1.5f
    float _halfShelfLength = 6.8f; //previos val 5.5f*/
    public int[] shopperInIsleCount = new int[IsleNum];
    public aisle[] aisles = new aisle[IsleNum];
    void  Start() {
        for (int i = 0; i < IsleNum; i++)
        {
            Transform go = GameObject.Find("Objects" + i).transform;
            for(int j = 0; j < go.childCount; j++)
            {
                if(go.GetChild(j).CompareTag("AisleWaypoint"))
                {
                    if (aisles[i].wp1 == Vector3.zero)
                    {
                        aisles[i].wp1 = go.GetChild(j).position;
                        continue;
                    }
                    if (aisles[i].wp2 == Vector3.zero)
                    {
                        aisles[i].wp2 = go.GetChild(j).position;
                        continue;
                    }
                }
            }
        }
        /* Shelves = new VR_Shelf[11]; //not actually shelves but the area in the middle of two shelves
         Transform sh = GameObject.Find("Shelf0").transform;
         Shelves[0].v1 = new Vector3(sh.position.x - _halfShelfWidth, 0, sh.position.z - _halfShelfLength);
         Shelves[0].v2 = new Vector3(sh.position.x, 0, sh.position.z - _halfShelfLength);
         Shelves[0].v3 = new Vector3(sh.position.x, 0, sh.position.z + _halfShelfLength);
         Shelves[0].v4 = new Vector3(sh.position.x - _halfShelfWidth, 0, sh.position.z + _halfShelfLength);

         for (int i = 1; i <= 9; i++) {
             Transform sh1 = GameObject.Find("Shelf" + (i - 1)).transform;
             Transform sh2 = GameObject.Find("Shelf" + (i)).transform;
             Shelves[i].v1 = new Vector3(sh1.position.x + _halfShelfWidth, 0, sh1.position.z - _halfShelfLength);
             Shelves[i].v2 = new Vector3(sh2.position.x - _halfShelfWidth, 0, sh1.position.z - _halfShelfLength);
             Shelves[i].v3 = new Vector3(sh2.position.x - _halfShelfWidth, 0, sh1.position.z + _halfShelfLength);
             Shelves[i].v4 = new Vector3(sh1.position.x + _halfShelfWidth, 0, sh1.position.z + _halfShelfLength);
         }

         sh = GameObject.Find("Shelf9").transform;
         Shelves[10].v1 = new Vector3(sh.position.x, 0, sh.position.z - _halfShelfLength);
         Shelves[10].v2 = new Vector3(sh.position.x + _halfShelfWidth, 0, sh.position.z - _halfShelfLength);
         Shelves[10].v3 = new Vector3(sh.position.x + _halfShelfWidth, 0, sh.position.z + _halfShelfLength);
         Shelves[10].v4 = new Vector3(sh.position.x, 0, sh.position.z + _halfShelfLength);
         */
    }
    public Vector3 FindClosestIsleWaypoint(Vector3 p, int IsleInd)
    {
        Vector3 wp1 = aisles[IsleInd].wp1;
        Vector3 wp2 = aisles[IsleInd].wp2;
        float a = Vector3.Distance(p,wp1);
        float b = Vector3.Distance(p,wp2);
        if(a < b)
        {
            return wp1;
        }
        return wp2;

    }
    public Vector3 FindFarthestIsleWaypoint(Vector3 p, int IsleInd)
    {
        Vector3 wp1 = aisles[IsleInd].wp1;
        Vector3 wp2 = aisles[IsleInd].wp2;
        float a = Vector3.Distance(p, wp1);
        float b = Vector3.Distance(p, wp2);
        if (a < b)
        {
            return wp2;
        }
        return wp1;

    }

    public void incrementShopper(int isleID)
    {
        shopperInIsleCount[isleID]++;
    }

    public int getShopperCount(int isleID) 
    {
        int shopperCount = 0;
        for(int i = 0; i <= isleID; i++)
        {
            shopperCount += shopperInIsleCount[i];
        }
        return shopperCount;
    }

    public Vector3 getOtherWaypoint(Vector3 closestShelfPos, int isleIndex)
    {
        if (aisles[isleIndex].wp1 == closestShelfPos)
        {
            return aisles[isleIndex].wp2;
        }
        else return aisles[isleIndex].wp1;
    }

private void OnDrawGizmos()
{
   foreach(aisle a in aisles)
   {
       Gizmos.color = Color.red;
       Gizmos.DrawSphere(a.wp1, 0.25f);
       Gizmos.DrawSphere(a.wp2, 0.25f);
   }
}
    //Find the point on s which is closest to p
    /* public Vector3 FindClosestShelfPos(Vector3 p, int shelfInd) {
     Vector3 cp;
     VR_Shelf s = Shelves[shelfInd];
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

 }*/
}
