using System;
using System.Collections.Generic;
using UnityEngine;

public class CoverFlowLayout : CollectionViewLayout
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
        return new Vector3(0, RotationAngle(indexOffset), 0);
    }

    protected override Vector3 Position(float indexOffset, int cellIndex)
    {
        return new Vector3(TranslationX(indexOffset), 0, TranslationZ(indexOffset));
    }

    protected override Vector3 Scale(float indexOffset, int cellIndex)
    {
        return new Vector3(1, 1, 1);
    }

    private float RotationAngle(float indexOffset)
    {
        if(indexOffset >= 1 || indexOffset <= -1)
        {
            return Sign(indexOffset) * m_sideAngles;
        }
        else
        {
            return Math.Abs(indexOffset) * Sign(indexOffset) * m_sideAngles;
        }
    }

    private float TranslationX(float indexOffset)
    {
        if(indexOffset >= 1 || indexOffset <= -1)
        {
            return indexOffset * m_sideGaps + Sign(indexOffset) * m_centralGap;
        }
        else
        {
            return Sign(indexOffset) * m_centralGap * Math.Abs(indexOffset);
        }
    }

    private float TranslationZ(float indexOffset)
    {
        if(indexOffset >= 1 || indexOffset <= -1)
        {
            return m_sideDepth;
        }
        else
        {
            return m_centralDepth - Math.Abs(indexOffset) * (m_centralDepth - m_sideDepth);
        }
    }
}

