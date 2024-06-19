using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : Singleton<PaintManager>{

    public Shader texturePaint;
    public Shader extendIslands;
    public Shader normalMaskPaint;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int blendOpID = Shader.PropertyToID("_BlendOp");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");
    int uvOffsetID = Shader.PropertyToID("_OffsetUV");
    int uvIslandsID = Shader.PropertyToID("_UVIslands");
    int paintTextureID = Shader.PropertyToID("_PaintTexture");
    int normalMaskID = Shader.PropertyToID("_PaintNormalMap");

    Material paintMaterial;
    Material extendMaterial;
    Material normalMaterial;

    CommandBuffer command;

    public override void Awake(){
        base.Awake();
        
        paintMaterial = new Material(texturePaint);
        extendMaterial = new Material(extendIslands);
        normalMaterial = new Material(normalMaskPaint);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }

    public void initTextures(Paintable paintable){
        RenderTexture mask = paintable.getMask();
        RenderTexture normalMask = paintable.getNormalMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();

        command.SetRenderTarget(mask);  
        command.SetRenderTarget(normalMask);
        command.SetRenderTarget(extend);
        command.SetRenderTarget(support);

        paintMaterial.SetFloat(prepareUVID, 1);
        command.SetRenderTarget(uvIslands);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }


    public void paint(Paintable paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, 
        Texture2D paintTexture = null, Texture2D normalPaintMap = null, Color? color = null){
        
        RenderTexture mask = paintable.getMask();
        RenderTexture normalMask = paintable.getNormalMask();
        RenderTexture uvIslands = paintable.getUVIslands();
        RenderTexture extend = paintable.getExtend();
        RenderTexture support = paintable.getSupport();
        Renderer rend = paintable.getRenderer();
        
        paintMaterial.SetTexture(paintTextureID, paintTexture);
        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, support);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        
        extendMaterial.SetFloat(uvOffsetID, paintable.extendsIslandOffset);
        extendMaterial.SetTexture(uvIslandsID, uvIslands);

        normalMaterial.SetTexture(normalMaskID, normalMask);

        command.SetRenderTarget(mask);
        command.DrawRenderer(rend, paintMaterial, 0);
        
        command.SetRenderTarget(support);
        command.Blit(mask, support);

        command.SetRenderTarget(extend);
        command.Blit(mask, extend, extendMaterial);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();

        //command.SetRenderTarget(normalMask);
        //command.DrawRenderer(rend, normalMaterial, 0);

        //command.SetRenderTarget(support);
       // command.Blit(normalMask, support);

        //command.SetRenderTarget(extend);
        //command.Blit(normalMask, extend, extendMaterial);

        //Graphics.ExecuteCommandBuffer(command);
        //command.Clear();
    }


}
