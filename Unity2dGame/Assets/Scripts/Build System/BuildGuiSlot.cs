using UnityEngine;
using System.Collections;


//[RequireComponent(typeof(SphereCollider))]
//[ExecuteInEditMode]
public class BuildGuiSlot : MonoBehaviour 
{
	
	public BuildBlockBase blockPrefab;
	BuildBlockBase guiBlock;
	
	IEnumerator Start()
	{
		//(this.collider as SphereCollider).radius = 2;
		
		/*
		if(guiBlock != null)
		{
			DestroyImmediate(guiBlock.gameObject);
		}
		*/
		
		guiBlock = (BuildBlockBase)Instantiate(blockPrefab, this.transform.position, Quaternion.identity);
		
		guiBlock.transform.parent = this.transform;

		while(guiBlock.dockingPoints == null)
		{
			yield return null;
		}
		
		guiBlock.enabled = false;	
			
		guiBlock.gameObject.layer = this.gameObject.layer;
		guiBlock.gameObject.hideFlags = HideFlags.DontSave;
		foreach(var dockingPoint in guiBlock.dockingPoints)
		{
			dockingPoint.gameObject.layer = this.gameObject.layer;
			dockingPoint.gameObject.hideFlags = HideFlags.DontSave;
		}
	}
	
	
	void Update()
	{
		guiBlock.transform.Rotate(new Vector3(1,1,1) * 100 * Time.deltaTime);  
    }


	void OnMouseDown()
	{
		Debug.Log("BuildGuiSlot Mouse Down");
	
		BuildBlockBase newBlock = (BuildBlockBase)Instantiate(blockPrefab, MousePointer.position, Quaternion.identity);
		newBlock.enabled = true;
		newBlock.StartDragging();
		return;
		
		//block.transform.position = MousePointer.position;
		//block.transform.rotation = Quaternion.identity;

	}
		
}
