/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text ;
using System.Runtime.InteropServices;
using live2d;
using live2d.framework;

[ExecuteInEditMode]
public class LAppModelProxy : MonoBehaviour
{
    public String path="";
    public int sceneNo = 0;

    private LAppModel model;

	private bool				isVisible = false ;

	private bool				isPlayingIdleAnim;

	private bool				isCurrentAnimLoop;

	private int					hairIndex;
	private int					clothesIndex;

	private const int			variation = 5;

	private List<string>		queuedAnimations;

	private string				lastAnimation;

	void Awake()
    {
        if (path == "") return;
        model = new LAppModel(this);

        LAppLive2DManager.Instance.AddModel(this);

        var filename = FileManager.getFilename(path);
        var dir = FileManager.getDirName(path);

        Debug.Log("Load " + dir +"  filename:"+ filename);
		model.LoadFromStreamingAssets(dir, filename);

		//model.GetLive2DModelUnity ().setRenderMode (Live2D.L2D_RENDER_DRAW_MESH);
		hairIndex = clothesIndex = 1;

		queuedAnimations = new List<string> ();
    }
	

	void OnRenderObject()
	{ 
		if(!isVisible) return;
		if(model==null)return;

		if(model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH_NOW)
		{
            //model.Draw();
		}
		
		if (LAppDefine.DEBUG_DRAW_HIT_AREA)
		{
			
			//model.DrawHitArea();
		}
	}
	

	void Update()
	{
		if(!isVisible) return;
        if (model == null) return;

        model.Update();
        if (model.GetLive2DModelUnity().getRenderMode() == Live2D.L2D_RENDER_DRAW_MESH)
        {
            model.Draw();
        }

		if (isPlayingIdleAnim && IsAnimationComplete ()) {
			isPlayingIdleAnim = false;
		}

		if (IsAnimationComplete ()){
			if (queuedAnimations.Count > 0) {
				PlayAnimation(queuedAnimations[0]);
				queuedAnimations.RemoveAt (0);
			}
			else if (isCurrentAnimLoop){
				PlayAnimation (lastAnimation);
			}
		}
	}


    public LAppModel GetModel()
    {
        return model;
    }


	public void SetVisible(bool isVisible)
	{
		this.isVisible = isVisible;
	}


	public bool GetVisible()
	{
		return isVisible;
	}


    public void ResetAudioSource()
    {
        Component[] components = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < components.Length; i++)
        {
            Destroy(components[i]);
        }
    }

	public void PlayAttackAnim()
	{
		isPlayingIdleAnim = false;
		model.StartMotion ("attack", 0, LAppDefine.PRIORITY_FORCE);
	}

	public void PlayDamageAnim()
	{
		isPlayingIdleAnim = false;
		model.StartRandomMotion ("damaged", LAppDefine.PRIORITY_NORMAL);
	}

	public void PlayAnimation(string animation)
	{
		isPlayingIdleAnim = false;
		model.StartMotion (animation, 0, LAppDefine.PRIORITY_FORCE);

		lastAnimation = animation;
	}

	public void PlayAnimationSequence(string[] animations, bool loopAnimation = false)
	{
		for (int i = 1; i < animations.Length; ++i) {
			queuedAnimations.Add (animations[i]);
		}
		PlayAnimation (animations [0]);
		isCurrentAnimLoop = loopAnimation;
	}

	public void PlayIdleAnim()
	{
		isPlayingIdleAnim = true;
		model.StartMotion ("idle", 0, LAppDefine.PRIORITY_IDLE);
	}

	public void PlayIdleFrontFaceAnim()
	{
		isPlayingIdleAnim = true;
		model.StartMotion ("frontface", 0, LAppDefine.PRIORITY_IDLE);
	}

	public void PlayCombatIdleAnim()
	{
		isPlayingIdleAnim = true;
		model.StartMotion ("idle_battle", 0, LAppDefine.PRIORITY_IDLE);
	}

	public void PlayTransformAnim()
	{
		model.StartMotion ("transform", 0, LAppDefine.PRIORITY_IDLE);
	}

	public void LoadProfile(){
		hairIndex = PlayerProfile.Get ().playerHairIndex;
		clothesIndex = PlayerProfile.Get ().playerClothesIndex;

		SetTexture ();
	}

	public void SaveProfile(){
		PlayerProfile.Get ().playerHairIndex = hairIndex;
		PlayerProfile.Get ().playerClothesIndex = clothesIndex;
	}

	public void SetCostume(int clothes, int hair){
		
		if (clothes <= variation && clothes > 0)
			clothesIndex = clothes;
		
		if (hair <= variation && hair > 0)
			hairIndex = hair;

		
		SetTexture ();
	}

	public void SetClothes(int clothes){
		if (clothes <= variation && clothes > 0)
			clothesIndex = clothes;

		SetTexture ();
	}

	public void SetHair(int hair){
		if (hair <= variation && hair > 0)
			hairIndex = hair;

		SetTexture ();
	}

	void SetTexture(){
		int textureIndex = 1 + (clothesIndex-1) * variation + (hairIndex-1); //texture 0 is transformed
		
		model.ChangeTexture (textureIndex);	
	}


//	public void ChangeHair(bool positive){
//		if (positive) {
//			hairIndex = (hairIndex < 3)? hairIndex + 1 : hairIndex;
//		} else {
//			hairIndex = (hairIndex > 0)? hairIndex - 1 : hairIndex;
//		}
//		int textureIndex = clothesIndex * variation + hairIndex;
//
//		model.ChangeTexture (textureIndex);
//
//		Debug.Log ("Hair: " + hairIndex + " Clothes: " + clothesIndex);
//	}	
//
//	public void ChangeClothes(bool positive){
//		if (positive) {
//			clothesIndex = (clothesIndex < 3)? clothesIndex + 1 : clothesIndex;
//		} else {
//			clothesIndex = (clothesIndex > 0)? clothesIndex - 1 : clothesIndex;
//		}
//		int textureIndex = clothesIndex * variation + hairIndex;
//		
//		model.ChangeTexture (textureIndex);
//		Debug.Log ("Hair: " + hairIndex + " Clothes: " + clothesIndex);
//	}

	public bool IsAnimationComplete()
	{
		return model.getMainMotionManager ().isFinished ();
	}

	public bool IsPlayingIdleAnimation()
	{
		return isPlayingIdleAnim;
	}
}