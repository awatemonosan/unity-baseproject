using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class SpriteTexture : MonoBehaviour {
	public Sprite sprite=null;
	Sprite oldSprite=null;
	Vector3 oldScale=Vector3.zero;
	// Use this for initialization
	void Start () {
		UpdateTexture();
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.localScale!=oldScale || sprite!=oldSprite)
			UpdateTexture();
	}
	
	void UpdateTexture(){
		oldScale=transform.localScale;
		oldSprite=sprite;
		if(sprite){
			renderer.sharedMaterial=new Material (Shader.Find("Transparent/Cutout/Diffuse"));
			Texture2D texture=new Texture2D((int)sprite.textureRect.width,(int)sprite.textureRect.height);
			texture.SetPixels(sprite.texture.GetPixels((int)sprite.textureRect.x,(int)sprite.textureRect.y,(int)sprite.textureRect.width,(int)sprite.textureRect.height));
			texture.filterMode=FilterMode.Point;
			texture.Apply();
			
			renderer.sharedMaterial.mainTexture=texture;
			renderer.sharedMaterial.mainTextureScale=new Vector2(transform.localScale.x*(sprite.rect.width / sprite.bounds.size.x)/sprite.textureRect.width,transform.localScale.y*(sprite.rect.width / sprite.bounds.size.x)/sprite.textureRect.height);
		}
	}
}

//--------------------------------------------------------------------------------

//Part of Awate's Standard Assets Pack