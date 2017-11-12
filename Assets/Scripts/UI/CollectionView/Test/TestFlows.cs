using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestFlows : MonoBehaviour {

    [SerializeField]
    private UICollectionView m_coverFlow;

    [SerializeField]
    private Color[] m_colourData;

    [SerializeField]
    private int m_numberOfCells = 10;

    [SerializeField]
    private int m_groupSizes = 10;

	// Use this for initialization
	void Start () 
    {
	    //Build cells
        if(m_coverFlow!=null)
        {
            //Build a bunch of cells - pass in data
            List<QuadCell.QuadCellData> data = new List<QuadCell.QuadCellData>();
            for (int i = 0; i < m_numberOfCells; i++)
            {
                if(m_colourData!=null && m_colourData.Length > 0)
                {
                    data.Add(new QuadCell.QuadCellData() { MainColor = m_colourData[(i / m_groupSizes) % m_colourData.Length] });
                }
                else
                {
                    data.Add(new QuadCell.QuadCellData());
                }

            }

            //Bleugh
            m_coverFlow.Data = new List<object>(data.ToArray());
        }
	}
}
