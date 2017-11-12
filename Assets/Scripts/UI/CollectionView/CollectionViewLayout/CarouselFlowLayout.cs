using UnityEngine;
using System.Collections;
using System;

public class CarouselFlowLayout : CollectionViewLayout
{
    [SerializeField]
    private float m_width = 10;

    [SerializeField]
    private float m_depth = 10;

    public override bool IndexWrap { get { return true; } }

    private double Angle { get; set; }

     #if UNITY_EDITOR
    private float m_lastTime;
    private float m_lastWidth, m_lastDepth;
    #endif
    
    #if UNITY_EDITOR
    void Awake()
    {
        m_lastTime = m_animTime;
        m_lastWidth = m_width;
        m_lastDepth = m_depth;
    }
    #endif
    
    #if UNITY_EDITOR
    void Update()
    {
        if(m_lastTime != m_animTime ||
           m_lastDepth != m_depth ||
           m_lastWidth != m_width)
        {
            //Relayout
            RequiresRelayout = true;

            m_lastTime = m_animTime;
            m_lastDepth = m_depth;
            m_lastWidth = m_width;
        }
    }
    #endif

	protected override void PreLayout(System.Collections.Generic.List<object> data, System.Collections.Generic.List<iCollectionViewCell> cells, bool dragging)
    {
        base.PreLayout(data, cells, dragging);

        //Work out Cost and Sint
        int totalCells = Math.Min(cells.Count, TotalCellsNeeded);
        totalCells += (totalCells / 3) + ((totalCells % 2 == 1) ? 1 : 0);
        Angle = (2 * Math.PI) / totalCells;
    }

    protected override Vector3 EulerAngle(float indexOffset, int cellIndex)
    {
        return new Vector3(0,0,0);
    }
    
    protected override Vector3 Position(float indexOffset, int cellIndex)
    {
        return new Vector3(TranslationX(indexOffset),
                           TranslationY(indexOffset),
                           TranslationZ(indexOffset));
    }

    protected override Vector3 Scale(float indexOffset, int cellIndex)
    {
        return new Vector3(1, 1, 1);
    }

    private float TranslationX(float indexOffset)
    {
        return (float) (m_width * Math.Sin(Angle * indexOffset));
    }

    private float TranslationY(float indexOffset)
    {
        return 0;
    }
    
    private float TranslationZ(float indexOffset)
    {
        return (float) (-m_depth * Math.Cos(Angle * indexOffset)) + m_depth;
    }
}
