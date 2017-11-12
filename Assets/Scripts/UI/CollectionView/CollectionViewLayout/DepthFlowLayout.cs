using UnityEngine;
using System.Collections;

public class DepthFlowLayout : CollectionViewLayout 
{

    [SerializeField]
    private float m_sideGaps = 0.2f;
    
    [SerializeField]
    private float m_centralGap = 1;
    
    [SerializeField]
    private float m_sideDepth = 0;
    
    [SerializeField]
    private float m_centralDepth = 1;
    
    [SerializeField]
    private float m_sideAngles = 90;
    
    [SerializeField]
    private bool m_wrap = false;
    
    public override bool IndexWrap { get { return m_wrap; } }

    #if UNITY_EDITOR
    private float m_lastTime, m_lastSideGaps, m_lastCentralGap, m_lastSideDepth, m_lastSideAngles, m_lastCentralDepth;
    private bool m_wrapBefore;
    #endif
    
    #if UNITY_EDITOR
    void Awake()
    {
        m_lastTime = m_animTime;
        m_lastSideGaps = m_sideGaps;
        m_lastCentralGap = m_centralGap;
        m_lastSideDepth = m_sideDepth;
        m_lastSideAngles = m_sideAngles;
        m_lastCentralDepth = m_centralDepth;
        m_wrapBefore = m_wrap;
    }
    #endif

    #if UNITY_EDITOR
    void Update()
    {
        if(m_lastTime != m_animTime ||
           m_lastSideGaps != m_sideGaps ||
           m_lastCentralGap != m_centralGap ||
           m_lastSideDepth != m_sideDepth ||
           m_lastSideAngles != m_sideAngles ||
           m_lastCentralDepth != m_centralDepth ||
           m_wrapBefore != m_wrap)
        {
            //Relayout
            RequiresRelayout = true;
            InstantLayout |= (m_wrapBefore != m_wrap);
            
            m_lastTime = m_animTime;
            m_lastSideGaps = m_sideGaps;
            m_lastCentralGap = m_centralGap;
            m_lastSideDepth = m_sideDepth;
            m_lastSideAngles = m_sideAngles;
            m_lastCentralDepth = m_centralDepth;
            m_wrapBefore = m_wrap;
        }
    }
    #endif

    protected override Vector3 EulerAngle(float indexOffset, int cellIndex)
    {
        return new Vector3(0,0,RotationAngle(indexOffset, cellIndex));
    }
    
    protected override Vector3 Position(float indexOffset, int cellIndex)
    {
        return new Vector3(TranslationX(indexOffset, cellIndex),
                           TranslationY(indexOffset, cellIndex),
                           TranslationZ(indexOffset, cellIndex));
    }
    
    protected override Vector3 Scale(float indexOffset, int cellIndex)
    {
        return new Vector3(1, 1, 1);
    }
    
    private float RotationAngle(float indexOffset, int cellIndex)
    {
        return 0;
    }
    
    private float TranslationX(float indexOffset, int cellIndex)
    {
        return 0;
    }
    
    private float TranslationY(float indexOffset, int cellIndex)
    {
        return 0;
    }
    
    private float TranslationZ(float indexOffset, int cellIndex)
    {
        return 0;
    }
}
