using UnityEngine;
using System.Collections;

//Has to be a monobehaviour as this acts as a "physical" pool of the cells too
using System.Collections.Generic;


public class CellPoolService : Singleton<CellPoolService> {

    //Prefabs
    private Dictionary<string, iCollectionViewCell> m_nibs;

    //Pools
	private Dictionary<string, List<iCollectionViewCell>> m_pools;

    //Guarantee this will always be a singleton only
    protected CellPoolService()
    {
		m_nibs = new Dictionary<string, iCollectionViewCell>();
		m_pools = new Dictionary<string, List<iCollectionViewCell>>();
    }

    //This will overwrite any previously registered!
	public void RegisterCell(iCollectionViewCell prefab)
    {
        if(m_nibs.ContainsKey(prefab.NibName))
        {
            m_nibs[prefab.NibName] = prefab;
        }
        else
        {
            m_nibs.Add(prefab.NibName, prefab);
        }
    }
	public void RegisterCellForNib(iCollectionViewCell prefab, string nib)
    {
        if(m_nibs.ContainsKey(nib))
        {
            m_nibs[nib] = prefab;
        }
        else
        {
            m_nibs.Add(nib, prefab);
        }
    }

	public iCollectionViewCell CreateCell(string nib, GameObject owner = null)
    {
        //Check we have nib registered
        if(!m_nibs.ContainsKey(nib))
            throw new UnityException(string.Format("No Prefab registered for {0}", nib));

        //Check pool
        if(m_pools.ContainsKey(nib))
        {
			List<iCollectionViewCell> pool = m_pools[nib];

            //Check for any not being used
			foreach(iCollectionViewCell cell in pool)
            {
                if(!cell.IsActive)
                {
                    cell.SetActive(true, owner);
                    return cell;
                }
            }

        }
        else
        {
            //Make Pool
			m_pools.Add(nib, new List<iCollectionViewCell>());
        }

        //No free cells, so make a new one
		iCollectionViewCell c = m_nibs[nib].CreateCell();
        c.SetActive(true, owner);

        m_pools[nib].Add(c);

        return c;
    }
	
}
