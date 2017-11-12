using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICollectionView : MonoBehaviour {

    private enum DragState
    {
        None,
        Starting,
        Dragging,
    }

    [SerializeField]
    private LayerMask m_inputDragLayer;

    [SerializeField]
    private Camera m_inputCamera;

    [SerializeField]
    private iCollectionViewCell m_cell;

    [SerializeField]
    private CollectionViewLayout m_layout;

    [SerializeField]
    private bool m_tapSelectEnabled = true;

    [SerializeField]
    private bool m_dragEnabled = true;

    [SerializeField]
    private float m_dragSpeed = 1;

    [SerializeField]
    private float m_minDragAmount = 0.05f;

    [SerializeField]
    private float m_maxDragAmount = 0.2f;

    [SerializeField]
    private GameObject m_dragPlane;

    private DragState m_dragState = DragState.None;
    private Vector2 m_lastScreenPos = Vector2.zero;
    private Vector2 m_totalDrag = Vector2.zero;
    private bool Dirty { get; set; }

	private CollectionViewLayout m_prevLayout;

    private List<object> m_data;
    public List<object> Data
    {
        get
        {
            return m_data;
        }
        set
        {
            m_data = value;
            Dirty = true;

            //Instant
            if(m_layout!=null)
                m_layout.InstantLayout = true;
        }
    }

    //Store cells here, the Layouts will deal with these
    private List<iCollectionViewCell> m_cells;

    void Awake()
    {
        m_prevLayout = m_layout;

        if(m_cell!=null && m_layout!=null)
            m_layout.RegisterCell(m_cell, OnCellClickEvent);

        //Register with any other components
		CollectionViewLayout[] layouts = GetComponents<CollectionViewLayout>();
		foreach(CollectionViewLayout layout in layouts)
        {
            if(m_layout!=layout)
                layout.RegisterCell(m_cell, OnCellClickEvent);
        }

        m_cells = new List<iCollectionViewCell>();
    }

    void Update()
    {
#if UNITY_EDITOR
        KeyboardInput();
        if(m_dragEnabled)
            MouseDrag();
#elif UNITY_IPHONE
        if(m_dragEnabled)
            TouchDrag();
#endif

        if(m_prevLayout!=m_layout)
        {
            m_layout.InstantLayout=true;
            m_layout.CurrentIndex = m_prevLayout.CurrentIndex;
            Dirty = true;
            m_prevLayout = m_layout;
        }

        if (m_layout != null)
        {
            if (Dirty || m_layout.RequiresRelayout)
            {
                m_layout.Layout(Data, m_cells, m_dragState == DragState.Dragging);
                Dirty = false;
            }
        }
    }

    public void Relayout()
    {
        Dirty = true;
    }

	public void SetLayout(CollectionViewLayout layout)
    {
        layout.RegisterCell(m_cell, OnCellClickEvent);
        layout.CurrentIndex = m_layout.CurrentIndex;
        m_prevLayout = m_layout;
        m_layout = layout;
        Dirty = true;
        m_layout.InstantLayout = true;
    }

    public void StepLeft()
    {
        if(m_layout.CurrentIndex > 0)
        {
            m_layout.CurrentIndex--;
            Dirty = true;
        }
        else if(m_layout.IndexWrap)
        {
            m_layout.CurrentIndex = Data.Count - 1;
            Dirty = true;
        }
    }

    public void StepRight()
    {
        if(m_layout.CurrentIndex < Data.Count - 1)
        {
            m_layout.CurrentIndex++;
            Dirty = true;
        }
        else if(m_layout.IndexWrap)
        {
            m_layout.CurrentIndex = 0;
            Dirty = true;
        }
    }

    private void OnCellClickEvent(object sender, EventArgs e)
    {
        if(!m_tapSelectEnabled || m_dragState == DragState.Dragging)
            return;

		iCollectionViewCell cell = sender as iCollectionViewCell;
        if(cell!=null && m_layout!=null)
        {
            StopDrag();
            m_layout.CurrentIndex = cell.Index;
            Dirty = true;
        }
    }

    private void KeyboardInput()
    {
        if(m_layout!=null && Data.Count > 0)
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                StepRight();
                
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StepLeft();
            }
        }
    }

    private void MouseDrag()
    {
        if(m_layout!=null && Data.Count > 0)
        {
            if(m_dragState == DragState.None)
            {
                //Look for drag start
                if(Input.GetMouseButtonDown(0))
                {
                    //See if we click on any cells, and snap to that
                    Camera cam = (m_inputCamera!=null) ? m_inputCamera : Camera.main;
                    
                    RaycastHit hit;
                    Ray r = cam.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(r, out hit, float.MaxValue, m_inputDragLayer.value))
                    {
                        //Click start
                        m_dragState = DragState.Starting;
                        m_totalDrag = Vector2.zero;
                        m_lastScreenPos = Input.mousePosition;
                    }
                }
            }
            else
            {
                //See if we've moved
                //If no mouse stop
                if(Input.GetMouseButton(0))
                {
                    //Drag
                    Vector2 newPoint = Input.mousePosition;
                    Vector2 delta = m_lastScreenPos - newPoint;
                    
                    //Normalize delta to screen size
                    delta.x /= Screen.width;
                    delta.y /= Screen.height;

                    if(delta.x > m_maxDragAmount)
                        delta.x = m_maxDragAmount;
                    else if(delta.x < -m_maxDragAmount)
                        delta.x = -m_maxDragAmount;

                    if(delta.y > m_maxDragAmount)
                        delta.y = m_maxDragAmount;
                    else if(delta.y < -m_maxDragAmount)
                        delta.y = -m_maxDragAmount;

                    if(m_dragState == DragState.Starting)
                    {
                        //Tally up
                        m_totalDrag.x += Math.Abs(delta.x);
                        m_totalDrag.y += Math.Abs(delta.y);
                        
                        //Dragged enough?
                        if(m_totalDrag.x >= m_minDragAmount || m_totalDrag.y >= m_minDragAmount)
                            m_dragState = DragState.Dragging;
                    }

                    if(m_dragState == DragState.Dragging)
                    {
                        //Update index based off delta and drag speed
                        m_layout.CurrentIndex += delta.x * m_dragSpeed;

                        if(m_layout.IndexWrap)
                        {
                            //Wrap the CUrrentIndex
                            if(m_layout.CurrentIndex < 0)
                                m_layout.CurrentIndex += Data.Count;
                            else if(m_layout.CurrentIndex >= Data.Count)
                                m_layout.CurrentIndex -= Data.Count;
                        }
                        Dirty = true;
                    }
                    
                    m_lastScreenPos = newPoint;
                }
                else
                {
                    StopDrag();
                }
            }
        }
    }

    private void TouchDrag()
    {
        //Single touch only
        if(Input.touchCount != 1)
        {
            StopDrag();
            return;
        }

        Touch t = Input.touches[0];

        if(m_layout!=null && Data.Count > 0)
        {
            if(m_dragState == DragState.None)
            {
                //Look for touch start
                if(t.phase == TouchPhase.Began)
                {
                    //See if we click on any cells, and snap to that
                    Camera cam = (m_inputCamera!=null) ? m_inputCamera : Camera.main;
                    
                    RaycastHit hit;
                    Vector3 point = new Vector3(t.position.x, t.position.y, 0);
                    Ray r = Camera.main.ScreenPointToRay(point);
                    if(Physics.Raycast(r, out hit, float.MaxValue, m_inputDragLayer.value))
                    {
                        //Click start
                        m_dragState = DragState.Starting;
                        m_totalDrag = Vector2.zero;
                        m_lastScreenPos = t.position;
                    }
                }
            }
            else
            {
                if(t.phase == TouchPhase.Moved)
                {
                    //Drag
                    Vector2 newPoint = t.position;
                    Vector2 delta = m_lastScreenPos - newPoint;
                    
                    //Normalize delta to screen size
                    delta.x /= Screen.width;
                    delta.y /= Screen.height;
                    
                    if(delta.x > m_maxDragAmount)
                        delta.x = m_maxDragAmount;
                    else if(delta.x < -m_maxDragAmount)
                        delta.x = -m_maxDragAmount;
                    
                    if(delta.y > m_maxDragAmount)
                        delta.y = m_maxDragAmount;
                    else if(delta.y < -m_maxDragAmount)
                        delta.y = -m_maxDragAmount;
                    
                    if(m_dragState == DragState.Starting)
                    {
                        //Tally up
                        m_totalDrag.x += Math.Abs(delta.x);
                        m_totalDrag.y += Math.Abs(delta.y);
                        
                        //Dragged enough?
                        if(m_totalDrag.x >= m_minDragAmount || m_totalDrag.y >= m_minDragAmount)
                            m_dragState = DragState.Dragging;
                    }
                    
                    if(m_dragState == DragState.Dragging)
                    {
                        //Update index based off delta and drag speed
                        m_layout.CurrentIndex += delta.x * m_dragSpeed;

                        if(m_layout.IndexWrap)
                        {
                            //Wrap the CUrrentIndex
                            if(m_layout.CurrentIndex < 0)
                                m_layout.CurrentIndex += Data.Count;
                            else if(m_layout.CurrentIndex >= Data.Count)
                                m_layout.CurrentIndex -= Data.Count;
                        }
                        Dirty = true;
                    }
                    
                    m_lastScreenPos = newPoint;
                }
                else if(t.phase != TouchPhase.Stationary)
                {
                    StopDrag();
                }
            }
        }
    }

    private void StopDrag()
    {
        m_dragState = DragState.None;
        m_totalDrag = Vector2.zero;
        m_lastScreenPos = Vector2.zero;
        Dirty = true;

        //Clamp the layout to the nearest index
        //TODO - Momentum?
        //      - Store velocity of movement (either take last Delta, or maybe have a seperate velocity being set)
        //      - Coroutine or in Update, update the Index based off momentum until velocity is near 0
        //      - When done, snap to nearest index like below...
        if(m_layout!=null)
        {
            m_layout.CurrentIndex = Mathf.Round(m_layout.CurrentIndex);

            if(!m_layout.IndexWrap)
            {
                if(m_layout.CurrentIndex < 0)
                    m_layout.CurrentIndex = 0;
                else if(m_layout.CurrentIndex > Data.Count - 1)
                    m_layout.CurrentIndex = Data.Count - 1;
            }
            else
            {
                //Wrap the CUrrentIndex
                if(m_layout.CurrentIndex < 0)
                    m_layout.CurrentIndex += Data.Count;
                else if(m_layout.CurrentIndex >= Data.Count)
                    m_layout.CurrentIndex -= Data.Count;
            }
        }
    }

    void OnDrawGizmos() 
    {
        if(m_dragPlane==null)
            return;

        Gizmos.color = Color.red;

        //Draw the corners of the quad
        float radius = 0.025f;
        Gizmos.DrawSphere(m_dragPlane.transform.position + new Vector3(-m_dragPlane.transform.localScale.x / 2, -m_dragPlane.transform.localScale.y / 2), radius);
        Gizmos.DrawSphere(m_dragPlane.transform.position + new Vector3(m_dragPlane.transform.localScale.x / 2, -m_dragPlane.transform.localScale.y / 2), radius);
        Gizmos.DrawSphere(m_dragPlane.transform.position + new Vector3(m_dragPlane.transform.localScale.x / 2, m_dragPlane.transform.localScale.y / 2), radius);
        Gizmos.DrawSphere(m_dragPlane.transform.position + new Vector3(-m_dragPlane.transform.localScale.x / 2, m_dragPlane.transform.localScale.y / 2), radius);

        Gizmos.DrawWireCube(m_dragPlane.transform.position, new Vector3(m_dragPlane.transform.localScale.x, m_dragPlane.transform.localScale.y));
    }
}
