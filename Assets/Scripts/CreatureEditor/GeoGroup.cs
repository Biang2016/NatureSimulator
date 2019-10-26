using System.Collections.Generic;

public class GeoGroup : PoolObject
{
    public List<GeoElement> AllGeos = new List<GeoElement>();
    public GeoGroupInfo GeoGroupInfo;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        foreach (GeoElement ge in AllGeos)
        {
            ge.PoolRecycle();
        }

        AllGeos.Clear();
    }
}