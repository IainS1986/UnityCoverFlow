using UnityEngine;
using System.Collections;
using System;

public class NGUICell : iCollectionViewCell {

    public int index;

    public void Update()
    {
        index = Index;
    }

    public override string NibName
    {
        get
        {
            return "NGUICell";
        }
    } 

    Material m_material;
    Renderer m_renderer;

    public override void SetData(object data)
    {
        base.SetData(data);

        if(m_renderer==null)
            m_renderer = GetComponent<Renderer>();

        if(m_renderer!=null && m_material==null)
        {
            m_material = m_renderer.material;
        }

        //Set Color
        if(m_material!=null)
        {
            QuadCell.QuadCellData quadData = data as QuadCell.QuadCellData;
            if(quadData!=null && quadData.MainColor != null)
            {
                m_material.color = quadData.MainColor;
                m_renderer.material = m_material;
            }
        }
    }
}
