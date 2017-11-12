using UnityEngine;
using System.Collections;
using System;

public abstract class iCollectionViewCell : MonoBehaviour
{
    public abstract string NibName { get; }

    public EventHandler OnClickEvent;

    public int Index { get; set; }

    public bool Snap { get; set; }

    public float FlowIndexOffset { get; set; }

    public bool IsActive { get; set; }

    public object Data { get; set; }

	public iCollectionViewCell CreateCell()
    {
		return (iCollectionViewCell)Instantiate(this);
    }

    public void OnMouseUpAsButton()
    {
        if(OnClickEvent!=null)
        {
            OnClickEvent(this, EventArgs.Empty);
        }
    }

    public void OnClick()
    {
        if(OnClickEvent != null)
        {
            OnClickEvent(this, EventArgs.Empty);
        }
    }

    public void SetActive(bool active, GameObject owner = null)
    {
        if(active)
        {
            if(owner!=null)
                transform.parent = owner.transform;

            OnCellReuse();
        }
        else
        {
            OnCellDestroy();
        }
    }

    //This will happen before Start!
    public virtual void SetData(object data)
    {
        Data = data;
    }

    protected virtual void OnCellDestroy()
    {
        IsActive = false;
        Data = null;
        OnClickEvent = null;
        Index = 0;
        FlowIndexOffset = 0;
        transform.parent = CellPoolService.Instance.transform;
        gameObject.SetActive(false);
    }
    
    //Called whena  cell is reactivated or created
    protected virtual void OnCellReuse()
    {
        IsActive = true;
        gameObject.SetActive(true);
    }

}
