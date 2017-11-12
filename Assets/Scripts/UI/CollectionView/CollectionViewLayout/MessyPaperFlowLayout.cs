using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MessyPaperFlowLayout : CollectionViewLayout
{
    [SerializeField]
    private Vector3 m_positionOffsetScale = new Vector3(1,1,1);

    [SerializeField]
    private float m_rotationOffsetScale = 20;

    [SerializeField]
    private Vector3 m_leftPosition = Vector3.zero;

    [SerializeField]
    private Vector3 m_rightPosition = Vector3.zero;

    [SerializeField]
    private Vector3 m_centralPosition = Vector3.zero;

    private System.Random m_random;
    private List<Vector3> m_positionOffsetsLeft = new List<Vector3>();
    private List<float> m_rotationOffsetsLeft = new List<float>();
    private List<Vector3> m_positionOffsetsRight = new List<Vector3>();
    private List<float> m_rotationOffsetsRight = new List<float>();
    private int RandomCount { get { return m_positionOffsetsLeft.Count; } }
    private bool ReRandom { get; set; }

    #if UNITY_EDITOR
    private float m_lastRotationOffsetScale, m_lastTime;
    private Vector3 m_lastPositionOffsetScale, m_lastLeftPosition, m_lastRightPosition, m_lastCentralPosition;
    #endif

    #if UNITY_EDITOR
    void Awake()
    {
        m_lastTime = m_animTime;
        m_lastPositionOffsetScale = m_positionOffsetScale;
        m_lastLeftPosition = m_leftPosition;
        m_lastRightPosition = m_rightPosition;
        m_lastRotationOffsetScale = m_rotationOffsetScale;
        m_lastCentralPosition = m_centralPosition;
    }
    #endif

    #if UNITY_EDITOR
    void Update()
    {
        if(m_lastTime != m_animTime ||
           m_lastPositionOffsetScale != m_positionOffsetScale ||
           m_lastLeftPosition != m_leftPosition ||
           m_lastRightPosition != m_rightPosition ||
           m_lastRotationOffsetScale != m_rotationOffsetScale ||
           m_lastCentralPosition != m_centralPosition)
        {
            //Relayout
            RequiresRelayout = true;
            ReRandom = true;
            
            m_lastTime = m_animTime;
            m_lastPositionOffsetScale = m_positionOffsetScale;
            m_lastLeftPosition = m_leftPosition;
            m_lastRightPosition = m_rightPosition;
            m_lastRotationOffsetScale = m_rotationOffsetScale;
            m_lastCentralPosition = m_centralPosition;
        }
    }
    #endif

	protected override void PreLayout (System.Collections.Generic.List<object> data, System.Collections.Generic.List<iCollectionViewCell> cells, bool dragging)
	{
		base.PreLayout (data, cells, dragging);

		//Do we need to calculate new random positions/rotations etc
        if (data.Count != RandomCount || ReRandom)
        {
            m_positionOffsetsLeft.Clear();
            m_rotationOffsetsLeft.Clear();
            m_positionOffsetsRight.Clear();
            m_rotationOffsetsRight.Clear();

            //Generate Random stuff
            m_random = new System.Random(0);
            //Add left hand side (n-1)
            //Add right hand side (n-1)
            for(int i=0; i<data.Count; i++)
            {
                m_positionOffsetsLeft.Add(new Vector3((float)(m_random.NextDouble() * m_positionOffsetScale.x),
                                                      (float)(m_random.NextDouble() * m_positionOffsetScale.y),
                                                      (float)(m_random.NextDouble() * m_positionOffsetScale.z)));

                m_positionOffsetsRight.Add(new Vector3((float)(m_random.NextDouble() * m_positionOffsetScale.x),
                                                       (float)(m_random.NextDouble() * m_positionOffsetScale.y),
                                                       (float)(m_random.NextDouble() * m_positionOffsetScale.z)));

                if(Math.Abs(m_rotationOffsetScale) <= 2)
                {
                    m_rotationOffsetsLeft.Add (0);
                    m_rotationOffsetsRight.Add(0);
                }
                else
                {
                    m_rotationOffsetsLeft.Add ((float)(m_random.Next(-(int)(Math.Abs (m_rotationOffsetScale) / 2), (int)(Math.Abs (m_rotationOffsetScale) / 2))));
                    m_rotationOffsetsRight.Add ((float)(m_random.Next(-(int)(Math.Abs (m_rotationOffsetScale) / 2), (int)(Math.Abs (m_rotationOffsetScale) / 2))));
                }
            }

            ReRandom = false;
        }
	}

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
        if (indexOffset >= 1)
        {
            return m_rotationOffsetsRight[cellIndex];
        } else if (indexOffset <= -1)
        {
            return  m_rotationOffsetsLeft[cellIndex];
        } else if (indexOffset > 0)
        {
            return Math.Abs(indexOffset) * m_rotationOffsetsRight[cellIndex];
        } else if (indexOffset < 0)
        {
            return Math.Abs(indexOffset) * m_rotationOffsetsLeft[cellIndex];
        } else
        {
            return 0;
        }
    }

    private float TranslationX(float indexOffset, int cellIndex)
    {
        if (indexOffset >= 1)
        {
            return m_rightPosition.x + m_positionOffsetsRight[cellIndex].x;
        } else if (indexOffset <= -1)
        {
            return m_leftPosition.x + m_positionOffsetsLeft[cellIndex].x;
        } else if (indexOffset > 0)
        {
            return m_centralPosition.x + indexOffset * (Math.Abs (m_rightPosition.x + m_positionOffsetsRight[cellIndex].x) - m_centralPosition.x);
        } else if (indexOffset < 0)
        {
            return m_centralPosition.x + indexOffset * (Math.Abs (m_leftPosition.x + m_positionOffsetsLeft[cellIndex].x) + m_centralPosition.x);
        } else
        {
            return m_centralPosition.x;
        }
    }
    
    private float TranslationY(float indexOffset, int cellIndex)
    {
        if (indexOffset >= 1)
        {
            return m_rightPosition.y + m_positionOffsetsRight[cellIndex].y;
        } else if (indexOffset <= -1)
        {
            return m_leftPosition.y + m_positionOffsetsLeft[cellIndex].y;
        } else if (indexOffset > 0)
        {
            return m_centralPosition.y + Math.Abs(indexOffset) * (m_rightPosition.y + m_positionOffsetsRight[cellIndex].y - m_centralPosition.y);
        } else if (indexOffset < 0)
        {
            return m_centralPosition.y + Math.Abs(indexOffset) * (m_leftPosition.y + m_positionOffsetsLeft[cellIndex].y - m_centralPosition.y);
        } else
        {
            return m_centralPosition.y;
        }
    }

    private float TranslationZ(float indexOffset, int cellIndex)
    {
        if (indexOffset >= 1 || indexOffset <= -1)
        {
            return Math.Abs(indexOffset) * 0.001f;
        }
        else
        {
            return m_centralPosition.z - Math.Abs(indexOffset) * (m_centralPosition.z - 0.001f);
        }
    }

}
