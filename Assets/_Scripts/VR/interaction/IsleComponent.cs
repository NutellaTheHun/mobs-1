using UnityEngine;

public class IsleData
{
    public int _left;
    public int _right;
    public int _total;
    public IsleData(int left, int right)
    {
        _left = left;
        _right = right;
        _total = left + right;
    }

}
public class IsleComponent : MonoBehaviour
{
    [SerializeField] public int IsleIndex;
    [SerializeField] IpadCountData _IpadCountData;
    private ObjComponent[] IpadList;
    private int LeftIpadCount = 0;
    private int RightIpadCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        IpadList = GetComponentsInChildren<ObjComponent>();
        _IpadCountData.Isle[IsleIndex] = GetIpadCount();
    }

    private IsleData GetIpadCount()
    {
        foreach(ObjComponent ipad in IpadList)
        {
            if (ipad.sideOfIsle == ObjComponent.ShelfSide.Left) LeftIpadCount++;
            else RightIpadCount++;
        }
        return new IsleData(LeftIpadCount, RightIpadCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateIsleCount(ObjComponent.ShelfSide sideOfIsle)
    {
        _IpadCountData.removeIpad(IsleIndex,sideOfIsle);
    }
}
