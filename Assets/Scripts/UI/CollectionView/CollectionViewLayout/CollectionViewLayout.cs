using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class CollectionViewLayout : MonoBehaviour
{
    [SerializeField]
    protected float m_animTime = 1;

    [SerializeField]
    private int m_maxCells = 16;

    protected string NibInUse { get; set; }
    
    protected EventHandler OnCellClick { get; set; }
    
    public float CurrentIndex { get; set; }
    
    public bool RequiresRelayout { get; set; }
    
    public bool InstantLayout { get; set; }

    public virtual int TotalCellsNeeded { get { return m_maxCells; } }

    public virtual bool IndexWrap { get { return false; } }

    /// <summary>
    /// This handles reuse, it will work out the indices we need, add delete/add cell
    /// as needed.
    /// 
    /// Call before layout
    /// </summary>
    /// <param name="data">Data - The whole data we are laying out.</param>
    /// <param name="cells">Cells - The array of cells we will add/delete from as needed</param>
	private void PrepareCells(List<object> data, List<iCollectionViewCell> cells)
    {
        int totCells = Math.Min(TotalCellsNeeded, data.Count);

        int curIndex = (int)Math.Round(CurrentIndex);
        int min = curIndex - (totCells / 2);
        int max = curIndex + (totCells / 2);
        
        if(!IndexWrap)
        {
            if(min < 0)
            {
                min = 0;
                max = totCells;
            }
            else if (max >= data.Count)
            {
                max = data.Count;
                min = max - totCells;
            }
        }
        
        //GO through cells, any with an index we no longer need, turn off
        int minVal = (min < 0) ? min + data.Count : min;
        int maxVal = (max >= data.Count) ? max - data.Count : max;
        
        
        if (!IndexWrap) {
            if (max >= data.Count) {
                maxVal = data.Count - 1;
                minVal = maxVal - m_maxCells;
            }

            min = minVal;
            max = maxVal;
        }

        
        for(int i=0; i<cells.Count; i++)
        {
            iCollectionViewCell c = cells[i];
            c.Snap = false;

            //Bizzare annoyance,min and max can sometimes be the same, i.e. we have enough cells for all the data fine
            if(maxVal == minVal)
                continue;
            
            //If maxval is bigger than minval then just check were in the range
            if(maxVal > minVal && (c.Index <= maxVal && c.Index >= minVal))
                continue;
            
            //If min value is bigger than max were in a "wrap"
            if(minVal > maxVal && (c.Index >= minVal || c.Index <= maxVal))
                continue;
            c.SetActive(false);
            cells.Remove(c);
            i--;
        }
        
        //Go through cells, add any indices we need and set their data TODO - How to do this quickly?
        for(int i = min; i <= max; i++)
        {
            int index = i;
            //Wrap index
            if(index<0)
                index += data.Count;
            else if(index >= data.Count)
                index -= data.Count;
            
            //Check if we have htis index
            bool found = false;
			foreach(iCollectionViewCell cell in cells)
            {
                if(cell.Index == index)
                {
                    found = true;
                    //Set Data if null
                    if(cell.Data == null)
                        cell.SetData(data[index]);
                    break;
                }
            }
            
            if(!found)
            {

                //Add a new one
				iCollectionViewCell c = CellPoolService.Instance.CreateCell(NibInUse, gameObject);
                c.transform.localPosition = Vector3.zero;
                c.Index = index;
                c.Snap = true;
                c.SetData(data[index]);

                if(OnCellClick!=null)
                    c.OnClickEvent += OnCellClick;
                
                cells.Add(c);
            }
        }
    }

    /// <summary>
    /// Register a cell for use with this layout
    /// </summary>
    /// <param name="cell">Cell - iCoverFlowCell being used</param>
    /// <param name="onCellClick">On cell click - Optional OnCellClick event</param>
	public void RegisterCell(iCollectionViewCell cell, EventHandler onCellClick = null)
    {   
        //Register pool
        CellPoolService.Instance.RegisterCell(cell);
        
        NibInUse = cell.NibName;
        if(onCellClick!=null)
            OnCellClick = onCellClick;
    }

    /// <summary>
    /// Layout function
    /// </summary>
    /// <param name="data">Data - The whole data we are laying out</param>
    /// <param name="cells">Cells - The cell list</param>
    /// <param name="dragging">If set to <c>true</c> we are currently dragging the layout dragging.</param>
	public virtual void Layout(List<object> data, List<iCollectionViewCell> cells, bool dragging = false)
    {
        PrepareCells(data, cells);

        PreLayout(data, cells);

        //Layout
		foreach(iCollectionViewCell cell in cells)
        {
            bool snap = InstantLayout || cell.Snap;
            //Get index Offset based on wrap
            float offset = cell.Index - CurrentIndex;

            if(IndexWrap)
            {
                if(offset > data.Count / 2)
                {
                    offset -= data.Count;
                }
                else if(offset < -data.Count / 2)
                {
                    offset += data.Count;
                }

                //We want to "snap" if we go from extreme left/right to the other side
                if(Math.Abs(cell.FlowIndexOffset - offset) > data.Count / 2)
                    snap = true;
            }

            cell.FlowIndexOffset = offset;
           
            Vector3 pos = Position(offset, cell.Index);
            Vector3 rot = EulerAngle(offset,  cell.Index);
            Vector3 scale = Scale(offset, cell.Index);

            //Position is local in layout but iTween is world
            pos += transform.position;

            if(snap)
            {
                //Kill all tween and snap
                iTween.Stop(cell.gameObject);

                if(cell.gameObject.transform.position != pos)
                    cell.gameObject.transform.position = pos;

                if(cell.gameObject.transform.localEulerAngles != rot)
                    cell.gameObject.transform.localEulerAngles = rot;

                if(cell.gameObject.transform.localScale != scale)
                    cell.gameObject.transform.localScale = scale;

            }
            else if(dragging)
            {
                //Kill all tweens and smooth update
                iTween.Stop(cell.gameObject);

                if(cell.gameObject.transform.position != pos)
                    iTween.MoveUpdate(cell.gameObject, pos, m_animTime);
                
                if(cell.gameObject.transform.localEulerAngles != rot)
                    iTween.RotateUpdate(cell.gameObject, rot, m_animTime);
                
                if(cell.gameObject.transform.localScale != scale)
                    iTween.ScaleUpdate(cell.gameObject, scale, m_animTime);
            }
            else
            {
                if(cell.gameObject.transform.position != pos)
                    iTween.MoveTo(cell.gameObject, pos, m_animTime);
                
                if(cell.gameObject.transform.localEulerAngles != rot)
                    iTween.RotateTo(cell.gameObject, rot, m_animTime);
                
                if(cell.gameObject.transform.localScale != scale)
                    iTween.ScaleTo(cell.gameObject, scale, m_animTime);
            }
        }

        PostLayout(data, cells);

        RequiresRelayout = false;
        InstantLayout = false;
    }

    protected abstract Vector3 EulerAngle(float indexOffset, int cellIndex);
    
    protected abstract Vector3 Position(float indexOffset, int cellIndex);

    protected abstract Vector3 Scale(float indexOffset, int cellIndex);

    protected virtual float Alpha(float indexOffset, int cellIndex) { return 1.0f; }

    /// <summary>
    /// Called before the cells are laid out, but after the cell reuse pass has been done. Cells will be the right size (array size).
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="cells">Cells.</param>
    /// <param name="dragging">If set to <c>true</c> dragging.</param>
	protected virtual void PreLayout(List<object> data, List<iCollectionViewCell> cells, bool dragging = false) { }

    /// <summary>
    /// Called after cells have been laid out (snapped or tweens added)
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="cells">Cells.</param>
    /// <param name="dragging">If set to <c>true</c> dragging.</param>
	protected virtual void PostLayout(List<object> data, List<iCollectionViewCell> cells, bool dragging = false) { }

    //Mathf.Sign(0) returns 1, but we want 0
    protected int Sign(float x)
    {
        if(x<0)
            return -1;
        if(x>0)
            return 1;
        
        return 0;
    }

}
